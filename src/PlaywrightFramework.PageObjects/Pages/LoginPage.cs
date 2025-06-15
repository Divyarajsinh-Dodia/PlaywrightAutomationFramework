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
    /// Enters username using visibility-aware method (improved version)
    /// </summary>
    /// <param name="username">Username to enter</param>
    [AllureStep("Enter username (visibility-aware): {username}")]
    public async Task<LoginPage> EnterUsernameVisibleAsync(string username)
    {
        // Use the new visibility-aware method
        await FillVisibleAsync(UsernameInputSelector, username);
        Logger.LogDebug("Entered username in visible field: {Username}", username);
        
        // Click next button only if it's visible
        await ClickVisibleAsync(UsernameNextButtonSelector);
        Logger.LogDebug("Clicked visible next button after entering username");
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

    /// <summary>
    /// Enters password using visibility-aware method (improved version)
    /// </summary>
    /// <param name="password">Password to enter</param>
    [AllureStep("Enter password (visibility-aware)")]
    public async Task<LoginPage> EnterPasswordVisibleAsync(string password)
    {
        // Use the new visibility-aware method
        await FillVisibleAsync(PasswordInputSelector, password);
        Logger.LogDebug("Entered password in visible field");
        
        // Click sign in button only if it's visible
        await ClickVisibleAsync(SignInButtonSelector);
        Logger.LogDebug("Clicked visible sign in button after entering password");
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    [AllureStep("Enter SMS")]
    public async Task<LoginPage> EnterSMSAsync()
    {
        await ClickVisibleAsync(SendSMSButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        await FillAsync(OTPInputSelector, GetSMSMessage());
        await ClickAsync(VerifyButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Enters SMS using visibility-aware methods (improved version)
    /// </summary>
    [AllureStep("Enter SMS (visibility-aware)")]
    public async Task<LoginPage> EnterSMSVisibleAsync()
    {
        // Wait for SMS button to be visible before clicking
        var smsButton = await WaitForVisibleElementAsync(SendSMSButtonSelector);
        if (smsButton == null)
        {
            throw new InvalidOperationException("SMS send button is not visible");
        }
        
        await ClickVisibleAsync(SendSMSButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        
        // Wait for OTP input to become visible
        var otpInput = await WaitForVisibleElementAsync(OTPInputSelector, 10000);
        if (otpInput == null)
        {
            throw new InvalidOperationException("OTP input field did not become visible");
        }
        
        await FillVisibleAsync(OTPInputSelector, GetSMSMessage());
        await ClickVisibleAsync(VerifyButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Performs complete login with visibility-aware methods
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    [AllureStep("Complete login (visibility-aware) with username: {username}")]
    public async Task<LoginPage> LoginVisibleAsync(string username, string password)
    {
        await EnterUsernameVisibleAsync(username);
        await EnterPasswordVisibleAsync(password);
        await EnterSMSVisibleAsync();
        
        Logger.LogInformation("Completed visibility-aware login for user: {Username}", username);
        return this;
    }

    /// <summary>
    /// Checks if login form is visible and ready for interaction
    /// </summary>
    [AllureStep("Check if login form is ready")]
    public async Task<bool> IsLoginFormReadyAsync()
    {
        Logger.LogDebug("Checking if login form is ready");
        
        // Check if all required login elements are visible
        var usernameVisible = await IsAnyVisibleAsync(UsernameInputSelector);
        var nextButtonVisible = await IsAnyVisibleAsync(UsernameNextButtonSelector);
        
        var isReady = usernameVisible && nextButtonVisible;
        Logger.LogDebug("Login form ready status: {IsReady} (Username: {UsernameVisible}, Next: {NextVisible})", 
            isReady, usernameVisible, nextButtonVisible);
        
        return isReady;
    }

    /// <summary>
    /// Checks if password form is visible and ready for interaction
    /// </summary>
    [AllureStep("Check if password form is ready")]
    public async Task<bool> IsPasswordFormReadyAsync()
    {
        Logger.LogDebug("Checking if password form is ready");
        
        var passwordVisible = await IsAnyVisibleAsync(PasswordInputSelector);
        var signInVisible = await IsAnyVisibleAsync(SignInButtonSelector);
        
        var isReady = passwordVisible && signInVisible;
        Logger.LogDebug("Password form ready status: {IsReady} (Password: {PasswordVisible}, SignIn: {SignInVisible})", 
            isReady, passwordVisible, signInVisible);
        
        return isReady;
    }

    /// <summary>
    /// Checks if SMS verification form is visible
    /// </summary>
    [AllureStep("Check if SMS form is ready")]
    public async Task<bool> IsSMSFormReadyAsync()
    {
        Logger.LogDebug("Checking if SMS form is ready");
        
        var smsButtonVisible = await IsAnyVisibleAsync(SendSMSButtonSelector);
        var otpInputVisible = await IsAnyVisibleAsync(OTPInputSelector, 500); // Short timeout for OTP check
        
        var isReady = smsButtonVisible || otpInputVisible;
        Logger.LogDebug("SMS form ready status: {IsReady} (SMS Button: {SmsVisible}, OTP Input: {OtpVisible})", 
            isReady, smsButtonVisible, otpInputVisible);
        
        return isReady;
    }

    /// <summary>
    /// Gets the current login step based on visible elements
    /// </summary>
    [AllureStep("Detect current login step")]
    public async Task<string> GetCurrentLoginStepAsync()
    {
        if (await IsAnyVisibleAsync(UsernameInputSelector))
        {
            return "Username";
        }
        else if (await IsAnyVisibleAsync(PasswordInputSelector))
        {
            return "Password";
        }
        else if (await IsAnyVisibleAsync(SendSMSButtonSelector))
        {
            return "SMS_Selection";
        }
        else if (await IsAnyVisibleAsync(OTPInputSelector))
        {
            return "OTP_Entry";
        }
        else
        {
            return "Unknown";
        }
    }

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

    #region JavaScript Action Methods

    /// <summary>
    /// Enters username using JavaScript (fallback method for difficult scenarios)
    /// </summary>
    /// <param name="username">Username to enter</param>
    [AllureStep("Enter username using JavaScript: {username}")]
    public async Task<LoginPage> EnterUsernameJavaScriptAsync(string username)
    {
        Logger.LogDebug("Entering username using JavaScript: {Username}", username);
        
        // Scroll to username field first
        await ScrollToLocatorJavaScriptAsync(UsernameInputSelector);
        
        // Clear any existing value and fill with JavaScript
        await ClearJavaScriptAsync(UsernameInputSelector);
        await FillJavaScriptAsync(UsernameInputSelector, username);
        
        // Click next button using JavaScript
        await ClickJavaScriptAsync(UsernameNextButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        
        Logger.LogDebug("Successfully entered username using JavaScript");
        return this;
    }

    /// <summary>
    /// Enters password using JavaScript (fallback method for difficult scenarios)
    /// </summary>
    /// <param name="password">Password to enter</param>
    [AllureStep("Enter password using JavaScript")]
    public async Task<LoginPage> EnterPasswordJavaScriptAsync(string password)
    {
        Logger.LogDebug("Entering password using JavaScript");
        
        // Scroll to password field first
        await ScrollToLocatorJavaScriptAsync(PasswordInputSelector);
        
        // Clear any existing value and fill with JavaScript
        await ClearJavaScriptAsync(PasswordInputSelector);
        await FillJavaScriptAsync(PasswordInputSelector, password);
        
        // Click sign in button using JavaScript
        await ClickJavaScriptAsync(SignInButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        
        Logger.LogDebug("Successfully entered password using JavaScript");
        return this;
    }

    /// <summary>
    /// Enters SMS OTP using JavaScript (fallback method for difficult scenarios)
    /// </summary>
    [AllureStep("Enter SMS using JavaScript")]
    public async Task<LoginPage> EnterSMSJavaScriptAsync()
    {
        Logger.LogDebug("Entering SMS using JavaScript");
        
        // Scroll to and click SMS button using JavaScript
        await ScrollToLocatorJavaScriptAsync(SendSMSButtonSelector);
        await ClickJavaScriptAsync(SendSMSButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        
        // Wait for OTP input to appear, then scroll to it
        var otpInput = await WaitForVisibleElementAsync(OTPInputSelector, 10000);
        if (otpInput == null)
        {
            throw new InvalidOperationException("OTP input field did not become visible");
        }
        
        await ScrollToLocatorJavaScriptAsync(OTPInputSelector);
        
        // Clear and fill OTP using JavaScript
        await ClearJavaScriptAsync(OTPInputSelector);
        await FillJavaScriptAsync(OTPInputSelector, GetSMSMessage());
        
        // Click verify button using JavaScript
        await ClickJavaScriptAsync(VerifyButtonSelector);
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        
        Logger.LogDebug("Successfully entered SMS using JavaScript");
        return this;
    }

    /// <summary>
    /// Performs complete login using JavaScript methods (ultimate fallback)
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    [AllureStep("Complete login using JavaScript methods: {username}")]
    public async Task<LoginPage> LoginJavaScriptAsync(string username, string password)
    {
        Logger.LogInformation("Starting complete login using JavaScript methods");
        
        await EnterUsernameJavaScriptAsync(username);
        await EnterPasswordJavaScriptAsync(password);
        await EnterSMSJavaScriptAsync();
        
        Logger.LogInformation("Completed login using JavaScript methods for user: {Username}", username);
        return this;
    }

    /// <summary>
    /// Force clicks any login element that might be obscured (emergency method)
    /// </summary>
    /// <param name="elementSelector">Selector of element to force click</param>
    [AllureStep("Force click login element using JavaScript")]
    public async Task<LoginPage> ForceClickLoginElementAsync(string elementSelector)
    {
        Logger.LogDebug("Force clicking login element using JavaScript: {Selector}", elementSelector);
        
        // Scroll to element first
        await ScrollToLocatorJavaScriptAsync(elementSelector);
        
        // Force click using JavaScript
        await ClickJavaScriptAsync(elementSelector);
        
        Logger.LogDebug("Successfully force clicked login element: {Selector}", elementSelector);
        return this;
    }

    /// <summary>
    /// Double clicks an element for special interactions (e.g., text selection)
    /// </summary>
    /// <param name="elementSelector">Selector of element to double click</param>
    [AllureStep("Double click element using JavaScript")]
    public async Task<LoginPage> DoubleClickElementAsync(string elementSelector)
    {
        Logger.LogDebug("Double clicking element using JavaScript: {Selector}", elementSelector);
        
        // Scroll to element first
        await ScrollToLocatorJavaScriptAsync(elementSelector);
        
        // Double click using JavaScript
        await DoubleClickJavaScriptAsync(elementSelector);
        
        Logger.LogDebug("Successfully double clicked element: {Selector}", elementSelector);
        return this;
    }

    /// <summary>
    /// Clears all login form fields using JavaScript (reset form)
    /// </summary>
    [AllureStep("Clear all login form fields using JavaScript")]
    public async Task<LoginPage> ClearAllLoginFieldsJavaScriptAsync()
    {
        Logger.LogDebug("Clearing all login form fields using JavaScript");
        
        var fieldsToCheck = new[]
        {
            UsernameInputSelector,
            PasswordInputSelector,
            OTPInputSelector
        };
        
        foreach (var field in fieldsToCheck)
        {
            try
            {
                // Check if field exists and is visible before clearing
                if (await IsAnyVisibleAsync(field, 1000))
                {
                    await ClearJavaScriptAsync(field);
                    Logger.LogDebug("Cleared field: {Field}", field);
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug("Could not clear field {Field}: {Error}", field, ex.Message);
                // Continue with other fields
            }
        }
        
        Logger.LogDebug("Completed clearing all visible login form fields");
        return this;
    }

    #endregion
} 