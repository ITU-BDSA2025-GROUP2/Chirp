using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
public class PlaywrightTests : PageTest
{
    private static TestServerFixture _fixture = new();
    private string ServerAddress => _fixture.ServerAddress;
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await _fixture.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _fixture.DisposeAsync();
    }
    
    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(ServerAddress); // Always return to root homepage
    }

    [Test]
    public async Task BasicTest()
    { 
        var response = await Page.GotoAsync(ServerAddress + "/Public");
        Assert.That(response!.Status, Is.EqualTo(200));
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
        // Act
        await LoginAccountIdentity(email, password);
        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync("Hello im a real uwu");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- listitem:\n  - paragraph:\n    - strong:\n      - link \"testbot@test.com\":\n        - /url: /testbot@test.com\n    - text: /Hello im a real uwu — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/");

    }

    [Test]
    public async Task FollowAUser()
    {
        var email = "testbot@test.com";
        var password = "test123?T";
        var username = "testbot";
        // Act
        await LoginAccountIdentity(email, password);

        await Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Jacqualine Gilcoine I wonder" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- listitem:\n  - paragraph:\n    - strong:\n      - link \"Jacqualine Gilcoine\":\n        - /url: /Jacqualine Gilcoine\n    - text: /Starbuck now is what we hear the worst\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - button \"Unfollow\"\n  - paragraph");
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- button \"Unfollow\"");
    }

    [Test]
    public async Task UnfollowAUser()
    {
        var email = "testbot@test.com";
        var password = "test123?T";
        var username = "testbot";
        // Act
        await LoginAccountIdentity(email, password);

        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- button \"Unfollow\"");
        await Page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Button).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- button \"Follow\"");
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- listitem:\n  - paragraph:\n    - strong:\n      - link \"Jacqualine Gilcoine\":\n        - /url: /Jacqualine Gilcoine\n    - text: /I wonder if he''d give a very shiny top hat and my outstretched hand and countless subtleties, to which it contains\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - button \"Follow\"\n  - paragraph");

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
        await Page.GotoAsync(ServerAddress);
        await Page.GetByRole(AriaRole.Link, new() { Name = "About Me" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" }).ClickAsync();
    }
}