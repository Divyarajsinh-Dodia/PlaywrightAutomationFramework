using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
using PlaywrightFramework.Core.Extensions;

namespace PlaywrightFramework.PageObjects.Pages;

/// <summary>
/// Page Object for Inventory Movement page
/// </summary>
public class InventoryMovementPage : FluentBasePage
{
    // Locators
    private ILocator NewButton => Locate("//span[contains(text(), 'New')]");
    
    /// <summary>
    /// Constructor for InventoryMovementPage
    /// </summary>
    public InventoryMovementPage(IPage page, TestConfiguration config, ILogger<InventoryMovementPage> logger, PageFactory pageFactory)
        : base(page, config, logger, pageFactory)
    {
    }
    
    /// <summary>
    /// Clicks on the New button
    /// </summary>
    public InventoryMovementPage ClickOnNewButton()
    {
        Logger.LogInformation("Clicking on New button");
        NewButton.ClickAsync().GetAwaiter().GetResult();
        Logger.LogInformation("New button clicked");
        WaitForLoad(WaitUntilState.NetworkIdle);
        return this;
    }
}
