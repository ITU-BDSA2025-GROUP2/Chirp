using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http;
using System.Net.Http.Json;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
public class integration_test : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public integration_test(WebApplicationFactory<Program> factory)
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
        Assert.Contains("Leave your horses below and nerving itself to concealment.", readResponse);
    }
}