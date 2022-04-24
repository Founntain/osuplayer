namespace OsuPlayer.IO.DbReader;

/// <summary>
/// Represets a collection from osu!
/// </summary>
public class Collection
{
    public List<string> BeatmapHashes;
    public string Name;

    public static Collection ReadFromReader(OsuDbReader r)
    {
        Collection collection = new();
        collection.ReadFromStream(r);
        return collection;
    }

    private void ReadFromStream(OsuDbReader r)
    {
        Name = r.ReadString();
        BeatmapHashes = new List<string>();
        var num = r.ReadInt32();
        for (var index = 0; index < num; ++index)
            BeatmapHashes.Add(r.ReadString());
    }
}