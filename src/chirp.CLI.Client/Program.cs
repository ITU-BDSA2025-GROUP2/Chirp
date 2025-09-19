namespace Chirp.CLI;

using Server;
using System.CommandLine;
using System.Net.Http.Json;

public class Program
{

    public static int Main(string[] args)
    {
        // Assign Web Host 
        var baseURL = "http://localhost:5000";
        using HttpClient client = new();
        client.BaseAddress =  new Uri(baseURL);


        // Set up CLI argument
        Option<bool> readOption = new Option<bool>("cheeps");
        Option<string?> cheepOption = new Option<string?>("cheep", "message");
        RootCommand rootCommand = new RootCommand
        {
            readOption,
            cheepOption
        };

        // Execution logic based on CLI argument
        rootCommand.SetAction(async parseResult =>
        {
            var read = parseResult.GetValue(readOption);
            var cheep = parseResult.GetValue(cheepOption);

            if (read)
            {
                // Send HTTP requests
                //var readRequestTask = client.GetAsync("/cheeps");
                var readResponse = await client.GetFromJsonAsync<IEnumerable<Messages>>("/cheeps");
                //var ou = await readResponse.Content.ReadFromJsonAsync<IEnumerable<Messages>>();
                UserInterface.PrintCheeps(readResponse);
                
            }
            else if (cheep != null)
            {
                long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string name = Environment.UserName;
                var newRecord = new Messages { Author = name, Message = cheep, Timestamp = time.ToString() };
                var json = JsonContent.Create(newRecord);

                //Console.WriteLine(await content.ReadAsStringAsync());

                var cheepRequestTask = client.PostAsync("/cheep", json);
                var cheepResponse = await cheepRequestTask;
            }

            return 0;
        });

        // Return results
        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
