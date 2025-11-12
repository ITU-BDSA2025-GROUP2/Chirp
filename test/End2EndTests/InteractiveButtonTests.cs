using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class InteractiveButtonTests : PageTest
{
    private string _mainUrl;
    
    
    [SetUp]
    public async Task Setup()
    {
        _mainUrl = "http://localhost:5273";
        
        await Page.GotoAsync(_mainUrl);
    }


    [Test]
    public void BasicTest()
    {
        Assert.Pass();
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
        // Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        
        
        var urlToCheck = _mainUrl + "/Identity/Account/Login";
        await Expect(Page).ToHaveURLAsync(urlToCheck);
    }
    
    [Test]
    public async Task RegisterClickTest()
    {
        // Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        
        
        var urlToCheck = _mainUrl + "/Identity/Account/Register";
        await Expect(Page).ToHaveURLAsync(urlToCheck);
    }

    [Test]
    public async Task CheepAuthorClickTest()
    {
        // Arrange
        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        // Act
        await authorLink.ClickAsync();

        // Assert
        await Expect(Page.Locator("h2")).ToHaveTextAsync($"{authorName}'s Timeline");
    }
    
}