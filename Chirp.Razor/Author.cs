
public class Author
{
    string name { get; set; }
    string email { get; set; }

    ICollection<Cheep> Cheeps { get; set; }
}