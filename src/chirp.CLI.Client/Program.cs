using System.Net;
using CsvHelper;
using SimpleDB;

IDatabaseRepository<Messages> database = new CsvDatabase<Messages>("src/SimpleDB/CsvDatabase.cs");

if (args[0] == "read")
{
    var record = database.Read();
    UserInterfaceTests.PrintCheeps(record);
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
    var newRecord = new Messages { Author = name, Message = msg, Timestamp = time.ToString() };
    database.Store(newRecord);
}