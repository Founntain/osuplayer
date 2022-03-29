using OsuPlayer.IO.Storage.LazerModels.Beatmaps;

namespace OsuPlayer.IO.Storage.LazerModels.Extensions;

public static class ScoreInfoExtensions
{
    /// <summary>
    /// A user-presentable display title representing this score.
    /// </summary>
    public static string GetDisplayTitle(this IScoreInfo scoreInfo)
    {
        return $"{scoreInfo.User.Username} playing {scoreInfo.Beatmap.GetDisplayTitle()}";
    }
}