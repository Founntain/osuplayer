using osu.Game.Beatmaps;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;
using Realms;
using Realms.Dynamic;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A realm reader used for reading the client.realm file from osu!lazer
/// </summary>
public class RealmReader
{
    private readonly IEnumerable<DynamicRealmObject> _beatmapInfos;
    private readonly Realm _realm;

    public RealmReader(Config? config = null)
    {
        config ??= new Config();

        if (string.IsNullOrWhiteSpace(config.Container.OsuPath)) return;

        var realmLoc = Path.Combine(config.Container.OsuPath, "client.realm");
        var realmConfig = new RealmConfiguration(realmLoc)
        {
            IsDynamic = true,
            IsReadOnly = true
        };

        _realm = Realm.GetInstance(realmConfig).Freeze();
        _beatmapInfos = _realm.DynamicApi.All("Beatmap").ToList().OfType<DynamicRealmObject>().ToList();
    }

    ~RealmReader()
    {
        _realm.Dispose();
    }

    /// <summary>
    /// Reads the client.realm file from the <paramref name="path" />
    /// </summary>
    /// <param name="path">the path where the client.realm is located</param>
    /// <returns>an <see cref="IList{T}" /> of <see cref="IMapEntryBase" /> read from the client.realm</returns>
    public static async Task<List<IMapEntryBase>?> Read(string path)
    {
        var realmLoc = Path.Combine(path, "client.realm");

        var realmConfig = new RealmConfiguration(realmLoc)
        {
            IsDynamic = true,
            IsReadOnly = true
        };

        var minBeatMaps = new List<IMapEntryBase>();

        var realm = await Realm.GetInstanceAsync(realmConfig);

        var objects = realm.DynamicApi.All("BeatmapSet").ToList();

        var beatmaps = objects.OfType<DynamicRealmObject>().ToList();

        foreach (var dynamicBeatmap in beatmaps)
        {
            var infos = dynamicBeatmap.DynamicApi.GetList<DynamicRealmObject>(nameof(BeatmapSetInfo.Beatmaps));
            var firstBeatmap = infos.First().DynamicApi;
            var metadata = firstBeatmap.Get<DynamicRealmObject>(nameof(BeatmapInfo.Metadata)).DynamicApi;
            var artist = metadata.Get<string>(nameof(BeatmapMetadata.Artist));
            var hash = firstBeatmap.Get<string>(nameof(BeatmapInfo.MD5Hash));
            var beatmapSetId = dynamicBeatmap.DynamicApi.Get<int>(nameof(BeatmapInfo.OnlineID));
            var title = metadata.Get<string>(nameof(BeatmapMetadata.Title));

            var totalTime = Enumerable.Max(infos.Select(x => x.DynamicApi.Get<double>(nameof(BeatmapInfo.Length))));
            var id = dynamicBeatmap.DynamicApi.Get<Guid>(nameof(BeatmapSetInfo.ID));

            minBeatMaps.Add(new RealmMapEntryBase
            {
                Artist = artist,
                Hash = hash,
                BeatmapSetId = beatmapSetId,
                Title = title,
                TotalTime = (int) totalTime,
                Id = id
            });
        }

        realm.Dispose();

        return minBeatMaps;
    }

    /// <summary>
    /// Queries all beatmaps in the realm
    /// </summary>
    /// <param name="query">
    /// a <see cref="Func{T, TResult}" /> where the query input is a <see cref="BeatmapInfo" /> and the
    /// query result is a <see cref="bool" />
    /// </param>
    /// <returns>
    /// the first <see cref="BeatmapInfo" /> where the <paramref name="query" /> returns true, or null if no map was
    /// found
    /// </returns>
    public DynamicRealmObject? QueryBeatmap(Func<DynamicRealmObject, bool> query)
    {
        return _beatmapInfos.FirstOrDefault(query);
    }
}