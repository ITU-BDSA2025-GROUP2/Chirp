using System.Net;
using CsvHelper;

using SimpleDB;

IDatabaseRepository<Messages> database = new CSVDatabase<Messages>();

if (args[0] == "read")
{

    var record = database.Read();
    foreach (var rs in record)
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
    var newRecord = new Messages {Author = name, Message = msg, Timestamp = time.ToString()};
    database.Store(newRecord);
}