using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PlaywrightTests : PageTest
{
    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync("http://localhost:5273");
    }


    [Test]
    public void BasicTest()
    {
        Assert.Pass();
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