
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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

        repository = new CheepRepository(context);

        var a1 = new Author() { AuthorId = 1, Name = "Helge", Email = "ropf@itu.dk", Cheeps = new List<Cheep>() };
        var a2 = new Author() { AuthorId = 2, Name = "Adrian", Email = "adho@itu.dk", Cheeps = new List<Cheep>() };

        var authors = new List<Author>() { a1, a2};

        var c1 = new Cheep() { CheepId = 1, AuthorId = a1.AuthorId, Author = a1, Text = "Join itu lan now", TimeStamp = DateTime.Parse("2023-08-01 13:14:37") };
        var c2 = new Cheep() { CheepId = 2, AuthorId = a2.AuthorId, Author = a2, Text = "test answer", TimeStamp = DateTime.Parse("2023-08-01 13:15:21") };
        var c3 = new Cheep() { CheepId = 3, AuthorId = a1.AuthorId, Author = a1, Text = "Madeleine says i make propaganda", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") };

        var cheeps = new List<Cheep>() { c1, c2, c3 };
        a1.Cheeps = new List<Cheep>() { c1, c3 };
        a2.Cheeps = new List<Cheep>() { c2 };

        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        context.SaveChanges();
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task ReadCheeps()
    {
        var Cheep = repository.ReadCheeps(0);


        Assert.Equal(    , Cheep(0));
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