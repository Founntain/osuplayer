﻿using Octokit;

namespace OsuPlayer.Network;

/// <summary>
/// A data structure for an update.
/// </summary>
public sealed class UpdateResponse
{
    public bool IsNewVersionAvailable { get; set; }
    public bool IsPrerelease { get; set; }
    public string? HtmlUrl { get; set; }
    public string? Version { get; set; }
    public string? PatchNotes { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
    public IReadOnlyList<ReleaseAsset>? Assets { get; set; }
}