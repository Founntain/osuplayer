using OsuPlayer.Data.LazerModels.Beatmaps;

namespace OsuPlayer.Data.LazerModels.Extensions;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
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