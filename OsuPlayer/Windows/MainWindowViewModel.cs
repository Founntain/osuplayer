using OsuPlayer.ViewModels;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class MainWindowViewModel : BaseWindowViewModel
{
    public readonly BassEngine BassEngine;
    public readonly Player Player;

    private BaseViewModel _mainView;

    public MainWindowViewModel(BassEngine engine, Player player)
    {
        BassEngine = engine;
        Player = player;

        //Generate new ViewModels here
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel(Player, BassEngine);

        SearchView = new SearchViewModel(Player);
        PlaylistView = new PlaylistViewModel(Player);
        PlaylistEditorView = new PlaylistEditorViewModel(Player);
        BlacklistEditorView = new BlacklistEditorViewModel(Player);
        HomeView = new HomeViewModel(Player);
        UserView = new UserViewModel(Player);
        EditUserView = new EditUserViewModel();
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel(Player);
        EqualizerView = new EqualizerViewModel(Player);
        UpdateView = new UpdateViewModel();
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
}