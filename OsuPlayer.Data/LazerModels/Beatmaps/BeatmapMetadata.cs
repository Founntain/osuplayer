using System.Text.Json.Serialization;
using JetBrains.Annotations;
using OsuPlayer.Data.LazerModels.Extensions;
using OsuPlayer.Data.LazerModels.Interfaces;
using Realms;

namespace OsuPlayer.Data.LazerModels.Beatmaps;

// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
[Serializable]
[MapTo("BeatmapMetadata")]
public class BeatmapMetadata : RealmObject, IBeatmapMetadataInfo
{
    public RealmUser Author { get; set; } = null!;

    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("title_unicode")]
    public string TitleUnicode { get; set; } = string.Empty;

    public string Artist { get; set; } = string.Empty;

    [JsonPropertyName("artist_unicode")]
    public string ArtistUnicode { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    [JsonPropertyName(@"tags")]
    public string Tags { get; set; } = string.Empty;

    /// <summary>
    /// The time in milliseconds to begin playing the track for preview purposes.
    /// If -1, the track should begin playing at 40% of its length.
    /// </summary>
    public int PreviewTime { get; set; } = -1;

    public string AudioFile { get; set; } = string.Empty;
    public string BackgroundFile { get; set; } = string.Empty;

    IUser IBeatmapMetadataInfo.Author => Author;

    public BeatmapMetadata(RealmUser? user = null)
    {
        Author = user ?? new RealmUser();
    }

    [UsedImplicitly] // Realm
    private BeatmapMetadata()
    {
    }

    public override string ToString()
    {
        return this.GetDisplayTitle();
    }
}