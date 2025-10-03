

public class Cheep
{
    public int CheepId { get; set; }
    public string text { get; set; }
    public DateTime timestamp { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }

}