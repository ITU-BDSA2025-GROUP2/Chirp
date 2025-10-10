

using Microsoft.EntityFrameworkCore;

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


    public async Task<List<CheepViewModel>> ReadCheeps(string name)
    {
        var query = from cheep in _dbContext.Cheeps
                    select new {cheep.Author.Name, cheep.Text, cheep.TimeStamp};
        var result = await query.ToListAsync();
        
        var returnList = new List<CheepViewModel>();
        foreach (var row in result)
        {
            returnList.Add(new CheepViewModel(row.Name, row.Text, row.TimeStamp.ToString()));
        }
        return returnList;
    }

    public Task UpdateCheep(Cheep alteredCheep)
    {
        return null;
    }
}