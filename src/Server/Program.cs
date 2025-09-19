namespace Server;

using System.Net.Http.Json;
using System.Text.Json;
class Program
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
            Console.WriteLine("IM RUNNING HERE!");
            //var jsonString = JsonSerializer.Serialize(message);
            database.Store(message);
        });

        app.Run();
    }



}