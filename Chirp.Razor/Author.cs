
public class Author
{
    public int AuthorId { get; set; }
    public string name { get; set; }
    public string email { get; set; }

    public ICollection<Cheep> Cheeps { get; set; }
}