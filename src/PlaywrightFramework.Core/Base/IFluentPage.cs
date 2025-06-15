using Microsoft.Playwright;

namespace PlaywrightFramework.Core.Base;

/// <summary>
/// Base interface for all fluent page objects
/// Provides a marker interface for pages that support fluent method chaining
/// </summary>
public interface IFluentPage
{
    /// <summary>
    /// Gets the current page URL
    /// </summary>
    string CurrentUrl { get; }
    
    /// <summary>
    /// Gets the underlying Playwright IPage instance
    /// </summary>
    IPage Page { get; }
}
