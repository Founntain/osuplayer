using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// a full beatmap entry with optionally used data
/// <remarks>only created on a <see cref="IMapEntryBase.ReadFullEntry" /> call</remarks>
/// </summary>
internal class RealmMapEntry : RealmMapEntryBase, IMapEntry
{
    public string BackgroundFileLocation { get; set; }
    public string ArtistUnicode { get; set; }
    public string TitleUnicode { get; set; }
    public string AudioFileName { get; set; }
    public string FolderName { get; set; }
    public string FolderPath { get; set; }
    public string FullPath { get; set; }
    public bool UseUnicode { get; set; }

    public override string GetSongName()
    {
        if (UseUnicode && !string.IsNullOrEmpty(ArtistUnicode) && !string.IsNullOrEmpty(TitleUnicode))
            return $"{ArtistUnicode} - {TitleUnicode}";

        return $"{Artist} - {Title}";
    }

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