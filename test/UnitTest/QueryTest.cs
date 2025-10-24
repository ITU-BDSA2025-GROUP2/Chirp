
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

        var testCheep = Cheep.Find(x => x.Author == "Helge");
        

        Assert.Equal(  "Helge"  , testCheep.Author  );
    }




    [Fact]
    public async Task ReadCheepsAuthor()
    {
        var Cheep = await repository.ReadCheepsPerson("Helge", 0);

        var testCheep = Cheep.Find(x => x.Author == "Helge");

        Assert.Equal("Join itu lan now", testCheep.Message);

        Assert.Null(Cheep.Find(x => x.Author == "Adrian"));
    }

    [Fact]
    public async Task ReadAuthor()
    {
        var Author = await repository.ReadAuthor("Helge", 0);

        Assert.Equal("ropf@itu.dk", Author.Email);
    }

    [Fact]
    public async Task ReadEmail()
    {
        var Email = await repository.ReadEmail("ropf@itu.dk", 0);

        Assert.Equal("Helge", Email.Author);
    }


}