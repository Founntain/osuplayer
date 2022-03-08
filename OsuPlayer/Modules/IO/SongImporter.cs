using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OsuPlayer.Modules.IO.DbReader;

namespace OsuPlayer.Modules.IO;

public sealed class SongImporter
{
    public ICollection<SongEntry> ImportSongs(string path)
    {
        var maps = ReadSongsFromDb(path).ToArray();

        if (!maps.Any()) return default;

        var songs = new ConcurrentBag<SongEntry>();

        Parallel.ForEach(maps, (entry, token) =>
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
                entry.AudioFileName,
                $"{path}\\Songs");

            song.TotalTime = entry.TotalTime;

            songs.Add(song);
        });

        return songs.OrderBy(x => x.SongName).ToArray();
    }

    private IEnumerable<MapEntry> ReadSongsFromDb(string path)
    {
        return DbReader.DbReader.ReadOsuDb(path)?.DistinctBy(x => x.BeatmapSetId).DistinctBy(x => x.Title)
            .Where(x => !string.IsNullOrEmpty(x.Title));
    }

    private ICollection<SongEntry> ConvertMapEntriesToSongs(IEnumerable<MapEntry> beatmapEntries, string path)
    {
        var songs = new ConcurrentBag<SongEntry>();

        Parallel.ForEach(beatmapEntries, (entry, token) =>
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
                entry.AudioFileName,
                $"{path}\\Songs");

            song.TotalTime = entry.TotalTime;

            songs.Add(song);
        });

        return songs.OrderBy(x => x.SongName).ToArray();
    }
}