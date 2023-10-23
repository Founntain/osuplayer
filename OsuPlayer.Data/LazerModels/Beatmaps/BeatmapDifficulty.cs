using Realms;

namespace OsuPlayer.Data.LazerModels.Beatmaps;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
[MapTo("BeatmapDifficulty")]
public class BeatmapDifficulty : EmbeddedObject, IBeatmapDifficultyInfo
{
    /// <summary>
    /// The default value used for all difficulty settings except <see cref="SliderMultiplier" /> and
    /// <see cref="SliderTickRate" />.
    /// </summary>
    public const float DEFAULT_DIFFICULTY = 5;

    public float DrainRate { get; set; } = IBeatmapDifficultyInfo.DEFAULT_DIFFICULTY;
    public float CircleSize { get; set; } = IBeatmapDifficultyInfo.DEFAULT_DIFFICULTY;
    public float OverallDifficulty { get; set; } = IBeatmapDifficultyInfo.DEFAULT_DIFFICULTY;
    public float ApproachRate { get; set; } = IBeatmapDifficultyInfo.DEFAULT_DIFFICULTY;

    public double SliderMultiplier { get; set; } = 1;
    public double SliderTickRate { get; set; } = 1;
}