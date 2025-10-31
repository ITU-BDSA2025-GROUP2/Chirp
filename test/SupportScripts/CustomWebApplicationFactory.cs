using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;
using SupportScripts;


public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    MemoryDBFactory mem = new MemoryDBFactory();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        
        builder.ConfigureTestServices(services =>
        {
            // remove the existing context configuration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ChatDBContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ChatDBContext>(options =>
                options.UseInMemoryDatabase("TestDB"));
        });
    }

}