using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.Remote.Protocol.Input;
using Avalonia.VisualTree;
using Avalonia.Win32;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Hotkeys;
using OsuPlayer.Network;
using OsuPlayer.Network.Discord;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private Player _player;
    private HotkeyInitializer _hotkeyInitializer;
    
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel, Player player)
    {
        ViewModel = viewModel;

        _player = player;
        
        Task.Run(_player.ImportSongsAsync);

        _hotkeyInitializer = new (this);
        
        InitializeComponent();

        using var config = new Config();
        TransparencyLevelHint = config.Container.TransparencyLevelHint;

        PlaylistManager.SetLastKnownPlaylistAsCurrentPlaylist();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            if (ViewModel != null)
                ViewModel.MainView = ViewModel.HomeView;

            var hotkeys = new List<Hotkey>()
            {
                new Hotkey(1, Key.Right, () =>
                {
                    _player.NextSong();
                })
            };
            
            _hotkeyInitializer.SetHotkeys(hotkeys);
            
            RegisterHotkeys();
        });

        AvaloniaXamlLoader.Load(this);
    }

    private void RegisterHotkeys()
    {
        _hotkeyInitializer.RegisterHotkeys();
    }

    private void UnregisterHotkeys()
    {
        _hotkeyInitializer.UnregisterHotkeys();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        using var config = new Config();

        config.Container.Volume = ViewModel.BassEngine.Volume;
        config.Container.Username = ProfileManager.User?.Name;
        config.Container.RepeatMode = ViewModel.Player.Repeat;
        config.Container.IsShuffle = ViewModel.Player.IsShuffle.Value;
        config.Container.ActivePlaylistId = ViewModel.Player.ActivePlaylistId;
        
        _player.DisposeDiscordClient();
        
        UnregisterHotkeys();
    }

    private async void Window_OnInitialized(object? sender, EventArgs e)
    {
        await using var config = new Config();

        var rpc = new DiscordClient();
        rpc.Initialize();

#if DEBUG
        // We are ignoring update checks if we are running in debug.
        // The local development version will always be greater than the latest release
        
        // Uncomment code below to force the update UI to show for testing purposes.
        
        // var result = await GitHubUpdater.CheckForUpdates(config.Container.ReleaseChannel);
        //
        // if (result.IsNewVersionAvailable)
        // {
        //     ViewModel.UpdateView.Update = result;
        //     ViewModel!.MainView = ViewModel.UpdateView;
        // }
#else
        var result = await GitHubUpdater.CheckForUpdates(config.Container.ReleaseChannel);

        if (result.IsNewVersionAvailable)
        {
            ViewModel.UpdateView.Update = result;
            ViewModel!.MainView = ViewModel.UpdateView;
        }
#endif

        var username = config.Container.Username;

        if (string.IsNullOrWhiteSpace(username)) return;

        var loginWindow = new LoginWindow(this, username);
        await loginWindow.ShowDialog(this);

        // We only want to update the user panel, when the home view is already open, to refresh the panel.
        if (ViewModel.MainView.GetType() != typeof(HomeViewModel)) return;

        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserNotLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.CurrentUser));

        ViewModel.HomeView.ProfilePicture = await ViewModel.HomeView.LoadProfilePicture();
    }

    private void MainSplitView_OnPaneClosed(object? sender, EventArgs e)
    {
        ViewModel.IsPaneOpen = false;
    }
}