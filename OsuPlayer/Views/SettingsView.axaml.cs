using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using Nein.Controls;
using Nein.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network.LastFM;
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
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
            {
                _mainWindow = mainWindow;
                ViewModel.MainWindow = mainWindow;
            }

            ViewModel.SettingsCategories =
                this.FindControl<CascadingWrapPanel>("SettingsGrid").Children;
        });

        AvaloniaXamlLoader.Load(this);
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

    private void ReportBug_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/osu-player/osuplayer/issues/new/choose");
    }

    private void JoinDiscord_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://discord.gg/RJQSc5B");
    }

    private void ContactUs_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/osu-player/osuplayer#contact");
    }

    private void OnUsePitch_Click(object? sender, RoutedEventArgs e)
    {
        var value = (sender as ToggleSwitch)?.IsChecked;

        var engine = Locator.Current.GetRequiredService<IAudioEngine>();

        engine.UpdatePlaybackMethod();
    }

    private async void LastFmAuth_OnClick(object? sender, RoutedEventArgs e)
    {
        var player = Locator.Current.GetRequiredService<IPlayer>() as Player;
        var window = Locator.Current.GetService<MainWindow>();

        using var config = new Config();

        // We only load the APIKey from the config, as it is the only key that we save
        // 1. Because we always need the api key for all the request
        // 2. The secret is only used for the first authentication of the token
        // 3. After that all subsequent last.fm api calls only need the api key and session key
        var api = new LastFmApi(config.Container.LastFmApiKey, config.Container.LastFmSecret);

        await api.LoadSessionKey();

        if (!api.IsAuthorized())
        {
            await api.GetAuthToken();
            api.AuthorizeToken();

            await MessageBox.ShowDialogAsync(window, "Close this window, when you are done, authenticating in the browser");
        
            await api.GetSessionKey();

            await api.SaveSessionKey();
        }
        
        var currentSong = player?.CurrentSong.Value;

        if (currentSong == default) return;

        await api.UpdateNowPlaying(currentSong.Title, currentSong.Artist, (ulong) TimeSpan.FromMicroseconds(currentSong.TotalTime).TotalSeconds);
    }
}