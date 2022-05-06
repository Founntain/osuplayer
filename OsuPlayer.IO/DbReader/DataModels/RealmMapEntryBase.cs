using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using Realms;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a minimal beatmap entry with only frequently used data
/// <remarks>created on call of <see cref="RealmReader.Read(string)" /></remarks>
/// </summary>
public class RealmMapEntryBase : IMapEntryBase
{
    public Guid Id { get; set; }
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Hash { get; set; }
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

    public async Task<IMapEntry?> ReadFullEntry(string path)
    {
        var realmLoc = Path.Combine(path, "client.realm");

        var realmConfig = new RealmConfiguration(realmLoc)
        {
            SchemaVersion = 14,
            IsReadOnly = true
        };

        var realm = await Realm.GetInstanceAsync(realmConfig);
        var beatmap = (BeatmapSetInfo) realm.DynamicApi.Find("BeatmapSet", Id);

        if (beatmap == default) return null;

        var audioFile = beatmap.Files.FirstOrDefault(x => x.Filename == beatmap.Metadata.AudioFile);
        var backgroundFile = beatmap.Files.FirstOrDefault(x => string.Equals(x.Filename, beatmap.Metadata.BackgroundFile, StringComparison.OrdinalIgnoreCase));

        if (audioFile == default) throw new NullReferenceException();

        var audioFolderName = Path.Combine($"{audioFile.File.Hash[0]}", $"{audioFile.File.Hash[0]}{audioFile.File.Hash[1]}");
        var backgroundFolderName = Path.Combine($"{backgroundFile?.File.Hash[0]}", $"{backgroundFile?.File.Hash[0]}{backgroundFile?.File.Hash[1]}");

        var newMap = new RealmMapEntry
        {
            Artist = Artist,
            ArtistUnicode = beatmap.Metadata.ArtistUnicode,
            Title = Title,
            TitleUnicode = beatmap.Metadata.TitleUnicode,
            AudioFileName = beatmap.Metadata.AudioFile,
            BackgroundFileLocation = string.IsNullOrEmpty(backgroundFolderName) ? string.Empty : Path.Combine(path, "files", backgroundFolderName, backgroundFile!.File.Hash),
            Hash = Hash,
            BeatmapSetId = beatmap.OnlineID,
            FolderName = audioFolderName,
            FolderPath = Path.Combine("files", audioFolderName),
            FullPath = Path.Combine(path, "files", audioFolderName, audioFile.File.Hash)
        };

        realm.Dispose();

        return newMap;
    }
}