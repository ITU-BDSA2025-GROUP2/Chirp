using Core.Interfaces;
using Core.Model;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace SupportScripts;

public class MemoryDbFactory
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ChatDbContext _context;

    public MemoryDbFactory()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var builder = new DbContextOptionsBuilder<ChatDbContext>().UseSqlite(connection);

        _context = new ChatDbContext(builder.Options);
        _context.Database.EnsureCreated();

        _cheepRepository = new CheepRepository(_context);
        _authorRepository = new AuthorRepository(_context);

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

        _context.Authors.AddRange(authors);
        _context.Cheeps.AddRange(cheeps);
        _context.SaveChanges();
    }

    public ICheepRepository GetCheepRepository()
    {
        return _cheepRepository;
    }

    public IAuthorRepository GetAuthorRepository()
    {
        return _authorRepository;
    }
    
    public ChatDbContext GetContext()
    {
        return _context;
    }
}