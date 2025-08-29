using CsvHelper.Configuration;
using TinyCsvParser.Mapping;

public class Messages
{
    public string Author { get; set; }
    public string Message { get; set; }
    public string Timestamp { get; set; }
}

public class csvMessageMapping : ClassMap<Messages>
{
    public csvMessageMapping()
        : base()
    {
        Map(x => x.Author).Name("Author");
        Map(x => x.Message).Name("Message");
        Map(x => x.Timestamp).Name("Timestamp");
    }
}