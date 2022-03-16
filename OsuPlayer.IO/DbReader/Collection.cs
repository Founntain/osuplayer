namespace OsuPlayer.IO.DbReader;

public class Collection
{
    public List<string> BeatmapHashes;
    public string Name;

    public static Collection ReadFromReader(DbReader r)
    {
        Collection collection = new();
        collection.ReadFromStream(r);
        return collection;
    }

    private void ReadFromStream(DbReader r)
    {
        Name = r.ReadString();
        BeatmapHashes = new List<string>();
        var num = r.ReadInt32();
        for (var index = 0; index < num; ++index)
            BeatmapHashes.Add(r.ReadString());
    }
}