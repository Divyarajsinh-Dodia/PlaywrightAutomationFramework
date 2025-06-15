using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace PlaywrightFramework.Core.Extensions;

public static class LocatorQueryExtensions
{
    public static async Task<string?> GetTextAsync(this ILocator locator, LocatorTextContentOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Getting text content from element");
        var text = await locator.TextContentAsync(options);
        logger.LogDebug("Got text content from element: {Text}", text);
        return text;
    }

    public static async Task<string> GetInnerTextAsync(this ILocator locator, LocatorInnerTextOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Getting inner text from element");
        var text = await locator.InnerTextAsync(options);
        logger.LogDebug("Got inner text from element: {Text}", text);
        return text;
    }

    public static async Task<string> GetInnerHTMLAsync(this ILocator locator, LocatorInnerHTMLOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Getting inner HTML from element");
        var html = await locator.InnerHTMLAsync(options);
        logger.LogDebug("Got inner HTML from element");
        return html;
    }

    public static async Task<string?> GetAttributeAsync(this ILocator locator, string attributeName, LocatorGetAttributeOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Getting attribute '{AttributeName}' from element", attributeName);
        var value = await locator.GetAttributeAsync(attributeName, options);
        logger.LogDebug("Got attribute '{AttributeName}' from element: {Value}", attributeName, value);
        return value;
    }

    public static async Task<bool> IsVisibleAsync(this ILocator locator, LocatorIsVisibleOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking if element is visible");
        var isVisible = await locator.IsVisibleAsync(options);
        logger.LogDebug("Element visibility: {IsVisible}", isVisible);
        return isVisible;
    }

    public static async Task<bool> IsEnabledAsync(this ILocator locator, LocatorIsEnabledOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking if element is enabled");
        var isEnabled = await locator.IsEnabledAsync(options);
        logger.LogDebug("Element enabled: {IsEnabled}", isEnabled);
        return isEnabled;
    }

    public static async Task<bool> IsCheckedAsync(this ILocator locator, LocatorIsCheckedOptions? options = null)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking if element is checked");
        var isChecked = await locator.IsCheckedAsync(options);
        logger.LogDebug("Element checked: {IsChecked}", isChecked);
        return isChecked;
    }

    public static async Task<bool> IsDisabledAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking if element is disabled");
        var isDisabled = await locator.IsDisabledAsync();
        logger.LogDebug("Element disabled: {IsDisabled}", isDisabled);
        return isDisabled;
    }

    public static async Task<bool> IsEditableAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking if element is editable");
        var isEditable = await locator.IsEditableAsync();
        logger.LogDebug("Element editable: {IsEditable}", isEditable);
        return isEditable;
    }

    public static async Task<bool> IsHiddenAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Checking if element is hidden");
        var isHidden = await locator.IsHiddenAsync();
        logger.LogDebug("Element hidden: {IsHidden}", isHidden);
        return isHidden;
    }

    public static async Task<int> CountAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        
        logger.LogDebug("Getting element count");
        var count = await locator.CountAsync();
        logger.LogDebug("Element count: {Count}", count);
        return count;
    }

    // Convenience methods for common attributes
    public static async Task<string?> GetValueAsync(this ILocator locator)
    {
        return await locator.GetAttributeAsync("value");
    }

    public static async Task<string?> GetPlaceholderAsync(this ILocator locator)
    {
        return await locator.GetAttributeAsync("placeholder");
    }

    public static async Task<string?> GetClassAsync(this ILocator locator)
    {
        return await locator.GetAttributeAsync("class");
    }

    public static async Task<string?> GetIdAsync(this ILocator locator)
    {
        return await locator.GetAttributeAsync("id");
    }

    public static async Task<string?> GetNameAsync(this ILocator locator)
    {
        return await locator.GetAttributeAsync("name");
    }

    public static async Task<string?> GetTypeAsync(this ILocator locator)
    {
        return await locator.GetAttributeAsync("type");
    }

    // Methods for collections
    public static async Task<List<string>> GetAllTextsAsync(this ILocator locator)
    {
        var logger = LocatorContext.CurrentLogger;
        var texts = new List<string>();
        var count = await locator.CountAsync();
        
        logger.LogDebug("Getting all texts from {Count} elements", count);
        
        for (int i = 0; i < count; i++)
        {
            var text = await locator.Nth(i).GetTextAsync();
            if (!string.IsNullOrEmpty(text))
            {
                texts.Add(text);
            }
        }
        
        logger.LogDebug("Got {Count} text values from elements", texts.Count);
        return texts;
    }

    public static async Task<List<string?>> GetAllAttributesAsync(this ILocator locator, string attributeName)
    {
        var logger = LocatorContext.CurrentLogger;
        var attributes = new List<string?>();
        var count = await locator.CountAsync();
        
        logger.LogDebug("Getting all '{AttributeName}' attributes from {Count} elements", attributeName, count);
        
        for (int i = 0; i < count; i++)
        {
            var attribute = await locator.Nth(i).GetAttributeAsync(attributeName);
            attributes.Add(attribute);
        }
        
        logger.LogDebug("Got {Count} '{AttributeName}' attributes from elements", attributes.Count, attributeName);
        return attributes;
    }
} 