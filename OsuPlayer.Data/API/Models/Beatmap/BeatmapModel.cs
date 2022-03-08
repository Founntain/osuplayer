namespace OsuPlayer.Data.API.Models.Beatmap;

public sealed class BeatmapModel
{
    public int Id { get; set; }
    public DateTime CreationTime { get; set; }
    public int BeatmapSetId { get; set; }
    public int BeatmapId { get; set; }
    public string BeatmapChecksum { get; set; }
    public string Artist { get; set; }
    public string ArtistUnicode { get; set; }
    public string Title { get; set; }
    public string TitleUnicode { get; set; }
    public string Creator { get; set; }
    public int TimesPlayed { get; set; }
    public int LastPlayedBy { get; set; }
    public Tuple<string, int> MostPlayedBy { get; set; }
}