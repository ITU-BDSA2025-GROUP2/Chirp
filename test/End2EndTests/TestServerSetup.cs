using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Infrastructure;
using Web;

namespace PlaywrightTests;

public class TestServerSetup : IAsyncDisposable
{
    private WebApplication? _app;
    public string BaseUrl { get; private set; } = default!;

    public async Task StartAsync()
    {
        _app = Program.BuildWebApplication();

        // Force Development environment to skip HTTPS redirection
        _app.Environment.EnvironmentName = "Development";

        // Seed database for tests
        using (var scope = _app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context); // ensure testbot exists
        }

        // Force Kestrel on random port
        _app.Urls.Add("http://127.0.0.1:0");
        await _app.StartAsync();

        var server = _app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()!;
        BaseUrl = addresses.Addresses.First();
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
            await _app.StopAsync();
    }
}