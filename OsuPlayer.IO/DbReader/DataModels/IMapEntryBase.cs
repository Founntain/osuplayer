using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader.DataModels;

public interface IMapEntryBase
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string BeatmapChecksum { get; set; }
    public int TotalTime { get; set; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    public string GetArtist();

    public string GetTitle();

    public string GetSongName();

    public Task<IMapEntry> ReadFullEntry(string path);
}