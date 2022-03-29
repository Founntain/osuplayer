namespace OsuPlayer.IO.Storage.LazerModels.Files;

public interface IFileInfo
{
    /// <summary>
    /// SHA-256 hash of the file content.
    /// </summary>
    string Hash { get; }
}