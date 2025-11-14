using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Xunit;


namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class InteractiveButtonTests : PageTest, IClassFixture<PlaywrightCustomWebApplicationFactory>
{
    private readonly string _serverAddress;
   // private IPage _page;
    
    //private IPlaywright _playWright;
    //private IBrowser _browser;

    public InteractiveButtonTests()
    {
        var factory = new PlaywrightCustomWebApplicationFactory();
        factory.CreateClient();
        
        _serverAddress = factory.ServerAddress;
    }

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync(_serverAddress);
    }

    [Test]
    public async Task BasicTest()
    { 
        //Assert.NotNull(_serverAddress);
        
        await Page.GotoAsync(_serverAddress);
    }

    [Test]
    public async Task PublicTimelineClickTest()
    {
        //await Page.GotoAsync(_serverAddress);
        //var link = Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" });
        /*await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Expect(Page.Locator("h2")).ToHaveTextAsync("Public Timeline");*/

        // Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        
        // Assert
        await Expect(Page.Locator("h2")).ToMatchAriaSnapshotAsync("- heading \"Public Timeline\" [level=2]");
    }

    [Test]
    public async Task LoginClickTest()
    {
        //await Page.GotoAsync(_serverAddress);
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });
        //await link.WaitForAsync();
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(_serverAddress + "Identity/Account/Login");
    }

    [Test]
    public async Task RegisterClickTest()
    {
        //await Page.GotoAsync(_serverAddress);
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Register" });
        //await link.WaitForAsync();
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(_serverAddress + "Identity/Account/Register");
    }

    [Test]
    public async Task CheepAuthorClickTest()
    {
        //await Page.GotoAsync(_serverAddress);
        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        //await cheep.WaitForAsync();

        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        await authorLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("h2")).ToHaveTextAsync($"{authorName}'s Timeline");
    }
}
