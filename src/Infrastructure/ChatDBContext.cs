namespace Infrastructure.Chirp;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class ChatDBContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }

    public DbSet<Author> Authors { get; set; }

    public ChatDBContext(DbContextOptions<ChatDBContext> options) : base(options)
    {
        
    }
}