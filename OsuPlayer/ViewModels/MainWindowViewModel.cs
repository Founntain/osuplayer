using System.Collections.ObjectModel;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class MainWindowViewModel : BaseViewModel, IScreen
{
    public readonly HomeViewModel HomeView;
    public readonly PartyViewModel PartyView;
    public readonly PlaylistViewModel PlaylistView;

    // internal ViewModels
    public readonly SearchViewModel SearchView;
    public readonly SettingsViewModel SettingsView;
    public readonly UserViewModel UserView;
    private BaseViewModel _mainView;

    public MainWindowViewModel()
    {
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel();

        SearchView = new SearchViewModel();
        PlaylistView = new PlaylistViewModel();
        HomeView = new HomeViewModel();
        UserView = new UserViewModel();
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel();
        //Generate new ViewModels here
    }

    public TopBarViewModel TopBar { get; }

    public PlayerControlViewModel PlayerControl { get; }

    public BaseViewModel MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }

    public ObservableCollection<AudioDevice> OutputDeviceComboboxItems { get; set; }

    public ReadOnlyObservableCollection<SongEntry> SongEntries
    {
        get => Core.Instance.Player.FilteredSongEntries;
        set => Core.Instance.Player.FilteredSongEntries = value;
    }

    public RoutingState Router { get; } = new();
}