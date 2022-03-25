using System.Diagnostics;
using System.Text;
using OsuPlayer.IO.Storage.Config;

namespace OsuPlayer.IO.DbReader;

/// <summary>
///     A <see cref="BinaryReader" /> to read the osu!.db to extract their beatmap data or to read from the collection.db
/// </summary>
public partial class DbReader : BinaryReader
{
    private DbReader(Stream input) : base(input)
    {
    }

    public static int OsuDbVersion;

    private byte[] _buf = new byte[512];

    public static async Task<List<MinimalMapEntry>?> ReadOsuDb(string osuPath)
    {
        var minBeatMaps = new List<MinimalMapEntry>();
        var dbLoc = Path.Combine(osuPath, "osu!.db");

        if (!File.Exists(dbLoc)) return null;

        await using var config = new Config();
        var unicode = (await config.ReadAsync()).UseSongNameUnicode;

        await using var file = File.OpenRead(dbLoc);
        using var reader = new DbReader(file);
        var ver = reader.ReadInt32();
        OsuDbVersion = ver;
        var flag = ver is >= 20160408 and < 20191107;

        reader.ReadInt32();
        reader.ReadBoolean();
        reader.ReadInt64();
        reader.ReadString();

        var mapCount = reader.ReadInt32();

        minBeatMaps.Capacity = mapCount;
        var prevId = -1;

        for (var i = 1; i < mapCount; i++)
        {
            if (flag)
                reader.ReadInt32(); //btlen

            if (prevId != -1)
            {
                var length = CalculateMapLength(reader, OsuDbVersion, out var newSetId);
                if (prevId == newSetId)
                {
                    prevId = newSetId;
                    continue;
                }

                reader.BaseStream.Seek(-length, SeekOrigin.Current);
            }

            var minBeatMap = new MinimalMapEntry
            {
                DbOffset = reader.BaseStream.Position
            };

            ReadFromStreamMinimal(reader, ver, osuPath, ref minBeatMap, out var curSetId);
            prevId = curSetId;
            minBeatMaps.Add(minBeatMap);
        }

        reader.ReadInt32(); //account rank

        await file.FlushAsync();
        reader.Dispose();
        return minBeatMaps;
    }

    private static long CalculateMapLength(DbReader r, int version, out int setId)
    {
        var initOffset = r.BaseStream.Position;

        r.GetStringLen();
        if (version >= 20121008)
        {
            r.GetStringLen();
        }

        r.GetStringLen();
        if (version >= 20121008)
        {
            r.GetStringLen();
        }

        r.GetStringLen();
        r.GetStringLen();
        r.GetStringLen();
        r.GetStringLen();
        r.GetStringLen();
        r.BaseStream.Seek(15, SeekOrigin.Current);
        if (version >= 20140609)
            r.BaseStream.Seek(16, SeekOrigin.Current);
        else
            r.BaseStream.Seek(4, SeekOrigin.Current);

        r.BaseStream.Seek(8, SeekOrigin.Current);
        if (OsuDbVersion >= 20140609)
        {
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
        }

        r.BaseStream.Seek(12, SeekOrigin.Current);
        var timingCnt = r.ReadInt32();
        r.BaseStream.Seek(timingCnt * 17, SeekOrigin.Current);
        r.BaseStream.Seek(4, SeekOrigin.Current);
        setId = r.ReadInt32();
        r.BaseStream.Seek(15, SeekOrigin.Current);
        r.GetStringLen();
        r.GetStringLen();
        r.BaseStream.Seek(2, SeekOrigin.Current);
        r.GetStringLen();
        r.BaseStream.Seek(10, SeekOrigin.Current);
        r.GetStringLen();
        if (version < 20140609)
            r.BaseStream.Seek(20, SeekOrigin.Current);
        else
            r.BaseStream.Seek(18, SeekOrigin.Current);

        return r.BaseStream.Position - initOffset;
    }

    public static List<Collection>? ReadCollections(string osuPath)
    {
        var collections = new List<Collection>();
        var colLoc = Path.Combine(osuPath, "collection.db");

        if (!File.Exists(colLoc)) return null;

        using (DbReader reader = new(File.OpenRead(colLoc)))
        {
            reader.ReadInt32(); //osuVersion
            var num = reader.ReadInt32();

            for (var i = 0; i < num; i++) collections.Add(Collection.ReadFromReader(reader));
        }

        return collections;
    }

    public string ReadString(bool ignore = false)
    {
        switch (ReadByte())
        {
            case 0:
                return string.Empty;
            case 11:
                var strLen = Read7BitEncodedInt();
                if (!ignore)
                {
                    BaseStream.Read(_buf, 0, strLen); // ReadBytes(strLen);
                    return Encoding.UTF8.GetString(_buf, 0, strLen);
                }

                BaseStream.Seek(strLen, SeekOrigin.Current);
                return string.Empty;
            default:
                throw new Exception();
        }
    }

    private int GetStringLen()
    {
        switch (ReadByte())
        {
            case 0:
                return 0;
            case 11:
                var strLen = Read7BitEncodedInt();
                BaseStream.Seek(strLen, SeekOrigin.Current);
                return strLen;
            default:
                throw new Exception();
        }
    }

    public void ReadStarRating()
    {
        var count = ReadInt32();
        BaseStream.Seek(14 * count, SeekOrigin.Current);
    }

    public DateTime ReadDateTime()
    {
        return new DateTime(ReadInt64(), DateTimeKind.Utc);
    }
}