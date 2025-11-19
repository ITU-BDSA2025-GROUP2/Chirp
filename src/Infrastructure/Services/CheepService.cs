using Core;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public record CheepViewModel(string Author, string Message, string Timestamp, string Email, string IsFollowed);
public record AuthorViewModel(string Author, string Email);


public class CheepService : ICheepService
{

    private CheepRepository _cheepRepository;
    private AuthorRepository _authorRepository;
    public CheepService(ChatDbContext dbContext)
    {
       
        //_dbcontext = dbContext;
        _authorRepository = new AuthorRepository(dbContext);
        _cheepRepository = new CheepRepository(dbContext);
    }

    public async Task<List<Cheep>> GetCheeps(int page)
    {
        return await _cheepRepository.ReadCheeps(page);
    }

    public async Task<List<Cheep>> GetCheepsFromAuthor(string author, int page)
    {
        // filter by the provided author name
        return await _cheepRepository.ReadCheepsPerson(author, page);
    }



    public async Task<Author> GetAuthor(string author, int page)
    {
        return await _authorRepository.ReturnBasedOnNameAsync(author, page);
    }


     public async Task<int> GetAuthorId(string email)
    {
        return await _authorRepository.ReturnAuthorsId(email);
    }

    public async Task<Author> GetEmail(string email, int page)
    {
        var result = await _authorRepository.ReturnBasedOnEmailAsync(email, page);
        return result[0];
    }


    public async Task<List<int>> GetFollowers(string email)
    {
        return await _authorRepository.ReturnFollowAuthorsIds(email);
    }

    public async Task CreateCheep(string author, string email, string msg)
    {
        await _cheepRepository.CreateCheep(author, email, msg);
    }



    public void AddFollowerId(Author author, int id)
    {
        _authorRepository.AddFollowerId(author, id);
    }

    public void RemoveFollowerId(Author author, int id)
    {
        _authorRepository.RemoveFollowerId(author, id);
    }


}



