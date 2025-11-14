using Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{

    private readonly ChatDbContext _dbContext;
    public AuthorRepository(ChatDbContext dbContext)
    {
        _dbContext = dbContext;
    }




    public void CreateAuthor(string name, string email)
    {
        var newAuthor = new Author()
        {
            AuthorId = FindNewAuthorId(),
            Name = name,
            Email = email,
            Cheeps = new List<Cheep>()
        };

        _dbContext.Authors.Add(newAuthor);
        _dbContext.SaveChanges();
    }

    public void AddFollowerId(Author author, int id)
    {
        var Current_Author = _dbContext.Authors.Find(author);
        Current_Author.Follows.Add(id);
        _dbContext.SaveChanges();

    }

    public void RemoveFollowerId(Author author, int id)
    {
        var Current_Author = _dbContext.Authors.Find(author);
        Current_Author.Follows.Remove(id);
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
            ).OrderByDescending(c => c.Name).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();

        return result;
    }

    public async Task<List<int>> ReturnFollowAuthorsIds(string email)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person.Follows
            );

        var result = await query.ToListAsync();

        return result[0];
        
    }

    public async Task<int> ReturnAuthorsId(string email)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Email == email
            select person.AuthorId
        );
        var result = await query.ToListAsync();

        return result[0];
    }

    public async Task<Author> ReturnBasedOnNameAsync(string name, int page = 0)
    {
        var query = (
            from person in _dbContext.Authors
            where person.Name == name
            select person
            ).OrderByDescending(c => c.Name).Skip(page * 32).Take(32);

        var result = await query.ToListAsync();

        return result[0];
    }
}