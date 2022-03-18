using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsuPlayer.Data.OsuPlayer.Database.Entities;

namespace OsuPlayer.IO.Database.Configurations;

public class SongConfiguration : IEntityTypeConfiguration<Song>
{
    public void Configure(EntityTypeBuilder<Song> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Playlists).WithMany(x => x.Songs);
    }
}