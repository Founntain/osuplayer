namespace OsuPlayer.Modules.IO.DbReader;

public class MapEntry
{
    public string Artist;
    public string ArtistUnicode;
    public string AudioFileName;
    public string BeatmapChecksum;
    public int BeatmapId;
    public int BeatmapSetId;
    public string Creator;
    private string difficultyName;
    public string FolderName;
    public string Title;
    public string TitleUnicode;

    public int TotalTime;

    private int ver;

    public static MapEntry ReadFromReader(DbReader r, int version)
    {
        MapEntry beatmapEntry = new();
        beatmapEntry.ReadFromStream(r, version);
        return beatmapEntry;
    }

    private void ReadFromStream(DbReader r, int version)
    {
        ver = version;
        Artist = r.ReadString();
        if (ver >= 20121008)
            ArtistUnicode = r.ReadString();
        Title = r.ReadString();
        if (ver >= 20121008)
            TitleUnicode = r.ReadString();
        Creator = r.ReadString();
        difficultyName = r.ReadString(); //Difficulty
        AudioFileName = r.ReadString();
        BeatmapChecksum = r.ReadString();
        r.ReadString(); //BeatmapFileName
        r.ReadByte(); //RankedStatus
        r.ReadUInt16(); //CountHitCircles
        r.ReadUInt16(); //CountSliders
        r.ReadUInt16(); //CountSpinners
        r.ReadDateTime(); //LastModifiedTime
        if (ver >= 20140609)
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
        if (ver >= 20140609)
        {
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
            r.ReadStarRating();
        }

        r.ReadInt32(); //DrainTimeSeconds
        TotalTime = r.ReadInt32();
        r.ReadInt32(); //AudioPreviewTime
        var timingCnt = r.ReadInt32();
        //for (int i = 0; i < timingCnt; i++)
        //{
        r.ReadBytes(17 * timingCnt);
        //r.ReadBytes(17*timingCnt);
        //}
        BeatmapId = r.ReadInt32();
        BeatmapSetId = r.ReadInt32();
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
        FolderName = r.ReadString();
        r.ReadDateTime(); //LastCheckAgainstOsuRepo
        r.ReadBoolean(); //IgnoreBeatmapSounds
        r.ReadBoolean(); //IgnoreBeatmapSkin
        r.ReadBoolean(); //DisableStoryBoard
        r.ReadBoolean(); //DisableVideo
        r.ReadBoolean(); //VisualOverride
        if (ver < 20140609)
            r.ReadInt16(); //OldUnknown1
        r.ReadInt32(); //LastEditTime
        r.ReadByte(); //ManiaScrollSpeed
    }

    public override string ToString()
    {
        return Artist + " - " + Title;
    }
}