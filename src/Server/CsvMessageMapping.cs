namespace Server;
    
using CsvHelper.Configuration;

public record Messages
{
    public required string Author { get; init; }
    public required string Message { get; init; }
    public required string Timestamp { get; init; }  
};

public class CsvMessageMapping : ClassMap<Messages>
{
    public CsvMessageMapping()
        //: base()
    {
        Map(x => x.Author).Name("Author");
        Map(x => x.Message).Name("Message");
        Map(x => x.Timestamp).Name("Timestamp");
    }
}