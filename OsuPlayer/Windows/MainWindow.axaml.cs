using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio;
using OsuPlayer.Network;
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
    }

    private async void Window_OnInitialized(object? sender, EventArgs e)
    {
        var result = await GitHubUpdater.CheckForUpdates(true);

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
    }
}