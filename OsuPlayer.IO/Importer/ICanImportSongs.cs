using DynamicData;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

public interface ICanImportSongs : ISongSource
{
    public Bindable<bool> SongsLoading { get; }

    public void OnSongImportFinished();
}