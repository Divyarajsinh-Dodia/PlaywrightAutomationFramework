@echo off
REM Playwright Automation Framework - Test Runner
REM This script provides easy test execution options

setlocal enabledelayedexpansion

echo.
echo 🚀 Playwright Automation Framework - Test Runner
echo ==================================================

REM Set default values
set BROWSER=Chrome
set ENVIRONMENT=QA
set TEST_FILTER=
set PARALLEL_WORKERS=4
set GENERATE_REPORT=false

REM Parse command line arguments
:parse
if "%~1"=="" goto :execute
if "%~1"=="--browser" (
    set BROWSER=%~2
    shift
    shift
    goto :parse
)
if "%~1"=="--environment" (
    set ENVIRONMENT=%~2
    shift
    shift
    goto :parse
)
if "%~1"=="--filter" (
    set TEST_FILTER=%~2
    shift
    shift
    goto :parse
)
if "%~1"=="--parallel" (
    set PARALLEL_WORKERS=%~2
    shift
    shift
    goto :parse
)
if "%~1"=="--report" (
    set GENERATE_REPORT=true
    shift
    goto :parse
)
if "%~1"=="--help" (
    goto :help
)
shift
goto :parse

:execute
echo.
echo 📋 Test Configuration:
echo    Browser: %BROWSER%
echo    Environment: %ENVIRONMENT%
echo    Parallel Workers: %PARALLEL_WORKERS%
if not "%TEST_FILTER%"=="" echo    Filter: %TEST_FILTER%
echo.

REM Set environment variables
set ENVIRONMENT=%ENVIRONMENT%

REM Create test results directory
if not exist "TestResults" mkdir TestResults
if not exist "allure-results" mkdir allure-results
if not exist "Logs" mkdir Logs

REM Build the dotnet test command
set TEST_COMMAND=dotnet test tests/PlaywrightFramework.Tests/PlaywrightFramework.Tests.csproj
set TEST_COMMAND=%TEST_COMMAND% --configuration Release
set TEST_COMMAND=%TEST_COMMAND% --logger "trx;LogFileName=test-results.trx"
set TEST_COMMAND=%TEST_COMMAND% --results-directory TestResults
set TEST_COMMAND=%TEST_COMMAND% -- NUnit.NumberOfTestWorkers=%PARALLEL_WORKERS%
set TEST_COMMAND=%TEST_COMMAND% TestRunParameters.Parameter(name=Browser,value=%BROWSER%)

if not "%TEST_FILTER%"=="" (
    set TEST_COMMAND=%TEST_COMMAND% --filter "%TEST_FILTER%"
)

echo 🧪 Running tests...
echo Command: %TEST_COMMAND%
echo.

REM Execute tests
%TEST_COMMAND%

set TEST_EXIT_CODE=%ERRORLEVEL%

echo.
if %TEST_EXIT_CODE% equ 0 (
    echo ✅ Tests completed successfully!
) else (
    echo ❌ Tests completed with failures. Exit code: %TEST_EXIT_CODE%
)

REM Generate Allure report if requested
if "%GENERATE_REPORT%"=="true" (
    echo.
    echo 📊 Generating Allure report...
    
    REM Check if Allure is installed
    where allure >nul 2>nul
    if %ERRORLEVEL% equ 0 (
        echo Opening Allure report in browser...
        allure serve allure-results
    ) else (
        echo ⚠️  Allure CLI not found. Install Allure to generate reports.
        echo    Download from: https://github.com/allure-framework/allure2/releases
    )
)

echo.
echo 📁 Test artifacts location:
echo    Test Results: TestResults/
echo    Allure Results: allure-results/
echo    Logs: Logs/
echo    Screenshots: TestResults/Screenshots/

exit /b %TEST_EXIT_CODE%

:help
echo.
echo Usage: run-tests.bat [options]
echo.
echo Options:
echo   --browser ^<browser^>        Browser to use (Chrome, Firefox, Safari, Edge)
echo   --environment ^<env^>        Environment to test (QA, Staging, Production)
echo   --filter ^<filter^>          Test filter expression
echo   --parallel ^<count^>         Number of parallel workers
echo   --report                   Generate and open Allure report
echo   --help                     Show this help message
echo.
echo Examples:
echo   run-tests.bat --browser Firefox --environment Staging
echo   run-tests.bat --filter "Category=Smoke" --report
echo   run-tests.bat --parallel 8 --browser Chrome
echo.
goto :eof 