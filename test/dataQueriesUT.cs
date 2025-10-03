using Chirp.Razor;

namespace test;

public class dataQueriesUT
{
    private readonly DataQueries dataQuery = new DataQueries();

    public dataQueriesUT()
    {
        dataQuery.QuerySetup();
    }

    [Fact]
    public void CheckDB()
    {
        Assert.True(File.Exists(Path.GetTempPath() + "Chirp.db"));

    }

    [Fact]
    public void CheckGetCheeps()
    {
        Assert.Equal(32, dataQuery.GetAllQuery(0).Count());
    }
    
    [Fact]
    public void CheckGetAuthorCheep()
    {
        Assert.Equal(1, dataQuery.GetCheepsFromAuthor("Helge",0).Count());
    }
}