using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.Modules.IO;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class HomeViewModel : BaseViewModel, IActivatableViewModel
{
    private ObservableCollection<SongEntry> _songs;
    public ViewModelActivator Activator { get; }

    public HomeViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() =>
            {

            }).DisposeWith(disposables);
        });
    }

    public ObservableCollection<SongEntry> Songs
    {
        get => _songs;
        set => this.RaiseAndSetIfChanged(ref _songs, value);
    }
}