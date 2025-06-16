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
        return (T)_pageCache.GetOrAdd(typeof(T), type =>
        {
            // Try constructor with ILogger<T>
            var genericLoggerType = typeof(ILogger<>).MakeGenericType(type);
            var ctorGeneric = type.GetConstructor(new[] { typeof(IPage), typeof(TestConfiguration), genericLoggerType, typeof(PageFactory) });
            if (ctorGeneric != null)
            {
                // Create ILogger<T> via extension
                var method = typeof(LoggerFactoryExtensions)
                    .GetMethod(nameof(LoggerFactoryExtensions.CreateLogger), new[] { typeof(ILoggerFactory) })!;
                var logger = method.MakeGenericMethod(type).Invoke(null, new object[] { _loggerFactory })!;
                return (IFluentPage)ctorGeneric.Invoke(new object[] { _page, _config, logger, this });
            }
            // Fallback to non-generic ILogger
            var ctor = type.GetConstructor(new[] { typeof(IPage), typeof(TestConfiguration), typeof(ILogger), typeof(PageFactory) });
            if (ctor != null)
            {
                var logger = _loggerFactory.CreateLogger(type);
                return (IFluentPage)ctor.Invoke(new object[] { _page, _config, logger, this });
            }
            throw new InvalidOperationException($"Cannot find suitable constructor for {type.Name}");
        });
    }
}
