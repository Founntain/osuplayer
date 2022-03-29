using JetBrains.Annotations;
using Newtonsoft.Json;
using OsuPlayer.IO.Storage.LazerModels.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Files;
using Realms;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

[MapTo("BeatmapSet")]
public class BeatmapSetInfo : RealmObject, IHasRealmFiles, IEquatable<BeatmapSetInfo>, IBeatmapSetInfo
{
    public BeatmapSetInfo(IEnumerable<BeatmapInfo>? beatmaps = null)
        : this()
    {
        ID = Guid.NewGuid();
        if (beatmaps != null)
            Beatmaps.AddRange(beatmaps);
    }

    [UsedImplicitly] // Realm
    private BeatmapSetInfo()
    {
    }

    [PrimaryKey] public Guid ID { get; set; }

    public IList<BeatmapInfo> Beatmaps { get; } = null!;

    [Ignored]
    public BeatmapOnlineStatus Status
    {
        get => (BeatmapOnlineStatus)StatusInt;
        set => StatusInt = (int)value;
    }

    [MapTo(nameof(Status))] public int StatusInt { get; set; } = (int)BeatmapOnlineStatus.None;

    public bool DeletePending { get; set; }

    /// <summary>
    /// Whether deleting this beatmap set should be prohibited (due to it being a system requirement to be present).
    /// </summary>
    public bool Protected { get; set; }

    [Indexed] public int OnlineID { get; set; } = -1;

    public DateTimeOffset DateAdded { get; set; }

    [JsonIgnore] public IBeatmapMetadataInfo Metadata => Beatmaps.FirstOrDefault()?.Metadata ?? new BeatmapMetadata();

    public double MaxStarDifficulty => Beatmaps.Count == 0 ? 0 : Beatmaps.Max(b => b.StarRating);

    public double MaxLength => Beatmaps.Count == 0 ? 0 : Beatmaps.Max(b => b.Length);

    public double MaxBPM => Beatmaps.Count == 0 ? 0 : Beatmaps.Max(b => b.BPM);

    public bool Equals(IBeatmapSetInfo? other)
    {
        return other is BeatmapSetInfo b && Equals(b);
    }

    IEnumerable<IBeatmapInfo> IBeatmapSetInfo.Beatmaps => Beatmaps;

    IEnumerable<INamedFileUsage> IHasNamedFiles.Files => Files;

    public bool Equals(BeatmapSetInfo? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        return ID == other.ID;
    }

    public IList<RealmNamedFileUsage> Files { get; } = null!;

    public string Hash { get; set; } = string.Empty;

    /// <summary>
    /// Returns the storage path for the file in this beatmapset with the given filename, if any exists, otherwise null.
    /// The path returned is relative to the user file storage.
    /// </summary>
    /// <param name="filename">The name of the file to get the storage path of.</param>
    public string? GetPathForFile(string filename)
    {
        return Files.SingleOrDefault(f => string.Equals(f.Filename, filename, StringComparison.OrdinalIgnoreCase))?.File.GetStoragePath();
    }

    public override string ToString()
    {
        return Metadata.GetDisplayString();
    }
}