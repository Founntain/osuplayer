using System.Collections.Generic;
using System.Reactive.Disposables;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel, IActivatableViewModel
{
    public HomeViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public ViewModelActivator Activator { get; }

    public List<MapEntry> SongEntries => Core.Instance.Player.SongSource!;

    private bool _songsLoading;
    public bool SongsLoading
    {
        get => new Config().Read().OsuPath != null && _songsLoading;
        set => this.RaiseAndSetIfChanged(ref _songsLoading, value);
    }
}