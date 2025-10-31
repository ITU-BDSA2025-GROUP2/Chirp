using Core;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class ChatDbContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }

    public DbSet<Author> Authors { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        
    }
}