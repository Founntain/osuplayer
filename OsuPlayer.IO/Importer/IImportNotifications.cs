using OsuPlayer.Extensions.Bindables;

namespace OsuPlayer.IO.Importer;

public interface IImportNotifications
{
    public Bindable<bool> SongsLoading { get; }

    public void OnImportStarted();
    public void OnImportFinished();
}