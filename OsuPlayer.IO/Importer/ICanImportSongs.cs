using OsuPlayer.Extensions.Bindables;

namespace OsuPlayer.IO.Importer;

public interface ICanImportSongs : ISongSource
{
    public Bindable<bool> SongsLoading { get; }

    public void OnSongImportFinished();
}