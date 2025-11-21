using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class InteractiveButtonTests : PageTest
{
    private readonly string _serverAddress;

    public InteractiveButtonTests()
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
    public async Task PublicTimelineClickTest()
    {
        // Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        
        // Assert
        await Expect(Page.Locator("h2")).ToMatchAriaSnapshotAsync("- heading \"Public Timeline\" [level=2]");
    }

    [Test]
    public async Task LoginClickTest()
    {
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(_serverAddress + "Identity/Account/Login");
    }

    [Test]
    public async Task RegisterClickTest()
    {
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Register" });
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(_serverAddress + "Identity/Account/Register");
    }

    [Test]
    public async Task CheepAuthorClickTest()
    {
        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        await authorLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("h2")).ToHaveTextAsync($"{authorName}'s Timeline");
    }
}
