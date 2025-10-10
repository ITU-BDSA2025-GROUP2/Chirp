

using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class CheepRepositoryTest
{
    private CheepRepository cheepRepository;

    ICheepRepository repository;

    public SqliteConnection connection;

    public CheepRepositoryTest()
    {
        // Here we generate a sqlite in memory db could be smart to make a support class.
        // that all tests call to create a test db to reduce code duplicationæ.

        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChatDBContext>().UseSqlite(connection);

        var context = new ChatDBContext(builder.Options);
        context.Database.EnsureCreated();

        repository = new CheepRepository(context);
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task FindNewIdTest()
    {
        Assert.Equal(1, repository.FindNewId());
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
        Assert.Equal(1, repository.FindNewId());
        repository.CreateAuthor("Tim", "tim@email.com");
        Assert.Equal(2, repository.FindNewId());
    }
}