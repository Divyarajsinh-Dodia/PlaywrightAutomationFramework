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
} 