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
        // Static flag to track if login has been performed for this test class
        private static bool _loginPerformed = false;
        private static readonly object _loginLock = new object();

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
            Logger.LogInformation("Starting one-time login setup for test class");
            
            // Get login page from PageFactory
            var loginPage = PageFactory.GetPage<LoginPage>();
            
            // Navigate to login page and perform login
            await loginPage.NavigateToLoginPageAsync();
            
            // Use the simplified, robust login method
            //await loginPage.LoginAsync("divyaraj.dodia.ext@envu.com", "May@1617");
            
            Logger.LogInformation("One-time login setup completed successfully");
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
