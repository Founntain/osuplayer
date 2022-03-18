using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.Data.OsuPlayer.Database.Entities;
using OsuPlayer.IO.Database;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlaylistViewModel : BaseViewModel, IActivatableViewModel
{
    private Playlist _selectedPlaylist;
    private ObservableCollection<Playlist> _playlists;

    public Playlist SelectedPlaylist
    {
        get => _selectedPlaylist;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylist, value);
    }

    public ObservableCollection<Playlist> Playlists
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