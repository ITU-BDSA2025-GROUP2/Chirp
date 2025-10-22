using System.Threading.Tasks;
using Chirp.Razor;
using Chirp.Razor.Chirp.Infrastructure.Chirp.Services;
//using Microsoft.AspNetCore.Mvc.RazorPages;

public record CheepViewModel(string Author, string Message, string Timestamp);

public record AuthorViewModel(string Author, string Email);

public class CheepService : ICheepService
{
    DBFacade facade = new DBFacade();

    private ChatDBContext _dbcontext;
    private CheepRepository _cheepRepository;
    public CheepService(ChatDBContext dbContext)
    {
        //facade.createDatabase();
        _dbcontext = dbContext;
        _cheepRepository = new CheepRepository(_dbcontext);
    }

    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1690892208)),
            new CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1690895308)),
        };

    public async Task<List<CheepViewModel>> GetCheeps(int page)
    {
        var result = await _cheepRepository.ReadCheeps(page);
        return result;
    }

    public async Task<List<CheepViewModel>> GetCheepsFromAuthor(string author, int page)
    {
        // filter by the provided author name
        return await _cheepRepository.ReadCheepsPerson(author, page);
    }

    public async Task<AuthorViewModel> GetAuthor(string author, int page)
    {
        var result = await _cheepRepository.ReadAuthor(author, page);
        return result;
    }

    public async Task<AuthorViewModel> GetEmail(string email, int page)
    {
        var result = await _cheepRepository.ReadEmail(email, page);
        return result;
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}



