namespace Chirp.Razor.Chirp.Infrastructure.Chirp.Services;

public interface ICheepService
{
    public Task<List<Cheep>> GetCheeps(int page);
    public Task<List<Cheep>> GetCheepsFromAuthor(string author, int page);

    public Task<Author> GetAuthor(string author, int page);
}