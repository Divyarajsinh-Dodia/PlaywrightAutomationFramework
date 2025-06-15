using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlaywrightFramework.PageObjects.Pages;

namespace PlaywrightFramework.Tests.Tests;

/// <summary>
/// Demonstrates the use of fluent method chaining in tests
/// </summary>
[TestFixture]
public class FluentLoginTests : Base
{
    [Test]
    [Description("Demonstrates fluent method chaining")]
    public void FluentLoginTest()
    {
        // Get login page from PageFactory
        var loginPage = PageFactory.GetPage<LoginPage>();

        // Example of fluent method chaining
        loginPage.NavigateToLoginPage()
            .EnterUsername("divyaraj.dodia.ext@envu.com")
            .EnterPassword("May@1617")
            .ClickOnLoginButton();
            // .HomePage
            // .SideMenu
            // .ClickOnSideMenu()
            // .ClickOnInventoryMovement()
            // .InventoryMovementPage
            // .ClickOnNewButton();
            
        Logger.LogInformation("Fluent method chaining test completed successfully");
        Assert.Pass("Fluent method chaining test completed successfully");
    }
    
    [Test]
    [Description("Demonstrates alternative fluent method chaining")]
    public void AlternativeFluentLoginTest()
    {
        // Even more concise fluent chaining starting from login page
        PageFactory.GetPage<LoginPage>()
            .EnterUsername("divyaraj.dodia.ext@envu.com")
            .EnterPassword("May@1617")
            .HomePage
            .SideMenu
            .ClickOnSideMenu()
            .ClickOnInventoryMovement()
            .ClickOnNewButton();
            
        Logger.LogInformation("Alternative fluent method chaining test completed successfully");
        Assert.Pass("Alternative fluent method chaining test completed successfully");
    }
}
