using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Configuration;
using System.Linq;

namespace PlaywrightFramework.Core.Drivers;

/// <summary>
/// Manages Playwright browser instances and provides a centralized way to create and configure browsers
/// </summary>
public class BrowserManager : IDisposable
{
    private readonly TestConfiguration _config;
    private readonly ILogger<BrowserManager> _logger;
    
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private readonly Dictionary<string, IBrowserContext> _contexts = new();
    private bool _disposed = false;

    public BrowserManager(TestConfiguration config, ILogger<BrowserManager> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the current browser instance
    /// </summary>
    public IBrowser? Browser => _browser;

    /// <summary>
    /// Gets the current browser context
    /// </summary>
    public IBrowserContext? Context => _context;

    /// <summary>
    /// Initializes Playwright and creates a browser instance
    /// </summary>
    /// <param name="browserType">Optional browser type override</param>
    /// <returns>Task representing the initialization operation</returns>
    public async Task InitializeAsync(string? browserType = null)
    {
        try
        {
            _logger.LogInformation("Initializing Playwright browser manager");
            
            // Install Playwright if needed
            var exitCode = Program.Main(new[] { "install" });
            if (exitCode != 0)
            {
                _logger.LogWarning("Playwright installation returned non-zero exit code: {ExitCode}", exitCode);
            }

            _playwright = await Playwright.CreateAsync();
            
            var targetBrowserType = browserType ?? _config.Browser.DefaultBrowser;
            _browser = await CreateBrowserAsync(targetBrowserType);
            
            _logger.LogInformation("Browser manager initialized successfully with {BrowserType}", targetBrowserType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize browser manager");
            throw;
        }
    }

    /// <summary>
    /// Creates a new browser context with the specified options
    /// </summary>
    /// <param name="contextName">Name for the context (optional)</param>
    /// <param name="options">Browser context options (optional)</param>
    /// <returns>The created browser context</returns>
    public async Task<IBrowserContext> CreateContextAsync(string? contextName = null, BrowserNewContextOptions? options = null)
    {
        if (_browser == null)
            throw new InvalidOperationException("Browser must be initialized before creating contexts");

        var contextOptions = options ?? CreateDefaultContextOptions();
        var context = await _browser.NewContextAsync(contextOptions);
        
        // Configure context settings
        await ConfigureContextAsync(context);
        
        if (!string.IsNullOrEmpty(contextName))
        {
            _contexts[contextName] = context;
            _logger.LogInformation("Created named browser context: {ContextName}", contextName);
        }
        else
        {
            _context = context;
            _logger.LogInformation("Created default browser context");
        }

        return context;
    }

    /// <summary>
    /// Gets a named browser context
    /// </summary>
    /// <param name="contextName">Name of the context</param>
    /// <returns>The browser context if found, null otherwise</returns>
    public IBrowserContext? GetContext(string contextName)
    {
        return _contexts.TryGetValue(contextName, out var context) ? context : null;
    }

    /// <summary>
    /// Creates a new page in the default context
    /// </summary>
    /// <returns>The created page</returns>
    public async Task<IPage> CreatePageAsync()
    {
        if (_context == null)
        {
            await CreateContextAsync();
        }

        var page = await _context!.NewPageAsync();
        await ConfigurePageAsync(page);
        
        _logger.LogInformation("Created new page");
        return page;
    }

    /// <summary>
    /// Creates a new page in the specified context
    /// </summary>
    /// <param name="contextName">Name of the context</param>
    /// <returns>The created page</returns>
    public async Task<IPage> CreatePageAsync(string contextName)
    {
        var context = GetContext(contextName);
        if (context == null)
            throw new ArgumentException($"Context '{contextName}' not found", nameof(contextName));

        var page = await context.NewPageAsync();
        await ConfigurePageAsync(page);
        
        _logger.LogInformation("Created new page in context: {ContextName}", contextName);
        return page;
    }

    /// <summary>
    /// Closes a named context
    /// </summary>
    /// <param name="contextName">Name of the context to close</param>
    public async Task CloseContextAsync(string contextName)
    {
        if (_contexts.TryGetValue(contextName, out var context))
        {
            await context.CloseAsync();
            _contexts.Remove(contextName);
            _logger.LogInformation("Closed context: {ContextName}", contextName);
        }
    }

    /// <summary>
    /// Creates a browser instance based on the specified type
    /// </summary>
    /// <param name="browserType">Type of browser to create</param>
    /// <returns>The created browser instance</returns>
    private async Task<IBrowser> CreateBrowserAsync(string browserType)
    {
        var launchOptions = CreateLaunchOptions();
        
        // Handle Firefox-specific maximization
        if (browserType.ToLowerInvariant() == "firefox" && _config.Browser.StartMaximized)
        {
            // Firefox doesn't support --start-maximized, so we use different arguments
            var firefoxArgs = launchOptions.Args?.ToList() ?? new List<string>();
            firefoxArgs.RemoveAll(arg => arg == "--start-maximized"); // Remove if present
            
            // For Firefox, we can use --width and --height or just rely on the viewport size
            if (!firefoxArgs.Any(arg => arg.StartsWith("--width")))
            {
                firefoxArgs.Add($"--width={_config.Browser.ViewportWidth}");
                firefoxArgs.Add($"--height={_config.Browser.ViewportHeight}");
            }
            
            launchOptions.Args = firefoxArgs;
        }
        
        IBrowser browser = browserType.ToLowerInvariant() switch
        {
            "chrome" or "chromium" => await _playwright!.Chromium.LaunchAsync(launchOptions),
            "firefox" => await _playwright!.Firefox.LaunchAsync(launchOptions),
            "safari" or "webkit" => await _playwright!.Webkit.LaunchAsync(launchOptions),
            "edge" => await _playwright!.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
            {
                Headless = launchOptions.Headless,
                Timeout = launchOptions.Timeout,
                Args = launchOptions.Args,
                Channel = "msedge"
            }),
            _ => throw new ArgumentException($"Unsupported browser type: {browserType}")
        };

        _logger.LogInformation("Created {BrowserType} browser instance", browserType);
        return browser;
    }

    /// <summary>
    /// Creates browser launch options based on configuration
    /// </summary>
    /// <returns>Browser launch options</returns>
    private BrowserTypeLaunchOptions CreateLaunchOptions()
    {
        var launchArgs = new List<string>(_config.Browser.LaunchArgs);
        
        // Add maximization arguments if StartMaximized is enabled and not in headless mode
        if (_config.Browser.StartMaximized && !_config.Browser.Headless)
        {
            // For Chromium-based browsers (Chrome, Edge), use --start-maximized
            if (!launchArgs.Contains("--start-maximized"))
            {
                launchArgs.Add("--start-maximized");
                _logger.LogInformation("Added --start-maximized launch argument");
            }
            
            // Remove any window-size arguments as they conflict with maximized mode
            launchArgs.RemoveAll(arg => arg.StartsWith("--window-size"));
            _logger.LogInformation("Removed window-size arguments to avoid conflicts with maximized mode");
        }
        else if (_config.Browser.StartMaximized && _config.Browser.Headless)
        {
            // In headless mode, use window-size instead of start-maximized
            if (!launchArgs.Any(arg => arg.StartsWith("--window-size")))
            {
                var width = Math.Max(_config.Browser.ViewportWidth, 1920);
                var height = Math.Max(_config.Browser.ViewportHeight, 1080);
                launchArgs.Add($"--window-size={width},{height}");
                _logger.LogInformation("Added --window-size={Width},{Height} for headless maximized mode", width, height);
            }
        }

        var options = new BrowserTypeLaunchOptions
        {
            Headless = _config.Browser.Headless,
            Timeout = _config.Browser.TimeoutMs,
            Args = launchArgs
        };

        return options;
    }

    /// <summary>
    /// Creates default browser context options based on configuration
    /// </summary>
    /// <returns>Browser context options</returns>
    private BrowserNewContextOptions CreateDefaultContextOptions()
    {
        var options = new BrowserNewContextOptions
        {
            AcceptDownloads = _config.Browser.AcceptDownloads,
            RecordVideoDir = _config.Execution.RecordVideo ? _config.Execution.OutputDirectory : null
        };

        // Handle viewport size based on StartMaximized setting
        if (_config.Browser.StartMaximized && !_config.Browser.Headless)
        {
            // When maximized and not headless, set viewport to null to let the browser use its natural window size
            options.ViewportSize = ViewportSize.NoViewport; // This is the key for maximized mode
            _logger.LogInformation("Viewport set to NULL (natural browser window size) for maximized mode");
        }
        else if (_config.Browser.StartMaximized && _config.Browser.Headless)
        {
            // In headless mode, we can't truly maximize, so use large viewport
            options.ViewportSize = new ViewportSize
            {
                Width = Math.Max(_config.Browser.ViewportWidth, 1920),
                Height = Math.Max(_config.Browser.ViewportHeight, 1080)
            };
            _logger.LogInformation("Headless mode detected - using large viewport {Width}x{Height} instead of maximized", 
                options.ViewportSize.Width, options.ViewportSize.Height);
        }
        else
        {
            // Use configured viewport size
            options.ViewportSize = new ViewportSize
            {
                Width = _config.Browser.ViewportWidth,
                Height = _config.Browser.ViewportHeight
            };
            _logger.LogInformation("Viewport set to configured size {Width}x{Height}", _config.Browser.ViewportWidth, _config.Browser.ViewportHeight);
        }

        // Note: Trace recording is handled separately through context.Tracing.StartAsync()
        // as RecordTraceDir is not available in newer Playwright versions

        return options;
    }

    /// <summary>
    /// Configures the browser context with additional settings
    /// </summary>
    /// <param name="context">The browser context to configure</param>
    private async Task ConfigureContextAsync(IBrowserContext context)
    {
        // Set default timeouts
        context.SetDefaultTimeout(_config.Browser.TimeoutMs);
        context.SetDefaultNavigationTimeout(_config.Browser.NavigationTimeoutMs);

        // Add request/response logging if needed
        if (_config.Logging.MinimumLevel == "Debug" || _config.Logging.MinimumLevel == "Trace")
        {
            context.Request += (sender, e) => _logger.LogDebug("Request: {Method} {Url}", e.Method, e.Url);
            context.Response += (sender, e) => _logger.LogDebug("Response: {Status} {Url}", e.Status, e.Url);
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// Configures a page with additional settings
    /// </summary>
    /// <param name="page">The page to configure</param>
    private async Task ConfigurePageAsync(IPage page)
    {
        // Set default timeouts
        page.SetDefaultTimeout(_config.Browser.TimeoutMs);
        page.SetDefaultNavigationTimeout(_config.Browser.NavigationTimeoutMs);

        // Configure console logging
        page.Console += (sender, e) => _logger.LogDebug("Console {Type}: {Text}", e.Type, e.Text);
        
        // Configure page error logging
        page.PageError += (sender, e) => _logger.LogError("Page Error: {Error}", e);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Disposes of all browser resources
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        try
        {
            // Close all named contexts
            foreach (var context in _contexts.Values)
            {
                context?.CloseAsync().GetAwaiter().GetResult();
            }
            _contexts.Clear();

            // Close default context
            _context?.CloseAsync().GetAwaiter().GetResult();
            
            // Close browser
            _browser?.CloseAsync().GetAwaiter().GetResult();
            
            // Dispose Playwright
            _playwright?.Dispose();
            
            _logger.LogInformation("Browser manager disposed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during browser manager disposal");
        }
        finally
        {
            _disposed = true;
        }
    }

    /// <summary>
    /// Async disposal method
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            // Close all named contexts
            foreach (var context in _contexts.Values)
            {
                if (context != null)
                    await context.CloseAsync();
            }
            _contexts.Clear();

            // Close default context
            if (_context != null)
                await _context.CloseAsync();
            
            // Close browser
            if (_browser != null)
                await _browser.CloseAsync();
            
            // Dispose Playwright
            _playwright?.Dispose();
            
            _logger.LogInformation("Browser manager disposed successfully (async)");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during async browser manager disposal");
        }
        finally
        {
            _disposed = true;
        }
    }
} 