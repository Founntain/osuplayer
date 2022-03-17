namespace OsuPlayer.IO.DbReader;

public class DbReader : BinaryReader
{
    private DbReader(Stream input) : base(input)
    {
    }

    public static List<MapEntry>? ReadOsuDb(string osuPath)
    {
        var beatmaps = new List<MapEntry>();
        var dbLoc = $"{osuPath}\\osu!.db";

        if (!File.Exists(dbLoc)) return null;

        using var file = File.OpenRead(dbLoc);
        using var reader = new DbReader(file);
        var ver = reader.ReadInt32();
        var flag = ver >= 20160408 && ver < 20191107;

        reader.ReadInt32();
        reader.ReadBoolean();
        reader.ReadInt64();
        reader.ReadString();

        var beatmapcount = reader.ReadInt32();

        for (var i = 1; i < beatmapcount; i++)
        {
            //var _ = reader.BaseStream.Position; //position

            if (flag)
                reader.ReadInt32(); //btlen

            ReadFromStream(reader, ver, osuPath, out var mapEntry);
            beatmaps.Add(mapEntry);
        }

        reader.ReadInt32(); //account rank

        reader.BaseStream.Dispose();
        reader.Dispose();
        file.Dispose();
        return beatmaps;
    }
    
    private static void ReadFromStream(DbReader r, int version, string osuPath, out MapEntry mapEntry)
    {
        mapEntry = new MapEntry
        {
            Ver = version,
            Artist = r.ReadString()
        };
        if (mapEntry.Artist.Length == 0)
            mapEntry.Artist = "Unkown Artist";
        if (mapEntry.Ver >= 20121008)
            mapEntry.ArtistUnicode = r.ReadString();
        mapEntry.Title = r.ReadString();
        if (mapEntry.Title.Length == 0)
            mapEntry.Title = "Unkown Title";
        if (mapEntry.Ver >= 20121008)
            mapEntry.TitleUnicode = r.ReadString();
        mapEntry.Creator = r.ReadString();
        mapEntry.DifficultyName = r.ReadString(); //Difficulty
        mapEntry.AudioFileName = r.ReadString();
        mapEntry.BeatmapChecksum = r.ReadString();
        r.ReadString(); //BeatmapFileName
        r.ReadByte(); //RankedStatus
        r.ReadUInt16(); //CountHitCircles
        r.ReadUInt16(); //CountSliders
        r.ReadUInt16(); //CountSpinners
        r.ReadDateTime(); //LastModifiedTime
        if (mapEntry.Ver >= 20140609)
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
        if (mapEntry.Ver >= 20140609)
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
        //for (int i = 0; i < timingCnt; i++)
        //{
        r.BaseStream.Position += 17 * timingCnt;
        //r.ReadBytes(17 * timingCnt);
        //r.ReadBytes(17*timingCnt);
        //}
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
        r.ReadString(); //SongSource
        r.ReadString(); //SongTags
        r.ReadInt16(); //OffsetOnline
        r.ReadString(); //TitleFont
        r.ReadBoolean(); //Unplayed
        r.ReadDateTime(); //LastPlayed
        r.ReadBoolean(); //IsOsz2
        mapEntry.FolderName = r.ReadString();
        r.ReadDateTime(); //LastCheckAgainstOsuRepo
        r.ReadBoolean(); //IgnoreBeatmapSounds
        r.ReadBoolean(); //IgnoreBeatmapSkin
        r.ReadBoolean(); //DisableStoryBoard
        r.ReadBoolean(); //DisableVideo
        r.ReadBoolean(); //VisualOverride
        if (mapEntry.Ver < 20140609)
            r.ReadInt16(); //OldUnknown1
        r.ReadInt32(); //LastEditTime
        r.ReadByte(); //ManiaScrollSpeed
        mapEntry.Fullpath = $"{osuPath}\\Songs\\{mapEntry.FolderName}\\{mapEntry.AudioFileName}";
    }

    public static List<Collection>? ReadCollections(string osuPath)
    {
        var collections = new List<Collection>();
        var colLoc = $"{osuPath}\\collection.db";

        if (!File.Exists(colLoc)) return null;

        using (DbReader reader = new(File.OpenRead(colLoc)))
        {
            reader.ReadInt32(); //osuVersion
            var num = reader.ReadInt32();

            for (var i = 0; i < num; i++) collections.Add(Collection.ReadFromReader(reader));
        }

        return collections;
    }

    public override string ReadString()
    {
        switch (ReadByte())
        {
            case 0:
                return string.Empty;
            case 11:
                return base.ReadString();
            default:
                throw new Exception();
        }
    }

    public void ReadStarRating()
    {
        var count = ReadInt32();
        BaseStream.Position += 14 * count;
        //ReadBytes(14 * count);
    }

    public DateTime ReadDateTime()
    {
        return new DateTime(ReadInt64(), DateTimeKind.Utc);
    }
}