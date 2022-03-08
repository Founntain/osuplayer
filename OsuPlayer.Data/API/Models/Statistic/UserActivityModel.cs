namespace OsuPlayer.Data.API.Models.Statistic;

public sealed class UserActivityModel
{
    public int Month { get; set; }
    public int Year { get; set; }
    public int SongsPlayed { get; set; }
    public int XpGained { get; set; }
}