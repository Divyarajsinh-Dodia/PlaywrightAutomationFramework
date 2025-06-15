using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;
using PlaywrightFramework.Core.Extensions;

namespace PlaywrightFramework.Core.Base;

/// <summary>
/// Base page class that provides common page functionality for Page Object Model
/// Enhanced with ILocator extension methods for improved usability
/// </summary>
public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly TestConfiguration Config;
    protected readonly ILogger Logger;

    protected BasePage(IPage page, TestConfiguration config, ILogger logger)
    {
        Page = page ?? throw new ArgumentNullException(nameof(page));
        Config = config ?? throw new ArgumentNullException(nameof(config));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Set the context for all ILocator extension methods
        LocatorContext.SetContext(config, logger);
    }

    /// <summary>
    /// Helper method to create locators with enhanced functionality
    /// </summary>
    protected ILocator Locate(string selector) => Page.Locator(selector);

    /// <summary>
    /// Gets the current page URL
    /// </summary>
    public string CurrentUrl => Page.Url;

    /// <summary>
    /// Gets the current page title
    /// </summary>
    public async Task<string> GetTitleAsync() => await Page.TitleAsync();

    /// <summary>
    /// Navigate to URL
    /// </summary>
    protected async Task NavigateToAsync(string url, WaitUntilState waitUntil = WaitUntilState.Load)
    {
        var fullUrl = url.StartsWith("http") ? url : Config.Application.BaseUrl.TrimEnd('/') + "/" + url.TrimStart('/');
        Logger.LogInformation("Navigating to: {Url}", fullUrl);
        await Page.GotoAsync(fullUrl, new PageGotoOptions { WaitUntil = waitUntil });
        Logger.LogInformation("Navigation completed to: {Url}", fullUrl);
    }

    /// <summary>
    /// General page-level wait methods
    /// </summary>
    protected async Task WaitForLoadAsync(WaitUntilState waitUntil = WaitUntilState.Load, int? timeout = null)
    {
        var timeoutMs = timeout ?? Config.Browser.NavigationTimeoutMs;
        Logger.LogDebug("Waiting for page load (condition: {WaitUntil}, timeout: {Timeout}ms)", waitUntil, timeoutMs);
        var loadState = waitUntil switch
        {
            WaitUntilState.Load => LoadState.Load,
            WaitUntilState.DOMContentLoaded => LoadState.DOMContentLoaded,
            WaitUntilState.NetworkIdle => LoadState.NetworkIdle,
            _ => LoadState.Load
        };
        await Page.WaitForLoadStateAsync(loadState, new PageWaitForLoadStateOptions { Timeout = timeoutMs });
        Logger.LogDebug("Page loaded (condition: {WaitUntil})", waitUntil);
    }

    protected async Task WaitAsync(int seconds)
    {
        Logger.LogDebug("Waiting for {Seconds} seconds", seconds);
        await Task.Delay(TimeSpan.FromSeconds(seconds));
    }
}