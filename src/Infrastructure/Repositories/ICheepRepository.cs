

public interface ICheepRepository
{
    

    Task CreateCheep(Cheep newCheep);


    Task<List<CheepViewModel>> ReadCheeps(string name);



    Task UpdateCheep(Cheep alteredCheep);
    

}