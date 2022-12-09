namespace OsuPlayer.IO.DbReader;

/// <summary>
/// Represents a collection from osu!
/// </summary>
public class Collection
{
    public string Name { get; set; } = string.Empty;
    public List<string> BeatmapHashes { get; private set; } = new();

    public Collection()
    {
    }

    public Collection(string name, List<string> beatmapHashes)
    {
        Name = name;
        BeatmapHashes = beatmapHashes;
    }
}