using OsuPlayer.IO.Storage.LazerModels.Beatmaps;

namespace OsuPlayer.IO.Storage.LazerModels.Extensions;

public static class BeatmapInfoExtionsions
{
    /// <summary>
    /// A user-presentable display title representing this beatmap.
    /// </summary>
    public static string GetDisplayTitle(this IBeatmapInfo beatmapInfo)
    {
        return $"{beatmapInfo.Metadata.GetDisplayTitle()} {GetVersionString(beatmapInfo)}".Trim();
    }

    private static string GetVersionString(IBeatmapInfo beatmapInfo)
    {
        return string.IsNullOrEmpty(beatmapInfo.DifficultyName) ? string.Empty : $"[{beatmapInfo.DifficultyName}]";
    }
}