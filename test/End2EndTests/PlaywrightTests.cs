using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Xunit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
public class PlaywrightTests :  PageTest
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
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync("- link \"Hello testbot@test.com\":\n  - /url: /Identity/Account/Manage/Index");

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

        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- button \"Follow\"");
        await Page.Locator("li:nth-child(16) > form > button").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- listitem:\n  - paragraph:\n    - strong:\n      - link \"Mellie Yost\":\n        - /url: /Mellie Yost\n    - text: /But what was behind the barricade\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - button \"Unfollow\"\n  - paragraph");
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

        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- button \"Follow\"");
        await Page.Locator("li:nth-child(16) > form > button").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- listitem:\n  - paragraph:\n    - strong:\n      - link \"Mellie Yost\":\n        - /url: /Mellie Yost\n    - text: /But what was behind the barricade\\. — \\d+\\/\\d+\\/\\d+ \\d+:\\d+:\\d+/\n  - button \"Unfollow\"\n  - paragraph");
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync("- button \"Unfollow\"");
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
}