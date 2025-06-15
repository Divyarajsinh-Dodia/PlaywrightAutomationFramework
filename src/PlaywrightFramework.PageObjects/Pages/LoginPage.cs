using Allure.NUnit.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
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
    private const string VerifyButtonSelector = "//input[@value='Verify']";

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
            var messages = MessageResource.Read(
                to: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                limit: 1);
            var latestMessage = messages.FirstOrDefault();

            if (latestMessage != null)
            {
                string smsBody = latestMessage.Body;
                string pattern = @"\d{6}";

                Match match = Regex.Match(smsBody, pattern);

                if (match.Success)
                {
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

    /// <summary>
    /// Performs complete login
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    [AllureStep("Complete login with username: {username}")]
    public async Task<LoginPage> LoginAsync(string username, string password)
    {
        await EnterUsernameAsync(username);
        await EnterPasswordAsync(password);
        await EnterSMSAsync();
        
        Logger.LogInformation("Completed login for user: {Username}", username);
        return this;
    }
} 