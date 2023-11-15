using Nein.Extensions.Bindables;

namespace OsuPlayer.IO.Importer;

/// <summary>
/// This interface provides a way to receive import notifications
/// </summary>
public interface IImportNotifications
{
    public Bindable<bool> SongsLoading { get; }

    /// <summary>
    /// Is called when importing songs started
    /// </summary>
    public void OnImportStarted();

    /// <summary>
    /// Is called when importing songs finished
    /// </summary>
    /// <param name="success">Indicates whether the import was successful or not</param>
    public void OnImportFinished(bool success);
}