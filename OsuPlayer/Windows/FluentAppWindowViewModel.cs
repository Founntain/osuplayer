using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Views;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public class FluentAppWindowViewModel : BaseWindowViewModel
{
    public readonly IPlayer Player;
    public readonly IProfileManagerService ProfileManager;

    private BaseViewModel? _mainView;

    public TopBarViewModel TopBar { get; }
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
    public PlayHistoryViewModel PlayHistoryView { get; set; }

    public BaseViewModel? MainView
    {
        get => _mainView;
        set => this.RaiseAndSetIfChanged(ref _mainView, value);
    }

    public FluentAppWindowViewModel(IAudioEngine engine, IPlayer player, IProfileManagerService profileManager, IShuffleServiceProvider? shuffleServiceProvider = null,
        IStatisticsProvider? statisticsProvider = null, ISortProvider? sortProvider = null, IHistoryProvider? historyProvider = null)
    {
        Player = player;
        ProfileManager = profileManager;

        PlayerControl = new PlayerControlViewModel(Player, engine);

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
    }
}