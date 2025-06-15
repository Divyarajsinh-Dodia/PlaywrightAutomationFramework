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

    #region Visibility-Aware Methods

    /// <summary>
    /// Clicks the first visible element matching the selector
    /// </summary>
    /// <param name="selector">Element selector</param>
    /// <param name="options">Click options</param>
    public async Task ClickVisibleAsync(string selector, LocatorClickOptions? options = null)
    {
        Logger.LogDebug("Clicking first visible element: {Selector}", selector);
        
        try
        {
            var visibleElement = Page.Locator(selector).Locator(":visible").First;
            await visibleElement.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            await visibleElement.ClickAsync(options);
            Logger.LogDebug("Successfully clicked visible element: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to click visible element: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Fills the first visible input element matching the selector
    /// </summary>
    /// <param name="selector">Input element selector</param>
    /// <param name="text">Text to fill</param>
    /// <param name="options">Fill options</param>
    public async Task FillVisibleAsync(string selector, string text, LocatorFillOptions? options = null)
    {
        Logger.LogDebug("Filling first visible element: {Selector} with text: {Text}", selector, text);
        
        try
        {
            var visibleElement = Page.Locator(selector).Locator(":visible").First;
            await visibleElement.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            await visibleElement.FillAsync(text, options);
            Logger.LogDebug("Successfully filled visible element: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to fill visible element: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Checks if any element matching the selector is visible
    /// </summary>
    /// <param name="selector">Element selector</param>
    /// <param name="timeout">Optional timeout in milliseconds</param>
    /// <returns>True if any element is visible, false otherwise</returns>
    public async Task<bool> IsAnyVisibleAsync(string selector, int? timeout = null)
    {
        Logger.LogDebug("Checking if any element is visible: {Selector}", selector);
        
        try
        {
            var timeoutMs = timeout ?? 1000; // Short timeout for visibility check
            var visibleElements = Page.Locator(selector).Locator(":visible");
            
            // Wait briefly to see if any elements become visible
            await visibleElements.First.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = timeoutMs 
            });
            
            var count = await visibleElements.CountAsync();
            var isVisible = count > 0;
            
            Logger.LogDebug("Visibility check for {Selector}: {IsVisible} ({Count} visible elements)", 
                selector, isVisible, count);
            return isVisible;
        }
        catch (TimeoutException)
        {
            Logger.LogDebug("No visible elements found for: {Selector}", selector);
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking visibility for: {Selector}", selector);
            return false;
        }
    }

    /// <summary>
    /// Waits for at least one element matching the selector to become visible
    /// </summary>
    /// <param name="selector">Element selector</param>
    /// <param name="timeoutMs">Timeout in milliseconds</param>
    /// <returns>First visible element or null if timeout</returns>
    public async Task<ILocator?> WaitForVisibleElementAsync(string selector, int? timeoutMs = null)
    {
        var timeout = timeoutMs ?? Config.Browser.TimeoutMs;
        Logger.LogDebug("Waiting for visible element: {Selector} (timeout: {Timeout}ms)", selector, timeout);
        
        try
        {
            var visibleElement = Page.Locator(selector).Locator(":visible").First;
            await visibleElement.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = timeout 
            });
            
            Logger.LogDebug("Found visible element: {Selector}", selector);
            return visibleElement;
        }
        catch (TimeoutException)
        {
            Logger.LogWarning("No visible element found within {Timeout}ms: {Selector}", timeout, selector);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error waiting for visible element: {Selector}", selector);
            return null;
        }
    }

    /// <summary>
    /// Gets the text content of the first visible element matching the selector
    /// </summary>
    /// <param name="selector">Element selector</param>
    /// <param name="timeout">Optional timeout in milliseconds</param>
    /// <returns>Text content of the first visible element</returns>
    public async Task<string?> GetVisibleTextAsync(string selector, int? timeout = null)
    {
        Logger.LogDebug("Getting text from first visible element: {Selector}", selector);
        
        var visibleElement = await WaitForVisibleElementAsync(selector, timeout);
        if (visibleElement == null)
        {
            Logger.LogWarning("No visible element found to get text from: {Selector}", selector);
            return null;
        }
        
        var text = await visibleElement.TextContentAsync();
        Logger.LogDebug("Got text from visible element {Selector}: {Text}", selector, text);
        return text;
    }

    #endregion

    #region JavaScript Actions

    /// <summary>
    /// Clicks an element using JavaScript (bypasses Playwright's clickability checks)
    /// </summary>
    /// <param name="selector">Element selector</param>
    public async Task ClickJavaScriptAsync(string selector)
    {
        Logger.LogDebug("Clicking element using JavaScript: {Selector}", selector);
        
        try
        {
            await Page.EvaluateAsync($@"
                const element = document.querySelector('{selector}');
                if (!element) throw new Error('Element not found: {selector}');
                element.click();
            ");
            
            Logger.LogDebug("Successfully clicked element using JavaScript: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to click element using JavaScript: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Fills an input element using JavaScript (bypasses validation and triggers events)
    /// </summary>
    /// <param name="selector">Input element selector</param>
    /// <param name="text">Text to fill</param>
    public async Task FillJavaScriptAsync(string selector, string text)
    {
        Logger.LogDebug("Filling element using JavaScript: {Selector} with text: {Text}", selector, text);
        
        try
        {
            // Escape the text to prevent JavaScript injection
            var escapedText = text.Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");
            
            await Page.EvaluateAsync($@"
                const element = document.querySelector('{selector}');
                if (!element) throw new Error('Element not found: {selector}');
                
                element.value = '{escapedText}';
                element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                element.dispatchEvent(new Event('change', {{ bubbles: true }}));
            ");
            
            Logger.LogDebug("Successfully filled element using JavaScript: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to fill element using JavaScript: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Scrolls to an element using JavaScript with smooth animation
    /// </summary>
    /// <param name="selector">Element selector to scroll to</param>
    /// <param name="behavior">Scroll behavior: 'smooth', 'instant', or 'auto'</param>
    /// <param name="block">Vertical alignment: 'start', 'center', 'end', or 'nearest'</param>
    /// <param name="inline">Horizontal alignment: 'start', 'center', 'end', or 'nearest'</param>
    public async Task ScrollToLocatorJavaScriptAsync(string selector, string behavior = "smooth", string block = "center", string inline = "nearest")
    {
        Logger.LogDebug("Scrolling to element using JavaScript: {Selector}", selector);
        
        try
        {
            await Page.EvaluateAsync($@"
                const element = document.querySelector('{selector}');
                if (!element) throw new Error('Element not found: {selector}');
                
                element.scrollIntoView({{
                    behavior: '{behavior}',
                    block: '{block}',
                    inline: '{inline}'
                }});
            ");
            
            // Wait a moment for scroll to complete
            await Task.Delay(500);
            
            Logger.LogDebug("Successfully scrolled to element using JavaScript: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to scroll to element using JavaScript: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Double clicks an element using JavaScript
    /// </summary>
    /// <param name="selector">Element selector</param>
    public async Task DoubleClickJavaScriptAsync(string selector)
    {
        Logger.LogDebug("Double clicking element using JavaScript: {Selector}", selector);
        
        try
        {
            await Page.EvaluateAsync($@"
                const element = document.querySelector('{selector}');
                if (!element) throw new Error('Element not found: {selector}');
                
                const event = new MouseEvent('dblclick', {{
                    view: window,
                    bubbles: true,
                    cancelable: true,
                    detail: 2
                }});
                
                element.dispatchEvent(event);
            ");
            
            Logger.LogDebug("Successfully double clicked element using JavaScript: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to double click element using JavaScript: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Clears an input element using JavaScript (removes all content and triggers events)
    /// </summary>
    /// <param name="selector">Input element selector</param>
    public async Task ClearJavaScriptAsync(string selector)
    {
        Logger.LogDebug("Clearing element using JavaScript: {Selector}", selector);
        
        try
        {
            await Page.EvaluateAsync($@"
                const element = document.querySelector('{selector}');
                if (!element) throw new Error('Element not found: {selector}');
                
                element.value = '';
                element.dispatchEvent(new Event('input', {{ bubbles: true }}));
                element.dispatchEvent(new Event('change', {{ bubbles: true }}));
            ");
            
            Logger.LogDebug("Successfully cleared element using JavaScript: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clear element using JavaScript: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Clears the value property of an element without triggering events (silent clear)
    /// </summary>
    /// <param name="selector">Element selector</param>
    public async Task ClearValueJavaScriptAsync(string selector)
    {
        Logger.LogDebug("Clearing element value using JavaScript (silent): {Selector}", selector);
        
        try
        {
            await Page.EvaluateAsync($@"
                const element = document.querySelector('{selector}');
                if (!element) throw new Error('Element not found: {selector}');
                
                element.value = '';
            ");
            
            Logger.LogDebug("Successfully cleared element value using JavaScript: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to clear element value using JavaScript: {Selector}", selector);
            throw;
        }
    }

    #endregion

    #region Keyboard and Mouse Actions

    /// <summary>
    /// Presses the Enter key on the currently focused element or page
    /// </summary>
    /// <param name="selector">Optional element selector to focus before pressing Enter</param>
    public async Task PressEnterAsync(string? selector = null)
    {
        Logger.LogDebug("Pressing Enter key{Selector}", selector != null ? $" on element: {selector}" : "");
        
        try
        {
            if (selector != null)
            {
                await Page.Locator(selector).PressAsync("Enter");
            }
            else
            {
                await Page.Keyboard.PressAsync("Enter");
            }
            
            Logger.LogDebug("Successfully pressed Enter key");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to press Enter key{Selector}", selector != null ? $" on element: {selector}" : "");
            throw;
        }
    }

    /// <summary>
    /// Presses the Tab key to move focus to the next element
    /// </summary>
    /// <param name="selector">Optional element selector to focus before pressing Tab</param>
    public async Task PressTabAsync(string? selector = null)
    {
        Logger.LogDebug("Pressing Tab key{Selector}", selector != null ? $" on element: {selector}" : "");
        
        try
        {
            if (selector != null)
            {
                await Page.Locator(selector).PressAsync("Tab");
            }
            else
            {
                await Page.Keyboard.PressAsync("Tab");
            }
            
            Logger.LogDebug("Successfully pressed Tab key");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to press Tab key{Selector}", selector != null ? $" on element: {selector}" : "");
            throw;
        }
    }

    /// <summary>
    /// Presses the Escape key to cancel operations or close dialogs
    /// </summary>
    /// <param name="selector">Optional element selector to focus before pressing Escape</param>
    public async Task PressEscapeAsync(string? selector = null)
    {
        Logger.LogDebug("Pressing Escape key{Selector}", selector != null ? $" on element: {selector}" : "");
        
        try
        {
            if (selector != null)
            {
                await Page.Locator(selector).PressAsync("Escape");
            }
            else
            {
                await Page.Keyboard.PressAsync("Escape");
            }
            
            Logger.LogDebug("Successfully pressed Escape key");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to press Escape key{Selector}", selector != null ? $" on element: {selector}" : "");
            throw;
        }
    }

    /// <summary>
    /// Presses Ctrl+A to select all content in the focused element or page
    /// </summary>
    /// <param name="selector">Optional element selector to focus before pressing Ctrl+A</param>
    public async Task PressCtrlAAsync(string? selector = null)
    {
        Logger.LogDebug("Pressing Ctrl+A (Select All){Selector}", selector != null ? $" on element: {selector}" : "");
        
        try
        {
            if (selector != null)
            {
                await Page.Locator(selector).PressAsync("Control+a");
            }
            else
            {
                await Page.Keyboard.PressAsync("Control+a");
            }
            
            Logger.LogDebug("Successfully pressed Ctrl+A");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to press Ctrl+A{Selector}", selector != null ? $" on element: {selector}" : "");
            throw;
        }
    }

    /// <summary>
    /// Types text slowly with a delay between each character for better reliability
    /// </summary>
    /// <param name="selector">Element selector to type into</param>
    /// <param name="text">Text to type</param>
    /// <param name="delayMs">Delay in milliseconds between each character (default: 100ms)</param>
    public async Task TypeTextSlowlyAsync(string selector, string text, int delayMs = 100)
    {
        Logger.LogDebug("Typing text slowly into element: {Selector} with {Delay}ms delay", selector, delayMs);
        
        try
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            
            // Focus the element first
            await element.FocusAsync();
            
            // Type each character with delay
            for (int i = 0; i < text.Length; i++)
            {
                await element.TypeAsync(text[i].ToString(), new LocatorTypeOptions { Delay = delayMs });
                
                // Additional delay between characters for very slow typing
                if (delayMs > 50)
                {
                    await Task.Delay(delayMs / 2);
                }
            }
            
            Logger.LogDebug("Successfully typed text slowly: {Length} characters", text.Length);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to type text slowly into element: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Right-clicks an element to open context menu
    /// </summary>
    /// <param name="selector">Element selector to right-click</param>
    /// <param name="options">Optional click options</param>
    public async Task RightClickAsync(string selector, LocatorClickOptions? options = null)
    {
        Logger.LogDebug("Right-clicking element: {Selector}", selector);
        
        try
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            
            var clickOptions = options ?? new LocatorClickOptions();
            clickOptions.Button = MouseButton.Right;
            
            await element.ClickAsync(clickOptions);
            
            Logger.LogDebug("Successfully right-clicked element: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to right-click element: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Hovers over an element with optional wait time
    /// </summary>
    /// <param name="selector">Element selector to hover over</param>
    /// <param name="waitAfterHoverMs">Time to wait after hovering (default: 500ms)</param>
    /// <param name="options">Optional hover options</param>
    public async Task HoverEnhancedAsync(string selector, int waitAfterHoverMs = 500, LocatorHoverOptions? options = null)
    {
        Logger.LogDebug("Hovering over element: {Selector}", selector);
        
        try
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            
            await element.HoverAsync(options);
            
            // Wait after hover to allow for hover effects
            if (waitAfterHoverMs > 0)
            {
                await Task.Delay(waitAfterHoverMs);
            }
            
            Logger.LogDebug("Successfully hovered over element: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to hover over element: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Double-clicks an element with enhanced reliability
    /// </summary>
    /// <param name="selector">Element selector to double-click</param>
    /// <param name="options">Optional double-click options</param>
    public async Task DoubleClickEnhancedAsync(string selector, LocatorDblClickOptions? options = null)
    {
        Logger.LogDebug("Double-clicking element: {Selector}", selector);
        
        try
        {
            var element = Page.Locator(selector);
            await element.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            
            await element.DblClickAsync(options);
            
            Logger.LogDebug("Successfully double-clicked element: {Selector}", selector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to double-click element: {Selector}", selector);
            throw;
        }
    }

    /// <summary>
    /// Drags an element from source to target location
    /// </summary>
    /// <param name="sourceSelector">Source element selector to drag from</param>
    /// <param name="targetSelector">Target element selector to drag to</param>
    /// <param name="options">Optional drag options</param>
    public async Task DragToAsync(string sourceSelector, string targetSelector, LocatorDragToOptions? options = null)
    {
        Logger.LogDebug("Dragging element from {Source} to {Target}", sourceSelector, targetSelector);
        
        try
        {
            var sourceElement = Page.Locator(sourceSelector);
            var targetElement = Page.Locator(targetSelector);
            
            // Wait for both elements to be visible
            await sourceElement.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            
            await targetElement.WaitForAsync(new LocatorWaitForOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = Config.Browser.TimeoutMs 
            });
            
            // Perform drag operation
            await sourceElement.DragToAsync(targetElement, options);
            
            Logger.LogDebug("Successfully dragged element from {Source} to {Target}", sourceSelector, targetSelector);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to drag element from {Source} to {Target}", sourceSelector, targetSelector);
            throw;
        }
    }

    /// <summary>
    /// Zooms in the page by the specified percentage
    /// </summary>
    /// <param name="percentage">Zoom percentage (e.g., 150 for 150%)</param>
    /// <param name="selector">Optional element selector to zoom relative to</param>
    public async Task ZoomInAsync(int percentage, string? selector = null)
    {
        Logger.LogDebug("Zooming in to {Percentage}%{Selector}", percentage, selector != null ? $" on element: {selector}" : "");
        
        try
        {
            var zoomLevel = percentage / 100.0;
            
            if (selector != null)
            {
                // Zoom relative to specific element
                await Page.EvaluateAsync($@"
                    const element = document.querySelector('{selector}');
                    if (element) {{
                        element.style.transform = 'scale({zoomLevel})';
                        element.style.transformOrigin = 'center center';
                    }}
                ");
            }
            else
            {
                // Zoom entire page
                await Page.EvaluateAsync($@"
                    document.body.style.zoom = '{zoomLevel}';
                ");
            }
            
            Logger.LogDebug("Successfully zoomed in to {Percentage}%", percentage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to zoom in to {Percentage}%", percentage);
            throw;
        }
    }

    /// <summary>
    /// Zooms out the page by the specified percentage
    /// </summary>
    /// <param name="percentage">Zoom percentage (e.g., 75 for 75%)</param>
    /// <param name="selector">Optional element selector to zoom relative to</param>
    public async Task ZoomOutAsync(int percentage, string? selector = null)
    {
        Logger.LogDebug("Zooming out to {Percentage}%{Selector}", percentage, selector != null ? $" on element: {selector}" : "");
        
        try
        {
            var zoomLevel = percentage / 100.0;
            
            if (selector != null)
            {
                // Zoom relative to specific element
                await Page.EvaluateAsync($@"
                    const element = document.querySelector('{selector}');
                    if (element) {{
                        element.style.transform = 'scale({zoomLevel})';
                        element.style.transformOrigin = 'center center';
                    }}
                ");
            }
            else
            {
                // Zoom entire page
                await Page.EvaluateAsync($@"
                    document.body.style.zoom = '{zoomLevel}';
                ");
            }
            
            Logger.LogDebug("Successfully zoomed out to {Percentage}%", percentage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to zoom out to {Percentage}%", percentage);
            throw;
        }
    }

    #endregion
} 