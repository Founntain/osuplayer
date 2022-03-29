namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public static class ScoreInfoExtensions
{
    /// <summary>
    /// A user-presentable display title representing this score.
    /// </summary>
    public static string GetDisplayTitle(this IScoreInfo scoreInfo) => $"{scoreInfo.User.Username} playing {scoreInfo.Beatmap.GetDisplayTitle()}";
}