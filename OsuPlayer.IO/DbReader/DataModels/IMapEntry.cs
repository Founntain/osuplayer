using Avalonia.Media.Imaging;

namespace OsuPlayer.IO.DbReader.DataModels;

public interface IMapEntry : IMapEntryBase
{
    public string ArtistUnicode { get; set; }
    public string TitleUnicode { get; set; }
    public string AudioFileName { get; set; }
    public string FolderName { get; set; }
    public string FolderPath { get; set; }
    public string FullPath { get; set; }
    public bool UseUnicode { get; set; }

    /// <summary>
    /// Gets the background image of this <see cref="IMapEntry" />
    /// </summary>
    /// <returns>a <see cref="Bitmap" /> of the background found. Returns null if the map doesn't have a background</returns>
    public Task<Bitmap?> FindBackground();
}