using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlaylistEditorViewModel : BaseViewModel
{
    private Playlist? _currentSelectedPlaylist;
    private bool _isDeletePlaylistPopupOpen;
    private bool _isNewPlaylistPopupOpen;
    private bool _isRenamePlaylistPopupOpen;
    private string? _newPlaylistnameText;
    private SourceList<Playlist>? _playlists;
    private List<MinimalMapEntry>? _selectedPlaylistItems;

    private List<MinimalMapEntry>? _selectedSongListItems;

    private Player _player;
    
    public PlaylistEditorViewModel(Player player)
    {
        _player = player;
        
        Activator = new ViewModelActivator();

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            if (Playlists.Count > 0 && CurrentSelectedPlaylist == default)
                CurrentSelectedPlaylist = Playlists.Items.ElementAt(0);
        });

        SelectedPlaylistItems = new();
        SelectedSongListItems = new();
    }

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

    public List<MinimalMapEntry> SongList => _player.SongSource!;

    public List<MinimalMapEntry> SelectedSongListItems
    {
        get => _selectedSongListItems;
        set => this.RaiseAndSetIfChanged(ref _selectedSongListItems, value);
    }

    public List<MinimalMapEntry> SelectedPlaylistItems
    {
        get => _selectedPlaylistItems;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylistItems, value);
    }
}