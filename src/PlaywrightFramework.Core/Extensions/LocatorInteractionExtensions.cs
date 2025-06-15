using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace PlaywrightFramework.Core.Extensions;

public static class LocatorInteractionExtensions
{
    public static async Task ClickAsync(this ILocator locator, LocatorClickOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Clicking element with locator");
        await locator.HighlightAsync();
        await locator.ClickAsync(options);
        logger.LogDebug("Clicked element with locator");
    }

    public static async Task DoubleClickAsync(this ILocator locator, LocatorDblClickOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Double clicking element with locator");
        await locator.HighlightAsync();
        await locator.DblClickAsync(options);
        logger.LogDebug("Double clicked element with locator");
    }

    public static async Task RightClickAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Right clicking element with locator");
        await locator.HighlightAsync();
        await locator.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
        logger.LogDebug("Right clicked element with locator");
    }

    public static async Task FillAsync(this ILocator locator, string text, LocatorFillOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Filling element with text: {Text}", text);
        await locator.HighlightAsync();
        await locator.FillAsync(text, options);
        logger.LogDebug("Filled element with text");
    }

    public static async Task TypeAsync(this ILocator locator, string text, LocatorTypeOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Typing text into element: {Text}", text);
        await locator.HighlightAsync();
        await locator.TypeAsync(text, options);
        logger.LogDebug("Typed text into element");
    }

    public static async Task ClearAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Clearing element");
        await locator.FillAsync("");
        logger.LogDebug("Cleared element");
    }

    public static async Task HoverAsync(this ILocator locator, LocatorHoverOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Hovering over element");
        await locator.HighlightAsync();
        await locator.HoverAsync(options);
        logger.LogDebug("Hovered over element");
    }

    public static async Task ScrollIntoViewAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Scrolling element into view");
        await locator.ScrollIntoViewIfNeededAsync();
        logger.LogDebug("Scrolled element into view");
    }

    public static async Task SelectOptionAsync(this ILocator locator, string value, LocatorSelectOptionOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Selecting option by value: {Value}", value);
        await locator.SelectOptionAsync(value, options);
        logger.LogDebug("Selected option by value: {Value}", value);
    }

    public static async Task SelectOptionByTextAsync(this ILocator locator, string text, LocatorSelectOptionOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Selecting option by text: {Text}", text);
        await locator.SelectOptionAsync(new SelectOptionValue { Label = text }, options);
        logger.LogDebug("Selected option by text: {Text}", text);
    }

    public static async Task CheckAsync(this ILocator locator, LocatorCheckOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking element");
        await locator.HighlightAsync();
        await locator.CheckAsync(options);
        logger.LogDebug("Checked element");
    }

    public static async Task UncheckAsync(this ILocator locator, LocatorUncheckOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Unchecking element");
        await locator.HighlightAsync();
        await locator.UncheckAsync(options);
        logger.LogDebug("Unchecked element");
    }

    public static async Task SetCheckedAsync(this ILocator locator, bool isChecked, LocatorSetCheckedOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Setting element checked state to: {IsChecked}", isChecked);
        await locator.HighlightAsync();
        await locator.SetCheckedAsync(isChecked, options);
        logger.LogDebug("Set element checked state to: {IsChecked}", isChecked);
    }

    // Enhanced highlighting with configuration
    public static async Task HighlightAsync(this ILocator locator)
    {
        var config = LocatorContext.CurrentConfig;
        var logger = LocatorContext.CurrentLogger;
        
        if (!config.Execution.HighlightElements) return;
        
        try
        {
            // Use Playwright's built-in highlight method
            await locator.HighlightAsync();
            
            // Add custom delay if configured
            if (config.Execution.HighlightDurationMs > 0)
            {
                await Task.Delay(config.Execution.HighlightDurationMs);
            }
            
            logger.LogDebug("Highlighted element for {Duration}ms", config.Execution.HighlightDurationMs);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Failed to highlight element");
        }
    }
} 