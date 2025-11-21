using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Web;

namespace PlaywrightTests;

public class PlaywrightCustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 1. REMOVE the existing ChatDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ChatDbContext>)
            );

            if (descriptor != null)
                services.Remove(descriptor);

            // 2. CREATE ONE SHARED IN-MEMORY SQLITE CONNECTION
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // IMPORTANT

            // 3. REGISTER the shared connection so DbContext stays alive
            services.AddSingleton(_connection);

            // 4. REGISTER ChatDbContext using the shared connection
            services.AddDbContext<ChatDbContext>((sp, options) =>
            {
                var conn = sp.GetRequiredService<SqliteConnection>();
                options.UseSqlite(conn);
            });

            // 5. BUILD THE SERVICE PROVIDER & INITIALIZE DATABASE
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _connection?.Dispose(); // keep alive until tests end
    }
}