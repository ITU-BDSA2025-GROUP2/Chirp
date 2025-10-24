using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using SupportScripts;

public class integration_test : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public integration_test(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task GetFromSpecificAuthor()
    {
        
        HttpClient client = _factory.CreateClient();
        var response = await client.GetAsync("/Helge");
        response.EnsureSuccessStatusCode();
        var readResponse = await response.Content.ReadAsStringAsync();
        Assert.Contains("Helge", readResponse);

    }

    [Fact]
    public async Task GetAll()
    {
        HttpClient client = _factory.CreateClient();
        var response = await client.GetAsync("");
        response.EnsureSuccessStatusCode();
        var readResponse = await response.Content.ReadAsStringAsync();
        
        Assert.Contains("For, owing to the blood of those fine whales, Hand, boys, over hand!", readResponse);
    }
}