

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class CheepRepository : ICheepRepository
{

    private readonly ChatDBContext _dbContext;
    public CheepRepository(ChatDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    public async Task CreateCheep(string author, string email, string msg)
    {


        // Run GetAuthor from email query, and you will get a list<Author> matching email
        // Using the list's count, determine if author exist?
        var authorFromQuery = await ReturnBasedOnEmailAsync(email); // Update this to be the query result
        
        /* Boolean authMissing = true;
        
        foreach (var auth in authorList)
        {
            if (auth.Email == email && auth.Name == author)
            {
              // Valid author
              authMissing = false;
              break;
            } 
        } */

        if (authorFromQuery.Count() == 0)
        {
            CreateAuthor(author, email);
        }

        authorFromQuery = await ReturnBasedOnEmailAsync(email);
        
        if (authorFromQuery.Count() > 1)
        {
            //TODO Throw error
            return;
        }
        

       var cheep = new Cheep()
        {
            CheepId = FindNewCheepId(),
            Text = msg,
            TimeStamp = DateTime.Now,
            AuthorId = authorFromQuery[0].AuthorId,
            Author = authorFromQuery[0],
        };

        _dbContext.Cheeps.Add(cheep);
        _dbContext.SaveChanges();
    }


    public void CreateAuthor(string name, string email)
    {
        var newAuthor = new Author() { AuthorId = FindNewAuthorId(), Name = name, Email = email };

        _dbContext.Authors.Add(newAuthor);
        _dbContext.SaveChanges();
    }

    #region Helper methods

    public int FindNewAuthorId()
    {
        var length = _dbContext.Authors.Count();
        return length + 1;
    }


    public int FindNewCheepId()
    {
        return _dbContext.Cheeps.Count() + 1;
    }

    

    #endregion

    
    public async void CreateCheep()
    {
        
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
        var result = await ReturnBasedOnNameAsync(name);

        var returnList = new AuthorViewModel(null, null);

        foreach (var row in result)
        {
            returnList = new AuthorViewModel(row.Name, row.Email);
        }
        return returnList;
    }

    public async Task<AuthorViewModel> ReadEmail(string email, int page = 0)
    {
       
        var result = await ReturnBasedOnEmailAsync(email);

        var returnList = new AuthorViewModel(null, null);
        
        foreach (var row in result)
        {
            returnList = new AuthorViewModel(row.Name, row.Email);
        }
        return returnList;
    }

    public async Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person
            ).Skip(page * 32).Take(32);

        return await query.ToListAsync();
    }
    
    public async Task<List<Author>> ReturnBasedOnNameAsync(string name, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Name == name
            select person
            ).Skip(page * 32).Take(32);

        return await query.ToListAsync();
    }


    public Task UpdateCheep(Cheep alteredCheep)
    {
        return null;
    }
}