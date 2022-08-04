namespace OsuPlayer.Network;

public sealed class UpdateResponse
{
    public bool IsNewVersionAvailable { get; set; }
    public bool IsPrerelease { get; set; }
    public string? HtmlUrl { get; set; }
    public string? Version { get; set; }
    public string? PatchNotes { get; set; }
    public DateTimeOffset? ReleaseDate { get; set; }
}