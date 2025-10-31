using Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{

    private readonly ChatDbContext _dbContext;
    private readonly IAuthorRepository _authorRepository;
    public CheepRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
        _authorRepository = new AuthorRepository(dbContext);
    }
    
    
    public async Task CreateCheep(string author, string email, string msg)
    {

        var authorFromQuery = await _authorRepository.ReturnBasedOnEmailAsync(email);

        if (authorFromQuery.Count() <= 0)
        {
            _authorRepository.CreateAuthor(author, email);   
        }

        authorFromQuery = await _authorRepository.ReturnBasedOnEmailAsync(email);

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




    public int FindNewCheepId()
    {
        return _dbContext.Cheeps.Count() + 1;
    }

    public async Task<List<Cheep>> ReadCheeps(int page = 0)
    {
        var query = (
            from cheep in _dbContext.Cheeps.Include(c => c.Author)
            select cheep).OrderByDescending(c => c.TimeStamp).Skip(page*32).Take(32);
        
        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<Cheep>> ReadCheepsPerson(string name, int page)
    {
        var query = (
            from cheep in _dbContext.Cheeps.Include(c => c.Author)
            where cheep.Author.Name == name
            select cheep
            ).OrderByDescending(c => c.TimeStamp).Skip(page * 32).Take(32);
        var result = await query.ToListAsync();

        
        return result;
    }

}