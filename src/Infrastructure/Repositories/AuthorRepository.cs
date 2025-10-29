

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AuthorRepository : IAuthorRepository
{

    private readonly ChatDBContext _dbContext;
    public AuthorRepository(ChatDBContext dbContext)
    {
        _dbContext = dbContext;
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
    

    #endregion

    
    

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
}