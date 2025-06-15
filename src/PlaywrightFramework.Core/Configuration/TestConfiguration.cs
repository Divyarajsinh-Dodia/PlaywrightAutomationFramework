using System.ComponentModel.DataAnnotations;

namespace PlaywrightFramework.Core.Configuration;

/// <summary>
/// Test configuration settings loaded from appsettings.json
/// </summary>
public class TestConfiguration
{
    public const string SectionName = "TestConfiguration";

    /// <summary>
    /// Browser configuration settings
    /// </summary>
    [Required]
    public BrowserConfiguration Browser { get; set; } = new();

    /// <summary>
    /// Application under test configuration
    /// </summary>
    [Required]
    public ApplicationConfiguration Application { get; set; } = new();

    /// <summary>
    /// Test execution configuration
    /// </summary>
    [Required]
    public ExecutionConfiguration Execution { get; set; } = new();

    /// <summary>
    /// Reporting configuration
    /// </summary>
    [Required]
    public ReportingConfiguration Reporting { get; set; } = new();

    /// <summary>
    /// Logging configuration
    /// </summary>
    [Required]
    public LoggingConfiguration Logging { get; set; } = new();
}

/// <summary>
/// Browser-specific configuration settings
/// </summary>
public class BrowserConfiguration
{
    /// <summary>
    /// Default browser to use (Chrome, Firefox, Safari, Edge)
    /// </summary>
    public string DefaultBrowser { get; set; } = "Chrome";

    /// <summary>
    /// Whether to run in headless mode
    /// </summary>
    public bool Headless { get; set; } = false;

    /// <summary>
    /// Browser viewport width
    /// </summary>
    public int ViewportWidth { get; set; } = 1920;

    /// <summary>
    /// Browser viewport height
    /// </summary>
    public int ViewportHeight { get; set; } = 1080;

    /// <summary>
    /// Whether to start browser maximized
    /// </summary>
    public bool StartMaximized { get; set; } = true;

    /// <summary>
    /// Whether to accept downloads
    /// </summary>
    public bool AcceptDownloads { get; set; } = true;

    /// <summary>
    /// Browser timeout in milliseconds
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Navigation timeout in milliseconds
    /// </summary>
    public int NavigationTimeoutMs { get; set; } = 30000;

    /// <summary>
    /// Additional browser launch arguments
    /// </summary>
    public List<string> LaunchArgs { get; set; } = new();
}

/// <summary>
/// Application under test configuration
/// </summary>
public class ApplicationConfiguration
{
    /// <summary>
    /// Base URL of the application under test
    /// </summary>
    [Required]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Test environment (Dev, QA, Staging, Prod)
    /// </summary>
    public string Environment { get; set; } = "QA";

    /// <summary>
    /// Application name
    /// </summary>
    public string Name { get; set; } = "Sample Application";

    /// <summary>
    /// API base URL if testing includes API calls
    /// </summary>
    public string? ApiBaseUrl { get; set; }

    /// <summary>
    /// Default user credentials
    /// </summary>
    public UserCredentials DefaultUser { get; set; } = new();

    /// <summary>
    /// Additional user credentials for different test scenarios
    /// </summary>
    public Dictionary<string, UserCredentials> Users { get; set; } = new();
}

/// <summary>
/// User credentials for login scenarios
/// </summary>
public class UserCredentials
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public Dictionary<string, string> AdditionalProperties { get; set; } = new();
}

/// <summary>
/// Test execution configuration
/// </summary>
public class ExecutionConfiguration
{
    /// <summary>
    /// Maximum number of retry attempts for failed tests
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 2;

    /// <summary>
    /// Delay between retry attempts in milliseconds
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Whether to capture screenshots on failure
    /// </summary>
    public bool CaptureScreenshotOnFailure { get; set; } = true;

    /// <summary>
    /// Whether to capture full page screenshots
    /// </summary>
    public bool FullPageScreenshots { get; set; } = true;

    /// <summary>
    /// Whether to record videos of test execution
    /// </summary>
    public bool RecordVideo { get; set; } = false;

    /// <summary>
    /// Whether to record traces
    /// </summary>
    public bool RecordTrace { get; set; } = false;

    /// <summary>
    /// Maximum number of parallel test workers
    /// </summary>
    public int MaxWorkers { get; set; } = Environment.ProcessorCount;

    /// <summary>
    /// Test data directory
    /// </summary>
    public string TestDataDirectory { get; set; } = "TestData";

    /// <summary>
    /// Output directory for test artifacts
    /// </summary>
    public string OutputDirectory { get; set; } = "TestResults";

    /// <summary>
    /// Whether to highlight elements during interactions (shows red borders)
    /// </summary>
    public bool HighlightElements { get; set; } = true;

    /// <summary>
    /// Duration to show element highlighting in milliseconds
    /// </summary>
    public int HighlightDurationMs { get; set; } = 1500;

    /// <summary>
    /// Border width for element highlighting
    /// </summary>
    public int HighlightBorderWidth { get; set; } = 3;
}

/// <summary>
/// Reporting configuration
/// </summary>
public class ReportingConfiguration
{
    /// <summary>
    /// Whether Allure reporting is enabled
    /// </summary>
    public bool AllureEnabled { get; set; } = true;

    /// <summary>
    /// Allure results directory
    /// </summary>
    public string AllureResultsDirectory { get; set; } = "allure-results";

    /// <summary>
    /// Whether to generate HTML reports
    /// </summary>
    public bool GenerateHtmlReport { get; set; } = true;

    /// <summary>
    /// Report title
    /// </summary>
    public string ReportTitle { get; set; } = "Playwright Automation Test Report";

    /// <summary>
    /// Whether to include environment information in reports
    /// </summary>
    public bool IncludeEnvironmentInfo { get; set; } = true;
}

/// <summary>
/// Logging configuration
/// </summary>
public class LoggingConfiguration
{
    /// <summary>
    /// Minimum log level (Trace, Debug, Information, Warning, Error, Critical)
    /// </summary>
    public string MinimumLevel { get; set; } = "Information";

    /// <summary>
    /// Whether to write logs to console
    /// </summary>
    public bool WriteToConsole { get; set; } = true;

    /// <summary>
    /// Whether to write logs to file
    /// </summary>
    public bool WriteToFile { get; set; } = true;

    /// <summary>
    /// Log file path template
    /// </summary>
    public string LogFilePathTemplate { get; set; } = "Logs/test-log-{Date}.txt";

    /// <summary>
    /// Whether to include structured logging
    /// </summary>
    public bool StructuredLogging { get; set; } = true;
} 