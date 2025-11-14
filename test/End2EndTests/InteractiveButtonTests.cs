using Microsoft.Playwright;
//using Microsoft.Playwright.Xunit;
using Microsoft.Playwright.NUnit;
using Xunit;
using Assert = NUnit.Framework.Assert;


namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class InteractiveButtonTests : PageTest, IClassFixture<PlaywrightCustomWebApplicationFactory>, IDisposable, IAsyncLifetime
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

    public async Task InitializeAsync()
    {
        //_playWright = await Microsoft.Playwright.Playwright.CreateAsync();
        //_browser = await _playWright.Chromium.LaunchAsync();
    }

    public async Task CreatePageAsync()
    {
        //_page = await _browser.NewPageAsync();
        //await _page.GotoAsync(_serverAddress);
    }
    
    public async Task DisposeAsync()
    {
        //await _browser.CloseAsync();
        //_playWright.Dispose();
    }
    
    public void Dispose()
    {
        //_page?.CloseAsync().GetAwaiter().GetResult();
        //_page = null;
    }
    
    [Test]
    public async Task BasicTest()
    { 
        await CreatePageAsync();
        
        Assert.NotNull(_serverAddress);
        
        await Page.GotoAsync(_serverAddress);
    }

    [Test]
    public async Task PublicTimelineClickTest()
    {
        await CreatePageAsync();
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" });
        await link.WaitForAsync();
        await link.ClickAsync();
        await Expect(Page.Locator("h2")).ToHaveTextAsync("Public Timeline");
    }

    [Test]
    public async Task LoginClickTest()
    {
        await CreatePageAsync();
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });
        //await link.WaitForAsync();
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(_serverAddress + "Identity/Account/Login");
    }

    [Test]
    public async Task RegisterClickTest()
    {
        await CreatePageAsync();
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Register" });
        await link.WaitForAsync();
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await Expect(Page).ToHaveURLAsync(_serverAddress + "Identity/Account/Register");
    }

    [Test]
    public async Task CheepAuthorClickTest()
    {
        await CreatePageAsync();
        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        //await cheep.WaitForAsync();

        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        await authorLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("h2")).ToHaveTextAsync($"{authorName}'s Timeline");
    }
}
