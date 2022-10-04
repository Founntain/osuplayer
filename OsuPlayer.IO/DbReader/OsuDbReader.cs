using System.Text;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A <see cref="BinaryReader" /> to read the osu!.db to extract their beatmap data or to read from the collection.db
/// </summary>
public class OsuDbReader : BinaryReader, IDatabaseReader
{
    public static int OsuDbVersion { get; private set; }

    private readonly byte[] _buf = new byte[512];

    public OsuDbReader(Stream input) : base(input)
    {
    }

    public OsuDbReader(Stream input, string path) : base(input)
    {
    }

    public Task<List<IMapEntryBase>?> ReadBeatmaps()
    {
        var minBeatMaps = new List<IMapEntryBase>();

        var ver = ReadInt32();
        OsuDbVersion = ver;
        var flag = ver is >= 20160408 and < 20191107;

        ReadInt32();
        ReadBoolean();
        ReadInt64();
        ReadString();

        var mapCount = ReadInt32();

        minBeatMaps.Capacity = mapCount;
        int? prevId = null;

        for (var i = 1; i < mapCount; i++)
        {
            if (flag)
                ReadInt32(); //bt length

            var hash = string.Empty;

            if (prevId != null)
            {
                var length = CalculateMapLength(out var newSetId, out hash);
                if (prevId == newSetId)
                {
                    prevId = newSetId;
                    continue;
                }

                BaseStream.Seek(-length, SeekOrigin.Current);
            }

            var minBeatMap = new DbMapEntryBase
            {
                DbOffset = BaseStream.Position,
                Hash = hash
            };

            ReadFromStream(ref minBeatMap);
            prevId = minBeatMap.BeatmapSetId;
            minBeatMaps.Add(minBeatMap);
        }

        ReadInt32(); //account rank

        Dispose();
        return Task.FromResult(minBeatMaps);
    }

    public Dictionary<string, int> GetBeatmapHashes()
    {
        var hashes = new Dictionary<string, int>();

        var ver = ReadInt32();
        OsuDbVersion = ver;
        var flag = ver is >= 20160408 and < 20191107;

        ReadInt32();
        ReadBoolean();
        ReadInt64();
        ReadString();

        var mapCount = ReadInt32();

        for (var i = 1; i < mapCount; i++)
        {
            if (flag)
                ReadInt32(); //bt length

            CalculateMapLength(out var setId, out var hash);

            if (hashes.Keys.Contains(hash)) continue;

            hashes.Add(hash, setId);
        }

        ReadInt32(); //account rank

        Dispose();
        return hashes;
    }

    public async Task<List<Collection>?> GetCollections(string path)
    {
        return await OsuCollectionReader.Read(path);
    }

    public static async Task<List<IMapEntryBase>?> Read(string path)
    {
        var dbLoc = Path.Combine(path, "osu!.db");

        if (!File.Exists(dbLoc)) return null;

        var file = File.OpenRead(dbLoc);

        var reader = new OsuDbReader(file, path);

        return await reader.ReadBeatmaps();
    }

    /// <summary>
    /// Reads a osu!.db map entry and fills a <see cref="DbMapEntryBase" /> with needed data
    /// </summary>
    /// <param name="dbMapEntryBase">a reference of a <see cref="DbMapEntryBase" /> to read the data to</param>
    private void ReadFromStream(ref DbMapEntryBase dbMapEntryBase)
    {
        dbMapEntryBase.Artist = string.Intern(ReadString());

        if (dbMapEntryBase.Artist.Length == 0)
            dbMapEntryBase.Artist = "Unknown Artist";

        if (OsuDbVersion >= 20121008)
            ReadString(true);

        dbMapEntryBase.Title = string.Intern(ReadString());

        if (dbMapEntryBase.Title.Length == 0)
            dbMapEntryBase.Title = "Unknown Title";

        if (OsuDbVersion >= 20121008)
            ReadString(true);

        ReadString(true);
        ReadString(true); //Difficulty
        ReadString(true);

        ReadString(true); //Hash

        ReadString(true); //BeatmapFileName

        BaseStream.Seek(OsuDbVersion >= 20140609 ? 39 : 27, SeekOrigin.Current);

        if (OsuDbVersion >= 20140609)
        {
            ReadStarRating();
            ReadStarRating();
            ReadStarRating();
            ReadStarRating();
        }

        ReadInt32(); //DrainTimeSeconds

        dbMapEntryBase.TotalTime = ReadInt32();

        ReadInt32(); //AudioPreviewTime

        var timingCnt = ReadInt32();

        BaseStream.Seek(17 * timingCnt, SeekOrigin.Current);
        BaseStream.Seek(4, SeekOrigin.Current);
        dbMapEntryBase.BeatmapSetId = ReadInt32();
        BaseStream.Seek(15, SeekOrigin.Current);

        ReadString(true); //SongSource
        ReadString(true); //SongTags
        ReadInt16(); //OffsetOnline
        ReadString(true); //TitleFont
        ReadBoolean(); //Unplayed
        ReadDateTime(); //LastPlayed
        ReadBoolean(); //IsOsz2
        ReadString(true);

        BaseStream.Seek(OsuDbVersion < 20140609 ? 20 : 18, SeekOrigin.Current);
    }

    /// <summary>
    /// Reads a osu!.db map entry and calculates the map length in bytes
    /// </summary>
    /// <param name="setId">outputs an <see cref="int" /> of the beatmap set id</param>
    /// <param name="hash">outputs an <see cref="string" /> of the beatmap hash</param>
    /// <returns>a <see cref="long" /> from the byte length of the current map</returns>
    private long CalculateMapLength(out int setId, out string hash)
    {
        var initOffset = BaseStream.Position;

        ReadString(true);
        if (OsuDbVersion >= 20121008) ReadString(true);

        ReadString(true);
        if (OsuDbVersion >= 20121008) ReadString(true);

        ReadString(true);
        ReadString(true);
        ReadString(true);
        hash = ReadString();
        ReadString(true);
        BaseStream.Seek(15, SeekOrigin.Current);
        BaseStream.Seek(OsuDbVersion >= 20140609 ? 16 : 4, SeekOrigin.Current);

        BaseStream.Seek(8, SeekOrigin.Current);
        if (OsuDbVersion >= 20140609)
        {
            ReadStarRating();
            ReadStarRating();
            ReadStarRating();
            ReadStarRating();
        }

        BaseStream.Seek(12, SeekOrigin.Current);
        var timingCnt = ReadInt32();
        BaseStream.Seek(timingCnt * 17, SeekOrigin.Current);
        BaseStream.Seek(4, SeekOrigin.Current);
        setId = ReadInt32();
        BaseStream.Seek(15, SeekOrigin.Current);
        ReadString(true);
        ReadString(true);
        BaseStream.Seek(2, SeekOrigin.Current);
        ReadString(true);
        BaseStream.Seek(10, SeekOrigin.Current);
        ReadString(true);
        BaseStream.Seek(OsuDbVersion < 20140609 ? 20 : 18, SeekOrigin.Current);

        return BaseStream.Position - initOffset;
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
                    _ = BaseStream.Read(_buf, 0, strLen);
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