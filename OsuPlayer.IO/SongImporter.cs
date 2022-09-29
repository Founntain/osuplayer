using OsuPlayeIO.DbReader;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO;

/// <summary>
/// Wrapper class for the <see cref="OsuDbReader" /> and <see cref="RealmReader" />
/// </summary>
public sealed class SongImporter
{
    /// <summary>
    /// Imports the songs from the osu!db or client.realm depending on the file present in the <paramref name="path" />
    /// directory
    /// </summary>
    /// <param name="path">the path to the osu!(lazer) root folder</param>
    /// <returns>an <see cref="ICollection{T}" /> of <see cref="IMapEntryBase" /> containing the imported songs</returns>
    public static async Task<ICollection<IMapEntryBase>?> ImportSongsAsync(string path)
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