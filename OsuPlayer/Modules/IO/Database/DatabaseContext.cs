using Microsoft.EntityFrameworkCore;

namespace OsuPlayer.Modules.IO.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext() : base()
    {
        Database.EnsureCreated();
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=osuplayer.db");
        optionsBuilder.UseLazyLoadingProxies();
    }
}