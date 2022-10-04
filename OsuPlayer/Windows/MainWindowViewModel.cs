using Avalonia.Media;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.Modules.Audio.Engine;
using OsuPlayer.Modules.Services;
using OsuPlayer.Views;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public class MainWindowViewModel : BaseWindowViewModel
{
    public readonly IPlayer Player;
    private bool _isPaneOpen;

    private BaseViewModel _mainView;
    private ExperimentalAcrylicMaterial _panelMaterial;

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

    public BaseViewModel MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }

    public ExperimentalAcrylicMaterial PanelMaterial
    {
        get => _panelMaterial;
        set => _panelMaterial = value;
    }

    public MainWindowViewModel()
    {
        var engine = Locator.Current.GetRequiredService<IAudioEngine>();
        Player = Locator.Current.GetRequiredService<IPlayer>();
        var statisticsProvider = Locator.Current.GetService<IStatisticsProvider>();
        var sortProvider = Locator.Current.GetService<ISortProvider>();

        //Generate new ViewModels here
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel(Player, engine);

        SearchView = new SearchViewModel(Player, sortProvider);
        PlaylistView = new PlaylistViewModel(Player);
        PlaylistEditorView = new PlaylistEditorViewModel(Player);
        BlacklistEditorView = new BlacklistEditorViewModel(Player, sortProvider);
        HomeView = new HomeViewModel(Player, statisticsProvider, sortProvider);
        UserView = new UserViewModel(Player);
        EditUserView = new EditUserViewModel();
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel(Player, sortProvider);
        EqualizerView = new EqualizerViewModel(Player);
        UpdateView = new UpdateViewModel();

        using var config = new Config();

        var backgroundColor = config.Container.BackgroundColor.ToColor();

        PanelMaterial = new ExperimentalAcrylicMaterial
        {
            BackgroundSource = AcrylicBackgroundSource.Digger,
            TintColor = backgroundColor,
            TintOpacity = 1,
            MaterialOpacity = 0.25
        };
    }
}