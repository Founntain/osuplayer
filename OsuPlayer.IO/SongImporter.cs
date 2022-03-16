using System.Collections.Concurrent;
using System.Diagnostics;
using OsuPlayer.IO.DbReader;

namespace OsuPlayer.IO;

public sealed class SongImporter
{
    public Task<ICollection<MapEntry>>? ImportSongs(string path)
    {
        if (string.IsNullOrEmpty(path)) return Task.FromResult<ICollection<MapEntry>>(default);

        var maps = DbReader.DbReader.ReadOsuDb(path)?.DistinctBy(x => x.BeatmapSetId)
            .DistinctBy(x => x.Title).Where(x => !string.IsNullOrEmpty(x.Title)).ToArray();

        if (!maps.Any()) return Task.FromResult<ICollection<MapEntry>>(default);

        return Task.FromResult<ICollection<MapEntry>>(maps.OrderBy(x => x.Title).ToArray());
    }
}