using OsuPlayer.IO.Storage.LazerModels.Beatmaps;

namespace OsuPlayer.IO.Storage.LazerModels.Extensions;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
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