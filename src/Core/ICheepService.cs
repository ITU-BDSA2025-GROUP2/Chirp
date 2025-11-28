namespace Core;

public interface ICheepService
{
    public Task<List<Cheep>> GetCheeps(int page);
    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int page);
    public Task<List<Cheep>> GetCheepsFromFollowed(List<int> follows, int page);
    public Task<Author> GetAuthor(string author, int page);
    public Task<int> GetAuthorId(string email);
    public Task<Author> GetEmail(string email, int page);
    public Task<List<int>> GetFollowers(string email);
    public Task CreateCheep(string author, string email, string msg);
    public Task CreateAuthor(string author, string email);
    public void AddFollowerId(Author author, int id);
    public void RemoveFollowerId(Author author, int id);
    public Task DeleteAuthor(string email);

    public Task<List<CheepViewModel>> GetAllCheeps(string userEmail, int page);
}