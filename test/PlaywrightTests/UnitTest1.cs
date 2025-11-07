using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;



namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{




    [SetUp]
    public async Task Setup()
    {
        
        await Page.GotoAsync("http://localhost:5273");
    }


    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public async Task LoginandLogout()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testbot@test.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("test123?T");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();


        //Assert
        await Expect(Page.Locator("body")).ToMatchAriaSnapshotAsync("- link \"Hello testbot@test.com\":\n  - /url: /Identity/Account/Manage/Index");

        //Logout
        await Page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();

        //Assert
        await Expect(Page.GetByRole(AriaRole.Paragraph)).ToMatchAriaSnapshotAsync("- paragraph: You have successfully logged out of the application.");



    }

    [Test]
    public async Task CheckCheepBox()
    {
        

        //Logging in
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testbot@test.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("test123?T");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();


        //Assert
        await Expect(Page.Locator("#Cheep_Text")).ToBeVisibleAsync();

    }
}