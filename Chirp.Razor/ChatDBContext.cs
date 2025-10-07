

using Microsoft.EntityFrameworkCore;


public class ChatDBContext : DbContext
{
    DbSet<Cheep> Cheeps { get; set; }

    DbSet<Author> Authors { get; set; }

    public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options)
    {
        
    }
}