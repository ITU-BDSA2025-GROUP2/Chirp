using CsvHelper;
using Microsoft.VisualBasic;
using System.Globalization;

if (args[0] == "read")
{
    //public IEnumerable<T> Read(int? limit = null) {
    using var reader = new StreamReader("chirp_cli_db.csv");
    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
    csv.Context.RegisterClassMap<csvMessageMapping>();
    var record = csv.GetRecords<Messages>();

    foreach (var rs in record)
    {
        DateTimeOffset dataTimeOffSet = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rs.Timestamp));
        DateTime time = dataTimeOffSet.DateTime;
        Console.WriteLine(rs.Author + " @ " + time + " " + rs.Message);
    }
    //}
}
else if (args[0] == "cheep")
{
    //public void Store(T record) {
    using var writer = new StreamWriter("chirp_cli_db.csv", true);
    using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

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
    csvWriter.WriteRecord(newRecord);
    //}
}