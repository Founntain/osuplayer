using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Extensions.ObjectExtensions;

public static class BeatmapModelExtensions
{
    public static string FullSongName(this BeatmapModel b) => $"{b.Artist} - {b.Title}";
    public static string FullSongNameUnicode(this BeatmapModel b) => $"{b.ArtistUnicode} - {b.TitleUnicode}";
    public static string TimesPlayedString(this BeatmapModel b) => $"{b.TimesPlayed}x played";
    public static string LastPlayedString(this BeatmapModel b) => string.IsNullOrEmpty(b.LastPlayedBy?.Name) 
        ? "Last played by an unknown user" 
        : $"Last played by {b.LastPlayedBy.Name}";
    public static string MostPlayedString(this BeatmapModel b) => b.MostPlayedBy == null 
        ? string.Empty 
        : $"{b.MostPlayedBy.Item1} played it most with {b.MostPlayedBy.Item2} times";
}