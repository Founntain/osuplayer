using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using OsuPlayer.IO.Storage.LazerModels.Beatmaps;
using Realms;

namespace OsuPlayer.IO.DbReader.DataModels;

public class RealmMapEntry : RealmMapEntryBase, IMapEntry
{
    public string ArtistUnicode { get; set; }
    public string TitleUnicode { get; set; }
    public string AudioFileName { get; set; }
    public int BeatmapSetId { get; set; }
    public string FolderName { get; set; }
    public string FolderPath { get; set; }
    public string FullPath { get; set; }
    public bool UseUnicode { get; set; }

    public async Task<Bitmap?> FindBackground(string path)
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

        var backgroundFile = beatmap.Files.FirstOrDefault(x => x.Filename == beatmap.Metadata.BackgroundFile);
        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        
        if (backgroundFile == default)
        {
            return new Bitmap(assets?.Open(new Uri("avares://OsuPlayer/Resources/defaultBg.jpg")));
        }

        var folderName = Path.Combine("files", $"{backgroundFile.File.Hash[0]}", $"{backgroundFile.File.Hash[0]}{backgroundFile.File.Hash[1]}");

        if (File.Exists(Path.Combine(path, folderName, backgroundFile.File.Hash)))
        {
            await using var stream = File.OpenRead(Path.Combine(path, folderName, backgroundFile.File.Hash));
            return await Task.Run(() => new Bitmap(stream));
        }

        return new Bitmap(assets?.Open(new Uri("avares://OsuPlayer/Resources/defaultBg.jpg")));
    }
}