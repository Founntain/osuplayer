using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO;

public sealed class SongImporter
{
    public static async Task<ICollection<IMapEntryBase>?> ImportSongs(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;

        IMapEntryBase[] maps = null;

        if (File.Exists(Path.Combine(path, "osu!.db")))
            maps = (await DbReader.DbReader.ReadOsuDb(path))?.DistinctBy(x => x.BeatmapChecksum)
                .DistinctBy(x => x.Title).Where(x => !string.IsNullOrEmpty(x.Title)).OrderBy(x => x.Title).ToArray();
        else if (File.Exists(Path.Combine(path, "client.realm")))
            maps = (await RealmReader.ReadRealm(path))?.DistinctBy(x => x.BeatmapChecksum)
                .DistinctBy(x => x.Title).Where(x => !string.IsNullOrEmpty(x.Title)).OrderBy(x => x.Title).ToArray();

        if (maps == null || !maps.Any()) return null;

        return maps;
    }
}