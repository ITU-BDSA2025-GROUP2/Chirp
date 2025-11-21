using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
public class PlaywrightTests : PageTest
{
    private readonly string _serverAddress;

    public PlaywrightTests()
    {
        // CustomWebAppFactory doesn't work fully, but is left here
        //var factory = new PlaywrightCustomWebApplicationFactory();
        //factory.CreateClient();

        _serverAddress = "http://localhost:5273/";
    }

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(_serverAddress);
    }

    [Test]
    public async Task BasicTest()
    {
        await Page.GotoAsync(_serverAddress);
    }

    [Test]
    public async Task LoginLogoutIdentityTest_AccountExists()
    {
        await LoginAccountIdentity("testbot@test.com", "test123?T");

        //Assert
        await Expect(Page.Locator("body"))
            .ToMatchAriaSnapshotAsync("- link \"Hello testbot@test.com\":\n  - /url: /Identity/Account/Manage/Index");

        //Logout
        await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        //Assert
        await Expect(Page.GetByRole(AriaRole.Paragraph))
            .ToMatchAriaSnapshotAsync("- paragraph: You have successfully logged out of the application.");
    }

    [Test]
    public async Task LoginLogoutIdentityTest_AccountDoesNotExist()
    {
        var email = "fakemail@test.com";
        var password = "123456";

        await LoginAccountIdentity(email, password);

        //Assert
        await Expect(Page.GetByRole(AriaRole.Alert)).ToMatchAriaSnapshotAsync("- listitem: Invalid login attempt.");
    }

    [Test]
    public async Task CheckCheepBoxTest_BoxOnlyExistsWhenLoggedIn()
    {
        // Assert Cheep Box does not exist
        await Expect(Page.Locator("#Cheep_Text")).ToHaveCountAsync(0);

        // Log into valid account
        await LoginAccountIdentity("testbot@test.com", "test123?T");


        // Assert Cheep Box exists
        await Expect(Page.Locator("#Cheep_Text")).ToBeVisibleAsync();
    }


    [Test]
    public async Task SendCheepTest_UserSendsValidCheep()
    {
        // Arrange
        var email = "testbot@test.com";
        var password = "test123?T";
        var username = "testbot";
        var cheepMessage = "Hello everybody! This is a REAL CHEEEEEEP";

        // Act
        await LoginAccountIdentity(email, password);
        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync(cheepMessage);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        // Assert
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync(
            $"- listitem:\n  - paragraph:\n    - strong:\n      - link \"{username}\":\n        - /url: /{username}\n    - text: /{cheepMessage} — \\d+-\\d+-\\d+ \\d+:\\d+:\\d+/");
        await Page.Locator("html").ClickAsync();
    }

    
    
    [Test]
    public async Task DeleteAccount_AccountGetsDeleted()
    {
        var username = "DELETEME";
        var email = "DELETEME@test.com";
        var password = "123456789Ab.";

        await RegisterAccountIdentity(email, password);
        await LoginAccountIdentity(email, password);
        
        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync("DELETE MY CHIRP");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await DeleteAccountIdentity();
        
        //await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        //await Page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" }).ClickAsync();
        
        
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync(
            "- link \"public timeline\":\n  - /url: /\n- text: \"|\"\n- list:\n  - listitem:\n    - link \"Register\":\n      - /url: /Identity/Account/Register\n  - listitem:\n    - link \"Login\":\n      - /url: /Identity/Account/Login");

        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();
        
        Assert.That(username != authorName);
    }


    private async Task LoginAccountIdentity(string email, string password)
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }


    private async Task RegisterAccountIdentity(string email, string password)
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(password);
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
    }

    private async Task DeleteAccountIdentity()
    {
        await Page.GotoAsync(_serverAddress);
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" }).ClickAsync();
    }
}