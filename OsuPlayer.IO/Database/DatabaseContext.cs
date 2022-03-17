using Microsoft.EntityFrameworkCore;
using OsuPlayer.IO.Database.Configurations;
using OsuPlayer.IO.Database.Entities;

namespace OsuPlayer.IO.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
        Database.EnsureDeleted();
        
        Database.EnsureCreated();
    }

    public DbSet<Playlist> Playlist { get; set; } = null!;
    public DbSet<Song> Songs { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=osuplayer.db");
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PlaylistConfiguration());
        modelBuilder.ApplyConfiguration(new SongConfiguration());
        modelBuilder.Seed();
    }
}