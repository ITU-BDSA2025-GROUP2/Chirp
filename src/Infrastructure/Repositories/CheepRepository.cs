

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

        var authorFromQuery = await ReturnBasedOnEmailAsync(email);

        if (authorFromQuery.Count() <= 0)
        {
            CreateAuthor(author, email);   
        }

        authorFromQuery = await ReturnBasedOnEmailAsync(email);

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

    public async Task<List<Cheep>> ReadCheeps(int page = 0)
    {


        var query = (
            from cheep in _dbContext.Cheeps
            select cheep).Skip(page*32).Take(32);
        
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<Cheep>> ReadCheepsPerson(string name, int page)
    {
        var query = (
            from cheep in _dbContext.Cheeps
            where cheep.Author.Name == name
            select cheep
            ).Skip(page * 32).Take(32);
        var result = await query.ToListAsync();

        
        return result;
    }

    public async Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person
            ).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();

        return result;
    }
    
    public async Task<Author> ReturnBasedOnNameAsync(string name, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Name == name
            select person
            ).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();

        return result[0];
    }


    public Task UpdateCheep(Cheep alteredCheep)
    {
        return null;
    }
}