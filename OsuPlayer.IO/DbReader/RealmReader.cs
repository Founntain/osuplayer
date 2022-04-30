using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using Realms;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A realm reader used for reading the client.realm file from osu!lazer
/// </summary>
public class RealmReader
{
    private readonly Realm _realm;
    private readonly IEnumerable<BeatmapInfo> _beatmapInfos;

    public RealmReader(Config? config = null)
    {
        config ??= new Config();

        if (string.IsNullOrWhiteSpace(config.Container.OsuPath)) return;

        var realmLoc = Path.Combine(config.Container.OsuPath, "client.realm");
        var realmConfig = new RealmConfiguration(realmLoc)
        {
            SchemaVersion = 14,
            IsReadOnly = true
        };

        _realm = Realm.GetInstance(realmConfig).Freeze();
        _beatmapInfos = _realm.DynamicApi.All("Beatmap").ToList().OfType<BeatmapInfo>();
    }

    ~RealmReader()
    {
        _realm.Dispose();
    }

    /// <summary>
    /// Reads the client.realm file from the <paramref name="path"/>
    /// </summary>
    /// <param name="path">the path where the client.realm is located</param>
    /// <returns>an <see cref="IList{T}"/> of <see cref="IMapEntryBase"/> read from the client.realm</returns>
    public static async Task<List<IMapEntryBase>?> Read(string path)
    {
        var realmLoc = Path.Combine(path, "client.realm");

        var realmConfig = new RealmConfiguration(realmLoc)
        {
            SchemaVersion = 14,
            IsReadOnly = true
        };

        var minBeatMaps = new List<IMapEntryBase>();

        var realm = await Realm.GetInstanceAsync(realmConfig);
        var beatmaps = realm.DynamicApi.All("BeatmapSet").ToList().OfType<BeatmapSetInfo>();

        foreach (var beatmap in beatmaps)
        {
            minBeatMaps.Add(new RealmMapEntryBase
            {
                Artist = beatmap.Metadata.Artist,
                BeatmapChecksum = beatmap.Hash,
                BeatmapSetId = beatmap.OnlineID,
                Title = beatmap.Metadata.Title,
                TotalTime = (int) beatmap.MaxLength,
                Id = beatmap.ID
            });
        }

        realm.Dispose();

        return minBeatMaps;
    }

    /// <summary>
    /// Queries all beatmaps in the realm
    /// </summary>
    /// <param name="query">a <see cref="Func{T, TResult}"/> where the query input is a <see cref="BeatmapInfo"/> and the query result is a <see cref="bool"/></param>
    /// <returns>the first <see cref="BeatmapInfo"/> where the <paramref name="query"/> returns true, or null if no map was found</returns>
    public BeatmapInfo? QueryBeatmap(Func<BeatmapInfo, bool> query)
    {
        return _beatmapInfos.FirstOrDefault(query);
    }
}