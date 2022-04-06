using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlaylistViewModel : BaseViewModel
{
    private ObservableCollection<Playlist> _playlists;
    private Playlist _selectedPlaylist;
    private readonly Player _player;


    public PlaylistViewModel(Player player)
    {
        Activator = new ViewModelActivator();

        _player = player;

        _player.PlaylistChanged += (sender, args) =>
        {
            var selection = _player.SelectedPlaylist.Value;
            Playlists = PlaylistManager.GetAllPlaylists().ToObservableCollection();
            this.RaisePropertyChanged(nameof(Playlists));
            _player.SelectedPlaylist.Value = Playlists.First(x => x.Id == selection!.Id);
            this.RaisePropertyChanged(nameof(SelectedPlaylist));
        };

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            Playlists = PlaylistManager.GetAllPlaylists().ToObservableCollection();

            if (Playlists.Count > 0 && SelectedPlaylist == default)
            {
                SelectedPlaylist = Playlists[0];

                PlaylistManager.SetCurrentPlaylist(SelectedPlaylist);
            }
        });
    }

    public ObservableCollection<Playlist> Playlists
    {
        get => _playlists;
        set => this.RaiseAndSetIfChanged(ref _playlists, value);
    }

    public Playlist? SelectedPlaylist
    {
        get => _player.SelectedPlaylist.Value;
        set
        {
            PlaylistManager.SetCurrentPlaylist(value);
            _player.SelectedPlaylist.Value = value;
            this.RaisePropertyChanged();
        }
    }
}