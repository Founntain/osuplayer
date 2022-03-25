namespace OsuPlayer.IO.DbReader;

public partial class DbReader
{
    private static void ReadFromStreamMinimal(DbReader r, int version, string osuPath, ref MinimalMapEntry mapEntry, out int setId)
    {
        mapEntry.Artist = string.Intern(r.ReadString());
        if (mapEntry.Artist.Length == 0)
            mapEntry.Artist = "Unknown Artist";
        if (version >= 20121008)
            r.ReadString(true);
        mapEntry.Title = r.ReadString();
        if (mapEntry.Title.Length == 0)
            mapEntry.Title = "Unknown Title";
        if (version >= 20121008)
            r.ReadString(true);
        r.ReadString(true);
        r.ReadString(true); //Difficulty
        r.ReadString(true);
        mapEntry.BeatmapChecksum = r.ReadString();
        r.ReadString(true); //BeatmapFileName
        if (version >= 20140609)
            r.BaseStream.Seek(39, SeekOrigin.Current);
        else
            r.BaseStream.Seek(27, SeekOrigin.Current);
        
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

        r.BaseStream.Seek(17 * timingCnt, SeekOrigin.Current);
        r.BaseStream.Seek(4, SeekOrigin.Current);
        setId = r.ReadInt32();
        r.BaseStream.Seek(15, SeekOrigin.Current);

        r.ReadString(true); //SongSource
        r.ReadString(true); //SongTags
        r.ReadInt16(); //OffsetOnline
        r.ReadString(true); //TitleFont
        r.ReadBoolean(); //Unplayed
        r.ReadDateTime(); //LastPlayed
        r.ReadBoolean(); //IsOsz2
        r.ReadString(true);
        if (version < 20140609)
            r.BaseStream.Seek(20, SeekOrigin.Current);
        else
            r.BaseStream.Seek(18, SeekOrigin.Current);
    }
    
    public static async Task<MapEntry?> ReadFullMapEntry(string osuPath, long readOffset)
    {
        var dbLoc = Path.Combine(osuPath, "osu!.db");
        if (!File.Exists(dbLoc)) return null;

        await using var file = File.OpenRead(dbLoc);
        using var r = new DbReader(file);
        r.BaseStream.Seek(readOffset, SeekOrigin.Begin);

        var mapEntry = new MapEntry();
        mapEntry.DbOffset = readOffset;
        mapEntry.Artist = string.Intern(r.ReadString());
        if (mapEntry.Artist.Length == 0)
            mapEntry.Artist = "Unknown Artist";
        if (OsuDbVersion >= 20121008)
            mapEntry.ArtistUnicode = r.ReadString();
        mapEntry.Title = r.ReadString();
        if (mapEntry.Title.Length == 0)
            mapEntry.Title = "Unknown Title";
        if (OsuDbVersion >= 20121008)
            mapEntry.TitleUnicode = r.ReadString();
        r.ReadString(true); //Creator
        r.ReadString(true); //Difficulty
        mapEntry.AudioFileName = r.ReadString();
        mapEntry.BeatmapChecksum = r.ReadString();
        r.ReadString(true); //BeatmapFileName
        r.ReadByte(); //RankedStatus
        r.ReadUInt16(); //CountHitCircles
        r.ReadUInt16(); //CountSliders
        r.ReadUInt16(); //CountSpinners
        r.ReadDateTime(); //LastModifiedTime
        if (OsuDbVersion >= 20140609)
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
        if (OsuDbVersion >= 20140609)
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

        mapEntry.BeatmapId = r.ReadInt32();
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
        if (OsuDbVersion < 20140609)
            r.ReadInt16(); //OldUnknown1
        r.ReadInt32(); //LastEditTime
        r.ReadByte(); //ManiaScrollSpeed
        mapEntry.FullPath = Path.Combine(osuPath, "Songs", mapEntry.FolderName, mapEntry.AudioFileName);
        mapEntry.FolderPath = Path.Combine(osuPath, "Songs", mapEntry.FolderName);

        return mapEntry;
    }
}