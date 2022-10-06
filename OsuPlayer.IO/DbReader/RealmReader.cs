using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using OsuPlayer.IO.Storage.LazerModels.Collections;
using Realms;
using Realms.Dynamic;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A realm reader used for reading the client.realm file from osu!lazer
/// </summary>
public class RealmReader : IDatabaseReader
{
    private readonly string _path;
    private readonly Realm _realm;

    public RealmReader(string path)
    {
        _path = string.Intern(path);

        var realmLoc = Path.Combine(_path, "client.realm");
        var realmConfig = new RealmConfiguration(realmLoc)
        {
            IsDynamic = true,
            IsReadOnly = true
        };

        _realm = Realm.GetInstance(realmConfig);
    }

    public Dictionary<string, int> GetBeatmapHashes()
    {
        var hashes = new Dictionary<string, int>();

        foreach (var dynamicRealmObject in _realm.DynamicApi.All("Beatmap").ToList().OfType<DynamicRealmObject>().ToList())
        {
            var hash = dynamicRealmObject.DynamicApi.Get<string>(nameof(BeatmapInfo.MD5Hash));
            var id = dynamicRealmObject.DynamicApi.Get<RealmObjectBase>(nameof(BeatmapInfo.BeatmapSet)).DynamicApi.Get<int>(nameof(BeatmapSetInfo.OnlineID));

            hashes.Add(hash, id);
        }

        return hashes;
    }

    public Task<List<Collection>> GetCollections(string path)
    {
        if (File.Exists(Path.Combine(path, "collection.db"))) return OsuCollectionReader.Read(path);

        var dynamicRealmObjects = _realm.DynamicApi.All(nameof(BeatmapCollection)).ToList().OfType<DynamicRealmObject>().ToList();

        var collections = new List<Collection>();

        foreach (var realmObject in dynamicRealmObjects)
        {
            var name = realmObject.DynamicApi.Get<string>(nameof(BeatmapCollection.Name));
            var hashes = realmObject.DynamicApi.GetList<string>(nameof(BeatmapCollection.BeatmapMD5Hashes));

            var col = new Collection(name, hashes.ToList());

            collections.Add(col);
        }

        return Task.FromResult(collections);
    }

    public Task<List<IMapEntryBase>?> ReadBeatmaps()
    {
        var minBeatMaps = new List<IMapEntryBase>();

        var beatmaps = _realm.DynamicApi.All("BeatmapSet").ToList().OfType<DynamicRealmObject>().ToList();

        foreach (var dynamicBeatmap in beatmaps)
        {
            var infos = dynamicBeatmap.DynamicApi.GetList<DynamicRealmObject>(nameof(BeatmapSetInfo.Beatmaps));
            var firstBeatmap = infos.First().DynamicApi;
            var metadata = firstBeatmap.Get<DynamicRealmObject>(nameof(BeatmapInfo.Metadata)).DynamicApi;
            var artist = metadata.Get<string>(nameof(BeatmapMetadata.Artist));
            var hash = firstBeatmap.Get<string>(nameof(BeatmapInfo.MD5Hash));
            var beatmapSetId = dynamicBeatmap.DynamicApi.Get<int>(nameof(BeatmapSetInfo.OnlineID));
            var title = metadata.Get<string>(nameof(BeatmapMetadata.Title));

            var totalTime = infos.Select(x => x.DynamicApi.Get<double>(nameof(BeatmapInfo.Length))).Max();
            var id = dynamicBeatmap.DynamicApi.Get<Guid>(nameof(BeatmapSetInfo.ID));

            minBeatMaps.Add(new RealmMapEntryBase
            {
                OsuPath = string.Intern(_path),
                Artist = string.Intern(artist),
                Hash = hash,
                BeatmapSetId = beatmapSetId,
                Title = title,
                TotalTime = (int) totalTime,
                Id = id
            });
        }

        return Task.FromResult(minBeatMaps);
    }

    ~RealmReader()
    {
        _realm.Dispose();
    }

    public void Dispose()
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
        using var reader = new RealmReader(path);

        return await reader.ReadBeatmaps();
    }
}