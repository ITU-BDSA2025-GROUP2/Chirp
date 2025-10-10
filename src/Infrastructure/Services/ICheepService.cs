namespace Chirp.Razor.Chirp.Infrastructure.Chirp.Services;

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int page);
}