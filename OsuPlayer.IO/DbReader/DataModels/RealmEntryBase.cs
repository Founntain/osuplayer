using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using Realms;

namespace OsuPlayer.IO.DbReader.DataModels;

public class RealmEntryBase : IMapEntryBase
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string BeatmapChecksum { get; set; }
    public int TotalTime { get; set; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    public virtual string GetArtist() => Artist;

    public virtual string GetTitle() => Title;

    public virtual string GetSongName() => $"{Artist} - {Title}";

    public Task<IMapEntry> ReadFullEntry(string path)
    {
        var realmLoc = Path.Combine(path, "client.realm");

        var realmConfig = new RealmConfiguration(realmLoc)
        {
            SchemaVersion = 14,
            IsReadOnly = true,
        };

        var realm = Realm.GetInstance(realmConfig);
        var beatmap = realm.DynamicApi.All("BeatmapSet").ToList().OfType<BeatmapSetInfo>().FirstOrDefault(x => x.Hash == BeatmapChecksum);

        return null;
    }
}