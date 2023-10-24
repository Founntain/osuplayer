namespace OsuPlayer.Data.DataModels;

/// <summary>
/// Represents a collection from osu!
/// </summary>
public class OsuCollection
{
    public string Name { get; set; } = string.Empty;
    public List<string> BeatmapHashes { get; private set; } = new();

    public OsuCollection()
    {
    }

    public OsuCollection(string name, List<string> beatmapHashes)
    {
        Name = name;
        BeatmapHashes = beatmapHashes;
    }
}