using CsvHelper;
using Microsoft.VisualBasic;
using System.Globalization;
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

    records.Add(new Messages { Author = name, Message = msg, Timestamp = time.ToString()});
    
    using (var writer = new StreamWriter("chirp_cli_db.csv"))
    using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csvWriter.WriteRecords(records); 
    }



}
  


