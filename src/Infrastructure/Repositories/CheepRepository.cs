

public class CheepRepository : ICheepRepository
{

    private readonly ChatDBContext _dbContext;
    public CheepRepository(ChatDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task CreateCheep(Cheep newCheep)
    {
        return null;
    }


    public async void CreateAuthor(string name, string email)
    {
        var newAuthor = new Author() { AuthorId = await FindNewId(), Name = name, Email = email };

        _dbContext.Authors.Add(newAuthor);
    }

    public async Task<int> FindNewId()
    {
        var length = _dbContext.Authors.Count();
        return length + 1;
    }

    public Task<List<Cheep>> ReadCheeps(string name)
    {
        return null;
    }

    public Task UpdateCheep(Cheep alteredCheep)
    {
        return null;
    }
}