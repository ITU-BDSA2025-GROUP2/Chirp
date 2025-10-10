

using System.Reflection;

public class CheepRepositoryTest
{
    private CheepRepository cheepRepository;

    public CheepRepositoryTest()
    {
        ChatDbContext dbContext = new ChatDbContext();
        cheepRepository = new CheepRepository(dbContext);
    }

    [Fact]
    public void Test1()
    {
        Assert.NotNull(cheepRepository);
    }

    [Fact]
    public async Task FindNewIdTest()
    {
        
    }
}