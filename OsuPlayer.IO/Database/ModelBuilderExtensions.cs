using Microsoft.EntityFrameworkCore;
using OsuPlayer.Data.OsuPlayer.Database.Entities;

namespace OsuPlayer.IO.Database;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder builder)
    {
        builder.Entity<Playlist>().HasData(new List<Playlist>()
        {
            new Playlist
            {
                Name = "Favorites"
            }
        });
    }
}