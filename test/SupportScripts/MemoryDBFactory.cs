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

        var a1 = new Author() { AuthorId = 1, Name = "Helge", Email = "ropf@itu.dk", Cheeps = new List<Cheep>() };
        var a2 = new Author() { AuthorId = 2, Name = "Adrian", Email = "adho@itu.dk", Cheeps = new List<Cheep>() };

        var authors = new List<Author>() { a1, a2};

        var c1 = new Cheep() { CheepId = 1, AuthorId = a1.AuthorId, Author = a1, Text = "Join itu lan now", TimeStamp = DateTime.Parse("2023-08-01 13:14:37") };
        var c2 = new Cheep() { CheepId = 2, AuthorId = a2.AuthorId, Author = a2, Text = "test answer", TimeStamp = DateTime.Parse("2023-08-01 13:15:21") };
        var c3 = new Cheep() { CheepId = 3, AuthorId = a1.AuthorId, Author = a1, Text = "Madeleine says i make propaganda", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") };
        var c4 = new Cheep() { CheepId = 4, AuthorId = a1.AuthorId, Author = a1, Text = "Vee says i make propaganda", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") };

        var cheeps = new List<Cheep>() { c1, c2, c3, c4 };
        a1.Cheeps = new List<Cheep>() { c1, c3, c4 };
        a2.Cheeps = new List<Cheep>() { c2 };

        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        context.SaveChanges();
    }

    public ICheepRepository GetCheepRepository()
    {
        return repository;
    }
    
    
}