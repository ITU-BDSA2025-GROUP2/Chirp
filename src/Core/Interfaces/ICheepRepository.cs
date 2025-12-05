using Core.Model;

namespace Core.Interfaces;

public interface ICheepRepository
{
    
    Task CreateCheep(Author author, string msg);
    Task<List<Cheep>> ReadCheeps(int page);
    Task<List<Cheep>> ReadCheepsPerson(string name, int page);
    Task<List<Cheep>> ReadCheepsFollowed(List<int> follows, int page);
    public int FindNewCheepId();
    public Task<List<int>> GetLikedAuthors(int cheepId);

    public void AddlikedId(Cheep cheep, int authorId);

    public void RemovelikedId(Cheep cheep, int authorId);
}