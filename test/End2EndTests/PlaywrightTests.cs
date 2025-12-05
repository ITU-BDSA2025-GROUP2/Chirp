using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;


[Parallelizable(ParallelScope.None)]
[TestFixture]
public class PlaywrightTests : PageTest
{
    private string ServerAddress => GlobalTestSetup.ServerAddress;
    private IPage _page;
    
    [SetUp]
    public async Task Setup()
    {
        var context = await GlobalTestSetup.Browser.NewContextAsync();
        _page = await context.NewPageAsync();
        await _page.GotoAsync(ServerAddress);
    }
    
    [TearDown]
    public async Task Teardown()
    {
        await _page.Context.CloseAsync(); // clean up context after each test
    }

    [Test]
    public async Task BasicTest()
    {
        var response = await _page.GotoAsync(GlobalTestSetup.ServerAddress);
        Assert.That(response!.Status, Is.EqualTo(200));
    }


    [Test]
    public async Task RegisterTest()
    {
        var email = "newAccount@test.com";
        var password = "Test12345!";
        var username = "newAccount";

        await RegisterAccountIdentity(email, password, username);
        
        await _page.GotoAsync(ServerAddress);

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(new Regex(".*/?"));
        //await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Logout" })).ToBeVisibleAsync();
        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Login" })).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task LoginLogoutIdentityTest_AccountDoesNotExist()
    {
        var email = "fakemail@test.com";
        var password = "123456";

        await LoginAccountIdentity(email, password);

        //Assert
        await Expect(_page.GetByRole(AriaRole.Alert)).ToMatchAriaSnapshotAsync("- listitem: User not found.");
    }
    

    [Test]
    public async Task DeleteAccount_AccountGetsDeleted()
    {
        var username = "DELETEME";
        var email = "DELETEME@test.com";
        var password = "123456789Ab.";

        await RegisterAccountIdentity(email, password, username);

        await Expect(_page.GetByRole(AriaRole.Link, new() { Name = "Login" }))
            .Not.ToBeVisibleAsync();

        await _page.Locator("#Cheep_Text").ClickAsync();
        await _page.Locator("#Cheep_Text").FillAsync("DELETE MY CHIRP");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await DeleteAccountIdentity();

        await Expect(_page.Locator("body")).ToMatchAriaSnapshotAsync(
            "- link \"public timeline\":\n  - /url: /\n- text: \"|\"\n- list:\n  - listitem:\n    - link \"Register\":\n      - /url: /Identity/Account/Register\n  - listitem:\n    - link \"Login\":\n      - /url: /Identity/Account/Login");

        var cheep = _page.GetByRole(AriaRole.Paragraph).First;
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
        var username = "newCheepAccount";
    
        // Act
        await RegisterAccountIdentity(email, password, username);
        await _page.Locator("#Cheep_Text").ClickAsync();
        await _page.Locator("#Cheep_Text").FillAsync(cheepText);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
    
        // Wait for the page to reload/update after posting
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    
        // Assert - Check that the first item in the message list is our new cheep
        var firstCheep = _page.Locator("#messagelist > li").First;
    
        // Simpler approach: verify individual elements without complex regex
        await Expect(firstCheep).ToContainTextAsync(cheepText);
        await Expect(firstCheep.GetByRole(AriaRole.Link).First).ToHaveTextAsync(username);
        await Expect(firstCheep.GetByRole(AriaRole.Link).First).ToHaveAttributeAsync("href", $"/{username}");
    }

    [Test]
    public async Task FollowAUser()
    {
        var email = "followAccount@test.com";
        var password = "test123?T";
        var username = "followAccount";
        
        await RegisterAccountIdentity(email, password, username);

        await _page.GetByRole(AriaRole.Listitem)
            .Filter(new() { HasText = "Jacqualine Gilcoine I wonder" })
            .GetByRole(AriaRole.Button, new() { Name = "Follow" })
            .ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await _page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        var firstCheep = _page.Locator("#messagelist > li").First;
        await Expect(firstCheep.GetByRole(AriaRole.Link)).ToContainTextAsync("Jacqualine Gilcoine");
        await Expect(firstCheep).ToContainTextAsync("Starbuck now is what we hear the worst.");
        var unfollowButton = firstCheep.GetByRole(AriaRole.Button, new() { Name = "Unfollow" });
        await Expect(unfollowButton).ToBeVisibleAsync();

        var unfollowButtons = _page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" });
        Assert.That(await unfollowButtons.CountAsync(), Is.GreaterThan(0));
    }


    private async Task LoginAccountIdentity(string email, string password)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync(password);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
    }


    private async Task RegisterAccountIdentity(string email, string password, string username)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" }).FillAsync(username);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync(email);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Password", Exact = true }).FillAsync(password);
        await _page.GetByRole(AriaRole.Textbox, new() { Name = "Confirm Password" }).FillAsync(password);

        await _page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        //await _page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();
        //await _page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
    }

    private async Task DeleteAccountIdentity()
    {
        // Navigate to About Me page with page parameter
        await _page.GotoAsync(ServerAddress + "/info?page=1");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        Console.WriteLine($"URL: {_page.Url}");

        // Wait for the page to render and check if the button exists
        var forgetButton = _page.GetByRole(AriaRole.Button, new() { Name = "Forget me!" });

        // If button doesn't exist, print debug info
        if (await forgetButton.CountAsync() == 0)
        {
            var bodyText = await _page.Locator("body").InnerTextAsync();
            Console.WriteLine($"Page content: {bodyText}");
            throw new Exception("'Forget me!' button not found on /info page");
        }

        await forgetButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}