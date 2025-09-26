using Chirp.Razor;

public class DBFacade
{
    private DataQueries Dq;
    
    public DBFacade(){
        Dq = new DataQueries();
    }

    public void RunQueries()
    {
        Dq.GetAllQuery();
    }
    
}