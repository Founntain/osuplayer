using System.Text;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A <see cref="BinaryReader" /> to read the osu!.db to extract their beatmap data or to read from the collection.db
/// </summary>
public class OsuDbReader : BinaryReader
{
    public static int OsuDbVersion;

    private readonly byte[] _buf = new byte[512];

    public OsuDbReader(Stream input) : base(input)
    {
    }

    /// <summary>
    /// Reads the osu!.db and skips duplicate beatmaps of one beatmap set
    /// </summary>
    /// <param name="osuPath">the osu! root path where the osu!.db is located</param>
    /// <returns> a <see cref="DbMapEntryBase" /> list</returns>
    public static async Task<List<IMapEntryBase>?> Read(string osuPath)
    {
        var minBeatMaps = new List<IMapEntryBase>();
        var dbLoc = Path.Combine(osuPath, "osu!.db");

        if (!File.Exists(dbLoc)) return null;

        await using var file = File.OpenRead(dbLoc);
        using var reader = new OsuDbReader(file);

        var ver = reader.ReadInt32();
        OsuDbVersion = ver;
        var flag = ver is >= 20160408 and < 20191107;

        reader.ReadInt32();
        reader.ReadBoolean();
        reader.ReadInt64();
        reader.ReadString();

        var mapCount = reader.ReadInt32();

        minBeatMaps.Capacity = mapCount;
        int? prevId = null;

        for (var i = 1; i < mapCount; i++)
        {
            if (flag)
                reader.ReadInt32(); //btlen

            if (prevId != null)
            {
                var length = CalculateMapLength(reader, out var newSetId, out _);
                if (prevId == newSetId)
                {
                    prevId = newSetId;
                    continue;
                }

                reader.BaseStream.Seek(-length, SeekOrigin.Current);
            }

            var minBeatMap = new DbMapEntryBase
            {
                DbOffset = reader.BaseStream.Position
            };

            ReadFromStream(reader, osuPath, ref minBeatMap);
            prevId = minBeatMap.BeatmapSetId;
            minBeatMaps.Add(minBeatMap);
        }

        reader.ReadInt32(); //account rank

        await file.FlushAsync();
        reader.Dispose();
        return minBeatMaps;
    }

    /// <summary>
    /// Reads all difficulties from the osu!.db with hashes and set ids
    /// </summary>
    /// <param name="osuPath">the osu! root path where the osu!.db is located</param>
    /// <returns>
    /// a <see cref="Dictionary{TKey,TValue}" /> where the <b>TKey</b> is the beatmap hash and the <b>TValue</b> is
    /// the set id
    /// </returns>
    public static async Task<Dictionary<string, int>?> ReadAllDiffs(string osuPath)
    {
        var hashes = new Dictionary<string, int>();
        var dbLoc = Path.Combine(osuPath, "osu!.db");

        if (!File.Exists(dbLoc)) return null;

        await using var file = File.OpenRead(dbLoc);
        using var reader = new OsuDbReader(file);

        var ver = reader.ReadInt32();
        OsuDbVersion = ver;
        var flag = ver is >= 20160408 and < 20191107;

        reader.ReadInt32();
        reader.ReadBoolean();
        reader.ReadInt64();
        reader.ReadString();

        var mapCount = reader.ReadInt32();

        for (var i = 1; i < mapCount; i++)
        {
            if (flag)
                reader.ReadInt32(); //btlen

            CalculateMapLength(reader, out var setId, out var hash);

            hashes.Add(hash, setId);
        }

        reader.ReadInt32(); //account rank

        await file.FlushAsync();
        reader.Dispose();
        return hashes;
    }

    /// <summary>
    /// Reads a osu!.db map entry and fills a <see cref="DbMapEntryBase" /> with needed data
    /// </summary>
    /// <param name="r">the <see cref="OsuDbReader" /> instance of the stream</param>
    /// <param name="osuPath">the osu! root path where the osu!.db is located</param>
    /// <param name="dbMapEntryBase">a reference of a <see cref="DbMapEntryBase" /> to read the data to</param>
    private static void ReadFromStream(OsuDbReader r, string osuPath, ref DbMapEntryBase dbMapEntryBase)
    {
        dbMapEntryBase.Artist = string.Intern(r.ReadString());

        if (dbMapEntryBase.Artist.Length == 0)
            dbMapEntryBase.Artist = "Unknown Artist";

        if (OsuDbVersion >= 20121008)
            r.ReadString(true);

        dbMapEntryBase.Title = r.ReadString();

        if (dbMapEntryBase.Title.Length == 0)
            dbMapEntryBase.Title = "Unknown Title";

        if (OsuDbVersion >= 20121008)
            r.ReadString(true);

        r.ReadString(true);
        r.ReadString(true); //Difficulty
        r.ReadString(true);

        dbMapEntryBase.Hash = r.ReadString();

        r.ReadString(true); //BeatmapFileName

        if (OsuDbVersion >= 20140609)
            r.BaseStream.Seek(39, SeekOrigin.Current);
        else
            r.BaseStream.Seek(27, SeekOrigin.Current);

        if (OsuDbVersion >= 20140609)
        {
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
        }

        r.ReadInt32(); //DrainTimeSeconds

        dbMapEntryBase.TotalTime = r.ReadInt32();

        r.ReadInt32(); //AudioPreviewTime

        var timingCnt = r.ReadInt32();

        r.BaseStream.Seek(17 * timingCnt, SeekOrigin.Current);
        r.BaseStream.Seek(4, SeekOrigin.Current);
        dbMapEntryBase.BeatmapSetId = r.ReadInt32();
        r.BaseStream.Seek(15, SeekOrigin.Current);

        r.ReadString(true); //SongSource
        r.ReadString(true); //SongTags
        r.ReadInt16(); //OffsetOnline
        r.ReadString(true); //TitleFont
        r.ReadBoolean(); //Unplayed
        r.ReadDateTime(); //LastPlayed
        r.ReadBoolean(); //IsOsz2
        r.ReadString(true);

        if (OsuDbVersion < 20140609)
            r.BaseStream.Seek(20, SeekOrigin.Current);
        else
            r.BaseStream.Seek(18, SeekOrigin.Current);
    }

    /// <summary>
    /// Reads a osu!.db map entry and calculates the map length in bytes
    /// </summary>
    /// <param name="r">the current <see cref="OsuDbReader" /> instance of the stream</param>
    /// <param name="setId">outputs an <see cref="int" /> of the beatmap set id</param>
    /// <param name="hash">outputs an <see cref="string" /> of the beatmap hash</param>
    /// <returns>a <see cref="long" /> from the byte length of the current map</returns>
    private static long CalculateMapLength(OsuDbReader r, out int setId, out string hash)
    {
        var initOffset = r.BaseStream.Position;

        r.ReadString(true);
        if (OsuDbVersion >= 20121008) r.ReadString(true);

        r.ReadString(true);
        if (OsuDbVersion >= 20121008) r.ReadString(true);

        r.ReadString(true);
        r.ReadString(true);
        r.ReadString(true);
        hash = r.ReadString();
        r.ReadString(true);
        r.BaseStream.Seek(15, SeekOrigin.Current);
        if (OsuDbVersion >= 20140609)
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
        r.ReadString(true);
        r.ReadString(true);
        r.BaseStream.Seek(2, SeekOrigin.Current);
        r.ReadString(true);
        r.BaseStream.Seek(10, SeekOrigin.Current);
        r.ReadString(true);
        if (OsuDbVersion < 20140609)
            r.BaseStream.Seek(20, SeekOrigin.Current);
        else
            r.BaseStream.Seek(18, SeekOrigin.Current);

        return r.BaseStream.Position - initOffset;
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
                    BaseStream.Read(_buf, 0, strLen);
                    return Encoding.UTF8.GetString(_buf, 0, strLen);
                }

                BaseStream.Seek(strLen, SeekOrigin.Current);
                return string.Empty;
            default:
                throw new Exception();
        }
    }

    /// <summary>
    /// Reads the star rating count and moves the base stream accordingly effectively skipping it
    /// </summary>
    public void ReadStarRating()
    {
        var count = ReadInt32();
        BaseStream.Seek(14 * count, SeekOrigin.Current);
    }

    /// <summary>
    /// Reads a <see cref="Int64" /> and converts it to UTC based time
    /// </summary>
    /// <returns>a <see cref="DateTime" /> converted from the read data</returns>
    public DateTime ReadDateTime()
    {
        return new DateTime(ReadInt64(), DateTimeKind.Utc);
    }
}