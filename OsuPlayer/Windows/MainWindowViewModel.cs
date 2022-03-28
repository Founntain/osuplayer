using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class MainWindowViewModel : BaseWindowViewModel
{
    private readonly BassEngine _bassEngine;
    public readonly Player Player;

    private BaseViewModel _mainView;

    public MainWindowViewModel(BassEngine engine, Player player)
    {
        _bassEngine = engine;
        Player = player;

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

    public EditUserViewModel EditUserView { get; }
    public HomeViewModel HomeView { get; }
    public PartyViewModel PartyView { get; }
    public PlaylistEditorViewModel PlaylistEditorViewModel { get; }
    public PlaylistViewModel PlaylistView { get; }
    public SearchViewModel SearchView { get; }
    public SettingsViewModel SettingsView { get; }
    public UserViewModel UserView { get; }
    public TopBarViewModel TopBar { get; }
    public PlayerControlViewModel PlayerControl { get; }

    public BaseViewModel MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }
}