namespace OsuPlayer.IO.DbReader;

/// <summary>
/// Represents a collection from osu!
/// </summary>
public class Collection
{
    public string Name { get; set; }
    public List<string> BeatmapHashes { get; } = new();
}