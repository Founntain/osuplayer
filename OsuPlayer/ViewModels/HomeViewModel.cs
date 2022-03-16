using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.IO;
using OsuPlayer.IO.DbReader;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class HomeViewModel : BaseViewModel, IActivatableViewModel
{
    private ObservableCollection<MapEntry> _songs;

    public HomeViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public ObservableCollection<MapEntry> Songs
    {
        get => _songs;
        set => this.RaiseAndSetIfChanged(ref _songs, value);
    }

    public ViewModelActivator Activator { get; }
}