
using Microsoft.EntityFrameworkCore;

public class DbContext
{
    DbSet<Cheep> Cheeps { get; set; }

    DbSet<Author> Authors { get; set; }

    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {

    }
}