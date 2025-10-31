using Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure;

public class ChatDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Cheep> Cheeps { get; set; }

    public DbSet<Author> Authors { get; set; }

    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        
    }
}