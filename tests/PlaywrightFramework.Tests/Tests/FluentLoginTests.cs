using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlaywrightFramework.PageObjects.Pages;
using PlaywrightFramework.Core.Base;

namespace PlaywrightFramework.Tests.Tests
{
    /// <summary>
    /// Demonstrates the use of fluent method chaining in tests
    /// </summary>
    [TestFixture]
    public class FluentLoginTests : BaseTest
    {
        private LoginPage _loginPage;

        [SetUp]
        public override async Task SetUpAsync()
        {
            await base.SetUpAsync();
            // perform login for each test
            _loginPage = PageFactory.GetPage<LoginPage>();
            await _loginPage.NavigateToLoginPageAsync();
            await _loginPage.EnterUsernameAsync("divyaraj.dodia.ext@envu.com");
            await _loginPage.EnterPasswordAsync("May@1617");
            await _loginPage.EnterSMSAsync();
        }

        [Test]
        [Description("Demonstrates fluent method chaining")]
        public void FluentLoginTest()
        {
            // Example of fluent page object chaining after login
            _loginPage.NavigateToLoginPage()
                      .EnterUsername("divyaraj.dodia.ext@envu.com")
                      .EnterPassword("May@1617");
            Logger.LogInformation("Fluent method chaining test completed successfully");
            Assert.Pass("Fluent method chaining test completed successfully");
        }
        
        [Test]
        [Description("Demonstrates alternative fluent method chaining")]
        public void AlternativeFluentLoginTest()
        {
            // Even more concise chaining
            PageFactory.GetPage<LoginPage>()
                      .EnterUsername("divyaraj.dodia.ext@envu.com")
                      .EnterPassword("May@1617");
            Logger.LogInformation("Alternative fluent method chaining test completed successfully");
            Assert.Pass("Alternative fluent method chaining test completed successfully");
        }
    }
}
