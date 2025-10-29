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
    public CheepService(ChatDBContext dbContext)
    {
       
        _dbcontext = dbContext;
        _cheepRepository = new CheepRepository(_dbcontext);
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
        return await _cheepRepository.ReturnBasedOnNameAsync(author, page);
    }

    public async Task<Author> GetEmail(string email, int page)
    {
        var result = await _cheepRepository.ReturnBasedOnEmailAsync(email, page);
        return result[0];
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}



