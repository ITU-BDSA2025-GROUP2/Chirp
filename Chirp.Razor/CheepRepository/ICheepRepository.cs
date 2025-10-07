

public interface ICheepRepository
{
    Task CreateCheep(CheepDTO newCheep);


    Task<List<CheepDTO>> ReadCheeps(string name);



    Task UpdateCheep(CheepDTO alteredCheep);
    

}