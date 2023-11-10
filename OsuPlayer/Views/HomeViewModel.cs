using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using DynamicData;
using Nein.Base;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.IO.Importer;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Views.HomeSubViews;
using ReactiveUI;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel
{
    private readonly Bindable<bool> _songsLoading = new();
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _sortedSongEntries;
    private readonly IProfileManagerService _profileManager;

    public readonly IPlayer Player;

    private List<AddToPlaylistContextMenuEntry>? _playlistContextMenuEntries;
    private List<Playlist>? _playlists;
    private Bitmap? _profilePicture;
    private IMapEntryBase? _selectedSong;

    public ReadOnlyObservableCollection<IMapEntryBase>? SortedSongEntries => _sortedSongEntries;

    public HomeUserPanelViewModel HomeUserPanelView { get; }

    public IMapEntryBase? SelectedSong
    {
        get => _selectedSong;
        set => this.RaiseAndSetIfChanged(ref _selectedSong, value);
    }

    public bool IsUserNotLoggedIn => CurrentUser == default || CurrentUser?.UniqueId == Guid.Empty;
    public bool IsUserLoggedIn => CurrentUser != default && CurrentUser?.UniqueId != Guid.Empty;

    public bool SongsLoading => new Config().Container.OsuPath != null && _songsLoading.Value;

    public User? CurrentUser => _profileManager.User;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
    }

    private bool _displayUserStats;

    public bool DisplayUserStats
    {
        get => _displayUserStats;
        set => this.RaiseAndSetIfChanged(ref _displayUserStats, value);
    }

    public List<AddToPlaylistContextMenuEntry>? PlaylistContextMenuEntries
    {
        get => _playlistContextMenuEntries;
        set => this.RaiseAndSetIfChanged(ref _playlistContextMenuEntries, value);
    }

    public HomeViewModel(IPlayer player, IStatisticsProvider? statisticsProvider, IProfileManagerService profileManager)
    {
        _profileManager = profileManager;

        HomeUserPanelView = new HomeUserPanelViewModel(statisticsProvider, _profileManager);

        Player = player;

        _songsLoading.BindTo(((IImportNotifications) Player).SongsLoading);
        _songsLoading.BindValueChanged(_ => this.RaisePropertyChanged(nameof(SongsLoading)));

        player.SongSourceProvider.Songs?.ObserveOn(AvaloniaScheduler.Instance).Bind(out _sortedSongEntries).Subscribe();

        this.RaisePropertyChanged(nameof(SortedSongEntries));

        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { SelectedSong = null; }).DisposeWith(disposables);

        _playlists = (await PlaylistManager.GetAllPlaylistsAsync())?.ToList();
        PlaylistContextMenuEntries = _playlists?.Select(x => new AddToPlaylistContextMenuEntry(x.Name, AddToPlaylist)).ToList();

        await using var config = new Config();

        DisplayUserStats = config.Container.DisplayerUserStats;
    }

    private async void AddToPlaylist(string name)
    {
        var playlist = _playlists?.FirstOrDefault(x => x.Name == name);

        if (playlist == null || SelectedSong == null) return;

        await PlaylistManager.AddSongToPlaylistAsync(playlist, SelectedSong);

        Player.TriggerPlaylistChanged(new PropertyChangedEventArgs(name));
    }
}