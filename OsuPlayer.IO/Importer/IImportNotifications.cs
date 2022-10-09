using OsuPlayer.Extensions.Bindables;

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
    public void OnImportFinished();
}