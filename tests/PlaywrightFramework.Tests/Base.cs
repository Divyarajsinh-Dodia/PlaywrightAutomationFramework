using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.PageObjects.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightFramework.Tests
{
    public class Base : BaseTest
    {
        private LoginPage _loginPage = null!;

        //private DashboardPage CreateDashboardPage()
        //{
        //    var dashboardLogger = Logger as ILogger<DashboardPage> ??
        //        new Microsoft.Extensions.Logging.Abstractions.NullLogger<DashboardPage>();
        //    return new DashboardPage(Page, Config, dashboardLogger);
        //}

        [OneTimeSetUp]
        public async Task SetupLoginTests()
        {
            var loginLogger = Logger as ILogger<LoginPage> ??
                new Microsoft.Extensions.Logging.Abstractions.NullLogger<LoginPage>();
            _loginPage = new LoginPage(Page, Config, loginLogger);
            await _loginPage.NavigateToLoginPageAsync();
            await _loginPage.EnterUsernameAsync("divyaraj.dodia.ext@envu.com");
            await _loginPage.EnterPasswordAsync("May@1617");
            await _loginPage.EnterSMSAsync();
        }
    }
}
