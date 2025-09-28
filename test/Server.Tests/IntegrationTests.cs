namespace Server.Tests;

using Xunit;
using Chirp.CLI;
using Microsoft.AspNetCore.Mvc.Testing;
using DocoptNet;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;


//Andrew Lock ASP.NET Core in Action, Third Edition Chapter 35 and 36

public class IntegrationTests :
    IClassFixture<WebApplicationFactory<ServerProgram>>
{
    private readonly WebApplicationFactory<ServerProgram> _factory;

    public IntegrationTests(WebApplicationFactory<ServerProgram> factory)
    {
        _factory = factory;
    }
    [Fact]
    public async Task CheepsRequest_ReturnsCheeps()
    {
        HttpClient client = _factory.CreateClient();

        var response = await client.GetAsync("/cheeps");

        response.EnsureSuccessStatusCode();

        var readResponse = await response.Content.ReadFromJsonAsync<IEnumerable<Messages>>();

        var firstMessage = readResponse?.FirstOrDefault();

        Assert.Equal("ropf", firstMessage?.Author);
        Assert.Equal("Hello, BDSA students!", firstMessage?.Message);
        Assert.Equal("1690891760", firstMessage?.Timestamp.ToString());
    }

    [Fact]
    public async Task StoreRequest_ReturnsNewStoredCheep()
    {

        var newRecord = new Messages { Author = "Vee", Message = "test", Timestamp = 123456789 };
        var json = JsonContent.Create(newRecord);

        HttpClient client = _factory.CreateClient();

        var response = await client.PostAsync("/cheep", json);

        response.EnsureSuccessStatusCode();

        var answer = await client.GetAsync("/cheeps");

        answer.EnsureSuccessStatusCode();

        var readResponse = await answer.Content.ReadFromJsonAsync<IEnumerable<Messages>>();

        var firstMessage = readResponse?.LastOrDefault();

        Assert.Equal("Vee", firstMessage?.Author);
        Assert.Equal("test", firstMessage?.Message);
        Assert.Equal("123456789", firstMessage?.Timestamp.ToString());
    }

}