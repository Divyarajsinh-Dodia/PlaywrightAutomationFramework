using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
using Allure.NUnit.Attributes;

namespace PlaywrightFramework.PageObjects.Pages;

/// <summary>
/// Page Object for Dashboard page demonstrating enterprise-level page object patterns
/// </summary>
public class DashboardPage : BasePage
{
    // Page URL
    private const string PageUrl = "/dashboard";
    
    // Main navigation locators
    private const string UserMenuSelector = "[data-testid='user-menu']";
    private const string LogoutButtonSelector = "[data-testid='logout-button']";
    private const string ProfileLinkSelector = "[data-testid='profile-link']";
    private const string SettingsLinkSelector = "[data-testid='settings-link']";
    
    // Dashboard content locators
    private const string WelcomeMessageSelector = "[data-testid='welcome-message']";
    private const string NotificationBellSelector = "[data-testid='notification-bell']";
    private const string NotificationCountSelector = "[data-testid='notification-count']";
    private const string SearchBoxSelector = "[data-testid='search-box']";
    private const string SearchButtonSelector = "[data-testid='search-button']";
    
    // Dashboard widgets
    private const string RecentActivityWidgetSelector = "[data-testid='recent-activity-widget']";
    private const string StatisticsWidgetSelector = "[data-testid='statistics-widget']";
    private const string QuickActionsWidgetSelector = "[data-testid='quick-actions-widget']";
    
    // Side navigation
    private const string SidebarSelector = "[data-testid='sidebar']";
    private const string SidebarToggleSelector = "[data-testid='sidebar-toggle']";
    private const string NavigationItemSelector = "[data-testid='nav-item']";
    
    // Loading states
    private const string LoadingSpinnerSelector = "[data-testid='loading-spinner']";
    private const string ContentLoadedSelector = "[data-testid='dashboard-content']";

    public DashboardPage(IPage page, TestConfiguration config, ILogger<DashboardPage> logger) 
        : base(page, config, logger)
    {
    }

    /// <summary>
    /// Navigates to the dashboard page
    /// </summary>
    [AllureStep("Navigate to dashboard page")]
    public async Task<DashboardPage> NavigateToDashboardAsync()
    {
        //await NavigateToAsync(PageUrl);
        await WaitForDashboardToLoadAsync();
        Logger.LogInformation("Navigated to dashboard page");
        return this;
    }

    /// <summary>
    /// Waits for the dashboard to fully load
    /// </summary>
    [AllureStep("Wait for dashboard to load")]
    public async Task<DashboardPage> WaitForDashboardToLoadAsync()
    {
        // Wait for loading spinner to disappear
        try
        {
            if (await IsVisibleAsync(LoadingSpinnerSelector))
            {
                await WaitForElementToBeHiddenAsync(LoadingSpinnerSelector);
            }
        }
        catch (Exception ex)
        {
            Logger.LogDebug("No loading spinner found: {Error}", ex.Message);
        }

        // Wait for main content to be visible
        await WaitForElementAsync(ContentLoadedSelector);
        Logger.LogDebug("Dashboard loaded successfully");
        return this;
    }

    /// <summary>
    /// Gets the welcome message text
    /// </summary>
    [AllureStep("Get welcome message")]
    public async Task<string> GetWelcomeMessageAsync()
    {
        var welcomeText = await GetTextAsync(WelcomeMessageSelector) ?? string.Empty;
        Logger.LogDebug("Welcome message: {WelcomeMessage}", welcomeText);
        return welcomeText;
    }

    /// <summary>
    /// Clicks on the user menu
    /// </summary>
    [AllureStep("Click user menu")]
    public async Task<DashboardPage> ClickUserMenuAsync()
    {
        await ClickAsync(UserMenuSelector);
        Logger.LogDebug("Clicked user menu");
        return this;
    }

    /// <summary>
    /// Logs out from the application
    /// </summary>
    [AllureStep("Logout from application")]
    public async Task<DashboardPage> LogoutAsync()
    {
        await ClickUserMenuAsync();
        await ClickAsync(LogoutButtonSelector);
        Logger.LogInformation("Logged out from application");
        return this;
    }

    /// <summary>
    /// Navigates to user profile
    /// </summary>
    [AllureStep("Navigate to user profile")]
    public async Task<DashboardPage> GoToProfileAsync()
    {
        await ClickUserMenuAsync();
        await ClickAsync(ProfileLinkSelector);
        Logger.LogDebug("Navigated to user profile");
        return this;
    }

    /// <summary>
    /// Navigates to settings page
    /// </summary>
    [AllureStep("Navigate to settings")]
    public async Task<DashboardPage> GoToSettingsAsync()
    {
        await ClickUserMenuAsync();
        await ClickAsync(SettingsLinkSelector);
        Logger.LogDebug("Navigated to settings");
        return this;
    }

    /// <summary>
    /// Performs a search using the search box
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    [AllureStep("Search for: {searchTerm}")]
    public async Task<DashboardPage> SearchAsync(string searchTerm)
    {
        await FillAsync(SearchBoxSelector, searchTerm);
        await ClickAsync(SearchButtonSelector);
        Logger.LogInformation("Performed search for: {SearchTerm}", searchTerm);
        return this;
    }

    /// <summary>
    /// Gets the notification count
    /// </summary>
    [AllureStep("Get notification count")]
    public async Task<int> GetNotificationCountAsync()
    {
        if (!await IsVisibleAsync(NotificationCountSelector))
        {
            Logger.LogDebug("No notification count visible");
            return 0;
        }

        var countText = await GetTextAsync(NotificationCountSelector) ?? "0";
        if (int.TryParse(countText, out var count))
        {
            Logger.LogDebug("Notification count: {Count}", count);
            return count;
        }

        Logger.LogWarning("Could not parse notification count: {CountText}", countText);
        return 0;
    }

    /// <summary>
    /// Clicks on the notification bell
    /// </summary>
    [AllureStep("Click notification bell")]
    public async Task<DashboardPage> ClickNotificationBellAsync()
    {
        await ClickAsync(NotificationBellSelector);
        Logger.LogDebug("Clicked notification bell");
        return this;
    }

    /// <summary>
    /// Toggles the sidebar visibility
    /// </summary>
    [AllureStep("Toggle sidebar")]
    public async Task<DashboardPage> ToggleSidebarAsync()
    {
        await ClickAsync(SidebarToggleSelector);
        Logger.LogDebug("Toggled sidebar");
        return this;
    }

    /// <summary>
    /// Checks if the sidebar is visible
    /// </summary>
    [AllureStep("Check if sidebar is visible")]
    public async Task<bool> IsSidebarVisibleAsync()
    {
        var isVisible = await IsVisibleAsync(SidebarSelector);
        Logger.LogDebug("Sidebar visible: {IsVisible}", isVisible);
        return isVisible;
    }

    /// <summary>
    /// Gets all navigation items from the sidebar
    /// </summary>
    [AllureStep("Get navigation items")]
    public async Task<List<string>> GetNavigationItemsAsync()
    {
        var navItems = new List<string>();
        var elements = GetElements(NavigationItemSelector);
        var count = await elements.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var itemText = await elements.Nth(i).TextContentAsync() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(itemText))
            {
                navItems.Add(itemText.Trim());
            }
        }

        Logger.LogDebug("Found {Count} navigation items", navItems.Count);
        return navItems;
    }

    /// <summary>
    /// Clicks on a specific navigation item
    /// </summary>
    /// <param name="itemName">Name of the navigation item to click</param>
    [AllureStep("Click navigation item: {itemName}")]
    public async Task<DashboardPage> ClickNavigationItemAsync(string itemName)
    {
        var elements = GetElements(NavigationItemSelector);
        var count = await elements.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var element = elements.Nth(i);
            var itemText = await element.TextContentAsync() ?? string.Empty;
            
            if (itemText.Trim().Equals(itemName, StringComparison.OrdinalIgnoreCase))
            {
                await element.ClickAsync();
                Logger.LogInformation("Clicked navigation item: {ItemName}", itemName);
                return this;
            }
        }

        throw new ArgumentException($"Navigation item '{itemName}' not found");
    }

    /// <summary>
    /// Validates that all main dashboard widgets are present
    /// </summary>
    [AllureStep("Validate dashboard widgets")]
    public async Task<bool> ValidateDashboardWidgetsAsync()
    {
        var widgets = new[]
        {
            RecentActivityWidgetSelector,
            StatisticsWidgetSelector,
            QuickActionsWidgetSelector
        };

        foreach (var widget in widgets)
        {
            if (!await IsVisibleAsync(widget))
            {
                Logger.LogError("Dashboard widget not found: {Widget}", widget);
                return false;
            }
        }

        Logger.LogInformation("All dashboard widgets are present");
        return true;
    }

    /// <summary>
    /// Gets statistics from the statistics widget
    /// </summary>
    [AllureStep("Get dashboard statistics")]
    public async Task<Dictionary<string, string>> GetDashboardStatisticsAsync()
    {
        var statistics = new Dictionary<string, string>();
        
        // This is a generic implementation - in a real scenario, you'd have specific selectors
        // for different statistics based on the actual dashboard structure
        var statsElements = GetElements($"{StatisticsWidgetSelector} [data-testid='stat-item']");
        var count = await statsElements.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var element = statsElements.Nth(i);
            var label = await element.Locator("[data-testid='stat-label']").TextContentAsync() ?? $"Stat{i + 1}";
            var value = await element.Locator("[data-testid='stat-value']").TextContentAsync() ?? "0";
            statistics[label.Trim()] = value.Trim();
        }

        Logger.LogDebug("Retrieved {Count} dashboard statistics", statistics.Count);
        return statistics;
    }

    /// <summary>
    /// Gets recent activity items
    /// </summary>
    [AllureStep("Get recent activity items")]
    public async Task<List<string>> GetRecentActivityItemsAsync()
    {
        var activities = new List<string>();
        var elements = GetElements($"{RecentActivityWidgetSelector} [data-testid='activity-item']");
        var count = await elements.CountAsync();

        for (int i = 0; i < count; i++)
        {
            var activityText = await elements.Nth(i).TextContentAsync() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(activityText))
            {
                activities.Add(activityText.Trim());
            }
        }

        Logger.LogDebug("Found {Count} recent activity items", activities.Count);
        return activities;
    }

    /// <summary>
    /// Validates that the user is properly logged in by checking for user-specific elements
    /// </summary>
    [AllureStep("Validate user is logged in")]
    public async Task<bool> ValidateUserLoggedInAsync()
    {
        var requiredElements = new[]
        {
            UserMenuSelector,
            WelcomeMessageSelector,
            ContentLoadedSelector
        };

        foreach (var element in requiredElements)
        {
            if (!await IsVisibleAsync(element))
            {
                Logger.LogError("Required element for logged-in user not found: {Element}", element);
                return false;
            }
        }

        Logger.LogInformation("User is properly logged in");
        return true;
    }

    /// <summary>
    /// Refreshes the dashboard content
    /// </summary>
    [AllureStep("Refresh dashboard")]
    public async Task<DashboardPage> RefreshDashboardAsync()
    {
        await RefreshAsync();
        await WaitForDashboardToLoadAsync();
        Logger.LogInformation("Dashboard refreshed");
        return this;
    }

    /// <summary>
    /// Takes a screenshot of the dashboard
    /// </summary>
    /// <param name="screenshotName">Name for the screenshot</param>
    [AllureStep("Take dashboard screenshot")]
    public async Task<string> TakeDashboardScreenshotAsync(string screenshotName = "dashboard")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{screenshotName}_{timestamp}";
        var screenshotPath = Path.Combine(Config.Execution.OutputDirectory, "Screenshots", $"{fileName}.png");
        
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
        
        await TakeScreenshotAsync(screenshotPath);
        Logger.LogInformation("Dashboard screenshot saved: {Path}", screenshotPath);
        
        return screenshotPath;
    }
} 