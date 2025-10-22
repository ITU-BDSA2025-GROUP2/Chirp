using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportScripts;

public class CheepRepositoryTest
{
    private CheepRepository cheepRepository;

    ICheepRepository repository;

    public SqliteConnection connection;

    public CheepRepositoryTest()
    {
        // Here we generate a sqlite in memory db could be smart to make a support class.
        // that all tests call to create a test db to reduce code duplicationæ.

        repository = new MemoryDBFactory().GetCheepRepository();
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task FindNewIdTest()
    {
        Assert.Equal(1, repository.FindNewAuthorId());
    }


    /*
    The test below was just to verify we could add to the new data base; 
    The test case should be updated with the queries to do the following instead of what is is currently
    doing.

    Assert.False(query author)
    add author
    Assert.True(query author)
    */ 

    [Fact]
    public async Task DoesItCreateAuthor()
    {
        Assert.Equal(1, repository.FindNewAuthorId());
        repository.CreateAuthor("Tim", "tim@email.com");
        Assert.Equal(2, repository.FindNewAuthorId());
    }


    [Fact]
    public async Task CreateCheep_WithExistingAuthor()
    {
        var author = "Helge";
        var email = "ropf@itu.dk";
        var msg = "HELLO WORLD!";
        
        //Values needs to be updated when merged with Vee and madelines code
        Assert.Equal(1, repository.FindNewAuthorId());
        Assert.Equal(1, repository.FindNewCheepId());
        repository.CreateCheep(author, email, msg);
        Assert.Equal(1, repository.FindNewAuthorId());
        Assert.Equal(2, repository.FindNewCheepId());
    }
    
    [Fact]
    public async Task CreateCheep_WithNewAuthor()
    {
        var author = "Helge";
        var email = "ropf@itu.dk";
        var msg = "HELLO WORLD!";
    
        //Values needs to be updated when merged with Vee and madelines code
        Assert.Equal(1, repository.FindNewAuthorId());
        Assert.Equal(1, repository.FindNewCheepId());
        repository.CreateCheep(author, email, msg);
        Assert.Equal(2, repository.FindNewAuthorId());
        Assert.Equal(2, repository.FindNewCheepId());
        
        
    }
    
    
}