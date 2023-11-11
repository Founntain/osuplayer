using System.Text;
using Nein.Extensions;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Services;
using Splat;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A <see cref="BinaryReader" /> to read the osu!.db to extract their beatmap data or to read from the collection.db
/// </summary>
public class OsuDbReader : BinaryReader, IDatabaseReader
{
    private readonly byte[] _buf = new byte[512];
    private readonly string _path;
    private static int _osuDbVersion;
    private readonly IDbReaderFactory _readerFactory;

    public OsuDbReader(Stream input, string path, IDbReaderFactory readerFactory) : base(input)
    {
        _readerFactory = readerFactory;

        _path = string.Intern(path);
    }

    public Task<List<IMapEntryBase>?> ReadBeatmaps()
    {
        var minBeatMaps = new List<IMapEntryBase>();

        var ver = ReadInt32();
        _osuDbVersion = ver;
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

            if (prevId != null)
            {
                var length = CalculateMapLength(out var newSetId, out _);

                if (prevId == newSetId)
                {
                    prevId = newSetId;
                    continue;
                }

                BaseStream.Seek(-length, SeekOrigin.Current);
            }

            ReadFromStream(out var minBeatMap);

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
        _osuDbVersion = ver;
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

    public async Task<List<OsuCollection>?> GetCollections(string path)
    {
        return await OsuCollectionReader.Read(path);
    }

    public IMapEntry? ReadFullEntry(string path, IMapEntryBase mapEntryBase, long? dbOffset = null, Guid? id = null)
    {
        try
        {
            if (dbOffset == null)
                return null;

            var version = _osuDbVersion;

            BaseStream.Seek(dbOffset.Value, SeekOrigin.Begin);

            ReadString(true); //Artist

            var artistUnicode = "Unknown Artist";
            if (version >= 20121008)
                artistUnicode = ReadString();

            ReadString(true); //Title

            var titleUnicode = "Unknown Title";
            if (version >= 20121008)
                titleUnicode = ReadString();

            ReadString(true); //Creator
            ReadString(true); //Difficulty

            var audioFileName = ReadString();
            ReadString(true); //Hash

            ReadString(true); //BeatmapFileName
            ReadByte(); //RankedStatus
            ReadUInt16(); //CountHitCircles
            ReadUInt16(); //CountSliders
            ReadUInt16(); //CountSpinners
            ReadDateTime(); //LastModifiedTime

            if (version >= 20140609)
            {
                ReadSingle(); //ApproachRate
                ReadSingle(); //CircleSize
                ReadSingle(); //HPDrainRate
                ReadSingle(); //OverallDifficulty
            }
            else
            {
                //Float
                ReadByte(); //ApproachRate
                ReadByte(); //CircleSize
                ReadByte(); //HPDrainRate
                ReadByte(); //OverallDifficulty
            }

            ReadDouble(); //SliderVelocity

            if (version >= 20140609)
            {
                ReadStarRating();
                ReadStarRating();
                ReadStarRating();
                ReadStarRating();
            }

            ReadInt32(); //DrainTimeSeconds
            ReadInt32(); //TotalTimeSeconds

            ReadInt32(); //AudioPreviewTime
            var timingCount = ReadInt32();

            BaseStream.Position += 17 * timingCount;

            ReadInt32();
            ReadInt32(); //beatmapSetId

            ReadInt32(); //ThreadId
            ReadByte(); //GradeStandard
            ReadByte(); //GradeTaiko
            ReadByte(); //GradeCtB
            ReadByte(); //GradeMania
            ReadInt16(); //OffsetLocal
            ReadSingle(); //StackLeniency
            ReadByte(); //GameMode
            ReadString(true); //SongSource
            ReadString(true); //SongTags
            ReadInt16(); //OffsetOnline
            ReadString(true); //TitleFont
            ReadBoolean(); //Unplayed
            ReadDateTime(); //LastPlayed
            ReadBoolean(); //IsOsz2

            var folderName = ReadString();

            ReadDateTime(); //LastCheckAgainstOsuRepo
            ReadBoolean(); //IgnoreBeatmapSounds
            ReadBoolean(); //IgnoreBeatmapSkin
            ReadBoolean(); //DisableStoryBoard
            ReadBoolean(); //DisableVideo
            ReadBoolean(); //

            if (version < 20140609)
                ReadInt16(); //OldUnknown1

            ReadInt32(); //LastEditTime
            ReadByte(); //ManiaScrollSpeed

            var fullPath = Path.Combine(path, "Songs", folderName, audioFileName);
            var folderPath = Path.Combine(path, "Songs", folderName);

            return new DbMapEntry
            {
                DbReaderFactory = _readerFactory,
                DbOffset = dbOffset.Value,
                OsuPath = path,
                Artist = mapEntryBase.Artist,
                ArtistUnicode = artistUnicode,
                Title = mapEntryBase.Title,
                TitleUnicode = titleUnicode,
                AudioFileName = audioFileName,
                BeatmapSetId = mapEntryBase.BeatmapSetId,
                FolderName = folderName,
                FolderPath = folderPath,
                FullPath = fullPath,
                Hash = mapEntryBase.Hash,
                TotalTime = mapEntryBase.TotalTime
            };
        }
        catch (Exception ex)
        {
            var loggingService = Locator.Current.GetRequiredService<ILoggingService>();

            loggingService.Log($"There was an error reading the beatmap ({path})", LogType.Error, ex.Message);

            return null;
        }
    }

    /// <summary>
    /// Reads a osu!.db map entry and fills a <see cref="DbMapEntryBase" /> with needed data
    /// </summary>
    /// <param name="minBeatmap">A <see cref="DbMapEntryBase" /> as an out parameter</param>
    private void ReadFromStream(out DbMapEntryBase minBeatmap)
    {
        var dbOffset = BaseStream.Position;
        var artist = string.Intern(ReadString());

        if (artist.Length == 0)
            artist = "Unknown Artist";

        if (_osuDbVersion >= 20121008)
            ReadString(true);

        var title = string.Intern(ReadString());

        if (title.Length == 0)
            title = "Unknown Title";

        if (_osuDbVersion >= 20121008)
            ReadString(true);

        ReadString(true);
        ReadString(true); //Difficulty
        ReadString(true);

        var hash = ReadString(); //Hash

        ReadString(true); //BeatmapFileName

        BaseStream.Seek(_osuDbVersion >= 20140609 ? 39 : 27, SeekOrigin.Current);

        if (_osuDbVersion >= 20140609)
        {
            ReadStarRating();
            ReadStarRating();
            ReadStarRating();
            ReadStarRating();
        }

        ReadInt32(); //DrainTimeSeconds

        var totalTime = ReadInt32();

        ReadInt32(); //AudioPreviewTime

        var timingCnt = ReadInt32();

        BaseStream.Seek(17 * timingCnt, SeekOrigin.Current);
        BaseStream.Seek(4, SeekOrigin.Current);

        var beatmapSetId = ReadInt32();

        BaseStream.Seek(15, SeekOrigin.Current);

        ReadString(true); //SongSource
        ReadString(true); //SongTags
        ReadInt16(); //OffsetOnline
        ReadString(true); //TitleFont
        ReadBoolean(); //Unplayed
        ReadDateTime(); //LastPlayed
        ReadBoolean(); //IsOsz2
        ReadString(true);

        BaseStream.Seek(_osuDbVersion < 20140609 ? 20 : 18, SeekOrigin.Current);

        minBeatmap = new DbMapEntryBase
        {
            DbReaderFactory = _readerFactory,
            OsuPath = string.Intern(_path),
            Artist = artist,
            Title = title,
            BeatmapSetId = beatmapSetId,
            DbOffset = dbOffset,
            Hash = hash,
            TotalTime = totalTime
        };
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
        if (_osuDbVersion >= 20121008) ReadString(true);

        ReadString(true);
        if (_osuDbVersion >= 20121008) ReadString(true);

        ReadString(true);
        ReadString(true);
        ReadString(true);

        hash = ReadString();

        ReadString(true);
        BaseStream.Seek(15, SeekOrigin.Current);
        BaseStream.Seek(_osuDbVersion >= 20140609 ? 16 : 4, SeekOrigin.Current);

        BaseStream.Seek(8, SeekOrigin.Current);
        if (_osuDbVersion >= 20140609)
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

        BaseStream.Seek(_osuDbVersion < 20140609 ? 20 : 18, SeekOrigin.Current);

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
                throw new InvalidOperationException("The byte that marks the string length is neither 0 nor 11");
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