

using System.Formats.Tar;

public interface ICheepRepository
{
    

    Task CreateCheep(string author, string email, string msg);


    Task<List<Cheep>> ReadCheeps(int page);

    Task<List<Cheep>> ReadCheepsPerson(string name, int page);
    Task UpdateCheep(Cheep alteredCheep);


    public void CreateAuthor(string name, string email);

    public int FindNewAuthorId();

    public int FindNewCheepId();

    public Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0);
    public Task<Author> ReturnBasedOnNameAsync(string name, int page = 0);
}