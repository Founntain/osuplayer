using OsuPlayer.Data.LazerModels.Beatmaps;

namespace OsuPlayer.Data.LazerModels.Extensions;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
public static class BeatmapMetadataInfoExtensions
{
    /// <summary>
    /// A user-presentable display title representing this metadata.
    /// </summary>
    public static string GetDisplayTitle(this IBeatmapMetadataInfo metadataInfo)
    {
        var author = string.IsNullOrEmpty(metadataInfo.Author.Username) ? string.Empty : $" ({metadataInfo.Author.Username})";

        var artist = string.IsNullOrEmpty(metadataInfo.Artist) ? "unknown artist" : metadataInfo.Artist;
        var title = string.IsNullOrEmpty(metadataInfo.Title) ? "unknown title" : metadataInfo.Title;

        return $"{artist} - {title}{author}".Trim();
    }
}