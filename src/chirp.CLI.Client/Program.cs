using System.Net;
using CsvHelper;
using SimpleDB;
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

namespace Chirp.CLI{
    public class Program {

    public static int Main(string[] args)
    {
        IDatabaseRepository<Messages> database = new CSVDatabase<Messages>();

        Option<bool> readOption = new Option<bool>("read");
        Option<string?> cheepOption = new Option<string?>("cheep", "message"); 
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
                var record = database.Read();
                UserInterface.PrintCheeps(record);
            }
            else if (cheep != null)
            {
                long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string name = Environment.UserName;
                var newRecord = new Messages { Author = name, Message = cheep, Timestamp = time.ToString() };
                database.Store(newRecord);
            }
            return 0;
        });

        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }

}
}
