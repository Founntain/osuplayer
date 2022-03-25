using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader;

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

    protected virtual string GetArtist()
    {
        return Artist;
    }

    protected virtual string GetTitle()
    {

        return Title;
    }

    protected virtual string GetSongName()
    {
        return $"{Artist} - {Title}";
    }
    
    public override string ToString()
    {
        return GetSongName();
    }
}