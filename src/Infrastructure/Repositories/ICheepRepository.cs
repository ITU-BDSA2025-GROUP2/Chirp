

using System.Formats.Tar;

public interface ICheepRepository
{
    

    Task CreateCheep(Cheep newCheep);


    Task<List<CheepViewModel>> ReadCheeps(int page);



    Task UpdateCheep(Cheep alteredCheep);
    

}