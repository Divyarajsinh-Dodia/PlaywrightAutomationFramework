using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
using PlaywrightFramework.Core.Extensions;

namespace PlaywrightFramework.PageObjects.Pages;

/// <summary>
/// Component class for the Side Menu
/// </summary>
public class SideMenuComponent
{
    private readonly IPage _page;
    private readonly TestConfiguration _config;
    private readonly ILogger _logger;
    private readonly PageFactory _pageFactory;
    
    // Locators
    private ILocator InventoryMovementMenuItem => _page.Locator("//span[contains(text(), 'Inventory movement')]");
    
    // Navigation properties
    private InventoryMovementPage _inventoryMovementPage;
    
    /// <summary>
    /// Access to InventoryMovementPage for fluent navigation
    /// </summary>
    public InventoryMovementPage InventoryMovementPage => 
        _inventoryMovementPage ??= _pageFactory.GetPage<InventoryMovementPage>();

    /// <summary>
    /// Constructor for SideMenuComponent
    /// </summary>
    public SideMenuComponent(IPage page, TestConfiguration config, ILogger logger, PageFactory pageFactory)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
    }
    
    /// <summary>
    /// Clicks on the side menu button
    /// </summary>
    public SideMenuComponent ClickOnSideMenu()
    {
        _logger.LogInformation("Clicking on side menu");
        _page.Locator("//button[@data-dyn-role='SideNavToggleButton']").ClickAsync().GetAwaiter().GetResult();
        _logger.LogInformation("Side menu clicked");
        return this;
    }
    
    /// <summary>
    /// Clicks on the Inventory Movement menu item
    /// </summary>
    public InventoryMovementPage ClickOnInventoryMovement()
    {
        _logger.LogInformation("Clicking on Inventory Movement menu item");
        InventoryMovementMenuItem.ClickAsync().GetAwaiter().GetResult();
        _logger.LogInformation("Inventory Movement menu item clicked");
        return InventoryMovementPage;
    }
}
