
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SupportScripts;
public class QueryTest
{
    private CheepRepository cheepRepository;

    ICheepRepository repository;

    public SqliteConnection connection;

    public QueryTest()
    {
        // Here we generate a sqlite in memory db could be smart to make a support class.
        // that all tests call to create a test db to reduce code duplicationæ.

        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChatDBContext>().UseSqlite(connection);

        var context = new ChatDBContext(builder.Options);
        context.Database.EnsureCreated();

        repository = new MemoryDBFactory().GetCheepRepository();

    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task ReadCheeps()
    {
        var Cheep = await repository.ReadCheeps(0);

        var testCheep = Cheep.Find(x => x.Author.Name == "Helge");
        

        Assert.Equal(  "Helge"  , testCheep.Author.Name  );
    }




    [Fact]
    public async Task ReadCheepsAuthor()
    {
        var Cheep = await repository.ReadCheepsPerson("Helge", 0);

        var testCheep = Cheep.Find(x => x.Author.Name == "Helge");

        Assert.Equal("Join itu lan now", testCheep.Text);

        Assert.Null(Cheep.Find(x => x.Author.Name == "Adrian"));
    }

    [Fact]
    public async Task ReadAuthor()
    {
        var author = await repository.ReturnBasedOnNameAsync("Helge", 0);

        Assert.Equal("ropf@itu.dk", author.Email);
    }

    [Fact]
    public async Task ReadEmail()
    {
        var author = await repository.ReturnBasedOnEmailAsync("ropf@itu.dk", 0);

        Assert.Equal("Helge", author[0].Name);
    }


}