using Core.Interfaces;
using Core.Model;
using SupportScripts;

namespace UnitTest;

public class CheepRepositoryTests
{
    private readonly MemoryDbFactory _memoryDb = new MemoryDbFactory();
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;

    public CheepRepositoryTests()
    {
        // Here we generate a sqlite in memory db could be smart to make a support class.
        // that all tests call to create a test db to reduce code duplicationæ.

        _cheepRepository = _memoryDb.GetCheepRepository();
        _authorRepository = _memoryDb.GetAuthorRepository();
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(_cheepRepository);
    }

    [Fact]
    public void FindNewIdTest()
    {
        Assert.Equal(3, _authorRepository.FindNewAuthorId().Result);
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
    public void DoesItCreateAuthor()
    {
        Assert.Equal(3, _authorRepository.FindNewAuthorId().Result);
        _authorRepository.CreateAuthor("Tim", "tim@email.com");
        Assert.Equal(4, _authorRepository.FindNewAuthorId().Result);
    }


    [Fact]
    public async Task CreateCheep_WithExistingAuthor()
    {
        var author = "Helge";
        var email = "ropf@itu.dk";
        var msg = "HELLO WORLD!";

        var authorObj = new Author()
        {
            Email = email,
            Name = author,
            AuthorId = 0,
            Cheeps = new List<Cheep>()
        };

        //Values needs to be updated when merged with Vee and madelines code
        Assert.Equal(3, _authorRepository.FindNewAuthorId().Result);
        Assert.Equal(5, _cheepRepository.FindNewCheepId());
        await _cheepRepository.CreateCheep(authorObj, msg);
        Assert.Equal(4, _authorRepository.FindNewAuthorId().Result);
        Assert.Equal(6, _cheepRepository.FindNewCheepId());
    }
}