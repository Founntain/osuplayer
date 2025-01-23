using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.LazerModels.Beatmaps;
using OsuPlayer.Data.LazerModels.Collections;
using OsuPlayer.Data.LazerModels.Files;
using OsuPlayer.IO.Storage.Config;
using Realms;
using Realms.Dynamic;
using Splat;

namespace OsuPlayer.IO.DbReader;

/// <summary>
/// A realm reader used for reading the client.realm file from osu!lazer
/// </summary>
public class RealmReader : IDatabaseReader
{
    private readonly string _path;
    private readonly Realm _realm;
    private readonly IDbReaderFactory _readerFactory;

    public RealmReader(string path, IDbReaderFactory readerFactory)
    {
        _readerFactory = readerFactory;

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
            var id = dynamicRealmObject.DynamicApi.Get<IRealmObjectBase>(nameof(BeatmapInfo.BeatmapSet)).DynamicApi.Get<int>(nameof(BeatmapSetInfo.OnlineID));

            hashes.Add(hash, id);
        }

        return hashes;
    }

    public Task<List<OsuCollection>> GetCollections(string path)
    {
        if (File.Exists(Path.Combine(path, "collection.db"))) return OsuCollectionReader.Read(path);

        var dynamicRealmObjects = _realm.DynamicApi.All(nameof(BeatmapCollection)).ToList().OfType<DynamicRealmObject>().ToList();

        var collections = new List<OsuCollection>();

        foreach (var realmObject in dynamicRealmObjects)
        {
            var name = realmObject.DynamicApi.Get<string>(nameof(BeatmapCollection.Name));
            var hashes = realmObject.DynamicApi.GetList<string>(nameof(BeatmapCollection.BeatmapMD5Hashes));

            var col = new OsuCollection(name, hashes.ToList());

            collections.Add(col);
        }

        return Task.FromResult(collections);
    }

    public IMapEntry? ReadFullEntry(string path, IMapEntryBase mapEntryBase, long? dbOffset = null, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(path) || id == null)
            return null;

        var beatmap = (DynamicRealmObject) _realm.DynamicApi.Find("BeatmapSet", id);

        if (beatmap == default)
            return null;

        var beatmaps = beatmap.DynamicApi.GetList<DynamicRealmObject>(nameof(BeatmapSetInfo.Beatmaps));
        var metadata = beatmaps.First().DynamicApi.Get<DynamicRealmObject>(nameof(BeatmapInfo.Metadata)).DynamicApi;

        var files = (RealmList<DynamicEmbeddedObject>) beatmap.DynamicApi.GetList<DynamicEmbeddedObject>(nameof(BeatmapSetInfo.Files));

        var audioFileName = metadata.Get<string>(nameof(BeatmapMetadata.AudioFile));
        var backgroundFileName = metadata.Get<string>(nameof(BeatmapMetadata.BackgroundFile));

        var audioFile = (IRealmObjectBase) files.FirstOrDefault(x =>
            string.Equals(x.DynamicApi.Get<string>(nameof(RealmNamedFileUsage.Filename)), audioFileName, StringComparison.CurrentCultureIgnoreCase));
        var backgroundFile = (IRealmObjectBase) files.FirstOrDefault(x =>
            string.Equals(x.DynamicApi.Get<string>(nameof(RealmNamedFileUsage.Filename)), backgroundFileName, StringComparison.CurrentCultureIgnoreCase));

        if (audioFile == null)
            return null;

        var audioHash = audioFile.DynamicApi.Get<IRealmObjectBase>(nameof(RealmNamedFileUsage.File)).DynamicApi.Get<string>(nameof(RealmFile.Hash));
        var backgroundHash = backgroundFile?.DynamicApi.Get<IRealmObjectBase>(nameof(RealmNamedFileUsage.File)).DynamicApi.Get<string>(nameof(RealmFile.Hash));

        var audioFolderName = Path.Combine($"{audioHash[0]}", $"{audioHash[0]}{audioHash[1]}");
        var backgroundFolderName = Path.Combine($"{backgroundHash?[0]}", $"{backgroundHash?[0]}{backgroundHash?[1]}");

        using var config = new Config();

        var newMap = new RealmMapEntry
        {
            DbReaderFactory = _readerFactory,
            Id = id.Value,
            OsuPath = string.Intern(path),
            Artist = string.Intern(mapEntryBase.Artist),
            ArtistUnicode = string.Intern(mapEntryBase.ArtistUnicode),
            Title = string.Intern(mapEntryBase.Title),
            TitleUnicode = string.Intern(mapEntryBase.TitleUnicode),
            AudioFileName = audioFileName,
            BackgroundFileLocation = string.IsNullOrEmpty(backgroundFolderName)
                ? string.Empty
                : Path.Combine(path, "files", backgroundFolderName, backgroundHash!),
            Hash = mapEntryBase.Hash,
            BeatmapSetId = mapEntryBase.BeatmapSetId,
            FolderName = audioFolderName,
            FolderPath = Path.Combine("files", audioFolderName),
            FullPath = Path.Combine(path, "files", audioFolderName, audioHash),
            UseUnicode = config.Container.UseSongNameUnicode
        };

        _realm.Dispose();

        return newMap;
    }

    public Task<List<IMapEntryBase>?> ReadBeatmaps()
    {
        using var config = new Config();

        var minBeatMaps = new List<IMapEntryBase>();

        var beatmaps = _realm.DynamicApi.All("BeatmapSet").ToList().OfType<DynamicRealmObject>().ToList();

        foreach (var dynamicBeatmap in beatmaps)
        {
            var infos = dynamicBeatmap.DynamicApi.GetList<DynamicRealmObject>(nameof(BeatmapSetInfo.Beatmaps));
            var firstBeatmap = infos.First().DynamicApi;
            var metadata = firstBeatmap.Get<DynamicRealmObject>(nameof(BeatmapInfo.Metadata)).DynamicApi;
            var artist = metadata.Get<string>(nameof(BeatmapMetadata.Artist));
            var artistUnicode = metadata.Get<string>(nameof(BeatmapMetadata.ArtistUnicode)) ?? string.Empty;
            var hash = firstBeatmap.Get<string>(nameof(BeatmapInfo.MD5Hash));
            var beatmapSetId = dynamicBeatmap.DynamicApi.Get<int>(nameof(BeatmapSetInfo.OnlineID));
            var title = metadata.Get<string>(nameof(BeatmapMetadata.Title));
            var titleUnicode = metadata.Get<string>(nameof(BeatmapMetadata.TitleUnicode)) ?? string.Empty;

            var totalTime = infos.Select(x => x.DynamicApi.Get<double>(nameof(BeatmapInfo.Length))).Max();
            var id = dynamicBeatmap.DynamicApi.Get<Guid>(nameof(BeatmapSetInfo.ID));

            minBeatMaps.Add(new RealmMapEntryBase
            {
                DbReaderFactory = _readerFactory,
                OsuPath = string.Intern(_path),
                Artist = string.Intern(artist),
                ArtistUnicode = string.Intern(artistUnicode),
                Hash = hash,
                BeatmapSetId = beatmapSetId,
                Title = title,
                TitleUnicode = string.Intern(titleUnicode),
                TotalTime = (int) totalTime,
                Id = id,
                UseUnicode = config.Container.UseSongNameUnicode
            });
        }

        return Task.FromResult(minBeatMaps);
    }

    public void Dispose()
    {
        _realm.Dispose();
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
        using var reader = new RealmReader(path, Locator.Current.GetService<IDbReaderFactory>());

        return await reader.ReadBeatmaps();
    }
}