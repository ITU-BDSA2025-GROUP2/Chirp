using NUnit.Framework;

namespace PlaywrightTests;

/// <summary>
/// Global setup that runs once for the entire test assembly.
/// This ensures the test server starts once and stays alive for all test classes.
/// </summary>
[SetUpFixture]
public class GlobalTestSetup
{
    private static TestServerFixture? _fixture;
    
    public static string ServerAddress => _fixture?.ServerAddress ?? 
                                          throw new InvalidOperationException("Test server not started");

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        Console.WriteLine("=== GLOBAL SETUP: Starting test server ===");
        _fixture = new TestServerFixture();
        await _fixture.StartAsync();
        Console.WriteLine($"=== GLOBAL SETUP: Test server started at {ServerAddress} ===");
    }

    [OneTimeTearDown]
    public async Task GlobalTearDown()
    {
        Console.WriteLine("=== GLOBAL TEARDOWN: Stopping test server ===");
        if (_fixture != null)
        {
            await _fixture.DisposeAsync();
            _fixture = null;
        }
        Console.WriteLine("=== GLOBAL TEARDOWN: Test server stopped ===");
    }
}