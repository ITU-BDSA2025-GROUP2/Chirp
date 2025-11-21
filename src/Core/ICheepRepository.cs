namespace Core;

public interface ICheepRepository
{
    
    Task CreateCheep(string author, string email, string msg);
    Task<List<Cheep>> ReadCheeps(int page);
    Task<List<Cheep>> ReadCheepsPerson(string name, int page);
    Task<List<Cheep>> ReadCheepsFollowed(List<int> follows, int page);
    public int FindNewCheepId();
}