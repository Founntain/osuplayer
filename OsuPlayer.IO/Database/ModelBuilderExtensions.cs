using Microsoft.EntityFrameworkCore;
using OsuPlayer.IO.Database.Entities;

namespace OsuPlayer.IO.Database;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Playlist>().HasData(new List<Playlist>()
        {
            new Playlist
            {
                Id = 1,
                Name = "Favorites"
            }
        });
    }
}