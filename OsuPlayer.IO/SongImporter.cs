using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using Realms;

namespace OsuPlayer.IO;

public sealed class SongImporter
{
    public static async Task<ICollection<IMapEntryBase>?> ImportSongs(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        var maps = (await DbReader.DbReader.ReadOsuDb(path))?.DistinctBy(x => x.BeatmapChecksum)
            .DistinctBy(x => x.Title).Where(x => !string.IsNullOrEmpty(x.Title)).OrderBy(x => x.Title).ToArray();

        if (maps == null || !maps.Any()) return null;

        return maps;
    }
}