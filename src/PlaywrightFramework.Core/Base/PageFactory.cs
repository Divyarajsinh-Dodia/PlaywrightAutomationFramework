using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;
using System.Collections.Concurrent;

namespace PlaywrightFramework.Core.Base;

/// <summary>
/// Factory class for creating and caching page objects
/// Enables fluent method chaining across different page objects
/// </summary>
public class PageFactory
{
    private readonly IPage _page;
    private readonly TestConfiguration _config;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<Type, IFluentPage> _pageCache = new();

    /// <summary>
    /// Initializes a new instance of the PageFactory class
    /// </summary>
    /// <param name="page">The Playwright page instance</param>
    /// <param name="config">The test configuration</param>
    /// <param name="loggerFactory">The logger factory</param>
    public PageFactory(IPage page, TestConfiguration config, ILoggerFactory loggerFactory)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    /// <summary>
    /// Gets or creates an instance of the specified page type
    /// </summary>
    /// <typeparam name="T">The type of the page to get or create</typeparam>
    /// <returns>The page instance</returns>
    public T GetPage<T>() where T : class, IFluentPage
    {
        return (T)_pageCache.GetOrAdd(typeof(T), CreatePage<T>);
    }

    /// <summary>
    /// Creates a new instance of the specified page type
    /// </summary>
    private IFluentPage CreatePage<T>(Type pageType) where T : class, IFluentPage
    {
        // Try to find constructor that takes IPage, TestConfiguration, ILogger<T>, PageFactory
        var genericLoggerType = typeof(ILogger<>).MakeGenericType(pageType);
        var constructor = pageType.GetConstructor(new[] { 
            typeof(IPage), 
            typeof(TestConfiguration), 
            genericLoggerType,
            typeof(PageFactory) 
        });

        if (constructor != null)
        {
            var logger = _loggerFactory.CreateLogger(pageType);
            return (IFluentPage)constructor.Invoke(new object[] { _page, _config, logger, this });
        }

        // Try with just ILogger (non-generic)
        constructor = pageType.GetConstructor(new[] { 
            typeof(IPage), 
            typeof(TestConfiguration), 
            typeof(ILogger),
            typeof(PageFactory) 
        });

        if (constructor != null)
        {
            var logger = _loggerFactory.CreateLogger(pageType);
            return (IFluentPage)constructor.Invoke(new object[] { _page, _config, logger, this });
        }

        throw new InvalidOperationException(
            $"Cannot create an instance of {pageType}. " +
            $"Expected constructor: {pageType.Name}(IPage, TestConfiguration, ILogger<{pageType.Name}>, PageFactory)");
    }
}
