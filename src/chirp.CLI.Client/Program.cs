namespace chirp.CLI;

using SimpleDB;
using System.CommandLine;

public class Program
{

    public static int Main(string[] args)
    {
        // Assign Web Host 
        var baseURL = "http://localhost:5088";
        using HttpClient client = new();
        client.BaseAddress =  new Uri(baseURL);


        // Instantie CSV Database
        var database = CsvDatabase<Messages>.Instance;

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
                var readRequestTask = client.GetAsync("/cheeps");
                var readResponse = await readRequestTask;
                
                /*
                var record = database.Read();
                UserInterface.PrintCheeps(record);
                */
            }
            else if (cheep != null)
            {
                long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string name = Environment.UserName;
                //var newRecord = new Messages { Author = name, Message = cheep, Timestamp = time.ToString() };
                //database.Store(newRecord);

                var content = new StringContent(name.ToString() + "," + cheep.ToString() + "," + time.ToString());

                //Console.WriteLine(await content.ReadAsStringAsync());

                var cheepRequestTask = client.PostAsync("/cheep", content);
                var cheepResponse = await cheepRequestTask;
            }

            return 0;
        });

        // Return results
        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }
}
