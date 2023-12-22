using System.Runtime.InteropServices;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Modules;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Views;
using OsuPlayer.Views.CustomControls;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public class FluentAppWindowViewModel : BaseWindowViewModel
{
    public readonly IPlayer Player;
    public readonly IProfileManagerService ProfileManager;

    private BaseViewModel? _mainView;
    private bool _displayBackgroundImage;
    private Bitmap? _backgroundImage;
    private float _backgroundBlurRadius;

    public PlayerControlViewModel PlayerControl { get; }

    public EditUserViewModel EditUserView { get; }
    public HomeViewModel HomeView { get; }
    public PartyViewModel PartyView { get; }
    public BlacklistEditorViewModel BlacklistEditorView { get; }
    public PlaylistEditorViewModel PlaylistEditorView { get; }
    public PlaylistViewModel PlaylistView { get; }
    public SearchViewModel SearchView { get; }
    public SettingsViewModel SettingsView { get; }
    public UserViewModel UserView { get; }
    public UpdateViewModel UpdateView { get; }
    public EqualizerViewModel EqualizerView { get; }
    public StatisticsViewModel StatisticsView { get; }
    public BeatmapsViewModel BeatmapView { get; }
    public ExportSongsViewModel ExportSongsView { get; }
    public PlayHistoryViewModel PlayHistoryView { get; }

    public AudioVisualizerViewModel AudioVisualizer { get; }

    public ExperimentalAcrylicMaterial? PanelMaterial { get; set; }

    public bool IsNonLinuxOs { get; }
    public bool IsLinuxOs { get; }

    public bool IsUserLoggedIn => ProfileManager.User != default && ProfileManager.User?.UniqueId != Guid.Empty;
    public bool IsUserNotLoggedIn => ProfileManager.User == default || ProfileManager.User?.UniqueId == Guid.Empty;

    private ReadOnlyObservableCollection<IMapEntryBase>? _songList;

    public ReadOnlyObservableCollection<IMapEntryBase>? SongList
    {
        get => _songList;
        set => this.RaiseAndSetIfChanged(ref _songList, value);
    }

    public bool DisplayBackgroundImage
    {
        get => _displayBackgroundImage;
        set => this.RaiseAndSetIfChanged(ref _displayBackgroundImage, value);
    }

    public float BackgroundBlurRadius
    {
        get => _backgroundBlurRadius;
        set => this.RaiseAndSetIfChanged(ref _backgroundBlurRadius, value);
    }

    public BaseViewModel? MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }

    public Bitmap? BackgroundImage
    {
        get => _backgroundImage;
        set => this.RaiseAndSetIfChanged(ref _backgroundImage, value);
    }

    public FluentAppWindowViewModel(IAudioEngine engine, IPlayer player, IProfileManagerService profileManager, IShuffleServiceProvider? shuffleServiceProvider = null,
        IStatisticsProvider? statisticsProvider = null, ISortProvider? sortProvider = null, IHistoryProvider? historyProvider = null)
    {
        Player = player;
        ProfileManager = profileManager;

        AudioVisualizer = new AudioVisualizerViewModel(Locator.Current.GetRequiredService<IAudioEngine>());

        PlayerControl = new PlayerControlViewModel(Player, engine, this);

        SearchView = new SearchViewModel(Player);
        PlaylistView = new PlaylistViewModel(Player);
        PlaylistEditorView = new PlaylistEditorViewModel(Player);
        BlacklistEditorView = new BlacklistEditorViewModel(Player);
        HomeView = new HomeViewModel(Player, statisticsProvider, profileManager);
        UserView = new UserViewModel(Player, profileManager);
        EditUserView = new EditUserViewModel(profileManager);
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel(Player, sortProvider, shuffleServiceProvider, profileManager);
        EqualizerView = new EqualizerViewModel(Player);
        UpdateView = new UpdateViewModel();
        StatisticsView = new StatisticsViewModel();
        BeatmapView = new BeatmapsViewModel(Player);
        ExportSongsView = new ExportSongsViewModel(Player.SongSourceProvider);
        PlayHistoryView = new PlayHistoryViewModel(Player, historyProvider, Player.SongSourceProvider);


        IsLinuxOs = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        IsNonLinuxOs = !IsLinuxOs;

        PanelMaterial = new ExperimentalAcrylicMaterial
        {
            BackgroundSource = AcrylicBackgroundSource.Digger,
            TintColor = Colors.Black,
            TintOpacity = 0.75,
            MaterialOpacity = 0.25
        };

        SongList = Player.SongSourceProvider.SongSourceList;

        Player.CurrentSongImage.BindValueChanged(d =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                BackgroundImage?.Dispose();

                if (!DisplayBackgroundImage)
                {
                    BackgroundImage = null;

                    return;
                }

                if (!string.IsNullOrEmpty(d.NewValue) && File.Exists(d.NewValue))
                {
                    BackgroundImage = BitmapExtensions.BlurBitmap(d.NewValue, BackgroundBlurRadius, 0.75f, 25);

                    return;
                }

                BackgroundImage = null;
            });
        }, true, true);
    }
}