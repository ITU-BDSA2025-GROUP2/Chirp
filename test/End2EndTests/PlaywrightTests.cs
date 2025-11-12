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
    public async Task LoginLogoutTest_AccountExists()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testbot@test.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("test123?T");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

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
    public async Task LoginLogoutTest_AccountDoesNotExist()
    {
        var email = "fakemail@test.com";
        var password = "123456";

        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        //Assert
        await Expect(Page.GetByRole(AriaRole.Alert)).ToMatchAriaSnapshotAsync("- listitem: Invalid login attempt.");
    }

    [Test]
    public async Task CheckCheepBoxTest_BoxOnlyExistsWhenLoggedIn()
    {
        // Assert Cheep Box does not exist
        await Expect(Page.Locator("#Cheep_Text")).ToHaveCountAsync(0);

        // Log into valid account
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testbot@test.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("test123?T");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();


        // Assert Cheep Box exists
        await Expect(Page.Locator("#Cheep_Text")).ToBeVisibleAsync();
    }


    [Test]
    public async Task SendCheepTest_UserSendsValidCheep()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testbot@test.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("test123?T");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync("Hello everybody! This is a REAL CHEEEEEEP");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        
        
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync(
            "- listitem:\n  - paragraph:\n    - strong:\n      - link \"testbot\":\n        - /url: /testbot\n    - text: /Hello everybody! This is a REAL CHEEEEEEP — \\d+-\\d+-\\d+ \\d+:\\d+:\\d+/");
        await Page.Locator("html").ClickAsync();
    }


    [Test]
    public async Task SendCheepTest_UserSendsInvalidCheep()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testbot@test.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("test123?T");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync("Hello everybody! THIIIIIIIIIIS IIIIIIIIS AAAAAAAAAAA FAAAAAAAAAAAAAAAAAAAAAKE ASSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSS CHEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEP");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        
        
        await Expect(Page.Locator("#messagelist")).ToMatchAriaSnapshotAsync(
            "- listitem:\n  - paragraph:\n    - strong:\n      - link \"testbot\":\n        - /url: /testbot\n    - text: /Hello everybody! This is a REAL CHEEEEEEP — \\d+-\\d+-\\d+ \\d+:\\d+:\\d+/");
        await Page.Locator("html").ClickAsync();
    }
}