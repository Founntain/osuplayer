using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader.DataModels;

public interface IMapEntryBase : IEquatable<IMapEntryBase>
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Hash { get; set; }
    public int BeatmapSetId { get; set; }
    public int TotalTime { get; set; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    public string GetArtist();

    public string GetTitle();

    public string GetSongName();

    /// <summary>
    /// Reads the full <see cref="IMapEntry" /> from this <see cref="IMapEntryBase" />
    /// </summary>
    /// <param name="path">the path to the osu! map</param>
    /// <returns>
    /// a full <see cref="IMapEntry" /> for extended usage. Returns null if the path doesn't exist or the map was not
    /// found.
    /// </returns>
    public Task<IMapEntry?> ReadFullEntry(string path);

    /// <summary>
    /// Gets the corresponding <see cref="IDatabaseReader"/> of the beatmap
    /// </summary>
    /// <param name="path">the osu! path</param>
    /// <returns>a <see cref="IDatabaseReader"/> instance of the specific database reader implementation</returns>
    public IDatabaseReader GetReader(string path);
}