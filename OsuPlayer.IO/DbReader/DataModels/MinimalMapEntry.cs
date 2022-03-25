using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a minimal beatmap entry with only frequently used data
/// <remarks>created on call of <see cref="DbReader.ReadOsuDb"/></remarks>
/// </summary>
public class MinimalMapEntry
{
    public string Artist { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string BeatmapChecksum { get; set; } = string.Empty;
    public int TotalTime { get; set; }
    public long DbOffset { get; set; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    /// <summary>
    /// Gets the artist
    /// <remarks>may be overridden for usage with <see cref="MapEntry.UseUnicode"/></remarks>
    /// </summary>
    /// <returns>the artist</returns>
    protected virtual string GetArtist()
    {
        return Artist;
    }
    
    /// <summary>
    /// Gets the title
    /// <remarks>may be overridden for usage with <see cref="MapEntry.UseUnicode"/></remarks>
    /// </summary>
    /// <returns>the title</returns>
    protected virtual string GetTitle()
    {

        return Title;
    }
    
    /// <summary>
    /// Gets a formatted version of artist and title
    /// <remarks>may be overridden for usage with <see cref="MapEntry.UseUnicode"/></remarks>
    /// </summary>
    /// <returns>the formatted song name</returns>
    protected virtual string GetSongName()
    {
        return $"{Artist} - {Title}";
    }
    
    public override string ToString()
    {
        return GetSongName();
    }
}