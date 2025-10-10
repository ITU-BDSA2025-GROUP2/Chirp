namespace Chirp.Razor.Chirp.Infrastructure.Chirp.Services;

public interface ICheepService
{
    public Task<List<CheepViewModel>> GetCheeps(int page);
    public Task<List<CheepViewModel>> GetCheepsFromAuthor(string author, int page);

    public Task<AuthorViewModel> GetAuthor(string author, int page);
}