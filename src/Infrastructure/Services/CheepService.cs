using System.Threading.Tasks;
using Chirp.Razor;
using Chirp.Razor.Chirp.Infrastructure.Chirp.Services;
//using Microsoft.AspNetCore.Mvc.RazorPages;

public record CheepViewModel(string Author, string Message, string Timestamp);

public record AuthorViewModel(string Author, string Email);

public class CheepService : ICheepService
{

    private ChatDBContext _dbcontext;
    private CheepRepository _cheepRepository;
    private AuthorRepository _authorRepository;
    public CheepService(ChatDBContext dbContext)
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

    public async Task<Author> GetEmail(string email, int page)
    {
        var result = await _authorRepository.ReturnBasedOnEmailAsync(email, page);
        return result[0];
    }


}



