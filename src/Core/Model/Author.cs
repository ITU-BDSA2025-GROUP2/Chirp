
using System.ComponentModel.DataAnnotations;

namespace Core.Model;

public class Author
{
    public required int AuthorId { get; set; }
    [StringLength(200)]
    public required string Name { get; set; }
    [StringLength(200)]
    public required string Email { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; }

    public List<int> Follows { get; set; } = new List<int>();
    
    public List<int> CheepLikes { get; set; } = new List<int>();
}