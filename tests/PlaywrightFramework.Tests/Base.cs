using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.PageObjects.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightFramework.Tests
{
    public class Base : BaseTest
    {
        protected LoginPage _loginPage = null!;
        protected DashboardPage _dashboardPage = null!;
        
        // Static flag to track if login has been performed for this test class
        private static bool _loginPerformed = false;
        private static readonly object _loginLock = new object();

        private DashboardPage CreateDashboardPage()
        {
            var dashboardLogger = Logger as ILogger<DashboardPage> ??
                new Microsoft.Extensions.Logging.Abstractions.NullLogger<DashboardPage>();
            return new DashboardPage(Page, Config, dashboardLogger);
        }

        /// <summary>
        /// Override the base SetUp to include one-time login setup
        /// This ensures login happens only once per test class
        /// </summary>
        public override async Task SetUpAsync()
        {
            // Call the base setup first (creates page, initializes helpers, etc.)
            await base.SetUpAsync();
            
            // Perform login only once per test class
            lock (_loginLock)
            {
                if (!_loginPerformed)
                {
                    // Perform login setup
                    SetupLoginAsync().Wait();
                    _loginPerformed = true;
                }
            }
        }

        /// <summary>
        /// Performs the login setup once per test class
        /// </summary>
        private async Task SetupLoginAsync()
        {
            Logger.LogInformation("Starting one-time login setup for test class using visibility-aware methods");
            
            var loginLogger = Logger as ILogger<LoginPage> ??
                new Microsoft.Extensions.Logging.Abstractions.NullLogger<LoginPage>();
            _loginPage = new LoginPage(Page, Config, loginLogger);
            
            await _loginPage.NavigateToLoginPageAsync();
            
            // Use visibility-aware methods for more reliable login
            await _loginPage.EnterUsernameVisibleAsync("divyaraj.dodia.ext@envu.com");
            await _loginPage.EnterPasswordVisibleAsync("May@1617");
            await _loginPage.EnterSMSVisibleAsync();
            
            // Initialize dashboard page for use in tests
            _dashboardPage = CreateDashboardPage();
            
            Logger.LogInformation("One-time login setup completed successfully using visibility-aware methods");
        }

        /// <summary>
        /// Reset the login flag when the test class is torn down
        /// </summary>
        public override async Task OneTimeTearDownAsync()
        {
            lock (_loginLock)
            {
                _loginPerformed = false;
            }
            
            await base.OneTimeTearDownAsync();
        }
    }
}
