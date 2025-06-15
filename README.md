# Playwright Automation Framework

A comprehensive, enterprise-level test automation framework built with **Playwright**, **C#**, **NUnit**, and **Allure** reporting. This framework is designed to be scalable, maintainable, and easy to use for teams of all sizes.

## 🚀 Features

### Core Framework Features
- **Cross-browser support**: Chrome, Firefox, Safari, Edge
- **Page Object Model (POM)**: Well-structured, maintainable page objects
- **Configuration Management**: Flexible configuration system with environment support
- **Parallel Execution**: Run tests in parallel for faster execution
- **Comprehensive Logging**: Structured logging with Serilog
- **Screenshot Management**: Automatic screenshot capture on failures
- **Video & Trace Recording**: Optional video and trace recording for debugging
- **Retry Mechanism**: Built-in retry logic for flaky tests
- **Test Data Management**: Support for JSON, CSV, and generated test data
- **Allure Reporting**: Rich, interactive test reports
- **CI/CD Integration**: GitHub Actions workflow included

### Enterprise Features
- **Dependency Injection**: Clean architecture with DI container
- **Environment Management**: Multiple environment configurations
- **User Management**: Role-based test execution
- **Security**: Secure credential management
- **Extensibility**: Easy to extend and customize
- **Documentation**: Comprehensive inline documentation
- **Best Practices**: Follows industry standards and patterns

## 📁 Project Structure

```
PlaywrightAutomationFramework/
├── .github/
│   └── workflows/
│       └── playwright-tests.yml          # CI/CD pipeline
├── src/
│   ├── PlaywrightFramework.Core/         # Core framework components
│   │   ├── Base/                         # Base classes (BaseTest, BasePage)
│   │   ├── Configuration/                # Configuration management
│   │   ├── Drivers/                      # Browser management
│   │   └── Helpers/                      # Utility helpers
│   ├── PlaywrightFramework.PageObjects/  # Page Object Model classes
│   │   └── Pages/                        # Page object implementations
│   └── PlaywrightFramework.Utilities/    # Additional utilities
├── tests/
│   └── PlaywrightFramework.Tests/        # Test implementations
│       ├── Tests/                        # Test classes
│       ├── TestData/                     # Test data files
│       └── appsettings.json              # Test configuration
├── PlaywrightAutomationFramework.sln     # Solution file
└── README.md                             # This file
```

## 🛠️ Prerequisites

- **.NET 8.0 SDK** or later
- **Visual Studio 2022** or **Visual Studio Code**
- **Git** for version control
- **Allure Command Line** (optional, for local report generation)

## ⚡ Quick Start

### 1. Clone the Repository
```bash
git clone <repository-url>
cd PlaywrightAutomationFramework
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Build the Solution
```bash
dotnet build
```

### 4. Install Playwright Browsers
```bash
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

### 5. Run Your First Test
```bash
dotnet test tests/PlaywrightFramework.Tests/PlaywrightFramework.Tests.csproj
```

## 🔧 Configuration

### Environment Configuration
The framework supports multiple environments through configuration files:

- `appsettings.json` - Default configuration
- `appsettings.Development.json` - Development environment
- `appsettings.QA.json` - QA environment
- `appsettings.Staging.json` - Staging environment

### Key Configuration Sections

#### Browser Configuration
```json
{
  "Browser": {
    "DefaultBrowser": "Chrome",
    "Headless": false,
    "ViewportWidth": 1920,
    "ViewportHeight": 1080,
    "TimeoutMs": 30000
  }
}
```

#### Application Configuration
```json
{
  "Application": {
    "BaseUrl": "https://your-app.com",
    "Environment": "QA",
    "DefaultUser": {
      "Username": "test@example.com",
      "Password": "password123"
    }
  }
}
```

## 📝 Writing Tests

### Basic Test Structure
```csharp
[TestFixture]
[AllureSuite("Your Test Suite")]
public class YourTests : BaseTest
{
    private YourPage _yourPage;

    [SetUp]
    public async Task Setup()
    {
        _yourPage = new YourPage(Page, Config, Logger);
        await _yourPage.NavigateToAsync();
    }

    [Test]
    [AllureFeature("Your Feature")]
    [AllureStory("Your Story")]
    public async Task YourTest_Scenario_ExpectedResult()
    {
        // Arrange
        var testData = TestDataHelper.GetUserCredentials();
        
        // Act
        await _yourPage.PerformActionAsync(testData.Username);
        
        // Assert
        Assert.That(await _yourPage.IsActionSuccessfulAsync(), Is.True);
    }
}
```

### Page Object Example
```csharp
public class LoginPage : BasePage
{
    private const string UsernameSelector = "[data-testid='username']";
    private const string PasswordSelector = "[data-testid='password']";
    private const string LoginButtonSelector = "[data-testid='login-button']";

    public LoginPage(IPage page, TestConfiguration config, ILogger<LoginPage> logger) 
        : base(page, config, logger) { }

    [AllureStep("Login with credentials")]
    public async Task<LoginPage> LoginAsync(string username, string password)
    {
        await FillAsync(UsernameSelector, username);
        await FillAsync(PasswordSelector, password);
        await ClickAsync(LoginButtonSelector);
        return this;
    }
}
```

## 📊 Test Data Management

### JSON Test Data
```json
{
  "users": [
    {
      "username": "testuser",
      "password": "password123",
      "role": "admin"
    }
  ]
}
```

### Loading Test Data
```csharp
// Load from JSON
var users = await TestDataHelper.LoadJsonDataAsync<UserList>("users");

// Generate random data
var randomUsers = TestDataHelper.GenerateRandomUsers(5);

// Get user by role
var adminUser = TestDataHelper.GetUserCredentials("Admin");
```

## 🎯 Running Tests

### Local Execution
```bash
# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=LoginTests"

# Run tests with specific tag
dotnet test --filter "Category=Smoke"

# Run tests in parallel
dotnet test -- NUnit.NumberOfTestWorkers=4
```

### Cross-Browser Testing
```bash
# Run on specific browser
dotnet test -- TestRunParameters.Parameter(name=Browser,value=Firefox)

# Run on multiple browsers (requires test configuration)
dotnet test -- TestRunParameters.Parameter(name=Browser,value=Chrome)
dotnet test -- TestRunParameters.Parameter(name=Browser,value=Firefox)
```

### Environment-Specific Testing
```bash
# Set environment
export ENVIRONMENT=Staging
dotnet test

# Or use configuration
dotnet test -- TestRunParameters.Parameter(name=Environment,value=Production)
```

## 📈 Allure Reporting

### Generate Local Reports
```bash
# After running tests
allure serve allure-results

# Or generate static report
allure generate allure-results --clean -o allure-report
```

### Report Features
- **Test execution overview**
- **Detailed test steps with screenshots**
- **Failed test analysis**
- **Historical trends**
- **Environment information**
- **Attachments (screenshots, logs, videos)**

## 🚀 CI/CD Integration

The framework includes a comprehensive GitHub Actions workflow that:

- Runs tests on multiple browsers
- Generates Allure reports
- Deploys reports to GitHub Pages
- Handles notifications and artifacts
- Supports manual triggers with parameters

### Workflow Features
- **Multi-browser execution**
- **Parallel test execution**
- **Artifact management**
- **Report generation and deployment**
- **Notification system**
- **Performance testing**

## 🏗️ Framework Architecture

### Core Components

#### BaseTest
- **Purpose**: Provides common test functionality
- **Features**: Browser management, configuration, logging, screenshots
- **Usage**: All test classes inherit from BaseTest

#### BasePage
- **Purpose**: Common page functionality for Page Object Model
- **Features**: Element interactions, waits, validations
- **Usage**: All page objects inherit from BasePage

#### BrowserManager
- **Purpose**: Manages Playwright browser instances
- **Features**: Multi-browser support, context management, configuration
- **Usage**: Automatically handled by BaseTest

#### Configuration System
- **Purpose**: Manages test configuration across environments
- **Features**: Environment-specific settings, user management, feature flags
- **Usage**: Injected into all framework components

## 🛡️ Best Practices

### Test Organization
- **Use descriptive test names**: `Login_WithValidCredentials_ShouldSucceed`
- **Follow AAA pattern**: Arrange, Act, Assert
- **Use Page Object Model**: Keep tests clean and maintainable
- **Tag tests appropriately**: Use Allure tags for better organization

### Page Objects
- **Use constants for selectors**: Easier maintenance
- **Implement fluent interface**: Method chaining for better readability
- **Add validation methods**: Verify page state
- **Use data-testid attributes**: More reliable than CSS selectors

### Test Data
- **Use external data files**: JSON, CSV for flexibility
- **Implement data builders**: For complex test data
- **Environment-specific data**: Different data for different environments
- **Avoid hardcoded values**: Use configuration or generation

### Error Handling
- **Implement retry logic**: For flaky operations
- **Take screenshots on failure**: For debugging
- **Use meaningful assertions**: Clear failure messages
- **Log important actions**: For troubleshooting

## 🔧 Customization and Extension

### Adding New Page Objects
1. Create a new class inheriting from `BasePage`
2. Define element selectors as constants
3. Implement page-specific methods
4. Add Allure step annotations
5. Include validation methods

### Adding New Test Data Sources
1. Extend `TestDataHelper` class
2. Add methods for new data sources
3. Implement proper error handling
4. Add logging for debugging

### Adding New Browsers
1. Update `BrowserManager` class
2. Add browser-specific configuration
3. Update CI/CD workflow
4. Test cross-browser compatibility

## 📚 Learning Resources

### Playwright Documentation
- [Playwright Official Docs](https://playwright.dev/dotnet/)
- [Playwright API Reference](https://playwright.dev/dotnet/docs/api/class-playwright)

### NUnit Documentation
- [NUnit Official Docs](https://docs.nunit.org/)
- [NUnit Attributes](https://docs.nunit.org/articles/nunit/writing-tests/attributes.html)

### Allure Documentation
- [Allure Framework](https://docs.qameta.io/allure/)
- [Allure .NET](https://github.com/allure-framework/allure-csharp)

## 🤝 Contributing

### Development Workflow
1. **Fork the repository**
2. **Create a feature branch**
3. **Make your changes**
4. **Add tests for new functionality**
5. **Run the test suite**
6. **Submit a pull request**

### Code Standards
- **Follow C# coding conventions**
- **Add XML documentation**
- **Include unit tests**
- **Update documentation**
- **Use meaningful commit messages**

## 📞 Support

### Getting Help
- **Check the documentation**: Most common issues are covered
- **Review existing tests**: Learn from examples
- **Check the logs**: Detailed logging for troubleshooting
- **Contact the team**: For framework-specific questions

### Common Issues
- **Browser not found**: Run `playwright install`
- **Configuration errors**: Check `appsettings.json`
- **Test failures**: Check screenshots and logs
- **Performance issues**: Review parallel execution settings

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎉 Acknowledgments

- **Playwright Team**: For the amazing browser automation library
- **NUnit Team**: For the robust testing framework
- **Allure Team**: For the excellent reporting framework
- **Community Contributors**: For feedback and improvements

---

**Happy Testing!** 🧪✨

For questions or support, please contact the test automation team or create an issue in the repository. 