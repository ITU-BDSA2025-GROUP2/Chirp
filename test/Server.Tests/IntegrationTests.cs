namespace Server.Tests;

using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Server;

//Andrew Lock ASP.NET Core in Action, Third Edition Chapter 35 and 36

public class IntegrationTests :
    IClassFixture<WebApplicationFactory<Chirp.CLI>>
{
    private readonly WebApplicationFactory<Chirp.CLI> _factory;

    public IntegrationTests(WebApplicationFactory<Chirp.CLI> factory)
    {
        _factory = factory;
    }
    [Fact]
    public async Task CheepsRequest_RetrunsCheeps()
    {
        HttpClient client = _factory.CreateClient();

        var response = await client.GetAsync("/cheeps");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Equal("cheeps output", content);
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