using System.ComponentModel;
using ABI.Windows.Devices.Sensors.Custom;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Nein.Base;
using OsuPlayer.Extensions;
using OsuPlayer.Modules;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Modules.Services;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class MainWindowViewModel : BaseWindowViewModel
{
    public readonly IPlayer Player;

    private bool _isPaneOpen;
    private BaseViewModel? _mainView;
    private ExperimentalAcrylicMaterial? _panelMaterial;

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref _isPaneOpen, value);
    }

    public EditUserViewModel EditUserView { get; }
    public HomeViewModel HomeView { get; }
    public PartyViewModel PartyView { get; }
    public BlacklistEditorViewModel BlacklistEditorView { get; }
    public PlaylistEditorViewModel PlaylistEditorView { get; }
    public PlaylistViewModel PlaylistView { get; }
    public SearchViewModel SearchView { get; }
    public SettingsViewModel SettingsView { get; }
    public UserViewModel UserView { get; }
    public TopBarViewModel TopBar { get; }
    public UpdateViewModel UpdateView { get; }
    public PlayerControlViewModel PlayerControl { get; }
    public EqualizerViewModel EqualizerView { get; }
    public StatisticsViewModel StatisticsView { get; }
    public BeatmapsViewModel BeatmapView { get; }

    private Bitmap? _backgroundImage;

    public Bitmap? BackgroundImage
    {
        get => _backgroundImage;
        set => this.RaiseAndSetIfChanged(ref _backgroundImage, value);
    }

    public BaseViewModel? MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }

    private float _backgroundBlurRadius;

    public float BackgroundBlurRadius
    {
        get => _backgroundBlurRadius;
        set => this.RaiseAndSetIfChanged(ref _backgroundBlurRadius, value);
    }

    public ExperimentalAcrylicMaterial? PanelMaterial
    {
        get => _panelMaterial;
        set => _panelMaterial = value;
    }

    private bool _displayBackgroundImage;

    public bool DisplayBackgroundImage
    {
        get => _displayBackgroundImage;
        set => this.RaiseAndSetIfChanged(ref _displayBackgroundImage, value);
    }

    public MainWindowViewModel(IAudioEngine engine, IPlayer player, IShuffleServiceProvider? shuffleServiceProvider = null,
        IStatisticsProvider? statisticsProvider = null, ISortProvider? sortProvider = null)
    {
        Player = player;

        //Generate new ViewModels here
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel(Player, engine);

        SearchView = new SearchViewModel(Player);
        PlaylistView = new PlaylistViewModel(Player);
        PlaylistEditorView = new PlaylistEditorViewModel(Player);
        BlacklistEditorView = new BlacklistEditorViewModel(Player);
        HomeView = new HomeViewModel(Player, statisticsProvider);
        UserView = new UserViewModel(Player);
        EditUserView = new EditUserViewModel();
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel(Player, sortProvider, shuffleServiceProvider);
        EqualizerView = new EqualizerViewModel(Player);
        UpdateView = new UpdateViewModel();
        StatisticsView = new StatisticsViewModel();
        BeatmapView = new BeatmapsViewModel();

        using var config = new Config();
        
        var backgroundColor = config.Container.BackgroundColor.ToColor();

        PanelMaterial = new ExperimentalAcrylicMaterial
        {
            BackgroundSource = AcrylicBackgroundSource.Digger,
            TintColor = backgroundColor,
            TintOpacity = 1,
            MaterialOpacity = 0.25
        };

        DisplayBackgroundImage = config.Container.DisplayBackgroundImage;
        BackgroundBlurRadius = config.Container.BackgroundBlurRadius;
        
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
                    BackgroundImage = BitmapExtensions.BlurBitmap(d.NewValue, blurRadius: BackgroundBlurRadius, opacity: 0.75f, quality: 25);

                    return;
                }

                BackgroundImage = null;
            });
        }, true, true);
    }
}