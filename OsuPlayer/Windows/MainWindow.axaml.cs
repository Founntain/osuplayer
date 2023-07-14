using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Nein.Base;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.Network;
using OsuPlayer.Network.LastFM;
using OsuPlayer.Styles;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Views;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public Miniplayer? Miniplayer;

    public FullscreenWindow? FullscreenWindow;

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

        this.WhenActivated(_ =>
        {
            if (ViewModel != null)
                ViewModel.MainView = ViewModel.HomeView;
        });

        using var config = new Config();
        TransparencyLevelHint = config.Container.BackgroundMode.ToWindowTransparencyLevelList();
        FontWeight = (FontWeight) config.Container.DefaultFontWeight;

        var backgroundColor = config.Container.BackgroundColor;

        Background = new SolidColorBrush(backgroundColor.ToColor());

        var accentColor = config.Container.AccentColor;

        ColorSetter.SetColor(accentColor.ToColor());

        Application.Current!.Resources["SmallerFontWeight"] = config.Container.GetNextSmallerFont().ToFontWeight();
        Application.Current!.Resources["DefaultFontWeight"] = config.Container.DefaultFontWeight.ToFontWeight();
        Application.Current!.Resources["BiggerFontWeight"] = config.Container.GetNextBiggerFont().ToFontWeight();

        FontFamily = config.Container.Font ?? FontManager.Current.DefaultFontFamily.Name;
        config.Container.Font ??= FontFamily.Name;

        Task.Run(async () =>
        {
            try
            {
                var window = Locator.Current.GetService<MainWindow>();
                var lastFmApi = Locator.Current.GetService<LastFmApi>();

                await using var config = new Config();

                var apiKey = config.Container.LastFmApiKey;
                var apiSecret = config.Container.LastFmSecret;
                var sessionKey = await lastFmApi.LoadSessionKeyAsync();

                if (!string.IsNullOrWhiteSpace(apiKey) || !string.IsNullOrWhiteSpace(apiSecret) || !sessionKey)
                {
                    Console.WriteLine("Can't connect to last.fm, because no apikey, apisecret or session key fast found");
                    return;
                }

                // We only load the APIKey from the config, as it is the only key that we save
                // 1. Because we always need the api key for all the request
                // 2. The secret is only used for the first authentication of the token
                // 3. After that all subsequent last.fm api calls only need the api key and session key
                lastFmApi.SetApiKeyAndSecret(apiKey, apiSecret);

                if (!lastFmApi.IsAuthorized())
                {
                    await lastFmApi.GetAuthToken();
                    lastFmApi.AuthorizeToken();

                    await MessageBox.ShowDialogAsync(window, "Close this window, when you are done, authenticating in the browser");

                    await lastFmApi.GetSessionKey();

                    await lastFmApi.SaveSessionKeyAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong happened when connecting to last.fm API {ex}");
            }
        });

#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnClosing(WindowClosingEventArgs e)
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
        if (!File.Exists("data/session.op"))
            return;

        var sessionToken = await File.ReadAllTextAsync("data/session.op");

        await ProfileManager.Login(sessionToken);

        // We only want to update the user panel, when the home view is already open, to refresh the panel.
        if (ViewModel?.MainView?.GetType() != typeof(HomeViewModel)) return;

        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.IsUserNotLoggedIn));
        ViewModel.HomeView.RaisePropertyChanged(nameof(ViewModel.HomeView.CurrentUser));

        ViewModel.HomeView.ProfilePicture = await ViewModel.HomeView.LoadProfilePicture();
    }
}