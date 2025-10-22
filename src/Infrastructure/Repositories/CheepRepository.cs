

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


    public async Task<List<CheepViewModel>> ReadCheeps(int page = 0)
    {


        var query = (
            from cheep in _dbContext.Cheeps
            select new
            {
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp
            }).Skip(page*32).Take(32);
        
        var result = await query.ToListAsync();

        var returnList = new List<CheepViewModel>();
        foreach (var row in result)
        {
            returnList.Add(new CheepViewModel(row.Name, row.Text, row.TimeStamp.ToString()));
        }
        return returnList;
    }

    public async Task<List<CheepViewModel>> ReadCheepsPerson(string name, int page)
    {
        var query = (
            from cheep in _dbContext.Cheeps
            where cheep.Author.Name == name
            select new
            {
                cheep.Author.Name,
                cheep.Text,
                cheep.TimeStamp
            }).Skip(page * 32).Take(32);
        var result = await query.ToListAsync();

        var returnList = new List<CheepViewModel>();
        foreach (var row in result)
        {
            returnList.Add(new CheepViewModel(row.Name, row.Text, row.TimeStamp.ToString()));
        }
        return returnList;
    }

    public async Task<AuthorViewModel> ReadAuthor(string name, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Name == name
            select new
            {
                person.Name,
                person.Email
            }).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();

        var returnList = new AuthorViewModel(null, null);

        foreach (var row in result)
        {
            returnList = new AuthorViewModel(row.Name, row.Email);
        }
        return returnList;
    }
    
    public async Task<AuthorViewModel> ReadEmail(string email, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select new
            {
                person.Name,
                person.Email
            }).Skip(page*32).Take(32);

        var result = await query.ToListAsync();

        var returnList = new AuthorViewModel( null, null);
        
        foreach (var row in result)
        {
            returnList = new AuthorViewModel(row.Name, row.Email);
        }
        return returnList;
    }


    public Task UpdateCheep(Cheep alteredCheep)
    {
        return null;
    }
}