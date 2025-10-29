using Chirp.Razor.Chirp.Infrastructure.Chirp.Services;
using Microsoft.EntityFrameworkCore;
using DbInit;
using Infrastructure;
using Infrastructure.Chirp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

public class Program
{

    private static CancellationTokenSource _cts;

    public static async Task<int> Main(string[] args)
    {
        _cts = new CancellationTokenSource();
        var builder = WebApplication.CreateBuilder(args);

        // Load database connection via configuration
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<ChatDBContext>(options => options.UseSqlite(connectionString));
        

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ChatDBContext>();

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication:github:clientId"];
                o.ClientSecret = builder.Configuration["authentication:github:clientSecret"];
                o.CallbackPath = "/signin-github";
            });
        
        
        
        
        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<ICheepService, CheepService>();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();

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
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        //Initialise Database
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ChatDBContext>();

            DbInitializer.SeedDatabase(context);
        }


        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.MapRazorPages();

        await app.RunAsync(_cts.Token);
        return 0;
    }
    
    public static void Stop()
    {
        _cts.Cancel();
    } 
}
    
    

    



//

