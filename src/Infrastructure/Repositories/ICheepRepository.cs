

public interface ICheepRepository
{
    

    void CreateCheep(string author, string email, string msg);


    Task<List<Cheep>> ReadCheeps(string name);



    Task UpdateCheep(Cheep alteredCheep);


    public void CreateAuthor(string name, string email);

    public int FindNewAuthorId();
    

}