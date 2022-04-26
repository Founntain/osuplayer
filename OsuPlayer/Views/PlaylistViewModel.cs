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
    public readonly Player Player;
    private ObservableCollection<Playlist> _playlists;

    public PlaylistViewModel(Player player)
    {
        Activator = new ViewModelActivator();

        Player = player;

        Player.PlaylistChanged += (sender, args) =>
        {
            var selection = Player.SelectedPlaylist.Value;

            if (selection == default)
                return;

            Playlists = PlaylistManager.GetAllPlaylists().ToObservableCollection();

            this.RaisePropertyChanged(nameof(Playlists));

            Player.SelectedPlaylist.Value = Playlists.First(x => x.Id == selection!.Id);

            this.RaisePropertyChanged(nameof(SelectedPlaylist));
        };

        this.WhenActivated(disposables =>
        {
            Disposable.Create(() => { }).DisposeWith(disposables);

            Playlists = PlaylistManager.GetAllPlaylists().ToObservableCollection();

            if (Playlists.Count > 0 && SelectedPlaylist == default)
            {
                var playlistStorage = new PlaylistStorage();
                SelectedPlaylist = Playlists.FirstOrDefault(x => x.Id == playlistStorage.Container.LastSelectedPlaylist) ?? Playlists[0];

                PlaylistManager.SetCurrentPlaylistAsync(SelectedPlaylist);
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
        get => Player.SelectedPlaylist.Value;
        set
        {
            PlaylistManager.SetCurrentPlaylistAsync(value);
            Player.SelectedPlaylist.Value = value;
            this.RaisePropertyChanged();
        }
    }
}