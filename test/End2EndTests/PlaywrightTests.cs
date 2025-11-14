using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
public class PlaywrightTests :  PageTest, IClassFixture<PlaywrightCustomWebApplicationFactory>, IDisposable, IAsyncLifetime
{
    private readonly string _serverAddress;
    private IPage _page;
    
    private IPlaywright _playWright;
    private IBrowser _browser;

    public PlaywrightTests()
    {
        var factory = new PlaywrightCustomWebApplicationFactory();
        _serverAddress = factory.ServerAddress;
        factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        _playWright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playWright.Chromium.LaunchAsync();
    }

    public async Task CreatePageAsync()
    {
        _page = await _browser.NewPageAsync();
        await _page.GotoAsync(_serverAddress);
    }
    
    public async Task DisposeAsync()
    {
        await _browser.CloseAsync();
        _playWright.Dispose();
    }
    
    public void Dispose()
    {
        _page?.CloseAsync().GetAwaiter().GetResult();
        _page = null;
    }
    
    [Fact]
    public async Task BasicTest()
    { 
        await CreatePageAsync();
        await _page.GotoAsync(_serverAddress);
    }

    [Fact]
    public async Task LoginLogoutIdentityTest_AccountExists()
    {
        await CreatePageAsync();
        await LoginAccountIdentity("testbot@test.com", "test123?T");

        //Assert
        await Expect(_page.Locator("body"))
            .ToMatchAriaSnapshotAsync("- link \"Hello testbot@test.com\":\n  - /url: /Identity/Account/Manage/Index");

        //Logout
        await _page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        //Assert
        await Expect(_page.GetByRole(AriaRole.Paragraph))
            .ToMatchAriaSnapshotAsync("- paragraph: You have successfully logged out of the application.");
    }

    [Fact]
    public async Task LoginLogoutIdentityTest_AccountDoesNotExist()
    {
        await CreatePageAsync();
        var email = "fakemail@test.com";
        var password = "123456";

        await LoginAccountIdentity(email, password);

        //Assert
        await Expect(_page.GetByRole(AriaRole.Alert)).ToMatchAriaSnapshotAsync("- listitem: Invalid login attempt.");
    }

    [Fact]
    public async Task CheckCheepBoxTest_BoxOnlyExistsWhenLoggedIn()
    {
        await CreatePageAsync();
        // Assert Cheep Box does not exist
        await Expect(_page.Locator("#Cheep_Text")).ToHaveCountAsync(0);

        // Log into valid account
        await LoginAccountIdentity("testbot@test.com", "test123?T");


        // Assert Cheep Box exists
        await Expect(_page.Locator("#Cheep_Text")).ToBeVisibleAsync();
    }


    [Fact]
    public async Task SendCheepTest_UserSendsValidCheep()
    {
        await CreatePageAsync();
        // Arrange
        var email = "testbot@test.com";
        var password = "test123?T";
        var username = "testbot";
        var cheepMessage = "Hello everybody! This is a REAL CHEEEEEEP";
        
        // Act
        await LoginAccountIdentity(email, password);
        await _page.Locator("#Cheep_Text").ClickAsync();
        await _page.Locator("#Cheep_Text").FillAsync(cheepMessage);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        // Assert
        await Expect(_page.Locator("#messagelist")).ToMatchAriaSnapshotAsync(
            $"- listitem:\n  - paragraph:\n    - strong:\n      - link \"{username}\":\n        - /url: /{username}\n    - text: /{cheepMessage} — \\d+-\\d+-\\d+ \\d+:\\d+:\\d+/");
        await _page.Locator("html").ClickAsync();
    }


    private async Task LoginAccountIdentity(string email, string password)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }
}