using JetBrains.Annotations;
using Newtonsoft.Json;
using OsuPlayer.IO.Storage.LazerModels.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Files;
using Realms;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

[Serializable]
[MapTo("Beatmap")]
public class BeatmapInfo : RealmObject, IHasGuidPrimaryKey, IBeatmapInfo, IEquatable<BeatmapInfo>
{
    public BeatmapInfo(RulesetInfo? ruleset = null, BeatmapDifficulty? difficulty = null, BeatmapMetadata? metadata = null)
    {
        ID = Guid.NewGuid();
        Ruleset = ruleset ?? new RulesetInfo
        {
            OnlineID = 0,
            ShortName = @"osu",
            Name = @"null placeholder ruleset"
        };
        Difficulty = difficulty ?? new BeatmapDifficulty();
        Metadata = metadata ?? new BeatmapMetadata();
        //UserSettings = new BeatmapUserSettings();
    }

    [UsedImplicitly]
    private BeatmapInfo()
    {
    }

    public RulesetInfo Ruleset { get; set; } = null!;

    public BeatmapDifficulty Difficulty { get; set; } = null!;

    public BeatmapMetadata Metadata { get; set; } = null!;

    public BeatmapSetInfo? BeatmapSet { get; set; }

    [Ignored] public RealmNamedFileUsage? File => BeatmapSet?.Files.FirstOrDefault(f => f.File.Hash == Hash);


    [Ignored]
    public BeatmapOnlineStatus Status
    {
        get => (BeatmapOnlineStatus)StatusInt;
        set => StatusInt = (int)value;
    }

    [MapTo(nameof(Status))] public int StatusInt { get; set; } = (int)BeatmapOnlineStatus.None;

    [JsonIgnore] public bool Hidden { get; set; }

    public string DifficultyName { get; set; } = string.Empty;

    [Indexed] public int OnlineID { get; set; } = -1;

    public double Length { get; set; }

    public double BPM { get; set; }

    public string Hash { get; set; } = string.Empty;

    public double StarRating { get; set; }

    public string MD5Hash { get; set; } = string.Empty;

    public bool Equals(IBeatmapInfo? other)
    {
        return other is BeatmapInfo b && Equals(b);
    }

    IBeatmapMetadataInfo IBeatmapInfo.Metadata => Metadata;
    IBeatmapSetInfo? IBeatmapInfo.BeatmapSet => BeatmapSet;
    IRulesetInfo IBeatmapInfo.Ruleset => Ruleset;
    IBeatmapDifficultyInfo IBeatmapInfo.Difficulty => Difficulty;

    public bool Equals(BeatmapInfo? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        return ID == other.ID;
    }

    [PrimaryKey] public Guid ID { get; set; }

    public override string ToString()
    {
        return this.GetDisplayTitle();
    }

    #region Properties we may not want persisted (but also maybe no harm?)

    public double AudioLeadIn { get; set; }

    public float StackLeniency { get; set; } = 0.7f;

    public bool SpecialStyle { get; set; }

    public bool LetterboxInBreaks { get; set; }

    public bool WidescreenStoryboard { get; set; } = true;

    public bool EpilepsyWarning { get; set; }

    public bool SamplesMatchPlaybackRate { get; set; } = true;

    public double DistanceSpacing { get; set; }

    public int BeatDivisor { get; set; }

    public int GridSize { get; set; }

    public double TimelineZoom { get; set; } = 1.0;

    /// <summary>
    /// The number of beats to move the countdown backwards (compared to its default location).
    /// </summary>
    public int CountdownOffset { get; set; }

    #endregion
}