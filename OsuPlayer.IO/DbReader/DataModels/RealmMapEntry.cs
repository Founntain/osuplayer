using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace OsuPlayer.IO.DbReader.DataModels;

public class RealmMapEntry : RealmMapEntryBase, IMapEntry
{
    public string BackgroundFileLocation { get; set; }
    public string ArtistUnicode { get; set; }
    public string TitleUnicode { get; set; }
    public string AudioFileName { get; set; }
    public string FolderName { get; set; }
    public string FolderPath { get; set; }
    public string FullPath { get; set; }
    public bool UseUnicode { get; set; }

    public async Task<Bitmap?> FindBackground()
    {
        if (File.Exists(BackgroundFileLocation))
        {
            await using var stream = File.OpenRead(BackgroundFileLocation);
            return await Task.Run(() => new Bitmap(stream));
        }

        var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();

        return new Bitmap(assets?.Open(new Uri("avares://OsuPlayer/Resources/defaultBg.jpg")));
    }
}