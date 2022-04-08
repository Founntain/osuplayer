using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio;
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
        
        Task.Run(player.ImportSongs);
        
        InitializeComponent();

        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;

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
        var configContainer = config.Read();
        configContainer.Volume = ViewModel.BassEngine.Volume;
    }
}