using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace PlaywrightFramework.Core.Extensions;

public static class LocatorActionExtensions
{
    public static async Task ClickAndWaitForNavigationAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;

        logger.LogDebug("Clicking element and waiting for navigation");
        await locator.ClickAsync();

        // Get the page from the locator
        var page = locator.Page;
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions
        {
            Timeout = timeout ?? config.Browser.NavigationTimeoutMs
        });

        logger.LogDebug("Navigation completed after click");
    }

    public static async Task ClearAndFillAsync(this ILocator locator, string text)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Clearing and filling element with new text: {Text}", text);
        await locator.ClearAsync();
        await locator.FillAsync(text);
        logger.LogDebug("Element cleared and filled");
    }

    public static async Task FillAndSubmitAsync(this ILocator locator, string text, ILocator submitElement)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Filling element and submitting form");
        await locator.FillAsync(text);
        await submitElement.ClickAsync();
        logger.LogDebug("Form filled and submitted");
    }

    public static async Task ClickIfVisibleAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;

        if (await locator.IsVisibleAsync())
        {
            await locator.ClickAsync();
            logger.LogDebug("Element was visible and clicked");
        }
        else
        {
            logger.LogDebug("Element not visible, skipping click");
        }
    }

    public static async Task ClickIfEnabledAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;

        if (await locator.IsEnabledAsync())
        {
            await locator.ClickAsync();
            logger.LogDebug("Element was enabled and clicked");
        }
        else
        {
            logger.LogDebug("Element not enabled, skipping click");
        }
    }

    public static async Task<bool> TryClickAsync(this ILocator locator, int timeoutMs = 5000)
    {
        var logger = LocatorContext.CurrentLogger;

        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
            await locator.ClickAsync();
            logger.LogDebug("Element found and clicked successfully");
            return true;
        }
        catch (TimeoutException)
        {
            logger.LogDebug("Element not found within timeout, click skipped");
            return false;
        }
    }

    public static async Task<bool> TryFillAsync(this ILocator locator, string text, int timeoutMs = 5000)
    {
        var logger = LocatorContext.CurrentLogger;

        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
            await locator.FillAsync(text);
            logger.LogDebug("Element found and filled successfully");
            return true;
        }
        catch (TimeoutException)
        {
            logger.LogDebug("Element not found within timeout, fill skipped");
            return false;
        }
    }

    public static async Task SelectOptionByIndexAsync(this ILocator locator, int index)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Selecting option by index: {Index}", index);
        var options = await locator.Locator("option").AllAsync();
        if (index >= 0 && index < options.Count)
        {
            var optionValue = await options[index].GetAttributeAsync("value");
            if (optionValue != null)
            {
                await locator.SelectOptionAsync(optionValue);
                logger.LogDebug("Selected option by index: {Index}", index);
            }
            else
            {
                throw new InvalidOperationException($"Option at index {index} has no value attribute");
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} is out of range. Available options: {options.Count}");
        }
    }

    public static async Task DoubleClickAndWaitAsync(this ILocator locator, int waitMs = 1000)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Double clicking element and waiting {WaitMs}ms", waitMs);
        await locator.DoubleClickAsync();
        await Task.Delay(waitMs);
        logger.LogDebug("Double click completed with wait");
    }

    public static async Task HoverAndClickAsync(this ILocator locator, ILocator targetElement)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Hovering over element and clicking target");
        await locator.HoverAsync();
        await targetElement.ClickAsync();
        logger.LogDebug("Hover and click sequence completed");
    }

    public static async Task ScrollToAndClickAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Scrolling to element and clicking");
        await locator.ScrollIntoViewAsync();
        await locator.ClickAsync();
        logger.LogDebug("Scroll and click completed");
    }

    public static async Task FocusAndTypeAsync(this ILocator locator, string text)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Focusing element and typing text: {Text}", text);
        await locator.FocusAsync();
        await locator.TypeAsync(text);
        logger.LogDebug("Focus and type completed");
    }

    public static async Task PressKeySequenceAsync(this ILocator locator, params string[] keys)
    {
        var logger = LocatorContext.CurrentLogger;

        logger.LogDebug("Pressing key sequence: {Keys}", string.Join(", ", keys));
        await locator.FocusAsync();

        foreach (var key in keys)
        {
            await locator.PressAsync(key);
        }

        logger.LogDebug("Key sequence completed");
    }
    
    /// <summary>
    /// Highlights an element with a simple colored border for a specified duration.
    /// </summary>
    /// <param name="locator">The element locator to highlight</param>
    /// <param name="color">The color of the highlight (default: "yellow")</param>
    /// <param name="durationMs">Duration in milliseconds before the highlight is removed (default: 1000ms)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task HighlightElementAsync(this ILocator locator, string color = "yellow", int durationMs = 1000)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Highlighting element with color: {Color} for {Duration}ms", color, durationMs);
        
        // Get the underlying page
        var page = locator.Page;
        
        // Apply a simple border highlight using JavaScript
        await locator.EvaluateAsync(@$"
            element => {{
                const originalStyle = element.style.cssText;
                element.style.border = '2px solid {color}';
                
                setTimeout(() => {{
                    element.style.cssText = originalStyle;
                }}, {durationMs});
            }}
        ");
        
        logger.LogDebug("Element highlighted, will be removed automatically after timeout");
    }
    
//     /// <summary>
// /// Clicks only the first visible element that matches the locator.
// /// Useful when multiple elements match the same selector but only some are visible.
// /// </summary>
// public static async Task<bool> ClickOnlyVisibleAsync(this ILocator locator)
// {
//     var logger = LocatorContext.CurrentLogger;
    
//     logger.LogDebug("Attempting to click only visible element from potential multiple matches");
    
//     // Get all matching elements
//     var elements = await locator.AllAsync();
    
//     // Iterate through elements to find the first visible one
//     for (int i = 0; i < elements.Count; i++)
//     {
//         var elementLocator = locator.Nth(i);
//         if (await elementLocator.IsVisibleAsync())
//         {
//             logger.LogDebug("Found visible element at index {Index}, clicking it", i);
//             await elementLocator.ClickAsync();
//             return true;
//         }
//     }
    
//     logger.LogWarning("No visible elements found to click");
//     return false;
// }

// /// <summary>
// /// Fills only the first visible input element that matches the locator.
// /// Useful when multiple elements match the same selector but only some are visible.
// /// </summary>
// public static async Task<bool> FillOnlyVisibleAsync(this ILocator locator, string text)
// {
//     var logger = LocatorContext.CurrentLogger;
    
//     logger.LogDebug("Attempting to fill only visible element from potential multiple matches with text: {Text}", text);
    
//     // Get all matching elements
//     var elements = await locator.AllAsync();
    
//     // Iterate through elements to find the first visible one
//     for (int i = 0; i < elements.Count; i++)
//     {
//         var elementLocator = locator.Nth(i);
//         if (await elementLocator.IsVisibleAsync())
//         {
//             logger.LogDebug("Found visible element at index {Index}, filling it", i);
//             await elementLocator.FillAsync(text);
//             return true;
//         }
//     }
    
//     logger.LogWarning("No visible elements found to fill");
//     return false;
// }

    /// <summary>
    /// Clicks only the first visible element that matches the locator with advanced options.
    /// Useful when multiple elements match the same selector but only some are visible.
    /// </summary>
    /// <param name="locator">The locator to find elements</param>
    /// <param name="timeout">Optional timeout in milliseconds for waiting for elements</param>
    /// <param name="strictVisibility">If true, ensures element is not only visible but also in viewport</param>
    /// <param name="clickOptions">Optional click options for the Playwright click action</param>
    /// <returns>True if a visible element was found and clicked, false otherwise</returns>
    public static async Task<bool> ClickOnlyVisibleAsync(
        this ILocator locator,
        int? timeout = null,
        bool strictVisibility = false,
        LocatorClickOptions? clickOptions = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;

        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Attempting to click only visible element from potential multiple matches (timeout: {Timeout}ms)", timeoutMs);

        try
        {
            // Use Playwright's filter capability instead of getting all elements and iterating
            // This is more efficient as it leverages Playwright's internal optimizations
            var visibleLocator = strictVisibility
                ? locator.Filter(new() { Has = locator.Page.Locator(":visible:not(:hidden):in-viewport") })
                : locator.Filter(new() { Has = locator.Page.Locator(":visible:not(:hidden)") });

            // First check count to avoid unnecessary operations
            var count = await visibleLocator.CountAsync();
            if (count == 0)
            {
                logger.LogWarning("No visible elements found to click");
                return false;
            }

            // Take first visible element
            var firstVisible = visibleLocator.First;

            // Ensure element is in scroll view (if it's strict visibility mode, this is already done)
            if (!strictVisibility)
            {
                await firstVisible.ScrollIntoViewIfNeededAsync();
            }

            // Add highlight for visual debugging if enabled
            await firstVisible.HighlightAsync();

            // Click with timeout option
            await firstVisible.ClickAsync(new LocatorClickOptions
            {
                // Merge provided options with timeout
                Button = clickOptions?.Button ?? MouseButton.Left,
                ClickCount = clickOptions?.ClickCount ?? 1,
                Delay = clickOptions?.Delay,
                Force = clickOptions?.Force ?? false,
                Modifiers = clickOptions?.Modifiers,
                Position = clickOptions?.Position,
                Timeout = clickOptions?.Timeout ?? timeoutMs,
                Trial = clickOptions?.Trial ?? false
            });

            logger.LogDebug("Successfully clicked visible element");
            return true;
        }
        catch (Exception ex) when (ex is TimeoutException || ex is PlaywrightException)
        {
            logger.LogWarning(ex, "Failed to click visible element: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Fills only the first visible input element that matches the locator with advanced options.
    /// Useful when multiple elements match the same selector but only some are visible.
    /// </summary>
    /// <param name="locator">The locator to find elements</param>
    /// <param name="text">The text to fill in the element</param>
    /// <param name="timeout">Optional timeout in milliseconds for waiting for elements</param>
    /// <param name="strictVisibility">If true, ensures element is not only visible but also in viewport</param>
    /// <param name="fillOptions">Optional fill options for the Playwright fill action</param>
    /// <returns>True if a visible element was found and filled, false otherwise</returns>
    public static async Task<bool> FillOnlyVisibleAsync(
        this ILocator locator, 
        string text,
        int? timeout = null,
        bool strictVisibility = false,
        LocatorFillOptions? fillOptions = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Attempting to fill only visible element with text: {Text} (timeout: {Timeout}ms)", text, timeoutMs);
        
        try
        {
            // First check if element is also editable
            var visibleLocator = strictVisibility
                ? locator.Filter(new() { Has = locator.Page.Locator(":visible:not(:hidden):not(:disabled):in-viewport") })
                : locator.Filter(new() { Has = locator.Page.Locator(":visible:not(:hidden):not(:disabled)") });
            
            // First check count to avoid unnecessary operations
            var count = await visibleLocator.CountAsync();
            if (count == 0)
            {
                logger.LogWarning("No visible and editable elements found to fill");
                return false;
            }
            
            // Take first visible element
            var firstVisible = visibleLocator.First;
            
            // Ensure element is in scroll view (if it's strict visibility mode, this is already done)
            if (!strictVisibility)
            {
                await firstVisible.ScrollIntoViewIfNeededAsync();
            }
            
            // Add highlight for visual debugging if enabled
            await firstVisible.HighlightAsync();
            
            // Verify element is actually editable
            if (!await firstVisible.IsEditableAsync())
            {
                logger.LogWarning("Element is visible but not editable");
                return false;
            }
            
            // Clear existing value first (this is more reliable than relying on fill to clear)
            await firstVisible.ClearAsync();
            
            // Fill with provided options
            await firstVisible.FillAsync(text, new LocatorFillOptions
            {
                Force = fillOptions?.Force ?? false,
                NoWaitAfter = fillOptions?.NoWaitAfter ?? false,
                Timeout = fillOptions?.Timeout ?? timeoutMs,
            });
            
            logger.LogDebug("Successfully filled visible element with text: {Text}", text);
            return true;
        }
        catch (Exception ex) when (ex is TimeoutException || ex is PlaywrightException)
        {
            logger.LogWarning(ex, "Failed to fill visible element: {ErrorMessage}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Gets the first visible element that matches the locator with advanced visibility options.
    /// </summary>
    /// <param name="locator">The locator to find elements</param>
    /// <param name="timeout">Optional timeout in milliseconds for waiting for elements</param>
    /// <param name="strictVisibility">If true, ensures element is not only visible but also in viewport</param>
    /// <param name="waitForVisible">If true, waits for element to be visible within timeout</param>
    /// <returns>The first visible locator, or null if none found</returns>
    public static async Task<ILocator?> GetFirstVisibleLocatorAsync(
        this ILocator locator,
        int? timeout = null,
        bool strictVisibility = false,
        bool waitForVisible = false)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Finding first visible element (timeout: {Timeout}ms, strictVisibility: {StrictVisibility})",
            timeoutMs, strictVisibility);
        
        try
        {
            // If we need to wait for visibility
            if (waitForVisible)
            {
                try
                {
                    await locator.WaitForAsync(new LocatorWaitForOptions 
                    { 
                        State = WaitForSelectorState.Visible,
                        Timeout = timeoutMs
                    });
                }
                catch (TimeoutException)
                {
                    logger.LogDebug("No elements became visible within timeout");
                    return null;
                }
            }
            
            // Optimized approach using Playwright's filter
            var visibleLocator = strictVisibility
                ? locator.Filter(new() { Has = locator.Page.Locator(":visible:not(:hidden):in-viewport") })
                : locator.Filter(new() { Has = locator.Page.Locator(":visible:not(:hidden)") });
            
            // Check if any elements match
            var count = await visibleLocator.CountAsync();
            if (count == 0)
            {
                logger.LogDebug("No visible elements found");
                return null;
            }
            
            logger.LogDebug("Found {Count} visible elements, returning first one", count);
            return visibleLocator.First;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error finding visible element: {ErrorMessage}", ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Performs an action on the first visible element that matches the locator.
    /// This is a generic method that can be used for any action on a visible element.
    /// </summary>
    /// <param name="locator">The locator to find elements</param>
    /// <param name="action">The action to perform on the first visible element</param>
    /// <param name="timeout">Optional timeout in milliseconds</param>
    /// <param name="strictVisibility">If true, ensures element is not only visible but also in viewport</param>
    /// <returns>True if action was performed on a visible element, false otherwise</returns>
    public static async Task<bool> WithFirstVisibleAsync(
        this ILocator locator,
        Func<ILocator, Task> action,
        int? timeout = null,
        bool strictVisibility = false)
    {
        var logger = LocatorContext.CurrentLogger;
        
        var firstVisible = await locator.GetFirstVisibleLocatorAsync(timeout, strictVisibility);
        if (firstVisible == null)
        {
            return false;
        }
        
        try
        {
            await action(firstVisible);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to perform action on visible element: {ErrorMessage}", ex.Message);
            return false;
        }
    }
}