using Microsoft.EntityFrameworkCore;

namespace OsuPlayer.IO.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=osuplayer.db");
        optionsBuilder.UseLazyLoadingProxies();
    }
}