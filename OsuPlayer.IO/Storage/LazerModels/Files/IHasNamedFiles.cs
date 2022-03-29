namespace OsuPlayer.IO.Storage.LazerModels.Files;

public interface IHasNamedFiles
{
    /// <summary>
    /// All files used by this model.
    /// </summary>
    IEnumerable<INamedFileUsage> Files { get; }
}