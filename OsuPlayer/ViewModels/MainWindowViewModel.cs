using System;
using System.Collections.ObjectModel;
using Avalonia.ReactiveUI;
using OsuPlayer.Audio;
using OsuPlayer.Extensions;
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
    public readonly SettingsViewModel SettingsView;
    
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

    public ObservableCollection<AudioDevice> OutputDeviceComboboxItems { get; set; }

    public MainWindowBaseViewModel()
    {
        TopBar = new TopBarViewModel();
        PlayerControl = new PlayerControlViewModel();
        
        SearchView = new SearchViewModel();
        PlaylistView = new PlaylistViewModel();
        HomeView = new HomeViewModel();
        UserView = new UserViewModel();
        PartyView = new PartyViewModel();
        SettingsView = new SettingsViewModel();

        mainView = HomeView;
        //Generate new ViewModels here
    }
}