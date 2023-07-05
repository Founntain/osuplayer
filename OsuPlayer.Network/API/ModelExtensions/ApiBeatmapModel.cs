using OsuPlayer.Api.Data.API.EntityModels;

namespace OsuPlayer.Network.API.ModelExtensions;

public class ApiBeatmapModel : BeatmapModel
{
    public string FullSongName => $"{Artist} - {Title}";
    public string FullSongNameUnicode => $"{ArtistUnicode} - {TitleUnicode}";
    public string LastPlayedString => string.IsNullOrEmpty(LastPlayedBy?.Name) ? "Last played by an unkown user" : $"Last played by {LastPlayedBy.Name}";
    public string TimesPlayedString => $"{TimesPlayed} times played";
    public string MostPlayedString => $"{MostPlayedBy.Item1} played it most with {MostPlayedBy.Item2} times";
}