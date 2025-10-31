using Core;
using SupportScripts;

namespace UnitTest;

public class QueryTests
{
    private readonly MemoryDbFactory _memoryDb = new MemoryDbFactory();
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;


    public QueryTests()
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
    public async Task ReadCheeps()
    {
        var cheeps = await _cheepRepository.ReadCheeps(0);

        var testCheep = cheeps.Find(x => x.Author.Name == "Helge");
        
        Assert.NotNull(testCheep);
        Assert.Equal(  "Helge"  , testCheep.Author.Name  );
    }




    [Fact]
    public async Task ReadCheepsAuthor()
    {
        var cheeps = await _cheepRepository.ReadCheepsPerson("Helge", 0);

        var testCheep = cheeps.Find(x => x.Author.Name == "Helge");
        Assert.NotNull(testCheep);
        Assert.Equal("Madeleine says i make propaganda", testCheep.Text);

        Assert.Null(cheeps.Find(x => x.Author.Name == "Adrian"));
    }

    [Fact]
    public async Task ReadAuthor()
    {
        var author = await _authorRepository.ReturnBasedOnNameAsync("Helge");

        Assert.Equal("ropf@itu.dk", author.Email);
    }

    [Fact]
    public async Task ReadEmail()
    {
        var author = await _authorRepository.ReturnBasedOnEmailAsync("ropf@itu.dk");

        Assert.Equal("Helge", author[0].Name);
    }


}