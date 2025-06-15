using Allure.NUnit.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using PlaywrightFramework.Core.Base;
using PlaywrightFramework.Core.Configuration;
using PlaywrightFramework.Core.Extensions;
using System.Text.RegularExpressions;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PlaywrightFramework.PageObjects.Pages;

/// <summary>
/// Page Object for Login page demonstrating best practices with ILocator extensions
/// </summary>
public class LoginPage : FluentBasePage
{
    // Page URL
    private const string PageUrl = "https://desd365-rolltest.sandbox.operations.dynamics.com/?cmp=US01&mi=DefaultDashboard";
    
    // ILocator properties - clean and type-safe
    private ILocator UsernameInput => Locate("//input[@name='loginfmt']");
    private ILocator NextButton => Locate("//input[@type='submit' and @value='Next']");
    private ILocator PasswordInput => Locate("//input[@Placeholder='Password']");
    private ILocator SignInButton => Locate("//input[@value='Sign in']");
    private ILocator SendSMSButton => Locate("//div[@data-value='OneWaySMS' and @role='button']");
    private ILocator OTPInput => Locate("//input[@placeholder='Code']");
    private ILocator VerifyButton => Locate("//input[@value='Verify']");

    // Navigation properties for fluent API
    private HomePage _homePage;
    
    /// <summary>
    /// Access to HomePage for fluent navigation
    /// </summary>
    public HomePage HomePage => _homePage ??= PageFactory.GetPage<HomePage>();

    public LoginPage(IPage page, TestConfiguration config, ILogger<LoginPage> logger, PageFactory pageFactory) 
        : base(page, config, logger, pageFactory)
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
        await UsernameInput.FillAsync(username);           // Beautiful extension method syntax!
        Logger.LogDebug("Entered username: {Username}", username);
        await NextButton.ClickAsync();                     // Clean and intuitive!
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
        await PasswordInput.FillAsync(password);           // Perfect syntax!
        Logger.LogDebug("Entered password");
        await SignInButton.ClickAsync();                   // So readable!
        Logger.LogDebug("Clicked sign in button after entering password");
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    [AllureStep("Enter SMS")]
    public async Task<LoginPage> EnterSMSAsync()
    {
        await SendSMSButton.ClickAsync();                  // Gorgeous!
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        await OTPInput.FillAsync(GetSMSMessage());         // Clean and clear!
        await VerifyButton.ClickAsync();                   // Perfect!
        await WaitForLoadAsync(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Validation methods using query extensions
    /// </summary>
    public async Task<bool> IsLoginFormReadyAsync()
    {
        return await UsernameInput.IsVisibleAsync() &&     // LocatorQueryExtensions
               await NextButton.IsEnabledAsync();          // LocatorQueryExtensions
    }

    public async Task<string?> GetUsernameValueAsync()
    {
        return await UsernameInput.GetValueAsync();        // Convenience method from LocatorQueryExtensions
    }

    public async Task<string?> GetPasswordPlaceholderAsync()
    {
        return await PasswordInput.GetPlaceholderAsync();  // Convenience method from LocatorQueryExtensions
    }

    /// <summary>
    /// Wait methods using wait extensions
    /// </summary>
    public async Task WaitForPasswordFieldAsync()
    {
        await PasswordInput.WaitForAsync();                // LocatorWaitExtensions
    }

    public async Task WaitForUsernameToBeEditableAsync()
    {
        await UsernameInput.WaitToBeEditableAsync();       // LocatorWaitExtensions
    }

    /// <summary>
    /// Advanced action examples
    /// </summary>
    public async Task TryEnterUsernameAsync(string username)
    {
        var success = await UsernameInput.TryFillAsync(username); // LocatorActionExtensions
        if (success)
        {
            await NextButton.ClickIfEnabledAsync();        // LocatorActionExtensions
        }
    }

    // Fluent (synchronous) methods for method chaining

    /// <summary>
    /// Navigates to the login page
    /// </summary>
    [AllureStep("Navigate to login page")]
    public LoginPage NavigateToLoginPage()
    {
        NavigateTo(PageUrl);
        WaitForLoad();
        Logger.LogInformation("Navigated to login page");
        return this;
    }

    /// <summary>
    /// Enters username in the username field
    /// </summary>
    [AllureStep("Enter username: {username}")]
    public LoginPage EnterUsername(string username)
    {
        UsernameInput.FillAsync(username).GetAwaiter().GetResult();
        Logger.LogDebug("Entered username: {Username}", username);
        NextButton.ClickAsync().GetAwaiter().GetResult();
        Logger.LogDebug("Clicked next button after entering username");
        WaitForLoad(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Enters password in the password field
    /// </summary>
    [AllureStep("Enter password")]
    public LoginPage EnterPassword(string password)
    {
        PasswordInput.FillAsync(password).GetAwaiter().GetResult();
        Logger.LogDebug("Entered password");
        SignInButton.ClickAsync().GetAwaiter().GetResult();
        Logger.LogDebug("Clicked sign in button after entering password");
        WaitForLoad(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Enters SMS verification code
    /// </summary>
    [AllureStep("Enter SMS")]
    public LoginPage EnterSMS()
    {
        SendSMSButton.ClickAsync().GetAwaiter().GetResult();
        WaitForLoad(WaitUntilState.NetworkIdle);
        OTPInput.FillAsync(GetSMSMessage()).GetAwaiter().GetResult();
        VerifyButton.ClickAsync().GetAwaiter().GetResult();
        WaitForLoad(WaitUntilState.NetworkIdle);
        return this;
    }

    /// <summary>
    /// Performs complete login and returns HomePage for fluent navigation
    /// </summary>
    [AllureStep("Complete login with username: {username}")]
    public HomePage Login(string username, string password)
    {
        EnterUsername(username);
        EnterPassword(password);
        EnterSMS();
        
        Logger.LogInformation("Completed login for user: {Username}", username);
        return HomePage;
    }

    /// <summary>
    /// Click Login Button and returns HomePage for fluent navigation
    /// </summary>
    [AllureStep("Click login button")]
    public HomePage ClickOnLoginButton()
    {
        // This method assumes username and password are already entered
        // and just clicks the final login button and returns the HomePage
        EnterSMS();
        Logger.LogInformation("Clicked login button and navigated to home page");
        return HomePage;
    }
}