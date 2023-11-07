using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using Nein.Controls;
using Nein.Extensions;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network.LastFm;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public partial class SettingsView : ReactiveControl<SettingsViewModel>
{
    private MainWindow? _mainWindow;

    public SettingsView()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            _mainWindow = Locator.Current.GetRequiredService<MainWindow>();
            ViewModel.MainWindow = _mainWindow;

            ViewModel.SettingsCategories = SettingsGrid.Children;
        });
    }

    private void SettingsView_OnInitialized(object? sender, EventArgs e)
    {
        var config = new Config();

        ViewModel!.OsuLocation = config.Container.OsuPath!;
    }

    public async void ImportSongsClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (_mainWindow == default) return;

        var dialog = new OpenFileDialog
        {
            Title = "Select your osu!.db or client.realm file",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions = new List<string>
                    {
                        "db",
                        "realm"
                    }
                }
            }
        };

        var result = await dialog.ShowAsync(_mainWindow);

        if (result == default)
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Did you even selected a file?!");
            return;
        }

        var path = result.FirstOrDefault();

        if (Path.GetFileName(path) != "osu!.db" && Path.GetFileName(path) != "client.realm")
        {
            await MessageBox.ShowDialogAsync(_mainWindow,
                "You had one job! Just one. Select your osu!.db or client.realm! Not anything else!");
            return;
        }

        var osuFolder = Path.GetDirectoryName(path);

        await using (var config = new Config())
        {
            (await config.ReadAsync()).OsuPath = osuFolder!;
            ViewModel.OsuLocation = osuFolder!;
        }

        var player = ViewModel.Player;

        await Task.Run(() => SongImporter.ImportSongsAsync(player.SongSourceProvider, player as IImportNotifications));
        //await Task.Run(ViewModel.Player.ImportSongsAsync);
    }

    public async void ImportCollectionsClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (_mainWindow == default) return;

        var player = ViewModel.Player;
        var success = await Task.Run(() => CollectionImporter.ImportCollectionsAsync(player.SongSourceProvider));

        MessageBox.Show(_mainWindow, success ? "Import successful. Have fun!" : "There are no collections in osu!", "Import complete!");
    }

    public async void LoginClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        var loginWindow = new LoginWindow
        {
            ViewModel = new LoginWindowViewModel()
        };

        await loginWindow.ShowDialog(_mainWindow);
    }

    private void OpenEqClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.EqualizerView;
    }

    private void OnUsePitch_Click(object? sender, RoutedEventArgs e)
    {
        var value = (sender as ToggleSwitch)?.IsChecked;

        using var config = new Config();

        config.Container.UsePitch = value ?? true;

        var engine = Locator.Current.GetRequiredService<IAudioEngine>();

        engine.UpdatePlaybackMethod();
    }

    private async void LastFmAuth_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = Locator.Current.GetService<MainWindow>();
        var lastFmApi = Locator.Current.GetService<ILastFmApiService>();

        await using var config = new Config();

        var apiKey = string.IsNullOrWhiteSpace(ViewModel.LastFmApiKey) ? config.Container.LastFmApiKey : ViewModel.LastFmApiKey;
        var apiSecret = string.IsNullOrWhiteSpace(ViewModel.LastFmApiKey) ? config.Container.LastFmSecret : ViewModel.LastFmApiSecret;

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
        {
            await MessageBox.ShowDialogAsync(window, "Please enter a API-Key and API-Secret before authorizing");
            return;
        }

        // We only load the APIKey from the config, as it is the only key that we save
        // 1. Because we always need the api key for all the request
        // 2. The secret is only used for the first authentication of the token
        // 3. After that all subsequent last.fm api calls only need the api key and session key
        lastFmApi.SetApiKeyAndSecret(apiKey, apiSecret);

        await lastFmApi.LoadSessionKeyAsync();

        ViewModel.IsLastFmAuthorized = lastFmApi.IsAuthorized();

        if (!ViewModel.IsLastFmAuthorized)
        {
            await lastFmApi.GetAuthToken();
            lastFmApi.AuthorizeToken();

            await MessageBox.ShowDialogAsync(window, "Close this window, when you are done, authenticating in the browser");

            await lastFmApi.GetSessionKey();

            await lastFmApi.SaveSessionKeyAsync();

            ViewModel.IsLastFmAuthorized = lastFmApi.IsAuthorized();
        }
    }

    private void ExportCollectionsClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == null) return;

        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.ExportSongsView;
    }

    private void OpenPlayerHistoryClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == null) return;

        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.PlayHistoryView;
    }

    private void ReportBug_OnClick(object? sender, RoutedEventArgs e) => GeneralExtensions.OpenUrl("https://github.com/founntain/osuplayer/issues/new/choose");

    private void JoinDiscord_OnClick(object? sender, RoutedEventArgs e) => GeneralExtensions.OpenUrl("https://discord.gg/RJQSc5B");

    private void ContactUs_OnClick(object? sender, RoutedEventArgs e) => GeneralExtensions.OpenUrl("https://github.com/founntain/osuplayer#-contact");

    private void Github_OnClick(object? sender, RoutedEventArgs e) => GeneralExtensions.OpenUrl("https://github.com/founntain/osuplayer");
}