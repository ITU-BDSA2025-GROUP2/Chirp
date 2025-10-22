namespace SupportScripts;

using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class MemoryDBFactory
{
    private CheepRepository cheepRepository;
    ICheepRepository repository;
    public SqliteConnection connection;
    

    public MemoryDBFactory()
    {
        connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChatDBContext>().UseSqlite(connection);

        var context = new ChatDBContext(builder.Options);
        context.Database.EnsureCreated();

        repository = new CheepRepository(context);
    }

    public ICheepRepository GetCheepRepository()
    {
        return repository;
    }
    
    
}