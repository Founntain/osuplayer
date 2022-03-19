using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Playlists;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlaylistViewModel : BaseViewModel, IActivatableViewModel
{
    private ObservableCollection<Playlist> _playlists;
    private Playlist _selectedPlaylist;

    public ObservableCollection<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public Playlist SelectedPlaylist
    {
        get => _selectedPlaylist;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylist, value);
    }

    public PlaylistViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            Playlists = PlaylistManager.GetAllPlaylists().ToObservableCollection();
            
            if (Playlists.Count > 0 && SelectedPlaylist == default)
            {
                SelectedPlaylist = Playlists[0];
            }
        });
        
        
    }

    public ViewModelActivator Activator { get; }

    public async void OpenPlaylistEditor()
    {
        var ps = await PlaylistManager.GetPlaylistStorageAsync();

        Core.Instance.MainWindow.ViewModel!.PlaylistEditorViewModel.Playlists = ps.Playlists.ToSourceList();
        
        Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.PlaylistEditorViewModel;
    }
}