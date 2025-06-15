using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
using PlaywrightFramework.Core.Extensions;

namespace PlaywrightFramework.PageObjects.Pages;

/// <summary>
/// Page Object for Home page
/// </summary>
public class HomePage : FluentBasePage
{
    // Page elements
    private ILocator SideMenuButton => Locate("//button[@data-dyn-role='SideNavToggleButton']");
    
    // Components
    private SideMenuComponent _sideMenu;
    
    /// <summary>
    /// Side menu component for fluent navigation
    /// </summary>
    public SideMenuComponent SideMenu => _sideMenu ??= new SideMenuComponent(Page, Config, Logger, PageFactory);

    /// <summary>
    /// Constructor for HomePage
    /// </summary>
    public HomePage(IPage page, TestConfiguration config, ILogger<HomePage> logger, PageFactory pageFactory)
        : base(page, config, logger, pageFactory)
    {
    }

    /// <summary>
    /// Checks if the home page is loaded
    /// </summary>
    public async Task<bool> IsHomePageLoadedAsync()
    {
        return await Page.TitleAsync() == "Dashboard - Microsoft Dynamics 365";
    }
    
    /// <summary>
    /// Synchronous method to check if home page is loaded
    /// </summary>
    public bool IsHomePageLoaded()
    {
        return IsHomePageLoadedAsync().GetAwaiter().GetResult();
    }
}
