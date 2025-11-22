using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Program = Web.Program;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.None)]
[TestFixture]
public class InteractiveButtonTests : PageTest
{
    private static bool _serverStarted = false;
    private static WebApplication? _app = null;
    private string _serverAddress = "http://127.0.0.1:5273";
    
    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        if (!_serverStarted)
        {
            // Check if port is already in use
            try
            {
                using var testClient = new HttpClient();
                testClient.Timeout = TimeSpan.FromSeconds(1);
                var testResult = await testClient.GetAsync(_serverAddress);
                Console.WriteLine($"WARNING: Port 5273 is already in use! Got status: {testResult.StatusCode}");
            }
            catch
            {
                Console.WriteLine("Port 5273 is free, proceeding with server start");
            }
            
            _app = Program.BuildWebApplication(environment: "Testing");
            
            // Initialize the database for testing
            using (var scope = _app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
                context.Database.EnsureDeleted(); // Clean slate for tests
                context.Database.EnsureCreated();
                DbInitializer.SeedDatabase(context);
            }
            
            Console.WriteLine("Routes:");
            foreach (var d in _app.Services.GetRequiredService<EndpointDataSource>().Endpoints)
            {
                Console.WriteLine(d.DisplayName);
            }
            
            Console.WriteLine("Content root: " + _app.Environment.ContentRootPath);
            Console.WriteLine("Pages folder exists? " + Directory.Exists(Path.Combine(_app.Environment.ContentRootPath, "Pages")));
            Console.WriteLine("wwwroot folder exists? " + Directory.Exists(Path.Combine(_app.Environment.ContentRootPath, "wwwroot")));

            _app.Urls.Clear();
            _app.Urls.Add(_serverAddress);
            
            try
            {
                // Start the server in a background task
                _ = Task.Run(async () => 
                {
                    try
                    {
                        await _app.RunAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Server crashed: " + ex.Message);
                    }
                });
                
                Console.WriteLine("Server starting on " + _serverAddress);
                
                // Give it a moment to actually start listening
                await Task.Delay(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER FAILED TO START:");
                Console.WriteLine(ex);
                throw;
            }
            
            _serverStarted = true;

            // Wait until server is actually reachable with retries
            bool serverReady = false;
            using var httpClient = new HttpClient(new HttpClientHandler 
            { 
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true 
            });
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    Console.WriteLine($"Attempt {i + 1}: Trying to connect to {_serverAddress}");
                    var result = await httpClient.GetAsync(_serverAddress);
                    Console.WriteLine($"Attempt {i + 1}: Got status code {result.StatusCode}");
                    
                    // Accept any response that isn't a connection error - even 404 means server is running
                    if ((int)result.StatusCode >= 200 && (int)result.StatusCode < 600)
                    {
                        serverReady = true;
                        Console.WriteLine($"Server is ready and responding with status {result.StatusCode}");
                        break;
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: HttpRequestException - {ex.Message}");
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: Timeout - {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {i + 1}: {ex.GetType().Name} - {ex.Message}");
                }

                await Task.Delay(500);
            }

            if (!serverReady)
            {
                throw new Exception($"Server failed to become ready within timeout period. Check if port 5273 is available and not blocked by firewall.");
            }
        }
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (_app != null)
        {
            await _app.StopAsync();
            await _app.DisposeAsync();
        }
    }
    
    [SetUp]
    public async Task Setup()
    {
        try
        {
            // Navigate to public timeline instead of root if root doesn't exist
            var targetUrl = _serverAddress + "/Public";
            await Page.GotoAsync(targetUrl, new() { WaitUntil = WaitUntilState.NetworkIdle, Timeout = 10000 });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to navigate in Setup: {ex.Message}");
            throw;
        }
    }

    [Test]
    public async Task BasicTest()
    { 
        Console.WriteLine("Testing: " + _serverAddress);
        // Test the /Public page instead of root
        var response = await Page.GotoAsync(_serverAddress + "/Public");
        Console.WriteLine("Response status: " + response.Status);
        Assert.That(response.Status, Is.EqualTo(200), "Server should return 200 OK");
        
        var content = await Page.ContentAsync();
        Assert.That(content, Is.Not.Empty, "Page content should not be empty");
    }

    [Test]
    public async Task PublicTimelineClickTest()
    {
        // Act
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Assert
        await Expect(Page.Locator("h2")).ToMatchAriaSnapshotAsync("- heading \"Public Timeline\" [level=2]");
    }

    [Test]
    public async Task LoginClickTest()
    {
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Login" });
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fixed: Added leading slash
        await Expect(Page).ToHaveURLAsync(_serverAddress + "/Identity/Account/Login");
    }

    [Test]
    public async Task RegisterClickTest()
    {
        var link = Page.GetByRole(AriaRole.Link, new() { Name = "Register" });
        await link.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Fixed: Added leading slash
        await Expect(Page).ToHaveURLAsync(_serverAddress + "/Identity/Account/Register");
    }

    [Test]
    public async Task CheepAuthorClickTest()
    {
        var cheep = Page.GetByRole(AriaRole.Paragraph).First;
        var authorLink = cheep.GetByRole(AriaRole.Link);
        var authorName = await authorLink.InnerTextAsync();

        await authorLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await Expect(Page.Locator("h2")).ToHaveTextAsync($"{authorName}'s Timeline");
    }
}