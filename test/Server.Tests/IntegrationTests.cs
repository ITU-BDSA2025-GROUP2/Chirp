namespace Server.Tests;

using Xunit;
using Chirp.CLI;
using Microsoft.AspNetCore.Mvc.Testing;


//Andrew Lock ASP.NET Core in Action, Third Edition Chapter 35 and 36

public class IntegrationTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IntegrationTests(WebApplicationFactory<Program> factory)
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

        var firstWord = content.Split(' ')[0];
        


        Assert.Equal("ropf", content);

    }
    /* [Fact]
    public async Task Status()
    {
        // Configure a HostBuilder to define the in-memory test app
        var hostBuilder = new HostBuilder()
        .ConfigureWebHost(webHost =>
            {
                webHost.Configure(app =>
                // adds the status-middleware as the only middleware
                // in the pipeline
                app.UseMiddleware<StatusMiddleware>());

                // Configure the host to use the TestServer instead of Kestrel
                webHost.UseTestServer();
                
            });
        IHost host = await hostBuilder.StartAsync();
        // Build and start the host
        HttpClient client = host.GetTestClient();
        // Create a HttpClient for interaction
        var response = await client.GetAsync("/ping");
        // Makes an in-memory request, which is handled by the app as normal
        // Verify the response was a success
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("pong", content);
    }
 */
}