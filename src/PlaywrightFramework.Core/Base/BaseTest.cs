using Allure.NUnit;
using Allure.NUnit.Attributes;
using Allure.Net.Commons;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using NUnit.Framework;
using PlaywrightFramework.Core.Configuration;
using PlaywrightFramework.Core.Drivers;
using PlaywrightFramework.Core.Helpers;
using Serilog;

namespace PlaywrightFramework.Core.Base;

/// <summary>
/// Base test class that provides common functionality for all test classes
/// </summary>
[AllureNUnit]
public abstract class BaseTest
{
    protected TestConfiguration Config { get; private set; } = null!;
    protected BrowserManager BrowserManager { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    protected Microsoft.Extensions.Logging.ILogger Logger { get; private set; } = null!;
    protected ScreenshotHelper ScreenshotHelper { get; private set; } = null!;
    protected TestDataHelper TestDataHelper { get; private set; } = null!;

    private IServiceProvider _serviceProvider = null!;
    private readonly string _testResultsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestResults");

    /// <summary>
    /// One-time setup for the entire test suite
    /// </summary>
    [OneTimeSetUp]
    public virtual async Task OneTimeSetUpAsync()
    {
        // Ensure test results directory exists
        Directory.CreateDirectory(_testResultsPath);

        // Setup configuration and services
        SetupServices();

        // Initialize browser manager
        BrowserManager = _serviceProvider.GetRequiredService<BrowserManager>();
        await BrowserManager.InitializeAsync();

        Logger.LogInformation("Test suite setup completed");

    }

    /// <summary>
    /// Setup before each test
    /// </summary>
    [SetUp]
    public virtual async Task SetUpAsync()
    {
        Logger.LogInformation("Starting test: {TestName}", TestContext.CurrentContext.Test.Name);

        // Create a new page for each test
        Page = await BrowserManager.CreatePageAsync();

        // Set the LocatorContext for element highlighting
        Extensions.LocatorContext.SetContext(Config, Logger);

        // Initialize helpers
        ScreenshotHelper = new ScreenshotHelper(Page, Config, Logger);
        //TestDataHelper = new TestDataHelper(Config, Logger);

        // Add Allure environment information
        AddAllureEnvironmentInfo();

        Logger.LogInformation("Test setup completed for: {TestName}", TestContext.CurrentContext.Test.Name);
    }

    /// <summary>
    /// Cleanup after each test
    /// </summary>
    [TearDown]
    public virtual async Task TearDownAsync()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        var testResult = TestContext.CurrentContext.Result.Outcome.Status;

        Logger.LogInformation("Test {TestName} completed with status: {Status}", testName, testResult);

        try
        {
            // Clean up LocatorContext
            Extensions.LocatorContext.ClearContext();

            // Capture screenshot if test failed and feature is enabled
            if (testResult == NUnit.Framework.Interfaces.TestStatus.Failed &&
                Config.Execution.CaptureScreenshotOnFailure)
            {
                await CaptureFailureScreenshotAsync();
            }

            // Attach page source on failure for debugging
            if (testResult == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                await AttachPageSourceAsync();
            }

            // Close the page
            if (Page != null)
            {
                await Page.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during test teardown for: {TestName}", testName);
        }

        Logger.LogInformation("Test teardown completed for: {TestName}", testName);
    }

    /// <summary>
    /// One-time cleanup for the entire test suite
    /// </summary>
    [OneTimeTearDown]
    public virtual async Task OneTimeTearDownAsync()
    {
        try
        {
            if (BrowserManager != null)
            {
                await BrowserManager.DisposeAsync();
            }

            Logger.LogInformation("Test suite teardown completed");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during test suite teardown");
        }
    }

    /// <summary>
    /// Navigates to the specified URL
    /// </summary>
    /// <param name="url">URL to navigate to (can be relative or absolute)</param>
    /// <param name="waitUntil">Wait condition for navigation</param>
    protected async Task NavigateToAsync(string url, WaitUntilState waitUntil = WaitUntilState.Load)
    {
        var fullUrl = url.StartsWith("http") ? url : Config.Application.BaseUrl.TrimEnd('/') + "/" + url.TrimStart('/');

        Logger.LogInformation("Navigating to: {Url}", fullUrl);

        await Page.GotoAsync(fullUrl, new PageGotoOptions { WaitUntil = waitUntil });

        Logger.LogInformation("Navigation completed to: {Url}", fullUrl);
    }

    /// <summary>
    /// Waits for the specified duration
    /// </summary>
    /// <param name="duration">Duration to wait</param>
    protected async Task WaitAsync(TimeSpan duration)
    {
        Logger.LogDebug("Waiting for {Duration}", duration);
        await Task.Delay(duration);
    }

    /// <summary>
    /// Waits for the specified number of seconds
    /// </summary>
    /// <param name="seconds">Number of seconds to wait</param>
    protected async Task WaitAsync(int seconds)
    {
        await WaitAsync(TimeSpan.FromSeconds(seconds));
    }

    /// <summary>
    /// Takes a screenshot with the specified name
    /// </summary>
    /// <param name="screenshotName">Name for the screenshot</param>
    protected async Task<string> TakeScreenshotAsync(string? screenshotName = null)
    {
        screenshotName ??= $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}";
        return await ScreenshotHelper.TakeScreenshotAsync(screenshotName);
    }

    /// <summary>
    /// Retries an action with the specified number of attempts
    /// </summary>
    /// <param name="action">Action to retry</param>
    /// <param name="maxAttempts">Maximum number of attempts</param>
    /// <param name="delay">Delay between attempts</param>
    protected async Task RetryAsync(Func<Task> action, int? maxAttempts = null, TimeSpan? delay = null)
    {
        maxAttempts ??= Config.Execution.MaxRetryAttempts;
        delay ??= TimeSpan.FromMilliseconds(Config.Execution.RetryDelayMs);

        var attempts = 0;
        Exception? lastException = null;

        while (attempts < maxAttempts)
        {
            try
            {
                await action();
                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempts++;

                if (attempts >= maxAttempts)
                    break;

                Logger.LogWarning("Attempt {Attempt} failed, retrying in {Delay}. Error: {Error}",
                    attempts, delay, ex.Message);

                await Task.Delay(delay.Value);
            }
        }

        Logger.LogError("All {MaxAttempts} retry attempts failed", maxAttempts);
        throw lastException ?? new InvalidOperationException("Retry operation failed");
    }

    /// <summary>
    /// Retries a function with the specified number of attempts
    /// </summary>
    /// <param name="func">Function to retry</param>
    /// <param name="maxAttempts">Maximum number of attempts</param>
    /// <param name="delay">Delay between attempts</param>
    /// <returns>Result of the function</returns>
    protected async Task<T> RetryAsync<T>(Func<Task<T>> func, int? maxAttempts = null, TimeSpan? delay = null)
    {
        maxAttempts ??= Config.Execution.MaxRetryAttempts;
        delay ??= TimeSpan.FromMilliseconds(Config.Execution.RetryDelayMs);

        var attempts = 0;
        Exception? lastException = null;

        while (attempts < maxAttempts)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempts++;

                if (attempts >= maxAttempts)
                    break;

                Logger.LogWarning("Attempt {Attempt} failed, retrying in {Delay}. Error: {Error}",
                    attempts, delay, ex.Message);

                await Task.Delay(delay.Value);
            }
        }

        Logger.LogError("All {MaxAttempts} retry attempts failed", maxAttempts);
        throw lastException ?? new InvalidOperationException("Retry operation failed");
    }

    /// <summary>
    /// Sets up dependency injection and configuration
    /// </summary>
    private void SetupServices()
    {
        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development"}.json",
                optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Setup services
        var services = new ServiceCollection();

        // Register configuration
        var testConfig = configuration.GetSection(TestConfiguration.SectionName).Get<TestConfiguration>()
            ?? throw new InvalidOperationException("TestConfiguration section not found in appsettings.json");
        Config = testConfig;
        services.AddSingleton(testConfig);

        // Setup logging
        SetupLogging(services, testConfig);

        // Register other services
        services.AddSingleton<BrowserManager>();
        services.AddTransient<ScreenshotHelper>();
        services.AddTransient<TestDataHelper>();

        _serviceProvider = services.BuildServiceProvider();
        Logger = _serviceProvider.GetRequiredService<ILogger<BaseTest>>();
    }

    /// <summary>
    /// Sets up logging configuration
    /// </summary>
    private void SetupLogging(IServiceCollection services, TestConfiguration config)
    {
        var loggerConfig = new LoggerConfiguration();

        // Set minimum level
        loggerConfig.MinimumLevel.Is(Enum.Parse<Serilog.Events.LogEventLevel>(config.Logging.MinimumLevel));

        // Configure console sink
        if (config.Logging.WriteToConsole)
        {
            loggerConfig.WriteTo.Console();
        }

        // Configure file sink
        if (config.Logging.WriteToFile)
        {
            var logPath = config.Logging.LogFilePathTemplate.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd"));
            loggerConfig.WriteTo.File(logPath, rollingInterval: RollingInterval.Day);
        }

        // Build logger
        var serilogLogger = loggerConfig.CreateLogger();
        Log.Logger = serilogLogger;

        // Add to services
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(serilogLogger);
        });
    }

    /// <summary>
    /// Captures a screenshot when a test fails
    /// </summary>
    private async Task CaptureFailureScreenshotAsync()
    {
        try
        {
            var screenshotPath = await TakeScreenshotAsync($"FAILURE_{TestContext.CurrentContext.Test.Name}");

            // TODO: Add Allure attachment when Allure API is properly configured

            Logger.LogInformation("Failure screenshot captured: {Path}", screenshotPath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to capture failure screenshot");
        }
    }

    /// <summary>
    /// Attaches page source for debugging failed tests
    /// </summary>
    private async Task AttachPageSourceAsync()
    {
        try
        {
            var pageSource = await Page.ContentAsync();
            var sourceFileName = $"page_source_{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.html";
            var sourcePath = Path.Combine(_testResultsPath, sourceFileName);

            await File.WriteAllTextAsync(sourcePath, pageSource);

            // TODO: Add Allure attachment when Allure API is properly configured

            Logger.LogInformation("Page source attached: {Path}", sourcePath);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to attach page source");
        }
    }

    /// <summary>
    /// Adds environment information to Allure report
    /// </summary>
    private void AddAllureEnvironmentInfo()
    {
        // Add test parameters to Allure report
        AllureLifecycle.Instance.UpdateTestCase(testResult =>
        {
            testResult.parameters.Add(new Parameter { name = "Environment", value = Config.Application.Environment });
            testResult.parameters.Add(new Parameter { name = "Browser", value = Config.Browser.DefaultBrowser });
            testResult.parameters.Add(new Parameter { name = "Base URL", value = Config.Application.BaseUrl });
            testResult.parameters.Add(new Parameter { name = "Headless", value = Config.Browser.Headless.ToString() });
            testResult.parameters.Add(new Parameter { name = "Test Name", value = TestContext.CurrentContext.Test.Name });
        });
    }
}