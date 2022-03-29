namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public static class BeatmapInfoExtionsions
{
    /// <summary>
    /// A user-presentable display title representing this beatmap.
    /// </summary>
    public static string GetDisplayTitle(this IBeatmapInfo beatmapInfo) => $"{beatmapInfo.Metadata.GetDisplayTitle()} {GetVersionString(beatmapInfo)}".Trim();
    
    private static string GetVersionString(IBeatmapInfo beatmapInfo) => string.IsNullOrEmpty(beatmapInfo.DifficultyName) ? string.Empty : $"[{beatmapInfo.DifficultyName}]";

}