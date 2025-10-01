using Chirp.Razor;

public class DBFacade
{
    private DataQueries Dq;

    public DBFacade()
    {
        Dq = new DataQueries();
    }

    public List<CheepViewModel> getAllCheeps(int page)
    {
        return Dq.GetAllQuery(page);
    }

    public List<CheepViewModel> getAllFromAuthor( string author, int page)
    {
        return Dq.GetCheepsFromAuthor(author, page);
    }
    
}