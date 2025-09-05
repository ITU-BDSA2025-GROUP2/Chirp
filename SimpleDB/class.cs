using CsvHelper.Configuration;


public record Messages
{
    public required string Author { get; init; }
    public required string Message { get; init; }
    public required string Timestamp { get; init; }  
};

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