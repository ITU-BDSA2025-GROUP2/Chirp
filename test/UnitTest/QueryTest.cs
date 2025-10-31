
using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SupportScripts;
public class QueryTest
{

    MemoryDBFactory memoryDB = new MemoryDBFactory();

    ICheepRepository cheepRepository;
    IAuthorRepository authorRepository;


    public QueryTest()
    {
        // Here we generate a sqlite in memory db could be smart to make a support class.
        // that all tests call to create a test db to reduce code duplicationæ.

        cheepRepository = memoryDB.GetCheepRepository();
        authorRepository = memoryDB.GetAuthorRepository();
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(cheepRepository);
    }

    [Fact]
    public async Task ReadCheeps()
    {
        var Cheep = await cheepRepository.ReadCheeps(0);

        var testCheep = Cheep.Find(x => x.Author.Name == "Helge");
        
        Assert.NotNull(testCheep);
        Assert.Equal(  "Helge"  , testCheep.Author.Name  );
    }




    [Fact]
    public async Task ReadCheepsAuthor()
    {
        var Cheep = await cheepRepository.ReadCheepsPerson("Helge", 0);

        var testCheep = Cheep.Find(x => x.Author.Name == "Helge");
        Assert.NotNull(testCheep);
        Assert.Equal("Madeleine says i make propaganda", testCheep.Text);

        Assert.Null(Cheep.Find(x => x.Author.Name == "Adrian"));
    }

    [Fact]
    public async Task ReadAuthor()
    {
        var author = await authorRepository.ReturnBasedOnNameAsync("Helge", 0);

        Assert.Equal("ropf@itu.dk", author.Email);
    }

    [Fact]
    public async Task ReadEmail()
    {
        var author = await authorRepository.ReturnBasedOnEmailAsync("ropf@itu.dk", 0);

        Assert.Equal("Helge", author[0].Name);
    }


}