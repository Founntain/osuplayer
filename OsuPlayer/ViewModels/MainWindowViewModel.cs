using Avalonia.ReactiveUI;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class MainWindowBaseViewModel : BaseViewModel, IScreen
{
    private BaseViewModel mainView;
    
    // internal ViewModels
    public readonly SearchViewModel SearchView;
    public readonly PlaylistViewModel PlaylistView;
    public readonly HomeViewModel HomeView;
    public readonly UserViewModel UserView;
    public readonly PartyViewModel PartyView;
    
    public RoutingState Router { get; } = new();
    
    public TopBarViewModel TopBar { get; }
    
    public PlayerControlViewModel PlayerControl { get; }

    public BaseViewModel MainView
    {
        get => mainView;
        set => this.RaiseAndSetIfChanged(ref mainView, value);
    }

    private string currentSongText = "currently playing nothing";
    public string CurrentSongText
    {
        get => currentSongText;
        set => this.RaiseAndSetIfChanged(ref currentSongText, value);
    }

    private double songPosition;
    public double SongPosition
    {
        get => songPosition;
        set => this.RaiseAndSetIfChanged(ref songPosition, value);
    }
    
    public MainWindowBaseViewModel()
    {
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel();
        
        SearchView = new SearchViewModel();
        PlaylistView = new PlaylistViewModel();
        HomeView = new HomeViewModel();
        UserView = new UserViewModel();
        PartyView = new PartyViewModel();

        mainView = HomeView;
        //Generate new ViewModels here
    }
}