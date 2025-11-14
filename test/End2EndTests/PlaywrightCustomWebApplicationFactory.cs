using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web;
using Microsoft.EntityFrameworkCore;


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

    
}

/*protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureTestServices(services =>
        {
            // remove the existing context configuration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ChatDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ChatDbContext>(options =>
                options.UseInMemoryDatabase("TestDB"));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            db.Database.EnsureCreated();

            if (!db.Users.Any())
            {
                DbInitializer.SeedDatabase(db);
            }
        });
    }*/