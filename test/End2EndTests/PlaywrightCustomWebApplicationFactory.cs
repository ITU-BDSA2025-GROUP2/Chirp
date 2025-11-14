using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web;


namespace PlaywrightTests;

public class PlaywrightCustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private IHost? _host;
    public string ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress.ToString();
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        //builder.UseEnvironment("Testing");

        var testHost = builder.Build();
        
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        
        _host = builder.Build();

        _host.Start();

        var server = _host.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>();
        
        ClientOptions.BaseAddress = addresses!.Addresses.Select(x => new Uri(x)).Last();  

        testHost.Start();  

        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        _host?.Dispose();
        //base.Dispose(disposing);
    }


    private void EnsureServer()
    {
        if (_host is null)
        {
            using var _ = CreateDefaultClient();
        }
    }

    
/*
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        builder.ConfigureTestServices(services =>
        {
            // remove the existing context configuration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ChatDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ChatDbContext>(options =>
                options.UseInMemoryDatabase("TestDB"));
        });
    }*/

    /*protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // 1. Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // 2. Register test DB (here: InMemory)
            services.AddDbContext<ChatDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDB");
            });

            // 3. Seed database
            using var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

            db.Database.EnsureCreated();

            SeedDatabase(db);   // <-- your custom seed method
        });
    }*/
}