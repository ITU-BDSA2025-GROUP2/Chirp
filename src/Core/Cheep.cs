using System.ComponentModel.DataAnnotations;

namespace Core;

public class Cheep
{
    public required int CheepId { get; set; }
    [StringLength((160))]
    public required string Text { get; set; }
    
    //not a predefined type (needs a fix somehow)
    public required DateTime TimeStamp { get; set; } 
    public required int AuthorId { get; set; }
    public required Author Author { get; set; }

    public List<int> PeopleLikes { get; set; } =  new List<int>();
}