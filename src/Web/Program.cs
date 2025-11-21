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
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context);
        }
        
        app.Run();
        
    }
    
    public static WebApplication BuildWebApplication(string[]? args = null, string? environment = null)
    {
        var baseDir = AppContext.BaseDirectory;
        string webProjectPath;
        
        Console.WriteLine($"[DEBUG] Base directory: {baseDir}");
        
        // For Testing environment, use the output directory where files are copied
        if (environment == "Testing")
        {
            // Check if Pages exists in the current output directory
            if (Directory.Exists(Path.Combine(baseDir, "Pages")))
            {
                webProjectPath = baseDir;
                Console.WriteLine($"[DEBUG] Using test output directory: {webProjectPath}");
            }
            else
            {
                // Fallback: try to find it in the source
                webProjectPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "Web"));
                Console.WriteLine($"[DEBUG] Pages not found in output, using source: {webProjectPath}");
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
                else if (File.Exists(webCsprojInSrc))
                {
                    foundDir = new DirectoryInfo(Path.Combine(currentDir.FullName, "src", "Web"));
                    break;
                }
                
                currentDir = currentDir.Parent;
            }
            
            webProjectPath = foundDir?.FullName ?? Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "src", "Web"));
            Console.WriteLine($"[DEBUG] Using source directory: {webProjectPath}");
        }
        
        Console.WriteLine($"[DEBUG] Pages folder exists: {Directory.Exists(Path.Combine(webProjectPath, "Pages"))}");
        Console.WriteLine($"[DEBUG] wwwroot folder exists: {Directory.Exists(Path.Combine(webProjectPath, "wwwroot"))}");
        
        if (Directory.Exists(Path.Combine(webProjectPath, "Pages")))
        {
            var pageFiles = Directory.GetFiles(Path.Combine(webProjectPath, "Pages"), "*.cshtml", SearchOption.AllDirectories);
            Console.WriteLine($"[DEBUG] Found {pageFiles.Length} .cshtml files:");
            foreach (var file in pageFiles.Take(10))
            {
                Console.WriteLine($"[DEBUG]   - {file}");
            }
        }
        
        var options = new WebApplicationOptions
        {
            Args = args ?? Array.Empty<string>(),
            EnvironmentName = environment ?? Environments.Development,
            ContentRootPath = webProjectPath,
            WebRootPath = Path.Combine(webProjectPath, "wwwroot")
        };
        
        var builder = WebApplication.CreateBuilder(options);

        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();

        // Load database connection via configuration.
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ChatDbContext>(options => options.UseSqlite(connectionString));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ChatDbContext>();
        
        if (!builder.Environment.IsEnvironment("Development") && !builder.Environment.IsEnvironment("Testing"))
        {
            builder.Services.AddAuthentication()
                .AddCookie()
                .AddGitHub(o =>
                {
                    o.ClientId = builder.Configuration["authentication_github_clientId"];
                    o.ClientSecret = builder.Configuration["authentication_github_clientSecret"];
                    o.CallbackPath = new PathString("/git-login");
                });
        }
        else
        {
            builder.Services.AddAuthentication()
                .AddCookie(); // Only cookie auth for tests
        }

        // Add services to the container.
        Console.WriteLine("[DEBUG] Adding Razor Pages services...");
        var razorPagesBuilder = builder.Services.AddRazorPages();
        
        // Explicitly add the Web assembly as an application part
        var webAssembly = typeof(Program).Assembly;
        Console.WriteLine($"[DEBUG] Web assembly: {webAssembly.FullName}");
        Console.WriteLine($"[DEBUG] Web assembly location: {webAssembly.Location}");
        
        razorPagesBuilder.AddApplicationPart(webAssembly);
        
        // Enable runtime compilation for Testing and Development
        if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
        {
            Console.WriteLine("[DEBUG] Enabling Razor Runtime Compilation...");
            razorPagesBuilder.AddRazorRuntimeCompilation(options =>
            {
                var pagesPath = Path.Combine(builder.Environment.ContentRootPath, "Pages");
                Console.WriteLine($"[DEBUG] Adding file provider for: {pagesPath}");
                options.FileProviders.Clear();
                options.FileProviders.Add(new Microsoft.Extensions.FileProviders.PhysicalFileProvider(pagesPath));
            });
        }
        
        Console.WriteLine("[DEBUG] Razor Pages services added");
        
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

        Console.WriteLine("[DEBUG] About to call MapRazorPages()...");
        app.MapRazorPages();
        Console.WriteLine("[DEBUG] MapRazorPages() completed");

        var endpoints = app.Services.GetRequiredService<EndpointDataSource>().Endpoints;
        Console.WriteLine($"[DEBUG] Mapped {endpoints.Count} endpoints after MapRazorPages()");
        
        if (endpoints.Count == 0)
        {
            Console.WriteLine("[DEBUG] WARNING: No endpoints were registered!");
            Console.WriteLine($"[DEBUG] Checking assemblies:");
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name?.Contains("Web") == true || a.GetName().Name?.Contains("Razor") == true);
            foreach (var asm in assemblies)
            {
                Console.WriteLine($"[DEBUG]   - {asm.GetName().Name} ({asm.Location})");
            }
        }
        else
        {
            foreach (var endpoint in endpoints.Take(10))
            {
                Console.WriteLine($"[DEBUG] Endpoint: {endpoint.DisplayName}");
            }
        }

        return app;
    }
}