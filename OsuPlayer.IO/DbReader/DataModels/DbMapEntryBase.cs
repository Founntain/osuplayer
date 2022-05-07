using OsuPlayer.Extensions;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a minimal beatmap entry with only frequently used data
/// <remarks>created on call of <see cref="OsuDbReader.Read(string)" /></remarks>
/// </summary>
public class DbMapEntryBase : IMapEntryBase
{
    public long DbOffset { get; set; }
    public string Artist { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public int BeatmapSetId { get; set; }
    public int TotalTime { get; set; }
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

    /// <summary>
    /// Gets a formatted version of artist and title
    /// <remarks>may be overridden for usage with <see cref="DbMapEntry.UseUnicode" /></remarks>
    /// </summary>
    /// <returns>the formatted song name</returns>
    public virtual string GetSongName()
    {
        return $"{Artist} - {Title}";
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

        var mapEntry = new DbMapEntry();

        mapEntry.DbOffset = DbOffset;
        mapEntry.Artist = string.Intern(r.ReadString());

        if (mapEntry.Artist.Length == 0)
            mapEntry.Artist = "Unknown Artist";

        if (version >= 20121008)
            mapEntry.ArtistUnicode = r.ReadString();

        mapEntry.Title = r.ReadString();

        if (mapEntry.Title.Length == 0)
            mapEntry.Title = "Unknown Title";
        if (version >= 20121008)
            mapEntry.TitleUnicode = r.ReadString();

        r.ReadString(true); //Creator
        r.ReadString(true); //Difficulty

        mapEntry.AudioFileName = r.ReadString();
        mapEntry.Hash = r.ReadString();

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
            r.ReadSingle(); //OveralDifficulty
        }
        else
        {
            //Float
            r.ReadByte(); //ApproachRate
            r.ReadByte(); //CircleSize
            r.ReadByte(); //HPDrainRate
            r.ReadByte(); //OveralDifficulty
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
        mapEntry.TotalTime = r.ReadInt32();

        r.ReadInt32(); //AudioPreviewTime
        var timingCnt = r.ReadInt32();

        r.BaseStream.Position += 17 * timingCnt;

        r.ReadInt32();
        mapEntry.BeatmapSetId = r.ReadInt32();

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

        mapEntry.FolderName = r.ReadString();

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

        mapEntry.FullPath = Path.Combine(osuPath, "Songs", mapEntry.FolderName, mapEntry.AudioFileName);
        mapEntry.FolderPath = Path.Combine(osuPath, "Songs", mapEntry.FolderName);

        return mapEntry;
    }

    public override string ToString()
    {
        return GetSongName();
    }
}