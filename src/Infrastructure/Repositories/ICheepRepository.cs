

public interface ICheepRepository
{
    

    Task CreateCheep(Cheep newCheep);


    Task<List<Cheep>> ReadCheeps(string name);



    Task UpdateCheep(Cheep alteredCheep);
    

}