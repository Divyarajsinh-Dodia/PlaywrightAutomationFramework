using System.Data;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PlaywrightFramework.Core.Configuration;
using System.Globalization;
using CsvHelper;
using Bogus;
using ClosedXML.Excel;

namespace PlaywrightFramework.Core.Helpers;

/// <summary>
/// Helper class for managing test data from various sources
/// </summary>
public class TestDataHelper
{
    private readonly TestConfiguration _config;
    private readonly ILogger _logger;
    private readonly string _testDataDirectory;

    public static string GetExcelPath()
    {
        string excelPath = @"C:\Users\Divyaraj.Dodia\Downloads\AutomationDemo_Data.xlsx";
        return excelPath;
    }

    private static readonly Dictionary<string, int> TestRowMapping = new Dictionary<string, int>();

    public static void TrackTestRow(string testName, int rowIndex)
    {
        TestRowMapping[testName] = rowIndex;
    }

    public static void WriteTestResult(string worksheetName, string testName, string journalNumber, bool passed)
    {
        if (!TestRowMapping.ContainsKey(testName))
            return;

        int rowIndex = TestRowMapping[testName];
        string path = GetExcelPath();

        try
        {
            using (var workbook = new XLWorkbook(path))
            {
                if (workbook.TryGetWorksheet(worksheetName, out var worksheet))
                {
                    int referenceNumberColumnIndex = -1;
                    int testResultColumnIndex = -1;

                    // Search for existing column headers
                    var headerRow = worksheet.Row(1);
                    foreach (var cell in headerRow.CellsUsed())
                    {
                        string headerValue = cell.GetString();
                        if (headerValue == "ReferenceNumber")
                            referenceNumberColumnIndex = cell.Address.ColumnNumber;
                        else if (headerValue == "TestResult")
                            testResultColumnIndex = cell.Address.ColumnNumber;
                    }

                    // If columns don't exist, create them at the end
                    if (referenceNumberColumnIndex == -1 || testResultColumnIndex == -1)
                    {
                        int lastColumnIndex = worksheet.LastColumnUsed().ColumnNumber();

                        referenceNumberColumnIndex = lastColumnIndex + 1;
                        testResultColumnIndex = lastColumnIndex + 2;

                        // Add headers
                        worksheet.Cell(1, referenceNumberColumnIndex).Value = "ReferenceNumber";
                        worksheet.Cell(1, testResultColumnIndex).Value = "TestResult";
                    }

                    // Write journal number (or placeholder) and test result
                    worksheet.Cell(rowIndex + 1, referenceNumberColumnIndex).Value = string.IsNullOrEmpty(journalNumber) ? "N/A" : journalNumber;
                    worksheet.Cell(rowIndex + 1, testResultColumnIndex).Value = passed ? "PASS" : "FAIL";

                    // Save changes
                    workbook.Save();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to write test results to Excel: {ex.Message}");
        }

    }
}