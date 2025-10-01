namespace Server;

public class ServerProgram
{

    private static WebApplicationBuilder builder;
    private static WebApplication app;
    
    private static CancellationTokenSource _cts = new CancellationTokenSource();


    public static async Task Main(string[] args)
    {
        builder = WebApplication.CreateBuilder(args);
        app = builder.Build();
        var database = CsvDatabase<Messages>.Instance;

        app.MapGet("/cheeps", () =>
        {

            return database.Read();
        });

        app.MapPost("/cheep", (Messages message) =>
        {
            database.Store(message);
        });

        await app.RunAsync(_cts.Token);
    }

    public static void Stop()
    {
        _cts.Cancel();
    }
}