

using System.Formats.Tar;

public interface ICheepRepository
{
    

    void CreateCheep(string author, string email, string msg);


    Task<List<CheepViewModel>> ReadCheeps(int page);

    Task<List<CheepViewModel>> ReadCheepsPerson(string name, int page);

    Task<AuthorViewModel> ReadAuthor(string name, int page);

    Task<AuthorViewModel> ReadEmail(string email, int page);


    Task UpdateCheep(Cheep alteredCheep);


    public void CreateAuthor(string name, string email);

    public int FindNewAuthorId();

    public int FindNewCheepId();

    public Task<List<Author>> ReturnBasedOnEmailAsync(string email, int page = 0);
}