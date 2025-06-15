using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;

namespace PlaywrightFramework.Core.Base;

/// <summary>
/// Base page class that provides common page functionality for Page Object Model
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
    }

    /// <summary>
    /// Gets the current page URL
    /// </summary>
    public string CurrentUrl => Page.Url;

    /// <summary>
    /// Gets the current page title
    /// </summary>
    public async Task<string> GetTitleAsync() => await Page.TitleAsync();

    /// <summary>
    /// Navigates to the specified URL
    /// </summary>
    /// <param name="url">URL to navigate to</param>
    /// <param name="waitUntil">Wait condition for navigation</param>
    public async Task NavigateToAsync(string url, WaitUntilState waitUntil = WaitUntilState.Load)
    {
        Logger.LogInformation("Navigating to: {Url}", url);
        await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = waitUntil });
    }

    /// <summary>
    /// Clicks an element by locator
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Click options</param>
    public async Task ClickAsync(string locator, PageClickOptions? options = null)
    {
        Logger.LogDebug("Clicking element: {Locator}", locator);
        await WaitForElementAsync(locator);
        await Page.ClickAsync(locator, options);
        Logger.LogDebug("Clicked element: {Locator}", locator);
    }

    /// <summary>
    /// Clicks an element by ILocator
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Click options</param>
    public async Task ClickAsync(ILocator locator, LocatorClickOptions? options = null)
    {
        Logger.LogDebug("Clicking element with locator");
        await locator.WaitForAsync();
        await locator.ClickAsync(options);
        Logger.LogDebug("Clicked element with locator");
    }

    /// <summary>
    /// Double clicks an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Double click options</param>
    public async Task DoubleClickAsync(string locator, PageDblClickOptions? options = null)
    {
        Logger.LogDebug("Double clicking element: {Locator}", locator);
        await WaitForElementAsync(locator);
        await Page.DblClickAsync(locator, options);
        Logger.LogDebug("Double clicked element: {Locator}", locator);
    }

    /// <summary>
    /// Right clicks an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Right click options</param>
    public async Task RightClickAsync(string locator, PageClickOptions? options = null)
    {
        Logger.LogDebug("Right clicking element: {Locator}", locator);
        await WaitForElementAsync(locator);
        
        var clickOptions = options ?? new PageClickOptions();
        clickOptions.Button = MouseButton.Right;
        
        await Page.ClickAsync(locator, clickOptions);
        Logger.LogDebug("Right clicked element: {Locator}", locator);
    }

    /// <summary>
    /// Types text into an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="text">Text to type</param>
    /// <param name="options">Type options</param>
    public async Task TypeAsync(string locator, string text, PageTypeOptions? options = null)
    {
        Logger.LogDebug("Typing text into element: {Locator}", locator);
        await WaitForElementAsync(locator);
        await Page.TypeAsync(locator, text, options);
        Logger.LogDebug("Typed text into element: {Locator}", locator);
    }

    /// <summary>
    /// Fills an input element with text
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="text">Text to fill</param>
    /// <param name="options">Fill options</param>
    public async Task FillAsync(string locator, string text, PageFillOptions? options = null)
    {
        Logger.LogDebug("Filling element: {Locator} with text: {Text}", locator, text);
        await WaitForElementAsync(locator);
        await Page.FillAsync(locator, text, options);
        Logger.LogDebug("Filled element: {Locator}", locator);
    }

    /// <summary>
    /// Clears an input element
    /// </summary>
    /// <param name="locator">Element locator</param>
    public async Task ClearAsync(string locator)
    {
        Logger.LogDebug("Clearing element: {Locator}", locator);
        await WaitForElementAsync(locator);
        await Page.FillAsync(locator, "");
        Logger.LogDebug("Cleared element: {Locator}", locator);
    }

    /// <summary>
    /// Gets text content of an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Text content options</param>
    /// <returns>Text content of the element</returns>
    public async Task<string?> GetTextAsync(string locator, PageTextContentOptions? options = null)
    {
        Logger.LogDebug("Getting text from element: {Locator}", locator);
        await WaitForElementAsync(locator);
        var text = await Page.TextContentAsync(locator, options);
        Logger.LogDebug("Got text from element: {Locator} - {Text}", locator, text);
        return text;
    }

    /// <summary>
    /// Gets inner text of an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Inner text options</param>
    /// <returns>Inner text of the element</returns>
    public async Task<string> GetInnerTextAsync(string locator, PageInnerTextOptions? options = null)
    {
        Logger.LogDebug("Getting inner text from element: {Locator}", locator);
        await WaitForElementAsync(locator);
        var text = await Page.InnerTextAsync(locator, options);
        Logger.LogDebug("Got inner text from element: {Locator} - {Text}", locator, text);
        return text;
    }

    /// <summary>
    /// Gets an attribute value from an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="name">Attribute name</param>
    /// <param name="options">Get attribute options</param>
    /// <returns>Attribute value</returns>
    public async Task<string?> GetAttributeAsync(string locator, string name, PageGetAttributeOptions? options = null)
    {
        Logger.LogDebug("Getting attribute '{AttributeName}' from element: {Locator}", name, locator);
        await WaitForElementAsync(locator);
        var value = await Page.GetAttributeAsync(locator, name, options);
        Logger.LogDebug("Got attribute '{AttributeName}' from element: {Locator} - {Value}", name, locator, value);
        return value;
    }

    /// <summary>
    /// Checks if an element is visible
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Is visible options</param>
    /// <returns>True if visible, false otherwise</returns>
    public async Task<bool> IsVisibleAsync(string locator, PageIsVisibleOptions? options = null)
    {
        Logger.LogDebug("Checking if element is visible: {Locator}", locator);
        var isVisible = await Page.IsVisibleAsync(locator, options);
        Logger.LogDebug("Element visibility check: {Locator} - {IsVisible}", locator, isVisible);
        return isVisible;
    }

    /// <summary>
    /// Checks if an element is enabled
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Is enabled options</param>
    /// <returns>True if enabled, false otherwise</returns>
    public async Task<bool> IsEnabledAsync(string locator, PageIsEnabledOptions? options = null)
    {
        Logger.LogDebug("Checking if element is enabled: {Locator}", locator);
        var isEnabled = await Page.IsEnabledAsync(locator, options);
        Logger.LogDebug("Element enabled check: {Locator} - {IsEnabled}", locator, isEnabled);
        return isEnabled;
    }

    /// <summary>
    /// Checks if an element is checked (for checkboxes/radio buttons)
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Is checked options</param>
    /// <returns>True if checked, false otherwise</returns>
    public async Task<bool> IsCheckedAsync(string locator, PageIsCheckedOptions? options = null)
    {
        Logger.LogDebug("Checking if element is checked: {Locator}", locator);
        var isChecked = await Page.IsCheckedAsync(locator, options);
        Logger.LogDebug("Element checked state: {Locator} - {IsChecked}", locator, isChecked);
        return isChecked;
    }

    /// <summary>
    /// Selects an option from a dropdown
    /// </summary>
    /// <param name="locator">Select element locator</param>
    /// <param name="values">Option values to select</param>
    /// <param name="options">Select option options</param>
    /// <returns>Array of selected option values</returns>
    public async Task<string[]> SelectOptionAsync(string locator, string[] values, PageSelectOptionOptions? options = null)
    {
        Logger.LogDebug("Selecting options in element: {Locator} - {Values}", locator, string.Join(", ", values));
        await WaitForElementAsync(locator);
        var selectedValues = await Page.SelectOptionAsync(locator, values, options);
        var selectedArray = selectedValues.ToArray();
        Logger.LogDebug("Selected options in element: {Locator} - {SelectedValues}", locator, string.Join(", ", selectedArray));
        return selectedArray;
    }

    /// <summary>
    /// Selects an option from a dropdown by value
    /// </summary>
    /// <param name="locator">Select element locator</param>
    /// <param name="value">Option value to select</param>
    /// <param name="options">Select option options</param>
    /// <returns>Array of selected option values</returns>
    public async Task<string[]> SelectOptionAsync(string locator, string value, PageSelectOptionOptions? options = null)
    {
        return await SelectOptionAsync(locator, new[] { value }, options);
    }

    /// <summary>
    /// Hovers over an element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="options">Hover options</param>
    public async Task HoverAsync(string locator, PageHoverOptions? options = null)
    {
        Logger.LogDebug("Hovering over element: {Locator}", locator);
        await WaitForElementAsync(locator);
        await Page.HoverAsync(locator, options);
        Logger.LogDebug("Hovered over element: {Locator}", locator);
    }

    /// <summary>
    /// Scrolls an element into view
    /// </summary>
    /// <param name="locator">Element locator</param>
    public async Task ScrollIntoViewAsync(string locator)
    {
        Logger.LogDebug("Scrolling element into view: {Locator}", locator);
        await Page.Locator(locator).ScrollIntoViewIfNeededAsync();
        Logger.LogDebug("Scrolled element into view: {Locator}", locator);
    }

    /// <summary>
    /// Waits for an element to be visible
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    public async Task WaitForElementAsync(string locator, int? timeout = null)
    {
        var timeoutMs = timeout ?? Config.Browser.TimeoutMs;
        Logger.LogDebug("Waiting for element: {Locator} (timeout: {Timeout}ms)", locator, timeoutMs);
        
        await Page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions 
        { 
            Timeout = timeoutMs 
        });
        
        Logger.LogDebug("Element found: {Locator}", locator);
    }

    /// <summary>
    /// Waits for an element to be hidden
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    public async Task WaitForElementToBeHiddenAsync(string locator, int? timeout = null)
    {
        var timeoutMs = timeout ?? Config.Browser.TimeoutMs;
        Logger.LogDebug("Waiting for element to be hidden: {Locator} (timeout: {Timeout}ms)", locator, timeoutMs);
        
        await Page.WaitForSelectorAsync(locator, new PageWaitForSelectorOptions 
        { 
            State = WaitForSelectorState.Hidden,
            Timeout = timeoutMs 
        });
        
        Logger.LogDebug("Element is hidden: {Locator}", locator);
    }

    /// <summary>
    /// Waits for page load
    /// </summary>
    /// <param name="waitUntil">Wait condition</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    public async Task WaitForLoadAsync(WaitUntilState waitUntil = WaitUntilState.Load, int? timeout = null)
    {
        var timeoutMs = timeout ?? Config.Browser.NavigationTimeoutMs;
        Logger.LogDebug("Waiting for page load (condition: {WaitUntil}, timeout: {Timeout}ms)", waitUntil, timeoutMs);
        
        // Convert WaitUntilState to LoadState
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

    /// <summary>
    /// Waits for a specific URL pattern
    /// </summary>
    /// <param name="urlPattern">URL pattern to wait for</param>
    /// <param name="timeout">Timeout in milliseconds</param>
    public async Task WaitForUrlAsync(string urlPattern, int? timeout = null)
    {
        var timeoutMs = timeout ?? Config.Browser.NavigationTimeoutMs;
        Logger.LogDebug("Waiting for URL pattern: {UrlPattern} (timeout: {Timeout}ms)", urlPattern, timeoutMs);
        
        await Page.WaitForURLAsync(urlPattern, new PageWaitForURLOptions { Timeout = timeoutMs });
        
        Logger.LogDebug("URL pattern matched: {UrlPattern}", urlPattern);
    }

    /// <summary>
    /// Executes JavaScript code
    /// </summary>
    /// <param name="expression">JavaScript expression</param>
    /// <param name="arg">Optional argument</param>
    /// <returns>Result of the JavaScript execution</returns>
    public async Task<T> EvaluateAsync<T>(string expression, object? arg = null)
    {
        Logger.LogDebug("Executing JavaScript: {Expression}", expression);
        var result = await Page.EvaluateAsync<T>(expression, arg);
        Logger.LogDebug("JavaScript execution completed");
        return result;
    }

    /// <summary>
    /// Gets all elements matching the locator
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <returns>List of matching elements</returns>
    public ILocator GetElements(string locator)
    {
        Logger.LogDebug("Getting elements: {Locator}", locator);
        return Page.Locator(locator);
    }

    /// <summary>
    /// Gets the count of elements matching the locator
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <returns>Number of matching elements</returns>
    public async Task<int> GetElementCountAsync(string locator)
    {
        Logger.LogDebug("Getting element count: {Locator}", locator);
        var count = await Page.Locator(locator).CountAsync();
        Logger.LogDebug("Element count: {Locator} - {Count}", locator, count);
        return count;
    }

    /// <summary>
    /// Takes a screenshot of the current page
    /// </summary>
    /// <param name="path">Path to save the screenshot</param>
    /// <param name="options">Screenshot options</param>
    public async Task TakeScreenshotAsync(string path, PageScreenshotOptions? options = null)
    {
        Logger.LogDebug("Taking screenshot: {Path}", path);
        
        var screenshotOptions = options ?? new PageScreenshotOptions
        {
            FullPage = Config.Execution.FullPageScreenshots
        };
        
        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = screenshotOptions.FullPage,
            Quality = screenshotOptions.Quality,
            Type = screenshotOptions.Type
        });
        
        Logger.LogDebug("Screenshot taken: {Path}", path);
    }

    /// <summary>
    /// Refreshes the current page
    /// </summary>
    /// <param name="options">Reload options</param>
    public async Task RefreshAsync(PageReloadOptions? options = null)
    {
        Logger.LogDebug("Refreshing page");
        await Page.ReloadAsync(options);
        Logger.LogDebug("Page refreshed");
    }

    /// <summary>
    /// Goes back in browser history
    /// </summary>
    /// <param name="options">Go back options</param>
    public async Task GoBackAsync(PageGoBackOptions? options = null)
    {
        Logger.LogDebug("Going back in browser history");
        await Page.GoBackAsync(options);
        Logger.LogDebug("Went back in browser history");
    }

    /// <summary>
    /// Goes forward in browser history
    /// </summary>
    /// <param name="options">Go forward options</param>
    public async Task GoForwardAsync(PageGoForwardOptions? options = null)
    {
        Logger.LogDebug("Going forward in browser history");
        await Page.GoForwardAsync(options);
        Logger.LogDebug("Went forward in browser history");
    }
} 