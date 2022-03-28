using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class MainWindowViewModel : BaseWindowViewModel
{
    public EditUserViewModel EditUserView;
    public HomeViewModel HomeView;
    public PartyViewModel PartyView;
    public PlaylistEditorViewModel PlaylistEditorViewModel;
    public PlaylistViewModel PlaylistView;

    // internal ViewModels
    public SearchViewModel SearchView;
    public SettingsViewModel SettingsView;
    public UserViewModel UserView;

    private BaseViewModel _mainView;

    private readonly BassEngine _bassEngine;
    public readonly Player Player;

    public MainWindowViewModel(BassEngine engine, Player player)
    {
        _bassEngine = engine;
        Player = player;
    }

    public void SetUp(MainWindow mainWindow)
    {
        Player.MainWindow = mainWindow;

        //Generate new ViewModels here
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel(Player, _bassEngine);

        SearchView = new SearchViewModel(Player);
        PlaylistView = new PlaylistViewModel();
        PlaylistEditorViewModel = new PlaylistEditorViewModel(Player);
        HomeView = new HomeViewModel(Player);
        UserView = new UserViewModel(Player);
        EditUserView = new EditUserViewModel();
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel(Player);
    }

    public TopBarViewModel TopBar { get; private set; }

    public PlayerControlViewModel PlayerControl { get; private set; }

    public BaseViewModel MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }
}