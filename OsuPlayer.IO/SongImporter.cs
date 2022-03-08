using System.Collections.Concurrent;
using OsuPlayer.IO.DbReader;

namespace OsuPlayer.IO;

public sealed class SongImporter
{
    public async Task<List<SongEntry>> ImportSongs(string path)
    {
        var maps = ReadonSongsFromDb(path);

        if (maps == null || maps.Count == 0) return default;

        return await ConvertMapEntriesToSongs(maps.ToArray()); ;
    }

    private ICollection<MapEntry>? ReadonSongsFromDb(string path)
    {
        return DbReader.DbReader.ReadOsuDb(path)?.DistinctBy(x => $"{x.FolderName}\\{x.AudioFileName}").ToList();
    }

    private async Task<List<SongEntry>> ConvertMapEntriesToSongs(IReadOnlyCollection<MapEntry> beatmapEntries)
    {
        var songs = new ConcurrentBag<SongEntry>();

        await Parallel.ForEachAsync(beatmapEntries, (entry, token) =>
        {
            var song = new SongEntry(
                entry.BeatmapSetId,
                entry.BeatmapId,
                entry.BeatmapChecksum,
                entry.Artist,
                entry.ArtistUnicode,
                entry.Title,
                entry.TitleUnicode,
                entry.FolderName,
                entry.AudioFileName);

            song.TotalTime = entry.TotalTime;
            
            songs.Add(song);
            
            return default;
        });

        return songs.OrderBy(x => x.SongName).ToList();
    }
}