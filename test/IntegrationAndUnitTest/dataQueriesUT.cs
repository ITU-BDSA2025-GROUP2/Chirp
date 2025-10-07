using Chirp.Razor;

namespace test;

public class dataQueriesUT
{
    private readonly DataQueries _dataQuery = new DataQueries();

    public dataQueriesUT()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", "dataQuery.db");

        _dataQuery.QuerySetup();
    }

    [Fact]
    public void CheckGetCheeps()
    {
        Assert.Equal(32, _dataQuery.GetAllQuery(0).Count());
    }
    
    [Fact]
    public void CheckGetAuthorCheep()
    {
        Assert.Equal(1, _dataQuery.GetCheepsFromAuthor("Helge",0).Count());
    }
}