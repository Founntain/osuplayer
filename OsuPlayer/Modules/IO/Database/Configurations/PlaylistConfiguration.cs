using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OsuPlayer.Modules.IO.Database.Entities;

namespace OsuPlayer.Modules.IO.Database.Configurations;

public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
{
    public void Configure(EntityTypeBuilder<Playlist> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.Songs);
    }
}