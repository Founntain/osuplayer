using System.Text;
using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a minimal beatmap entry with only frequently used data
/// <remarks>created on call of <see cref="OsuDbReader.Read(string)" /></remarks>
/// </summary>
public class DbMapEntryBase : IMapEntryBase
{
    public long DbOffset { get; init; }
    public string Artist { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Hash { get; init; } = string.Empty;
    public int BeatmapSetId { get; init; }
    public int TotalTime { get; init; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    /// <summary>
    /// Gets the artist
    /// <remarks>may be overridden for usage with <see cref="DbMapEntry.UseUnicode" /></remarks>
    /// </summary>
    /// <returns>the artist</returns>
    public virtual string GetArtist()
    {
        return Artist;
    }

    /// <summary>
    /// Gets the title
    /// <remarks>may be overridden for usage with <see cref="DbMapEntry.UseUnicode" /></remarks>
    /// </summary>
    /// <returns>the title</returns>
    public virtual string GetTitle()
    {
        return Title;
    }

    public string GetSongName()
    {
        return $"{GetArtist()} - {GetTitle()}";
    }

    public override string ToString()
    {
        return GetSongName();
    }

    /// <summary>
    /// Reads a osu!.db map entry and fills a full <see cref="DbMapEntry" /> with data
    /// </summary>
    /// <param name="osuPath">a <see cref="string" /> of the osu! path</param>
    /// <returns>a new <see cref="DbMapEntry" /> generated from osu!.db data</returns>
    public async Task<IMapEntry?> ReadFullEntry(string osuPath)
    {
        var version = OsuDbReader.OsuDbVersion;

        var dbLoc = Path.Combine(osuPath, "osu!.db");

        if (!File.Exists(dbLoc)) return null;

        await using var file = File.OpenRead(dbLoc);

        using var r = new OsuDbReader(file);

        r.BaseStream.Seek(DbOffset, SeekOrigin.Begin);

        r.ReadString(true); //Artist

        var artistUnicode = "Unknown Artist";
        if (version >= 20121008)
            artistUnicode = r.ReadString();

        r.ReadString(true); //Title

        var titleUnicode = "Unknown Title";
        if (version >= 20121008)
            titleUnicode = r.ReadString();

        r.ReadString(true); //Creator
        r.ReadString(true); //Difficulty

        var audioFileName = r.ReadString();
        r.ReadString(true); //Hash

        r.ReadString(true); //BeatmapFileName
        r.ReadByte(); //RankedStatus
        r.ReadUInt16(); //CountHitCircles
        r.ReadUInt16(); //CountSliders
        r.ReadUInt16(); //CountSpinners
        r.ReadDateTime(); //LastModifiedTime

        if (version >= 20140609)
        {
            r.ReadSingle(); //ApproachRate
            r.ReadSingle(); //CircleSize
            r.ReadSingle(); //HPDrainRate
            r.ReadSingle(); //OverallDifficulty
        }
        else
        {
            //Float
            r.ReadByte(); //ApproachRate
            r.ReadByte(); //CircleSize
            r.ReadByte(); //HPDrainRate
            r.ReadByte(); //OverallDifficulty
        }

        r.ReadDouble(); //SliderVelocity

        if (version >= 20140609)
        {
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
        }

        r.ReadInt32(); //DrainTimeSeconds
        r.ReadInt32(); //TotalTimeSeconds

        r.ReadInt32(); //AudioPreviewTime
        var timingCount = r.ReadInt32();

        r.BaseStream.Position += 17 * timingCount;

        r.ReadInt32();
        r.ReadInt32(); //beatmapSetId

        r.ReadInt32(); //ThreadId
        r.ReadByte(); //GradeStandard
        r.ReadByte(); //GradeTaiko
        r.ReadByte(); //GradeCtB
        r.ReadByte(); //GradeMania
        r.ReadInt16(); //OffsetLocal
        r.ReadSingle(); //StackLeniency
        r.ReadByte(); //GameMode
        r.ReadString(true); //SongSource
        r.ReadString(true); //SongTags
        r.ReadInt16(); //OffsetOnline
        r.ReadString(true); //TitleFont
        r.ReadBoolean(); //Unplayed
        r.ReadDateTime(); //LastPlayed
        r.ReadBoolean(); //IsOsz2

        var folderName = r.ReadString();

        r.ReadDateTime(); //LastCheckAgainstOsuRepo
        r.ReadBoolean(); //IgnoreBeatmapSounds
        r.ReadBoolean(); //IgnoreBeatmapSkin
        r.ReadBoolean(); //DisableStoryBoard
        r.ReadBoolean(); //DisableVideo
        r.ReadBoolean(); //

        if (version < 20140609)
            r.ReadInt16(); //OldUnknown1

        r.ReadInt32(); //LastEditTime
        r.ReadByte(); //ManiaScrollSpeed

        var fullPath = Path.Combine(osuPath, "Songs", folderName, audioFileName);
        var folderPath = Path.Combine(osuPath, "Songs", folderName);

        return new DbMapEntry
        {
            Artist = Artist,
            ArtistUnicode = artistUnicode,
            Title = Title,
            TitleUnicode = titleUnicode,
            AudioFileName = audioFileName,
            BeatmapSetId = BeatmapSetId,
            DbOffset = DbOffset,
            FolderName = folderName,
            FolderPath = folderPath,
            FullPath = fullPath,
            Hash = Hash,
            TotalTime = TotalTime
        };
    }

    public IDatabaseReader? GetReader(string path)
    {
        var dbLoc = Path.Combine(path, "osu!.db");

        if (!File.Exists(dbLoc)) return null;

        var file = File.OpenRead(dbLoc);

        return new OsuDbReader(file, path);
    }

    public static bool operator ==(DbMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash == right?.Hash;
    }

    public static bool operator !=(DbMapEntryBase? left, IMapEntryBase? right)
    {
        return left?.Hash != right?.Hash;
    }

    public bool Equals(IMapEntryBase? other)
    {
        return Hash == other?.Hash;
    }

    public override bool Equals(object? other)
    {
        if (other is IMapEntryBase map)
            return Hash == map.Hash;

        return false;
    }

    public int CompareTo(IMapEntryBase? other)
    {
        return string.Compare(Hash, other?.Hash, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return BitConverter.ToInt32(Encoding.UTF8.GetBytes(Hash));
    }
}