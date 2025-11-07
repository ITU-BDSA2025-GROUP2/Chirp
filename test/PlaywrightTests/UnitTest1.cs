using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
   

   

    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public async Task HasTitle()
    {
       await Page.GotoAsync("https://localhost:5274");
       
       await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }
}