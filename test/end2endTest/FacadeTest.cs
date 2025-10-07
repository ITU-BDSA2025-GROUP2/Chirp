using Chirp.Razor;
namespace test;

public class FacadeTest
{

    private readonly DBFacade _facade = new DBFacade();


    public FacadeTest()
    {
        Environment.SetEnvironmentVariable("CHIRPDBPATH", "facade.db");
        
        
        _facade.createDatabase();
    }

    [Fact]
    public void CheckDB()
    {
        Assert.True(File.Exists(Path.GetTempPath() + "Chirp.db"));

    }

    [Fact]
    public void CheckGetCheeps()
    {
        Assert.Equal(32, _facade.getAllCheeps(0).Count());
    }
    
    [Fact]
    public void CheckGetAuthorCheep()
    {
        Assert.Equal(1, _facade.getAllFromAuthor("Helge",0).Count());
    }



}