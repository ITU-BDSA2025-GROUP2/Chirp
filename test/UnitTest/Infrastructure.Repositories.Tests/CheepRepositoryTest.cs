

using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class CheepRepositoryTest
{
    private CheepRepository cheepRepository;

    ICheepRepository repository;

    public CheepRepositoryTest()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
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
/* 
    [Fact]
    public async Task FindNewIdTest()
    {
        
    } */
}