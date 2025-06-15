using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;

namespace PlaywrightFramework.Core.Helpers;

/// <summary>
/// Helper class for managing screenshots
/// </summary>
public class ScreenshotHelper
{
    private readonly IPage _page;
    private readonly TestConfiguration _config;
    private readonly ILogger _logger;
    private readonly string _screenshotDirectory;

    public ScreenshotHelper(IPage page, TestConfiguration config, ILogger logger)
    {
        _page = page ?? throw new ArgumentNullException(nameof(page));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _screenshotDirectory = Path.Combine(_config.Execution.OutputDirectory, "Screenshots");
        Directory.CreateDirectory(_screenshotDirectory);
    }

    /// <summary>
    /// Takes a screenshot with the specified name
    /// </summary>
    /// <param name="screenshotName">Name for the screenshot (without extension)</param>
    /// <param name="fullPage">Whether to capture full page screenshot</param>
    /// <returns>Path to the saved screenshot</returns>
    public async Task<string> TakeScreenshotAsync(string screenshotName, bool? fullPage = null)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var fileName = $"{screenshotName}_{timestamp}.png";
            var filePath = Path.Combine(_screenshotDirectory, fileName);

            var options = new PageScreenshotOptions
            {
                Path = filePath,
                FullPage = fullPage ?? _config.Execution.FullPageScreenshots,
                Type = ScreenshotType.Png
            };

            await _page.ScreenshotAsync(options);
            
            _logger.LogInformation("Screenshot saved: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to take screenshot: {ScreenshotName}", screenshotName);
            throw;
        }
    }

    /// <summary>
    /// Takes a screenshot of a specific element
    /// </summary>
    /// <param name="locator">Element locator</param>
    /// <param name="screenshotName">Name for the screenshot</param>
    /// <returns>Path to the saved screenshot</returns>
    public async Task<string> TakeElementScreenshotAsync(string locator, string screenshotName)
    {
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            var fileName = $"{screenshotName}_element_{timestamp}.png";
            var filePath = Path.Combine(_screenshotDirectory, fileName);

            var element = _page.Locator(locator);
            await element.ScreenshotAsync(new LocatorScreenshotOptions
            {
                Path = filePath,
                Type = ScreenshotType.Png
            });

            _logger.LogInformation("Element screenshot saved: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to take element screenshot: {Locator}", locator);
            throw;
        }
    }

    /// <summary>
    /// Takes a screenshot and returns it as byte array
    /// </summary>
    /// <param name="fullPage">Whether to capture full page screenshot</param>
    /// <returns>Screenshot as byte array</returns>
    public async Task<byte[]> TakeScreenshotAsBytesAsync(bool? fullPage = null)
    {
        try
        {
            var options = new PageScreenshotOptions
            {
                FullPage = fullPage ?? _config.Execution.FullPageScreenshots,
                Type = ScreenshotType.Png
            };

            var screenshotBytes = await _page.ScreenshotAsync(options);
            _logger.LogDebug("Screenshot captured as byte array");
            return screenshotBytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to take screenshot as bytes");
            throw;
        }
    }

    /// <summary>
    /// Compares two screenshots for visual differences
    /// </summary>
    /// <param name="baselineScreenshotPath">Path to baseline screenshot</param>
    /// <param name="currentScreenshotName">Name for current screenshot</param>
    /// <param name="threshold">Difference threshold (0.0 to 1.0)</param>
    /// <returns>True if screenshots match within threshold, false otherwise</returns>
    public async Task<bool> CompareScreenshotsAsync(string baselineScreenshotPath, string currentScreenshotName, double threshold = 0.1)
    {
        try
        {
            if (!File.Exists(baselineScreenshotPath))
            {
                _logger.LogWarning("Baseline screenshot not found: {BaselinePath}", baselineScreenshotPath);
                return false;
            }

            var currentScreenshotPath = await TakeScreenshotAsync(currentScreenshotName);
            
            // Note: This is a basic implementation. For more advanced visual comparison,
            // consider using specialized libraries like ImageSharp or integration with visual testing tools
            var baselineBytes = await File.ReadAllBytesAsync(baselineScreenshotPath);
            var currentBytes = await File.ReadAllBytesAsync(currentScreenshotPath);

            if (baselineBytes.Length != currentBytes.Length)
            {
                _logger.LogInformation("Screenshot comparison failed - different file sizes");
                return false;
            }

            var differentPixels = 0;
            for (int i = 0; i < baselineBytes.Length; i++)
            {
                if (baselineBytes[i] != currentBytes[i])
                {
                    differentPixels++;
                }
            }

            var differencePercentage = (double)differentPixels / baselineBytes.Length;
            var isMatch = differencePercentage <= threshold;

            _logger.LogInformation("Screenshot comparison - Difference: {DifferencePercentage:P2}, Threshold: {Threshold:P2}, Match: {IsMatch}", 
                differencePercentage, threshold, isMatch);

            return isMatch;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to compare screenshots");
            return false;
        }
    }

    /// <summary>
    /// Saves a baseline screenshot for future comparisons
    /// </summary>
    /// <param name="baselineName">Name for the baseline screenshot</param>
    /// <returns>Path to the saved baseline screenshot</returns>
    public async Task<string> SaveBaselineScreenshotAsync(string baselineName)
    {
        try
        {
            var baselineDirectory = Path.Combine(_screenshotDirectory, "Baselines");
            Directory.CreateDirectory(baselineDirectory);

            var fileName = $"{baselineName}_baseline.png";
            var filePath = Path.Combine(baselineDirectory, fileName);

            var options = new PageScreenshotOptions
            {
                Path = filePath,
                FullPage = _config.Execution.FullPageScreenshots,
                Type = ScreenshotType.Png
            };

            await _page.ScreenshotAsync(options);
            
            _logger.LogInformation("Baseline screenshot saved: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save baseline screenshot: {BaselineName}", baselineName);
            throw;
        }
    }

    /// <summary>
    /// Cleans up old screenshots based on age
    /// </summary>
    /// <param name="maxAge">Maximum age of screenshots to keep</param>
    public void CleanupOldScreenshots(TimeSpan maxAge)
    {
        try
        {
            var cutoffDate = DateTime.Now - maxAge;
            var files = Directory.GetFiles(_screenshotDirectory, "*.png", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.CreationTime < cutoffDate)
                {
                    File.Delete(file);
                    _logger.LogDebug("Deleted old screenshot: {FilePath}", file);
                }
            }

            _logger.LogInformation("Screenshot cleanup completed. Removed files older than {MaxAge}", maxAge);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old screenshots");
        }
    }
} 