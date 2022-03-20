using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.DbReader;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlaylistEditorViewModel : BaseViewModel
{
    private Playlist _currentSelectedPlaylist;
    private ObservableCollection<string> _playlist;
    private SourceList<Playlist> _playlists;
    
    private List<MapEntry> _selectedSongListItems;
    private List<MapEntry> _selectedPlaylistItems;
    private string _newPlaylistnameText;
    private bool _isNewPlaylistPopupOpen;
    private bool _isRenamePlaylistPopupOpen;
    private bool _isDeletePlaylistPopupOpen;

    public bool IsDeletePlaylistPopupOpen
    {
        get => _isDeletePlaylistPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isDeletePlaylistPopupOpen, value);
    }

    public bool IsRenamePlaylistPopupOpen
    {
        get => _isRenamePlaylistPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isRenamePlaylistPopupOpen, value);
    }

    public bool IsNewPlaylistPopupOpen
    {
        get => _isNewPlaylistPopupOpen;
        set => this.RaiseAndSetIfChanged(ref _isNewPlaylistPopupOpen, value);
    }

    public string NewPlaylistnameText
    {
        get => _newPlaylistnameText;
        set => this.RaiseAndSetIfChanged(ref _newPlaylistnameText, value);
    }

    public Playlist CurrentSelectedPlaylist
    {
        get => _currentSelectedPlaylist;
        set
        {
            _currentSelectedPlaylist = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(Playlists));
        }
    }

    public SourceList<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public PlaylistEditorViewModel()
    {
        Activator = new ViewModelActivator();
        
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            if (Playlists.Count > 0 && CurrentSelectedPlaylist == default)
            {
                CurrentSelectedPlaylist = Playlists.Items.ElementAt(0);
            }
        });

        SelectedPlaylistItems = new();
        SelectedSongListItems = new();
    }

    public List<MapEntry> SongList => Core.Instance.Player.SongSource!;

    public List<MapEntry> SelectedSongListItems
    {
        get => _selectedSongListItems;
        set => this.RaiseAndSetIfChanged(ref _selectedSongListItems, value);
    }

    public List<MapEntry> SelectedPlaylistItems
    {
        get => _selectedPlaylistItems;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylistItems, value);
    }
}