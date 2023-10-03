namespace OsuPlayer.IO.DbReader.Interfaces;

public interface IMapEntry : IMapEntryBase
{
    public string ArtistUnicode { get; }
    public string TitleUnicode { get; }
    public string AudioFileName { get; }
    public string FolderName { get; }
    public string FolderPath { get; }
    public string FullPath { get; }
    public bool UseUnicode { get; set; }

    /// <summary>
    /// Gets the background image of this <see cref="IMapEntry" />
    /// </summary>
    /// <returns>the path of the background found. Returns null if the map doesn't have a background</returns>
    public Task<string?> FindBackground();
}