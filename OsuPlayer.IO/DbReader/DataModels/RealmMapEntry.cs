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
    public string BackgroundFileLocation { get; set; } = string.Empty;
    public string ArtistUnicode { get; set; } = string.Empty;
    public string TitleUnicode { get; set; } = string.Empty;
    public string AudioFileName { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;
    public string FullPath { get; set; } = string.Empty;
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