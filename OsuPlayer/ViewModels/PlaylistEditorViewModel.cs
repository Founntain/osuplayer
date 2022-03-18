using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using OsuPlayer.IO.DbReader;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class PlaylistEditorViewModel : BaseViewModel, IActivatableViewModel
{
    private ObservableCollection<MapEntry> _playlist;
    private List<MapEntry> _selectedSonglistItems;
    private List<MapEntry> _selectedPlaylistItems;

    public PlaylistEditorViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });

        SelectedPlaylistItems = new();
        SelectedSonglistItems = new();
        
        Playlist = new();
    }

    public ViewModelActivator Activator { get; }

    public List<MapEntry> Songlist => Core.Instance.Player.SongSource!;

    public ObservableCollection<MapEntry> Playlist
    {
        get => _playlist;
        set => this.RaiseAndSetIfChanged(ref _playlist, value);
    }

    public List<MapEntry> SelectedSonglistItems
    {
        get => _selectedSonglistItems;
        set => this.RaiseAndSetIfChanged(ref _selectedSonglistItems, value);
    }

    public List<MapEntry> SelectedPlaylistItems
    {
        get => _selectedPlaylistItems;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylistItems, value);
    }
}