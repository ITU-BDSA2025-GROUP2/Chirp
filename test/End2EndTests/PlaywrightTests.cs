using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.None)]
[TestFixture]
public class PlaywrightTests : PageTest
{
    private static readonly TestServerFixture Fixture = new();
    private string ServerAddress => Fixture.ServerAddress;

    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await Fixture.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await Fixture.DisposeAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(ServerAddress);
    }

    [Test]
    public async Task BasicTest()
    {
        var response = await Page.GotoAsync(ServerAddress + "/Public");
        Assert.That(response!.Status, Is.EqualTo(200));
    }


    [Test]
    public async Task RegisterAndLoginTest()
    {
        var email = "newAccount@test.com";
        var password = "Test12345!";

        await RegisterAccountIdentity(email, password);
        await LoginAccountIdentity(email, password);

        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(new Regex(".*/?"));
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Register" })).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" })).Not.ToBeVisibleAsync();
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
    public async Task DeleteAccount_AccountGetsDeleted()
    {
        var username = "DELETEME";
        var email = "DELETEME@test.com";
        var password = "123456789Ab.";

        await RegisterAccountIdentity(email, password);
        await LoginAccountIdentity(email, password);

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Login" }))
            .Not.ToBeVisibleAsync();

        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync("DELETE MY CHIRP");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await DeleteAccountIdentity();

        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync(
            "- link \"public timeline\":\n  - /url: /\n- text: \"|\"\n- list:\n  - listitem:\n    - link \"Register\":\n      - /url: /Identity/Account/Register\n  - listitem:\n    - link \"Login\":\n      - /url: /Identity/Account/Login");

        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        Assert.That(username != authorName);
    }

    
    [Test]
    public async Task SendCheepTest_UserSendsValidCheep()
    {
        // Arrange
        var email = "newCheepAccount@test.com";
        var password = "test123?T";
        var cheepText = "Hello im a real uwu";
    
        // Act
        await RegisterAccountIdentity(email, password);
        await LoginAccountIdentity(email, password);
        await Page.Locator("#Cheep_Text").ClickAsync();
        await Page.Locator("#Cheep_Text").FillAsync(cheepText);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    
        // Wait for the page to reload/update after posting
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    
        // Assert - Check that the first item in the message list is our new cheep
        var firstCheep = Page.Locator("#messagelist > li").First;
    
        // Simpler approach: verify individual elements without complex regex
        await Expect(firstCheep).ToContainTextAsync(cheepText);
        await Expect(firstCheep.GetByRole(AriaRole.Link).First).ToHaveTextAsync(email);
        await Expect(firstCheep.GetByRole(AriaRole.Link).First).ToHaveAttributeAsync("href", $"/{email}");
    
        // Verify timestamp format exists
        var cheepFullText = await firstCheep.InnerTextAsync();
        Assert.That(cheepFullText, Does.Match(@"\d{2}-\d{2}-\d{4} \d{2}:\d{2}:\d{2}"));
    }

    [Test]
    public async Task FollowAUser()
    {
        var email = "followAccount@test.com";
        var password = "test123?T";
        
        await RegisterAccountIdentity(email, password);
        await LoginAccountIdentity(email, password);
        
        await Page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Jacqualine Gilcoine I wonder" })
            .GetByRole(AriaRole.Button)
            .ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    
        var firstCheep = Page.Locator("#messagelist > li").First;
        await Expect(firstCheep).ToMatchAriaSnapshotAsync(
            "- listitem:\n  - paragraph:\n    - strong:\n      - link \"Jacqualine Gilcoine\":\n        - /url: /Jacqualine Gilcoine\n    - text: /Starbuck now is what we hear the worst\\. — \\d{2}-\\d{2}-\\d{4} \\d{2}:\\d{2}:\\d{2}/\n  - button \"Unfollow\"\n  - paragraph");
        var unfollowButtons = Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" });
        var buttonCount = await unfollowButtons.CountAsync();
        Assert.That(buttonCount, Is.GreaterThan(0), "Should have at least one Unfollow button");
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
        // Navigate to About Me page with page parameter
        await Page.GotoAsync(ServerAddress + "/info?page=1");
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine($"URL: {Page.Url}");

        // Wait for the page to render and check if the button exists
        var forgetButton = Page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" });

        // If button doesn't exist, print debug info
        if (await forgetButton.CountAsync() == 0)
        {
            var bodyText = await Page.Locator("body").InnerTextAsync();
            Console.WriteLine($"Page content: {bodyText}");
            throw new Exception("'Forget me!' button not found on /info page");
        }

        await forgetButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}