using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader;

public class MapEntry
{
    public string Artist;
    public string ArtistString => GetArtist();
    public string ArtistUnicode;
    public string AudioFileName;
    public string BeatmapChecksum;
    public int BeatmapId;
    public int BeatmapSetId;
    public string Creator;
    public string DifficultyName;
    public string FolderName;
    public string Title;
    public string TitleString => GetTitle();
    public string TitleUnicode;
    public string Fullpath;
    
    public string SongName => GetSongName();

    public int TotalTime;
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();

    public int Ver;

    private string GetArtist()
    {
        if (!Config.GetConfigInstance().UseSongNameUnicode) return Artist;

        return !string.IsNullOrWhiteSpace(ArtistUnicode) ? ArtistUnicode : Artist;
    }

    private string GetTitle()
    {
        if (!Config.GetConfigInstance().UseSongNameUnicode) return Title;

        return !string.IsNullOrWhiteSpace(TitleUnicode) ? TitleUnicode : Title;
    }

    private string GetSongName()
    {
        if (!Config.GetConfigInstance().UseSongNameUnicode) return $"{Artist} - {Title}";

        if (!string.IsNullOrWhiteSpace(ArtistUnicode) && !string.IsNullOrWhiteSpace(TitleUnicode))
            return $"{ArtistUnicode} - {TitleUnicode}";

        return $"{Artist} - {Title}";
    }
    
    public override string ToString()
    {
        return Artist + " - " + Title;
    }
}