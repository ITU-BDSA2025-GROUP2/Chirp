using Chirp.Razor;

public class DBFacade
{
    private DataQueries Dq;

    public DBFacade()
    {
        Dq = new DataQueries();
    }

    public List<CheepViewModel> getAllCheeps()
    {
        return Dq.GetAllQuery();
    }

    public List<CheepViewModel> getAllFromAuthor( string author)
    {
        return Dq.GetCheepsFromAuthor(author);
    }
    
}