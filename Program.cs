using CsvHelper;
using Microsoft.VisualBasic;
using System.Globalization;
using System;
using System.Collections.Generic;
using DocoptNet;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.Runtime.CompilerServices;
class program {


    /*
    List<Messages> records = new List<Messages>();

    using (var reader = new StreamReader("chirp_cli_db.csv"))
    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
        csv.Context.RegisterClassMap<csvMessageMapping>();
        var record = csv.GetRecords<Messages>();
        foreach (var message in record)
        {
            records.Add(message);
        }
    }

    if (args[0] == "read")
    {

        foreach (var rs in records)
        {
            DateTimeOffset dataTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rs.Timestamp));
            DateTime time = dataTimeOffSet.DateTime;
            Console.WriteLine(rs.Author + " @ " + time + " " + rs.Message);
        }


    }
    else if (args[0] == "cheep")
    {
        long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        string name = Environment.UserName;
        string msg = "";
        foreach (var arg in args)
        {
            if (arg == "cheep")
            {
                continue;
            }
            msg = msg + " " + arg;
        }

        records.Add(new Messages { Author = name, Message = msg, Timestamp = time.ToString() });

        using (var writer = new StreamWriter("chirp_cli_db.csv"))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
        }
    }*/

    static int Main(string[] args)
    {

        Option<bool> readOption = new Option<bool>("read");
        Option<string> cheepOption = new Option<string>("cheep", "message"); 
        RootCommand rootCommand = new RootCommand
        {
             readOption,
             cheepOption
        };


        rootCommand.SetAction(parseResult =>
        {
            var read = parseResult.GetValue(readOption);
            var cheep = parseResult.GetValue(cheepOption);

            if (read)
            {
                Console.WriteLine("We are reading!!!!");
            }
            else if (cheep != null)
            {
                Console.WriteLine($"This is the message sent {cheep}");
            }
            return 0;
        });



        ParseResult parseResult = rootCommand.Parse(args);
        Console.WriteLine(parseResult.Errors);

        if (parseResult.Errors.Count > 0)
        {
            foreach (var error in parseResult.Errors)
            {
                Console.WriteLine(error.Message);
            }
        }
        parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }



static void HandleCommands()
{
    // TODO: a great app
}

}