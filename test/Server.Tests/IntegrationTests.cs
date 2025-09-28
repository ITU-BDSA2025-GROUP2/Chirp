namespace Server.Tests;

using Xunit;
using Chirp.CLI;
using Microsoft.AspNetCore.Mvc.Testing;
using DocoptNet;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;


public record Messages
{
    public required string Author { get; init; }
    public required string Message { get; init; }
    public required long Timestamp { get; init; }  
};

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
        var content = await response.Content.ReadAsStringAsync();

        //Convert this into a json deserialisation.
        var stringcombination = content[12].ToString() + content[13] + content[14] + content[15];
        String Messagecombination = "";
        String timestampcombination = "";
        for (int i = 29; i < 50; i++)
        {
            Messagecombination = Messagecombination + content[i];
        }

        for (int x = 64; x < 74; x++)
        {
            timestampcombination = timestampcombination + content[x];
        }

        Assert.Equal("ropf", stringcombination);
        Assert.Equal("Hello, BDSA students!", Messagecombination);
        Assert.Equal("1690891760", timestampcombination);

    }

    

}