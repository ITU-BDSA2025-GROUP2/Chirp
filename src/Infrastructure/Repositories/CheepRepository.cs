

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


    public Task<List<Cheep>> ReadCheeps(string name)
    {
        return null;
    }

    public Task UpdateCheep(Cheep alteredCheep)
    {
        return null;
    }
}