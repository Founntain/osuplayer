using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Importer;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Modules.Services;
using ReactiveUI;
using SkiaSharp;

namespace OsuPlayer.Views;

public class HomeViewModel : BaseViewModel
{
    private readonly Bindable<bool> _songsLoading = new();

    public readonly IPlayer Player;
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _sortedSongEntries;
    private readonly BindableList<ObservableValue> _graphValues = new();
    private Bitmap? _profilePicture;
    private List<Playlist>? _playlists;
    private List<AddToPlaylistContextMenuEntry> _playlistContextMenuEntries;

    public ReadOnlyObservableCollection<IMapEntryBase>? SortedSongEntries => _sortedSongEntries;
    public IMapEntryBase? SelectedSong { get; set; }
    public ObservableCollection<ISeries> Series { get; set; }

    public Axis[] Axes { get; set; } =
    {
        new()
        {
            IsVisible = false,
            Labels = null
        }
    };

    public bool IsUserNotLoggedIn => CurrentUser == default || CurrentUser?.Id == Guid.Empty;
    public bool IsUserLoggedIn => CurrentUser != default && CurrentUser?.Id != Guid.Empty;

    public bool SongsLoading => new Config().Container.OsuPath != null && _songsLoading.Value;

    public User? CurrentUser => ProfileManager.User;

    public Bitmap? ProfilePicture
    {
        get => _profilePicture;
        set => this.RaiseAndSetIfChanged(ref _profilePicture, value);
    }

    public List<AddToPlaylistContextMenuEntry>? PlaylistContextMenuEntries
    {
        get => _playlistContextMenuEntries;
        set => this.RaiseAndSetIfChanged(ref _playlistContextMenuEntries, value);
    }

    public HomeViewModel(IPlayer player, IStatisticsProvider? statisticsProvider)
    {
        Player = player;
        var statisticsProvider1 = statisticsProvider;

        _songsLoading.BindTo(((IImportNotifications) Player).SongsLoading);
        _songsLoading.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongsLoading)));

        if (statisticsProvider1 != null)
        {
            _graphValues.BindTo(statisticsProvider1.GraphValues);
            _graphValues.BindCollectionChanged((sender, args) =>
            {
                Series!.First().Values = _graphValues;

                this.RaisePropertyChanged(nameof(Series));
            });

            statisticsProvider1.UserDataChanged += (sender, args) => this.RaisePropertyChanged(nameof(CurrentUser));
        }

        player.SongSourceProvider.Songs?.ObserveOn(AvaloniaScheduler.Instance).Bind(out _sortedSongEntries).Subscribe();

        this.RaisePropertyChanged(nameof(SortedSongEntries));

        Activator = new ViewModelActivator();

        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        _playlists = (await PlaylistManager.GetAllPlaylistsAsync())?.ToList();
        PlaylistContextMenuEntries = _playlists?.Select(x => new AddToPlaylistContextMenuEntry(x.Name, AddToPlaylist)).ToList();

        ProfilePicture = await LoadProfilePicture();

        Series = new ObservableCollection<ISeries>
        {
            new LineSeries<ObservableValue>
            {
                Name = "XP gained",
                Values = _graphValues,
                Fill = new LinearGradientPaint(new[]
                {
                    new SKColor(128, 0, 128, 0),
                    new SKColor(128, 0, 128, 25),
                    new SKColor(128, 0, 128, 50),
                    new SKColor(128, 0, 128, 75),
                    new SKColor(128, 0, 128, 100),
                    new SKColor(128, 0, 128, 125),
                    new SKColor(128, 0, 128, 150),
                    new SKColor(128, 0, 128, 175),
                    new SKColor(128, 0, 128, 200),
                    new SKColor(128, 0, 128, 225),
                    SKColors.Purple
                }, new SKPoint(.5f, 1f), new SKPoint(.5f, 0f)),
                Stroke = new SolidColorPaint(SKColors.MediumPurple),
                GeometrySize = 0,
                GeometryFill = new SolidColorPaint(SKColors.MediumPurple),
                GeometryStroke = new SolidColorPaint(SKColors.Purple)
            }
        };
    }

    private async void AddToPlaylist(string name)
    {
        var playlist = _playlists?.FirstOrDefault(x => x.Name == name);

        if (playlist == null || SelectedSong == null) return;

        await PlaylistManager.AddSongToPlaylistAsync(playlist, SelectedSong);

        Player.TriggerPlaylistChanged(new PropertyChangedEventArgs(name));
    }

    internal async Task<Bitmap?> LoadProfilePicture()
    {
        if (CurrentUser == default) return default;

        var profilePicture = await ApiAsync.GetProfilePictureAsync(CurrentUser.Name);

        if (profilePicture == default) return default;

        await using (var stream = new MemoryStream(Convert.FromBase64String(profilePicture)))
        {
            return await Task.Run(() => new Bitmap(stream));
        }
    }
}