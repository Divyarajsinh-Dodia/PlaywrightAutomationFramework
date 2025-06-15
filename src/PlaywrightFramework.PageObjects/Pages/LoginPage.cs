using Allure.NUnit.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PlaywrightFramework.PageObjects.Pages;

/// <summary>
/// Page Object for Login page demonstrating best practices
/// </summary>
public class LoginPage : BasePage
{
    // Page URL
    private const string PageUrl = "https://desd365-rolltest.sandbox.operations.dynamics.com/?cmp=US01&mi=DefaultDashboard";
    
    // Locators - using constants for maintainability
    private const string UsernameInputSelector = "//input[@name='loginfmt']";
    private const string UsernameNextButtonSelector = "//input[@type='submit' and @value='Next']";
    private const string PasswordInputSelector = "//input[@Placeholder='Password']";
    private const string SignInButtonSelector = "//input[@value='Sign in']";
    private const string SendSMSButtonSelector = "//div[@data-value='OneWaySMS' and @role='button']";
    private const string OTPInputSelector = "//input[@placeholder='Code']";
    private const string LoginButtonSelector = "[data-testid='login-button']";
    private const string VerifyButtonSelector = "//input[@value='Verify']";
    private const string ErrorMessageSelector = "[data-testid='error-message']";
    private const string ForgotPasswordLinkSelector = "[data-testid='forgot-password-link']";
    private const string RememberMeCheckboxSelector = "[data-testid='remember-me-checkbox']";
    private const string ShowPasswordButtonSelector = "[data-testid='show-password-button']";
    private const string LoadingSpinnerSelector = "[data-testid='loading-spinner']";

    public LoginPage(IPage page, TestConfiguration config, ILogger<LoginPage> logger) 
        : base(page, config, logger)
    {
    }

    public static string GetSMSMessage()
    {
        Thread.Sleep(2000);
        // Twilio credentials
        const string accountSid = "ACca5dbeb88b48938bbed36ef375428f76"; // Replace with your Account SID
        const string authToken = "9b270c9da672367236b0e436fb60809e";   // Replace with your Auth Token
        const string twilioPhoneNumber = "+19703153791"; // Replace with your Twilio phone number

        // Initialize Twilio client
        TwilioClient.Init(accountSid, authToken);

        try
        {
            // Fetch received messages (messages sent TO your Twilio number)
            var messages = MessageResource.Read(
                to: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                limit: 1);
            // Get the latest message
            var latestMessage = messages.FirstOrDefault();

            if (latestMessage != null)
            {
                //Console.WriteLine("From: " + latestMessage.From);
                //Console.WriteLine("To: " + latestMessage.To);
                //Console.WriteLine("Message: " + latestMessage.Body);
                //Console.WriteLine("Date Sent: " + latestMessage.DateSent);
                string smsBody = latestMessage.Body;
                string pattern = @"\d{6}";

                Match match = Regex.Match(smsBody, pattern);

                if (match.Success)
                {
                    // Console.WriteLine("Extracted Code: " + match.Value);
                    return match.Value;
                }
                else
                {
                    Console.WriteLine("No code found.");
                    return "No code found.";
                }
            }
            else
            {
                Console.WriteLine("No messages received.");
                return "Having trouble retriving SMS";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return "Error occurred while retrieving SMS";
        }
    }

    /// <summary>
    /// Navigates to the login page
    /// </summary>
    [AllureStep("Navigate to login page")]
    public async Task<LoginPage> NavigateToLoginPageAsync()
    {
        await NavigateToAsync(PageUrl);
        await WaitForLoadAsync();
        Logger.LogInformation("Navigated to login page");
        return this;
    }

    /// <summary>
    /// Enters username in the username field
    /// </summary>
    /// <param name="username">Username to enter</param>
    [AllureStep("Enter username: {username}")]
    public async Task<LoginPage> EnterUsernameAsync(string username)
    {
        await FillAsync(UsernameInputSelector, username);
        Logger.LogDebug("Entered username: {Username}", username);
        await ClickAsync(UsernameNextButtonSelector);
        Logger.LogDebug("Clicked next button after entering username");
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Enters password in the password field
    /// </summary>
    /// <param name="password">Password to enter</param>
    [AllureStep("Enter password")]
    public async Task<LoginPage> EnterPasswordAsync(string password)
    {
        await FillAsync(PasswordInputSelector, password);
        Logger.LogDebug("Entered password");
        await ClickAsync(SignInButtonSelector);
        Logger.LogDebug("Clicked sign in button after entering password");
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    [AllureStep("Enter SMS")]
    public async Task<LoginPage> EnterSMSAsync()
    {
        await ClickAsync(SendSMSButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        await FillAsync(OTPInputSelector, GetSMSMessage());
        await ClickAsync(VerifyButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    ///// <summary>
    ///// Clicks the login button
    ///// </summary>
    //[AllureStep("Click login button")]
    //public async Task<LoginPage> ClickLoginButtonAsync()
    //{
    //    await ClickAsync(LoginButtonSelector);
    //    Logger.LogDebug("Clicked login button");
    //    return this;
    //}

    ///// <summary>
    ///// Performs complete login action with credentials
    ///// </summary>
    ///// <param name="username">Username</param>
    ///// <param name="password">Password</param>
    //[AllureStep("Login with username: {username}")]
    //public async Task<LoginPage> LoginAsync(string username, string password)
    //{
    //    await EnterUsernameAsync(username);
    //    await EnterPasswordAsync(password);
    //    await ClickLoginButtonAsync();

    //    // Wait for login to complete (either success or error)
    //    await WaitForLoginCompleteAsync();

    //    Logger.LogInformation("Login attempt completed for user: {Username}", username);
    //    return this;
    //}

    ///// <summary>
    ///// Performs login using user credentials from configuration
    ///// </summary>
    ///// <param name="userRole">User role (optional, uses default if not specified)</param>
    //[AllureStep("Login with user role: {userRole}")]
    //public async Task<LoginPage> LoginWithUserRoleAsync(string? userRole = null)
    //{
    //    var testDataHelper = new Core.Helpers.TestDataHelper(Config, Logger);
    //    var credentials = testDataHelper.GetUserCredentials(userRole);

    //    return await LoginAsync(credentials.Username, credentials.Password);
    //}

    ///// <summary>
    ///// Toggles the "Remember Me" checkbox
    ///// </summary>
    //[AllureStep("Toggle remember me checkbox")]
    //public async Task<LoginPage> ToggleRememberMeAsync()
    //{
    //    await ClickAsync(RememberMeCheckboxSelector);
    //    Logger.LogDebug("Toggled remember me checkbox");
    //    return this;
    //}

    ///// <summary>
    ///// Clicks the "Show Password" button to toggle password visibility
    ///// </summary>
    //[AllureStep("Toggle password visibility")]
    //public async Task<LoginPage> TogglePasswordVisibilityAsync()
    //{
    //    await ClickAsync(ShowPasswordButtonSelector);
    //    Logger.LogDebug("Toggled password visibility");
    //    return this;
    //}

    ///// <summary>
    ///// Clicks the "Forgot Password" link
    ///// </summary>
    //[AllureStep("Click forgot password link")]
    //public async Task<LoginPage> ClickForgotPasswordAsync()
    //{
    //    await ClickAsync(ForgotPasswordLinkSelector);
    //    Logger.LogDebug("Clicked forgot password link");
    //    return this;
    //}

    ///// <summary>
    ///// Waits for login process to complete (success or failure)
    ///// </summary>
    //private async Task WaitForLoginCompleteAsync()
    //{
    //    // Wait for loading spinner to disappear or error message to appear
    //    try
    //    {
    //        // First wait for loading spinner if it appears
    //        if (await IsVisibleAsync(LoadingSpinnerSelector))
    //        {
    //            await WaitForElementToBeHiddenAsync(LoadingSpinnerSelector);
    //        }

    //        // Give a moment for the result to be processed
    //        await Task.Delay(1000);
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.LogDebug("No loading spinner found or error waiting for login completion: {Error}", ex.Message);
    //    }
    //}

    //// Validation Methods

    ///// <summary>
    ///// Checks if an error message is displayed
    ///// </summary>
    ///// <returns>True if error message is visible</returns>
    //[AllureStep("Check if error message is displayed")]
    //public async Task<bool> IsErrorMessageDisplayedAsync()
    //{
    //    var isVisible = await IsVisibleAsync(ErrorMessageSelector);
    //    Logger.LogDebug("Error message visible: {IsVisible}", isVisible);
    //    return isVisible;
    //}

    ///// <summary>
    ///// Gets the error message text
    ///// </summary>
    ///// <returns>Error message text or empty string if not visible</returns>
    //[AllureStep("Get error message text")]
    //public async Task<string> GetErrorMessageAsync()
    //{
    //    if (await IsErrorMessageDisplayedAsync())
    //    {
    //        var errorText = await GetTextAsync(ErrorMessageSelector) ?? string.Empty;
    //        Logger.LogDebug("Error message: {ErrorMessage}", errorText);
    //        return errorText;
    //    }
    //    return string.Empty;
    //}

    ///// <summary>
    ///// Checks if the login button is enabled
    ///// </summary>
    ///// <returns>True if login button is enabled</returns>
    //[AllureStep("Check if login button is enabled")]
    //public async Task<bool> IsLoginButtonEnabledAsync()
    //{
    //    var isEnabled = await IsEnabledAsync(LoginButtonSelector);
    //    Logger.LogDebug("Login button enabled: {IsEnabled}", isEnabled);
    //    return isEnabled;
    //}

    ///// <summary>
    ///// Checks if the remember me checkbox is checked
    ///// </summary>
    ///// <returns>True if remember me is checked</returns>
    //[AllureStep("Check if remember me is checked")]
    //public async Task<bool> IsRememberMeCheckedAsync()
    //{
    //    var isChecked = await IsCheckedAsync(RememberMeCheckboxSelector);
    //    Logger.LogDebug("Remember me checked: {IsChecked}", isChecked);
    //    return isChecked;
    //}

    ///// <summary>
    ///// Validates that all required elements are present on the page
    ///// </summary>
    ///// <returns>True if all elements are present</returns>
    //[AllureStep("Validate login page elements")]
    //public async Task<bool> ValidatePageElementsAsync()
    //{
    //    var elementsToCheck = new[]
    //    {
    //        UsernameInputSelector,
    //        PasswordInputSelector,
    //        LoginButtonSelector,
    //        RememberMeCheckboxSelector,
    //        ForgotPasswordLinkSelector
    //    };

    //    foreach (var selector in elementsToCheck)
    //    {
    //        if (!await IsVisibleAsync(selector))
    //        {
    //            Logger.LogError("Required element not found: {Selector}", selector);
    //            return false;
    //        }
    //    }

    //    Logger.LogInformation("All login page elements are present");
    //    return true;
    //}

    ///// <summary>
    ///// Gets the current values from the form fields
    ///// </summary>
    ///// <returns>Dictionary containing form field values</returns>
    //[AllureStep("Get form field values")]
    //public async Task<Dictionary<string, string>> GetFormValuesAsync()
    //{
    //    var values = new Dictionary<string, string>
    //    {
    //        ["username"] = await GetAttributeAsync(UsernameInputSelector, "value") ?? string.Empty,
    //        ["password"] = await GetAttributeAsync(PasswordInputSelector, "value") ?? string.Empty,
    //        ["rememberMe"] = (await IsRememberMeCheckedAsync()).ToString()
    //    };

    //    Logger.LogDebug("Retrieved form values");
    //    return values;
    //}

    ///// <summary>
    ///// Clears all form fields
    ///// </summary>
    //[AllureStep("Clear all form fields")]
    //public async Task<LoginPage> ClearFormAsync()
    //{
    //    await ClearAsync(UsernameInputSelector);
    //    await ClearAsync(PasswordInputSelector);

    //    // Uncheck remember me if it's checked
    //    if (await IsRememberMeCheckedAsync())
    //    {
    //        await ToggleRememberMeAsync();
    //    }

    //    Logger.LogDebug("Cleared all form fields");
    //    return this;
    //}

    /// <summary>
    /// Takes a screenshot of the login page
    /// </summary>
    /// <param name="screenshotName">Name for the screenshot</param>
    /// <returns>Path to the screenshot</returns>
    [AllureStep("Take login page screenshot")]
    public async Task<string> TakeLoginPageScreenshotAsync(string screenshotName = "login_page")
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{screenshotName}_{timestamp}";
        var screenshotPath = Path.Combine(Config.Execution.OutputDirectory, "Screenshots", $"{fileName}.png");
        
        // Ensure directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath)!);
        
        await TakeScreenshotAsync(screenshotPath);
        Logger.LogInformation("Login page screenshot saved: {Path}", screenshotPath);
        
        return screenshotPath;
    }
} 