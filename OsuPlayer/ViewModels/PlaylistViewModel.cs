using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.IO.DbReader;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlaylistViewModel : BaseViewModel, IActivatableViewModel
{
    private ObservableCollection<MapEntry> _playlists;
    
    public ObservableCollection<MapEntry> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public PlaylistViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public ViewModelActivator Activator { get; }

    public void OpenPlaylistEditor()
    {
        Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.PlaylistEditorViewModel;
    }
}