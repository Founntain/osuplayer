using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Windowing;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Extensions.EnumExtensions;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network;
using OsuPlayer.Services;
using OsuPlayer.Styles;
using OsuPlayer.UI_Extensions;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public partial class FluentAppWindow : FluentReactiveWindow<FluentAppWindowViewModel>
{
    private readonly ILoggingService _loggingService;
    private readonly IProfileManagerService _profileManager;

    public Miniplayer? Miniplayer;
    public FullscreenWindow? FullscreenWindow;

    public FluentAppWindow(FluentAppWindowViewModel viewModel, ILoggingService loggingService)
    {
        ViewModel = viewModel;

        _profileManager = ViewModel.ProfileManager;
        _loggingService = loggingService;

        var player = ViewModel.Player;

        Task.Run(() => SongImporter.ImportSongsAsync(player.SongSourceProvider, player as IImportNotifications));

        InitializeComponent();

        // Setting AppWindow Properties
        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        TransparencyLevelHint = new [] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };

        // Loading config stuff
        using var config = new Config();

        _loggingService.Log("Loaded config successfully", LogType.Success, config.Container);

        var backgroundColor = config.Container.BackgroundColor;

        Background = new SolidColorBrush(backgroundColor.ToColor());

        var accentColor = config.Container.AccentColor;

        ColorSetter.SetColor(accentColor.ToColor());

        Application.Current!.Resources["SmallerFontWeight"] = config.Container.GetNextSmallerFont().ToFontWeight();
        Application.Current!.Resources["DefaultFontWeight"] = config.Container.DefaultFontWeight.ToFontWeight();
        Application.Current!.Resources["BiggerFontWeight"] = config.Container.GetNextBiggerFont().ToFontWeight();

        FontFamily = config.Container.Font ?? FontManager.Current.DefaultFontFamily;
        config.Container.Font ??= FontFamily.Name;

        // Setting up last.fm stuff if enabled
        Task.Run(async () =>
        {
            try
            {
                var window = Locator.Current.GetService<FluentAppWindow>();
                var lastFmApi = Locator.Current.GetService<ILastFmApiService>();
                var loggingService = Locator.Current.GetService<ILoggingService>();

                await using var config = new Config();

                var apiKey = config.Container.LastFmApiKey;
                var apiSecret = config.Container.LastFmSecret;
                var sessionKey = await lastFmApi.LoadSessionKeyAsync();

                if (!string.IsNullOrWhiteSpace(apiKey) || !string.IsNullOrWhiteSpace(apiSecret) || !sessionKey)
                {
                    loggingService.Log("Can't connect to last.fm, because no apikey, apisecret or session key fast found", LogType.Warning);
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

        this.WhenActivated(_ =>
        {
            if (ViewModel != null)
                ViewModel.MainView = ViewModel.HomeView;
        });
    }

    private void AppNavigationView_OnItemInvoked(object? sender, NavigationViewItemInvokedEventArgs e)
    {
        if (e.IsSettingsInvoked)
        {
            ViewModel!.MainView = ViewModel.SettingsView;

            return;
        }

        switch (e.InvokedItemContainer.Tag)
        {
            case "BeatmapsNavigation":
            {
                ViewModel!.MainView = ViewModel.BeatmapView;
                break;
            }
            case "SearchNavigation":
            {
                ViewModel!.MainView = ViewModel.SearchView;
                break;
            }
            case "PlaylistNavigation":
            {
                ViewModel!.MainView = ViewModel.PlaylistView;
                break;
            }
            case "HomeNavigation":
            {
                ViewModel!.MainView = ViewModel.HomeView;
                break;
            }
            case "UserNavigation":
            {
                ViewModel!.MainView = ViewModel.UserView;
                break;
            }
            case "PartyNavigation":
            {
                ViewModel!.MainView = ViewModel.PartyView;
                break;
            }
            case "StatisticsNavigation":
            {
                ViewModel!.MainView = ViewModel.StatisticsView;
                break;
            }
            case "MiniplayerNavigation":
            {
                OpenMiniplayer();
                break;
            }
            default:
            {
                ViewModel!.MainView = ViewModel!.HomeView;
                break;
            }
        }
    }

    private async void SearchBox_OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        var acb = (sender as AutoCompleteBox);

        if (acb?.SelectedItem is IMapEntryBase map)
        {
            var result = await ViewModel?.Player?.TryPlaySongAsync(map);

            if (result)
            {
                acb.Text = null;
                return;
            }
        }

        e.Handled = true;
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

    private void OpenMiniplayer()
    {
        if (Miniplayer != null)
            return;

        Miniplayer = new Miniplayer(ViewModel.Player, Locator.Current.GetRequiredService<IAudioEngine>());

        Miniplayer.Show();

        WindowState = WindowState.Minimized;
    }
}