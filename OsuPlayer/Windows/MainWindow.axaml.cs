using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Network;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public partial class MainWindow : ReactivePlayerWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel, Player player)
    {
        ViewModel = viewModel;

        Task.Run(player.ImportSongsAsync);

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
        });

        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        using var config = new Config();

        config.Container.Volume = ViewModel.BassEngine.Volume;
        config.Container.Username = ProfileManager.User?.Name;
    }

    private async void Window_OnInitialized(object? sender, EventArgs e)
    {
        await using var config = new Config();

        var result = await GitHubUpdater.CheckForUpdates(config.Container.ReleaseChannel);

#if DEBUG
        // We are ignoring update checks if we are running in debug.
        // The local development version will always be greater than the latest release
#else
        if (result.Item1)
        {
            ViewModel.UpdateView.UpdateUrl = result.Item2;
            ViewModel.UpdateView.NewVersion = result.Item3;
            ViewModel!.MainView = ViewModel.UpdateView;
        }
#endif

        var username = config.Container.Username;

        if (string.IsNullOrWhiteSpace(username)) return;

        var loginWindow = new LoginWindow(username);
        await loginWindow.ShowDialog(this);

        // We only want to update the user panel, when the home view is already open, to refresh the panel.
        if (ViewModel.MainView.GetType() != typeof(HomeViewModel)) return;

        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserNotLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.CurrentUser));

        ViewModel.HomeView.ProfilePicture = await ViewModel.HomeView.LoadProfilePicture();
    }
}