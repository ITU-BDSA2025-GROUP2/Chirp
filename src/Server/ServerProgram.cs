namespace Server;

public class ServerProgram
{

    private static WebApplicationBuilder builder;
    private static WebApplication app;


    public static void Main(string[] args)
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

        app.StartAsync();
    }

    public static void Stop()
    {
        app.StopAsync();
    }
}