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



}