using System.Text;
using OsuPlayer.Data.DataModels;

namespace OsuPlayer.IO.DbReader;

public class OsuCollectionReader : BinaryReader
{
    private readonly byte[] _buf = new byte[512];

    public OsuCollectionReader(Stream input) : base(input)
    {
    }

    /// <summary>
    /// Reads the collection from the collection.db
    /// </summary>
    /// <param name="osuPath">the osu full path</param>
    /// <returns>a <see cref="OsuCollection" /> list</returns>
    public static async Task<List<OsuCollection>?> Read(string osuPath)
    {
        var collections = new List<OsuCollection>();
        var colLoc = Path.Combine(osuPath, "collection.db");

        if (!File.Exists(colLoc)) return null;

        await using var file = File.OpenRead(colLoc);
        using OsuCollectionReader reader = new(file);

        reader.ReadInt32(); //osuVersion

        var num = reader.ReadInt32();

        for (var i = 0; i < num; i++) collections.Add(ReadFromStream(reader));

        await file.FlushAsync();
        reader.Dispose();

        return collections;
    }

    private static OsuCollection ReadFromStream(OsuCollectionReader r)
    {
        var collection = new OsuCollection
        {
            Name = r.ReadString()
        };

        var num = r.ReadInt32();

        for (var index = 0; index < num; ++index)
            collection.BeatmapHashes.Add(r.ReadString());

        return collection;
    }

    /// <summary>
    /// Returns a ULEB128 length encoded string from the base stream
    /// </summary>
    /// <param name="ignore">the string will not be read and the base stream will skip it</param>
    /// <returns>
    /// a <see cref="string" /> containing the read string if string mark byte was 11 or an empty string if
    /// <paramref name="ignore" /> is true or the string mark byte was 0
    /// </returns>
    /// <exception cref="Exception">throws if the string mark byte is neither 0 nor 11</exception>
    private string ReadString(bool ignore = false)
    {
        switch (ReadByte())
        {
            case 0:
                return string.Empty;
            case 11:
                var strLen = Read7BitEncodedInt();
                if (!ignore)
                {
                    BaseStream.Read(_buf, 0, strLen);
                    return Encoding.UTF8.GetString(_buf, 0, strLen);
                }

                BaseStream.Seek(strLen, SeekOrigin.Current);
                return string.Empty;
            default:
                throw new Exception();
        }
    }
}