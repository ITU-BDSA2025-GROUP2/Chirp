

using System.Formats.Tar;

public interface ICheepRepository
{
    

    Task CreateCheep(Cheep newCheep);


    Task<List<CheepViewModel>> ReadCheeps(int page);

    Task<List<CheepViewModel>> ReadCheepsPerson(string name, int page);

    Task<AuthorViewModel> ReadAuthor(string name, int page);

    Task<AuthorViewModel> ReadEmail(string email, int page);


    Task UpdateCheep(Cheep alteredCheep);
    

}