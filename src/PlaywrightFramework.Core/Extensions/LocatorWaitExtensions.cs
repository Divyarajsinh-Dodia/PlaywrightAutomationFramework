using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace PlaywrightFramework.Core.Extensions;

public static class LocatorWaitExtensions
{
    public static async Task WaitForAsync(this ILocator locator, LocatorWaitForOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeout = options?.Timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element (timeout: {Timeout}ms)", timeout);
        await locator.WaitForAsync(options);
        logger.LogDebug("Element found and visible");
    }

    public static async Task WaitForHiddenAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element to be hidden (timeout: {Timeout}ms)", timeoutMs);
        await locator.WaitForAsync(new LocatorWaitForOptions 
        { 
            State = WaitForSelectorState.Hidden, 
            Timeout = timeoutMs 
        });
        logger.LogDebug("Element is hidden");
    }

    public static async Task WaitForAttachedAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element to be attached (timeout: {Timeout}ms)", timeoutMs);
        await locator.WaitForAsync(new LocatorWaitForOptions 
        { 
            State = WaitForSelectorState.Attached, 
            Timeout = timeoutMs 
        });
        logger.LogDebug("Element is attached");
    }

    public static async Task WaitForDetachedAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element to be detached (timeout: {Timeout}ms)", timeoutMs);
        await locator.WaitForAsync(new LocatorWaitForOptions 
        { 
            State = WaitForSelectorState.Detached, 
            Timeout = timeoutMs 
        });
        logger.LogDebug("Element is detached");
    }

    public static async Task WaitForTextAsync(this ILocator locator, string expectedText, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for text in element: '{ExpectedText}' (timeout: {Timeout}ms)", expectedText, timeoutMs);
        
        await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
        
        // Wait for specific text content
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            var currentText = await locator.TextContentAsync();
            if (currentText != null && currentText.Contains(expectedText))
            {
                logger.LogDebug("Text found in element: '{ExpectedText}'", expectedText);
                return;
            }
            await Task.Delay(100);
        }
        
        throw new TimeoutException($"Text '{expectedText}' not found in element within {timeoutMs}ms");
    }

    public static async Task WaitForAttributeAsync(this ILocator locator, string attributeName, string expectedValue, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for attribute in element: '{AttributeName}'='{ExpectedValue}' (timeout: {Timeout}ms)", 
            attributeName, expectedValue, timeoutMs);
        
        await locator.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
        
        // Wait for specific attribute value
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            var currentValue = await locator.GetAttributeAsync(attributeName);
            if (currentValue == expectedValue)
            {
                logger.LogDebug("Attribute value found: '{AttributeName}'='{ExpectedValue}'", attributeName, expectedValue);
                return;
            }
            await Task.Delay(100);
        }
        
        throw new TimeoutException($"Attribute '{attributeName}={expectedValue}' not found in element within {timeoutMs}ms");
    }

    public static async Task WaitToBeEnabledAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element to be enabled (timeout: {Timeout}ms)", timeoutMs);
        
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            if (await locator.IsEnabledAsync())
            {
                logger.LogDebug("Element is now enabled");
                return;
            }
            await Task.Delay(100);
        }
        
        throw new TimeoutException($"Element not enabled within {timeoutMs}ms");
    }

    public static async Task WaitToBeDisabledAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element to be disabled (timeout: {Timeout}ms)", timeoutMs);
        
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            if (await locator.IsDisabledAsync())
            {
                logger.LogDebug("Element is now disabled");
                return;
            }
            await Task.Delay(100);
        }
        
        throw new TimeoutException($"Element not disabled within {timeoutMs}ms");
    }

    public static async Task WaitToBeEditableAsync(this ILocator locator, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element to be editable (timeout: {Timeout}ms)", timeoutMs);
        
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            if (await locator.IsEditableAsync())
            {
                logger.LogDebug("Element is now editable");
                return;
            }
            await Task.Delay(100);
        }
        
        throw new TimeoutException($"Element not editable within {timeoutMs}ms");
    }

    public static async Task WaitForCountAsync(this ILocator locator, int expectedCount, int? timeout = null)
    {
        var logger = LocatorContext.CurrentLogger;
        var config = LocatorContext.CurrentConfig;
        
        var timeoutMs = timeout ?? config.Browser.TimeoutMs;
        logger.LogDebug("Waiting for element count to be {ExpectedCount} (timeout: {Timeout}ms)", expectedCount, timeoutMs);
        
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(timeoutMs))
        {
            var currentCount = await locator.CountAsync();
            if (currentCount == expectedCount)
            {
                logger.LogDebug("Element count is now {ExpectedCount}", expectedCount);
                return;
            }
            await Task.Delay(100);
        }
        
        var finalCount = await locator.CountAsync();
        throw new TimeoutException($"Element count not {expectedCount} within {timeoutMs}ms. Current count: {finalCount}");
    }
} 