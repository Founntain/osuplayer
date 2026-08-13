using Nein.Extensions;

namespace OsuPlayer.Data.DataModels.Interfaces;

public interface IMapEntryBase : IEquatable<IMapEntryBase>, IComparable<IMapEntryBase>
{
    public IDbReaderFactory DbReaderFactory { get; init; }

    public string Artist { get; }
    public string ArtistUnicode { get; }
    public string Title { get; }
    public string TitleUnicode { get; }
    public string Hash { get; }
    public int BeatmapSetId { get; }
    public int TotalTime { get; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string DisplaySongName => GetDisplaySongName();
    public string ArtistString => GetDisplayArtist();
    public string TitleString => GetDisplayTitle();

    public bool UseUnicode { get; set; }

    public string GetArtist();

    public string GetTitle();

    /// <summary>
    /// Gets the artist for display, preferring the original Unicode metadata.
    /// </summary>
    public string GetDisplayArtist()
    {
        return string.IsNullOrWhiteSpace(ArtistUnicode) ? Artist : ArtistUnicode;
    }

    /// <summary>
    /// Gets the title for display, preferring the original Unicode metadata.
    /// </summary>
    public string GetDisplayTitle()
    {
        return string.IsNullOrWhiteSpace(TitleUnicode) ? Title : TitleUnicode;
    }

    /// <summary>
    /// Gets the Unicode-first artist and title used by the UI.
    /// </summary>
    public string GetDisplaySongName()
    {
        return $"{GetDisplayArtist()} - {GetDisplayTitle()}";
    }

    /// <summary>
    /// Gets a formatted version of artist and title
    /// </summary>
    /// <returns>the formatted song name</returns>
    public string GetSongName()
    {
        return $"{GetArtist()} - {GetTitle()}";
    }

    /// <summary>
    /// Reads the full <see cref="IMapEntry" /> from this <see cref="IMapEntryBase" />
    /// </summary>
    /// <returns>
    /// a full <see cref="IMapEntry" /> for extended usage. Returns null if the path doesn't exist or the map was not
    /// found.
    /// </returns>
    public IMapEntry? ReadFullEntry();

    /// <summary>
    /// Gets the corresponding <see cref="IDatabaseReader" /> of the beatmap
    /// </summary>
    /// <returns>a <see cref="IDatabaseReader" /> instance of the specific database reader implementation</returns>
    public IDatabaseReader GetReader();
}
