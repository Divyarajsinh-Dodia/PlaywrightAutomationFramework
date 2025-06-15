using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;

namespace PlaywrightFramework.Core.Base;

/// <summary>
/// Base page class for fluent page objects
/// Provides common functionality for pages that support fluent method chaining
/// </summary>
public abstract class FluentBasePage : BasePage, IFluentPage
{
    /// <summary>
    /// Gets the page factory for creating page instances
    /// </summary>
    protected readonly PageFactory PageFactory;

    /// <summary>
    /// Gets the Playwright IPage instance
    /// </summary>
    public new IPage Page => base.Page;

    /// <summary>
    /// Initializes a new instance of the FluentBasePage class
    /// </summary>
    /// <param name="page">The Playwright page instance</param>
    /// <param name="config">The test configuration</param>
    /// <param name="logger">The logger</param>
    /// <param name="pageFactory">The page factory</param>
    protected FluentBasePage(IPage page, TestConfiguration config, ILogger logger, PageFactory pageFactory)
        : base(page, config, logger)
    {
        PageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
    }

    /// <summary>
    /// Wrapper for the asynchronous WaitForLoadAsync method to support fluent API
    /// </summary>
    protected void WaitForLoad(WaitUntilState waitUntil = WaitUntilState.Load, int? timeout = null)
    {
        WaitForLoadAsync(waitUntil, timeout).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Wrapper for the asynchronous WaitAsync method to support fluent API
    /// </summary>
    protected void Wait(int seconds)
    {
        WaitAsync(seconds).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Wrapper for the asynchronous NavigateToAsync method to support fluent API
    /// </summary>
    protected void NavigateTo(string url, WaitUntilState waitUntil = WaitUntilState.Load)
    {
        NavigateToAsync(url, waitUntil).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Safely gets the title of the current page synchronously
    /// </summary>
    public string GetTitle()
    {
        return GetTitleAsync().GetAwaiter().GetResult();
    }
}
