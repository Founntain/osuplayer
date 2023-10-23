using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.Storage.Config;
using Splat;

namespace OsuPlayer.IO.Importer;

/// <summary>
/// Wrapper class for the <see cref="OsuDbReader" /> and <see cref="RealmReader" />
/// </summary>
public static class SongImporter
{
    /// <summary>
    /// Imports the songs from either the osu!.db or client.realm using the <see cref="SongImporter" />. <br />
    /// Imported songs are stored in <see cref="ISongSourceProvider.SongSource" />. <br />
    /// Also plays the first song depending on the <see cref="StartupSong" /> config.
    /// <seealso cref="DoImportAsync" />
    /// </summary>
    /// <param name="songSourceProvider">The <see cref="ISongSourceProvider" /> which will provide the songs</param>
    /// <param name="importNotificationsDestination">The <see cref="IImportNotifications" /> to handle import events</param>
    public static async Task ImportSongsAsync(ISongSourceProvider songSourceProvider, IImportNotifications? importNotificationsDestination = null)
    {
        importNotificationsDestination?.OnImportStarted();

        await using (var config = new Config())
        {
            var songEntries = (await DoImportAsync((await config.ReadAsync()).OsuPath!))?.ToList();

            if (songEntries == null || !songEntries.Any()) return;

            songSourceProvider.SongSource.Edit(list =>
            {
                list.Clear();
                list.AddRange(songEntries.OrderBy(x => x.Title));
            });

            if (songSourceProvider.SongSourceList == null || !songSourceProvider.SongSourceList.Any()) return;
        }

        importNotificationsDestination?.OnImportFinished();
    }

    /// <summary>
    /// Imports the songs from the osu!db or client.realm depending on the file present in the <paramref name="path" />
    /// directory
    /// </summary>
    /// <param name="path">the path to the osu!(lazer) root folder</param>
    /// <returns>an <see cref="ICollection{T}" /> of <see cref="IMapEntryBase" /> containing the imported songs</returns>
    private static async Task<IEnumerable<IMapEntryBase>?> DoImportAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        IEnumerable<IMapEntryBase>? readMaps = null;

        var dbReaderFactory = Locator.Current.GetService<IDbReaderFactory>();

        if (File.Exists(Path.Combine(path, "osu!.db")))
            dbReaderFactory.Type = IDbReaderFactory.CreationType.OsuDb;
        else if (File.Exists(Path.Combine(path, "client.realm")))
            dbReaderFactory.Type = IDbReaderFactory.CreationType.Realm;

        using var reader = dbReaderFactory.CreateDatabaseReader(path);

        readMaps = await reader.ReadBeatmaps();

        if (readMaps == null) return null;

        var maps = readMaps.DistinctBy(x => x.Hash).OrderBy(x => x.BeatmapSetId)
            //.DistinctBy(x => x.Title)
            .Where(x => !string.IsNullOrEmpty(x.Title)).ToArray();

        return !maps.Any() ? null : maps;
    }
}