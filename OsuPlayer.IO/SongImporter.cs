using OsuPlayer.IO.DbReader;

namespace OsuPlayer.IO;

public sealed class SongImporter
{
    public static async Task<ICollection<MinimalMapEntry>?> ImportSongs(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        var maps = (await DbReader.DbReader.ReadOsuDb(path))?.DistinctBy(x => x.BeatmapChecksum)
            .DistinctBy(x => x.Title).Where(x => !string.IsNullOrEmpty(x.Title)).OrderBy(x => x.Title).ToArray();

        if (maps == null || !maps.Any()) return null;

        return maps;
    }
}