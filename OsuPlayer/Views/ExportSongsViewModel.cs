using System.Reactive.Disposables;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Importer;
using OsuPlayer.IO.Storage.Playlists;
using ReactiveUI;

namespace OsuPlayer.Views;

public class ExportSongsViewModel : BaseViewModel
{
    private readonly ISongSourceProvider _songSourceProvider;

    private Playlist? _selectedPlaylist;

    public Playlist? SelectedPlaylist
    {
        get => _selectedPlaylist;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPlaylist, value);

            if (value == null)
            {
                SelectedPlaylistSongs = new();

                return;
            }
            
            SelectedPlaylistSongs = _songSourceProvider.GetMapEntriesFromHash(value.Songs, out _).ToObservableCollection();
        }
    }

    private ObservableCollection<Playlist> _playlists;

    public ObservableCollection<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    private ObservableCollection<IMapEntryBase> _selectedPlaylistSongs;

    public ObservableCollection<IMapEntryBase> SelectedPlaylistSongs
    {
        get => _selectedPlaylistSongs;
        set => this.RaiseAndSetIfChanged(ref _selectedPlaylistSongs, value);
    }
    
    public ExportSongsViewModel(ISongSourceProvider songSourceProvider)
    {
        Activator = new ViewModelActivator();
        
        _songSourceProvider = songSourceProvider;

        _selectedPlaylistSongs = new();
        
        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);
        
        using var playlists = new PlaylistStorage();

        Playlists = playlists.Container.Playlists.ToObservableCollection();
    }
}