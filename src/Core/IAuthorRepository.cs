namespace Core;

public interface IAuthorRepository
{

    public void CreateAuthor(string name, string email);
    public int FindNewAuthorId();
    public Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0);
    
    public Task<List<int>> ReturnFollowAuthorsIds(string email);

    public Task<int> ReturnAuthorsId(string email);

    public Task<Author> ReturnBasedOnNameAsync(string name, int page = 0);
}