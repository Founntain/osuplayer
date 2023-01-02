using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.Importer;
using OsuPlayer.Network;
using OsuPlayer.Styles;
using OsuPlayer.Views;
using ReactiveUI;

namespace OsuPlayer.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public Miniplayer? Miniplayer;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainWindowViewModel viewModel)
    {
        ViewModel = viewModel;

        var player = ViewModel.Player;

        Task.Run(() => SongImporter.ImportSongsAsync(player.SongSourceProvider, player as IImportNotifications));

        InitializeComponent();

        using var config = new Config();
        TransparencyLevelHint = config.Container.TransparencyLevelHint;
        FontWeight = (FontWeight) config.Container.DefaultFontWeight;

        var backgroundColor = config.Container.BackgroundColor;

        Background = new SolidColorBrush(backgroundColor.ToColor());

        var accentColor = config.Container.AccentColor;

        ColorSetter.SetColor(accentColor.ToColor());

        config.Container.BackgroundColor = backgroundColor;
        config.Container.AccentColor = accentColor;

        Application.Current!.Resources["SmallerFontWeight"] = config.Container.GetSmallerFont().ToFontWeight();
        Application.Current!.Resources["DefaultFontWeight"] = config.Container.DefaultFontWeight.ToFontWeight();
        Application.Current!.Resources["BiggerFontWeight"] = config.Container.GetBiggerFont().ToFontWeight();

        FontFamily = config.Container.Font ?? FontManager.Current.DefaultFontFamilyName;

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (ViewModel != null)
                ViewModel.MainView = ViewModel.HomeView;
        });

        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (ViewModel == default) return;

        using var config = new Config();

        config.Container.Volume = ViewModel.Player.Volume.Value;
        config.Container.Username = ProfileManager.User?.Name;
        config.Container.RepeatMode = ViewModel.Player.RepeatMode.Value;
        config.Container.IsShuffle = ViewModel.Player.IsShuffle.Value;
        config.Container.SelectedPlaylist = ViewModel.Player.SelectedPlaylist.Value?.Id;

        ViewModel.Player.DisposeDiscordClient();
    }

    private async void Window_OnInitialized(object? sender, EventArgs e)
    {
        // var rpc = new DiscordClient();
        // rpc.Initialize();

        if (Debugger.IsAttached) return;

        if (ViewModel == default) return;

        await using var config = new Config();

        var result = await GitHub.CheckForUpdates(config.Container.ReleaseChannel);

        if (!result.IsNewVersionAvailable) return;

        ViewModel.UpdateView.Update = result;
        ViewModel.MainView = ViewModel.UpdateView;
    }

    private void MainSplitView_OnPaneClosed(object? sender, EventArgs e)
    {
        if (ViewModel == default) return;

        ViewModel.IsPaneOpen = false;
    }

    private async void Window_OnOpened(object? sender, EventArgs e)
    {
        var sessionToken = await File.ReadAllTextAsync("session.op");

        await ProfileManager.Login(sessionToken);

        // We only want to update the user panel, when the home view is already open, to refresh the panel.
        if (ViewModel?.MainView?.GetType() != typeof(HomeViewModel)) return;

        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserNotLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.CurrentUser));

        ViewModel.HomeView.ProfilePicture = await ViewModel.HomeView.LoadProfilePicture();
    }
}