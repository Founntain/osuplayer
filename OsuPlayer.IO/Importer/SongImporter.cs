using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;

namespace OsuPlayer.IO.Importer;

/// <summary>
/// Wrapper class for the <see cref="OsuDbReader" /> and <see cref="RealmReader" />
/// </summary>
public static class SongImporter
{
    /// <summary>
    /// Imports the songs from either the osu!.db or client.realm using the <see cref="SongImporter" />. <br />
    /// Imported songs are stored in <see cref="SongSource" />. <br />
    /// Also plays the first song depending on the <see cref="StartupSong" /> config.
    /// <seealso cref="DoImportAsync" />
    /// </summary>
    public static async Task ImportSongsAsync(ICanImportSongs importSongsDestination, ISortableSongs songSorter)
    {
        importSongsDestination.SongsLoading.Value = true;

        await using (var config = new Config())
        {
            var songEntries = await DoImportAsync((await config.ReadAsync()).OsuPath!);

            if (songEntries == null) return;

            importSongsDestination.SongSource.Value = songEntries.OrderBy(x => songSorter.CustomSorter(x, config.Container.SortingMode)).ThenBy(x => x.Title).ToSourceList();

            importSongsDestination.SongsLoading.Value = false;

            if (importSongsDestination.SongSourceList == null || !importSongsDestination.SongSourceList.Any()) return;
        }

        importSongsDestination.OnSongImportFinished();
    }

    /// <summary>
    /// Imports the songs from the osu!db or client.realm depending on the file present in the <paramref name="path" />
    /// directory
    /// </summary>
    /// <param name="path">the path to the osu!(lazer) root folder</param>
    /// <returns>an <see cref="ICollection{T}" /> of <see cref="IMapEntryBase" /> containing the imported songs</returns>
    private static async Task<ICollection<IMapEntryBase>?> DoImportAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        IEnumerable<IMapEntryBase>? readMaps = null;

        if (File.Exists(Path.Combine(path, "osu!.db")))
            readMaps = await OsuDbReader.Read(path);
        else if (File.Exists(Path.Combine(path, "client.realm")))
            readMaps = await RealmReader.Read(path);

        if (readMaps == null) return null;

        var maps = readMaps?.DistinctBy(x => x.Hash).OrderBy(x => x.BeatmapSetId)
            //.DistinctBy(x => x.Title)
            .Where(x => !string.IsNullOrEmpty(x.Title)).ToArray();

        if (!maps.Any()) return null;

        return maps;
    }
}