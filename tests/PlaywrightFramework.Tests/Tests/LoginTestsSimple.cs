using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Helpers;
using PlaywrightFramework.PageObjects.Pages;

namespace PlaywrightFramework.Tests.Tests;

/// <summary>
/// Simplified login test suite demonstrating framework capabilities without Allure dependencies
/// </summary>
[TestFixture]
public class LoginTestsSimple : Base
{
    //private LoginPage _loginPage = null!;
    
    //private DashboardPage CreateDashboardPage()
    //{
    //    var dashboardLogger = Logger as ILogger<DashboardPage> ?? 
    //        new Microsoft.Extensions.Logging.Abstractions.NullLogger<DashboardPage>();
    //    return new DashboardPage(Page, Config, dashboardLogger);
    //}

    //[SetUp]
    //public async Task SetupLoginTests()
    //{
    //    var loginLogger = Logger as ILogger<LoginPage> ??
    //        new Microsoft.Extensions.Logging.Abstractions.NullLogger<LoginPage>();
    //    _loginPage = new LoginPage(Page, Config, loginLogger);
    //    await _loginPage.NavigateToLoginPageAsync();
    //    await _loginPage.EnterUsernameAsync("divyaraj.dodia.ext@envu.com");
    //    await _loginPage.EnterPasswordAsync("May@1617");
    //    await _loginPage.EnterSMSAsync();
    //}

    private const string WORKSHEET_NAME = "InventoryMovement";

    public static IEnumerable<TestCaseData> LoadInventoryMovementData()
    {
        string path = TestDataHelper.GetExcelPath();
        using (var workbook = new XLWorkbook(path))
        {
            var worksheet = workbook.Worksheet(WORKSHEET_NAME);
            var rows = worksheet.RowsUsed().Skip(1).ToList(); // Skip header row

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                string environment = row.Cell(1).GetString();
                string entity = row.Cell(2).GetString();
                string warehouse = row.Cell(3).GetString();
                string offsetAccount = row.Cell(4).GetString();
                string itemNumber = row.Cell(5).GetString();
                string itemQty = row.Cell(6).GetString();
                string batchNumber = row.Cell(7).GetString();
                string location = row.Cell(8).GetString();

                string testName = $"InventoryMovement_{entity}_{warehouse}_{offsetAccount}_{itemNumber}_{itemQty}_{batchNumber}_{location}";

                // Track the row index (i is zero-based index in our filtered collection)
                TestDataHelper.TrackTestRow(testName, i + 1);

                yield return new TestCaseData(environment, entity, warehouse, offsetAccount, itemNumber, itemQty, batchNumber, location)
                    .SetName(testName);
            }
        }
    }

    [TestCaseSource(nameof(LoadInventoryMovementData))]
    [Description("Verify successful login with valid credentials")]
    public async Task Login_WithValidCredentials_ShouldSucceed(string environment, string entity, string warehouse, string offsetAccount, string itemNumber, string itemQty, string batchNumber, string location)
    {
        Console.WriteLine($"Running login test with parameters: Environment={environment}, Entity={entity}, Warehouse={warehouse}, OffsetAccount={offsetAccount}, ItemNumber={itemNumber}, ItemQty={itemQty}, BatchNumber={batchNumber}, Location={location}");
        Console.WriteLine("Starting login test with valid credentials.");
    }
} 