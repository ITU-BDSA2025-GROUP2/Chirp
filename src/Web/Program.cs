using Core;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Web;

public class Program
{
    public static void Main(string[] args)
    {
        var app = BuildWebApplication(args);

        //Initialise Database
        if (!app.Environment.IsEnvironment("Testing"))
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context);
        }

        app.Run();
    }

    public static WebApplication BuildWebApplication(string[]? args = null, string? environment = null)
    {
        var baseDir = AppContext.BaseDirectory;
        string webProjectPath;

        // For Testing environment, use the output directory where files are copied
        if (environment == "Testing")
        {
            // Check if Pages exists in the current output directory
            if (Directory.Exists(Path.Combine(baseDir, "Pages")))
            {
                webProjectPath = baseDir;
            }
            else
            {
                // Fallback: try to find it in the source
                webProjectPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "Web"));
            }
        }
        else
        {
            // For non-testing, try to find the Web project source directory
            var currentDir = new DirectoryInfo(baseDir);
            DirectoryInfo? foundDir = null;

            while (currentDir != null)
            {
                var webCsprojDirect = Path.Combine(currentDir.FullName, "Web.csproj");
                var webCsprojInSrc = Path.Combine(currentDir.FullName, "src", "Web", "Web.csproj");

                if (File.Exists(webCsprojDirect))
                {
                    foundDir = currentDir;
                    break;
                }

                if (File.Exists(webCsprojInSrc))
                {
                    foundDir = new DirectoryInfo(Path.Combine(currentDir.FullName, "src", "Web"));
                    break;
                }

                currentDir = currentDir.Parent;
            }

            webProjectPath = foundDir?.FullName ??
                             Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "Web"));
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args ?? Array.Empty<string>(),
            EnvironmentName = environment ?? Environments.Development,
            ContentRootPath = webProjectPath,
            WebRootPath = Path.Combine(webProjectPath, "wwwroot")
        });

        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();

        // Configure database based on environment
        if (builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite("DataSource=TestDb;Mode=Memory;Cache=Shared"));
        }
        else
        {
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ChatDbContext>(options =>
                options.UseSqlite(connectionString));
        }

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ChatDbContext>();


        // Load User Secrets given Dev/Testing environment
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            builder.Configuration.AddUserSecrets<Program>(optional: true);
        }

        var githubClientId = builder.Configuration["authentication_github_clientId"];
        var githubClientSecret = builder.Configuration["authentication_github_clientSecret"];
        var hasGitHubCredentials = !string.IsNullOrEmpty(githubClientId) && !string.IsNullOrEmpty(githubClientSecret);

        // Ensure github credentials exist
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("Testing") &&
            !hasGitHubCredentials)
        {
            throw new Exception(
                "GitHub OAuth credentials missing");
        }

        var authBuilder = builder.Services.AddAuthentication().AddCookie();
        if (hasGitHubCredentials)
        {
            Console.WriteLine($"[DEBUG] GitHub OAuth enabled - ClientId: {githubClientId}");
            authBuilder.AddGitHub(o =>
            {
                o.ClientId = githubClientId!;
                o.ClientSecret = githubClientSecret!;
                o.CallbackPath = new PathString("/git-login");
            });
        }
        else
        {
            Console.WriteLine("[DEBUG] GitHub OAuth disabled - no credentials configured");
        }
        
        // Configure Razor Pages with specific settings for testing
        var razorPagesBuilder = builder.Services.AddRazorPages(options =>
        {
            // Set the root directory for pages
            options.RootDirectory = "/Pages";
        });

        // Enable runtime compilation for Testing and Development
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            razorPagesBuilder.AddRazorRuntimeCompilation();
        }

        // Explicitly configure MVC to use the Web assembly
        builder.Services.AddMvc()
            .AddApplicationPart(typeof(Program).Assembly);

        builder.Services.AddScoped<ICheepService, CheepService>();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Define configuration settings for our Identity
        });

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsEnvironment("Testing"))
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }
        }

        if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        app.MapRazorPages();

        return app;
    }
}