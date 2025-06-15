using Microsoft.Extensions.Logging;
using PlaywrightFramework.Core.Configuration;

namespace PlaywrightFramework.Core.Extensions;

public static class LocatorContext
{
    private static readonly ThreadLocal<TestConfiguration> _currentConfig = new();
    private static readonly ThreadLocal<ILogger> _currentLogger = new();

    public static void SetContext(TestConfiguration config, ILogger logger)
    {
        _currentConfig.Value = config;
        _currentLogger.Value = logger;
    }

    internal static TestConfiguration CurrentConfig => 
        _currentConfig.Value ?? throw new InvalidOperationException("Config context not set. Call LocatorContext.SetContext() first.");
    
    internal static ILogger CurrentLogger => 
        _currentLogger.Value ?? throw new InvalidOperationException("Logger context not set. Call LocatorContext.SetContext() first.");

    public static void ClearContext()
    {
        _currentConfig.Value = null;
        _currentLogger.Value = null;
    }
} 