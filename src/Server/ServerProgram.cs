namespace Server;

public class ServerProgram
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        var database = CsvDatabase<Messages>.Instance;
        
        app.MapGet("/cheeps", () =>
        {
            
            return database.Read();
        });

        app.MapPost("/cheep", (Messages message) =>
        {
            database.Store(message);
        });

        app.Run();
    }
}