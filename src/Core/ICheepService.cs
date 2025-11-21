namespace Core;

public interface ICheepService
{
    public Task<List<Cheep>> GetCheeps(int page);
    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int page);
    public Task<Author> GetAuthor(string author, int page);
    public Task<Author> GetEmail(string email, int page);

    public Task CreateCheep(string author, string email, string msg);

    public Task DeleteAuthor(string email);
}