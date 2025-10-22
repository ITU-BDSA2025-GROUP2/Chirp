using System.ComponentModel.DataAnnotations;

public class Cheep
{
    public int CheepId { get; set; }
    [StringLength((160))]
    public string Text { get; set; }
    
    //not a predefined type (needs a fix somehow)
    public DateTime TimeStamp { get; set; } 
    public int AuthorId { get; set; }
    public Author Author { get; set; }

}