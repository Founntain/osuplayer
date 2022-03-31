using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using Realms;

namespace OsuPlayer.IO.DbReader.DataModels;

public class RealmMapEntryBase : IMapEntryBase
{
    public string Artist { get; set; }
    public string Title { get; set; }
    public string BeatmapChecksum { get; set; }
    public int BeatmapSetId { get; set; }
    public int TotalTime { get; set; }
    public string TotalTimeString => TimeSpan.FromMilliseconds(TotalTime).FormatTime();
    public string SongName => GetSongName();
    public string ArtistString => GetArtist();
    public string TitleString => GetTitle();

    public virtual string GetArtist()
    {
        return Artist;
    }

    public virtual string GetTitle()
    {
        return Title;
    }

    public virtual string GetSongName()
    {
        return $"{Artist} - {Title}";
    }

    public async Task<IMapEntry> ReadFullEntry(string path)
    {
        var realmLoc = Path.Combine(path, "client.realm");

        var realmConfig = new RealmConfiguration(realmLoc)
        {
            SchemaVersion = 14,
            IsReadOnly = true
        };

        var realm = await Realm.GetInstanceAsync(realmConfig);
        var beatmap = realm.DynamicApi.All("BeatmapSet").ToList().OfType<BeatmapSetInfo>().FirstOrDefault(x => x.Hash == BeatmapChecksum);

        if (beatmap == default) throw new NullReferenceException();

        var audioFile = beatmap.Files.FirstOrDefault(x => x.Filename == beatmap.Metadata.AudioFile);

        if (audioFile == default) throw new NullReferenceException();

        var folderName = Path.Combine($"{audioFile.File.Hash[0]}", $"{audioFile.File.Hash[0]}{audioFile.File.Hash[1]}");

        var newMap = new RealmMapEntry
        {
            Artist = Artist,
            ArtistUnicode = beatmap.Metadata.ArtistUnicode,
            Title = Title,
            TitleUnicode = beatmap.Metadata.TitleUnicode,
            AudioFileName = beatmap.Metadata.AudioFile,
            BeatmapChecksum = BeatmapChecksum,
            BeatmapSetId = beatmap.OnlineID,
            FolderName = folderName,
            FolderPath = Path.Combine("files", folderName),
            FullPath = Path.Combine(path, "files", folderName, audioFile.File.Hash)
        };

        realm.Dispose();

        return newMap;
    }
}