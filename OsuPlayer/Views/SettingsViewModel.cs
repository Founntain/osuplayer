using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using Nein.Base;
using Nein.Controls.Interfaces;
using OsuPlayer.Api.Data.API.EntityModels;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions.EnumExtensions;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Network;
using OsuPlayer.Network.Data;
using OsuPlayer.Styles;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class SettingsViewModel : BaseViewModel
{
    public FluentAppWindow MainWindow;

    public readonly IPlayer Player;

    private readonly IProfileManagerService _profileManager;

    private readonly Bindable<bool> _blacklistSkip = new();
    private readonly Bindable<bool> _playlistEnableOnPlay = new();
    private readonly Bindable<SortingMode> _sortingMode = new();
    private readonly FluentAvaloniaTheme _faTheme;

    private bool _usePitch;
    private bool _useDiscordRpc;
    private bool _displayUserStats;
    private bool _enableScrobbling;
    private bool _useLeftNavigationPosition;
    private bool _displayBackgroundImage;
    private float _backgroundBlurRadius;
    private string _lastFmApiKey = string.Empty;
    private string _lastFmApiSecret = string.Empty;
    private string _osuLocation = string.Empty;
    private string _patchnotes = string.Empty;
    private string _settingsSearchQ = string.Empty;
    private string _currentAppTheme = _system;
    private const string _system = "System";
    private const string _dark = "Dark";
    private const string _light = "Light";
    private const string _lowQuality = "Low Quality";
    private const string _mediumQuality = "Medium Quality";
    private const string _highQuality = "High Quality";
    private string? _selectedFont;
    private KnownColors _selectedAccentColor;
    private KnownColors _selectedBackgroundColor;
    private FontWeights _selectedFontWeight;
    private StartupSong _selectedStartupSong;
    private AudioDevice? _selectedAudioDevice;
    private IShuffleImpl? _selectedShuffleAlgorithm;
    private BackgroundMode _backgroundMode;
    private ReleaseChannels _selectedReleaseChannel;
    private IShuffleServiceProvider? _shuffleServiceProvider;
    private List<OsuPlayerContributor>? _contributors;
    private string _renderingMode;

    public string[] AppThemes { get; } = { _system, _light , _dark };
    public string[] RenderingModes { get; } = { _highQuality, _mediumQuality, _lowQuality  };

    public string RenderingMode
    {
        get => _renderingMode;
        set
        {
            this.RaiseAndSetIfChanged(ref _renderingMode, value);

            using var config = new Config();

            var renderingMode = GetRenderingMode(value);

            config.Container.RenderingMode = renderingMode;

            MainWindow.SetRenderMode(renderingMode);
        }
    }

    public bool UseLeftNavigationPosition
    {
        get => _useLeftNavigationPosition;
        set
        {
            this.RaiseAndSetIfChanged(ref _useLeftNavigationPosition, value);

            using var config = new Config();

            config.Container.UseLeftNavigationPosition = value;

            MainWindow.AppNavigationView.PaneDisplayMode = value ? NavigationViewPaneDisplayMode.Left : NavigationViewPaneDisplayMode.Top;
        }
    }

    public bool DisplayUserStats
    {
        get => _displayUserStats;
        set
        {
            this.RaiseAndSetIfChanged(ref _displayUserStats, value);

            using var config = new Config();

            config.Container.DisplayerUserStats = value;

            var mainWindowViewModel = Locator.Current.GetService<FluentAppWindowViewModel>();

            mainWindowViewModel.HomeView.DisplayUserStats = value;

            mainWindowViewModel.HomeView.RaisePropertyChanged(nameof(mainWindowViewModel.HomeView.DisplayUserStats));
        }
    }

    private bool _isLastFmAuthorized;

    public bool IsLastFmAuthorized
    {
        get => _isLastFmAuthorized;
        set => this.RaiseAndSetIfChanged(ref _isLastFmAuthorized, value);
    }

    public bool EnableScrobbling
    {
        get => _enableScrobbling;
        set
        {
            this.RaiseAndSetIfChanged(ref _enableScrobbling, value);

            using var config = new Config();

            config.Container.EnableScrobbling = value;
        }
    }

    public string LastFmApiKey
    {
        get => _lastFmApiKey;
        set => this.RaiseAndSetIfChanged(ref _lastFmApiKey, value);
    }

    public string LastFmApiSecret
    {
        get => _lastFmApiSecret;
        set => this.RaiseAndSetIfChanged(ref _lastFmApiSecret, value);
    }

    public float BackgroundBlurRadius
    {
        get => _backgroundBlurRadius;
        set
        {
            this.RaiseAndSetIfChanged(ref _backgroundBlurRadius, value);

            using var config = new Config();

            config.Container.BackgroundBlurRadius = value;

            if (MainWindow?.ViewModel == null) return;

            MainWindow.ViewModel.BackgroundBlurRadius = value;
        }
    }

    public bool DisplayBackgroundImage
    {
        get => _displayBackgroundImage;
        set
        {
            this.RaiseAndSetIfChanged(ref _displayBackgroundImage, value);

            using var config = new Config();

            config.Container.DisplayBackgroundImage = value;

            if (MainWindow?.ViewModel == null) return;

            MainWindow.ViewModel.DisplayBackgroundImage = value;

            if (MainWindow?.ViewModel?.PlayerControl == null) return;

            MainWindow.ViewModel.PlayerControl.DisplayBackgroundImage = !value;
        }
    }

    public bool UsePitch
    {
        get => _usePitch;
        set
        {
            this.RaiseAndSetIfChanged(ref _usePitch, value);

            using var config = new Config();
            config.Container.UsePitch = value;
        }
    }

    public List<OsuPlayerContributor>? Contributors
    {
        get => _contributors;
        set => this.RaiseAndSetIfChanged(ref _contributors, value);
    }

    public string Patchnotes
    {
        get => _patchnotes;
        set => this.RaiseAndSetIfChanged(ref _patchnotes, value);
    }

    public UserModel? CurrentUser => _profileManager.User;

    public string OsuLocation
    {
        get => $"osu! location: {_osuLocation}";
        set => this.RaiseAndSetIfChanged(ref _osuLocation, value);
    }

    public IEnumerable<ReleaseChannels> ReleaseChannels => Enum.GetValues<ReleaseChannels>();

    public ReleaseChannels SelectedReleaseChannel
    {
        get => _selectedReleaseChannel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedReleaseChannel, value);

            using var config = new Config();
            config.Container.ReleaseChannel = value;
        }
    }

    public IEnumerable<BackgroundMode> BackgroundModes => Enum.GetValues<BackgroundMode>();

    public IEnumerable<KnownColors> KnownColors => Enum.GetValues<KnownColors>();

    public IEnumerable<FontWeights> AvailableFontWeights => Enum.GetValues<FontWeights>();

    public FontWeights SelectedFontWeight
    {
        get => _selectedFontWeight;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFontWeight, value);

            using var config = new Config();
            config.Container.DefaultFontWeight = value;

            Application.Current!.Resources["SmallerFontWeight"] = config.Container.GetNextSmallerFont().ToFontWeight();
            Application.Current!.Resources["DefaultFontWeight"] = value.ToFontWeight();
            Application.Current!.Resources["BiggerFontWeight"] = config.Container.GetNextBiggerFont().ToFontWeight();

            Debug.WriteLine("SMALLER FONT: " + config.Container.GetNextSmallerFont().ToFontWeight());
            Debug.WriteLine("NORMAL FONT: " + value.ToFontWeight());
            Debug.WriteLine("BIGGER FONT: " + config.Container.GetNextBiggerFont().ToFontWeight());
        }
    }

    public IEnumerable<string> Fonts => FontManager.Current.SystemFonts.Select(x => x.Name);

    public string? SelectedFont
    {
        get => _selectedFont;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedFont, value);

            if (value == null)
                return;

            using var config = new Config();

            config.Container.Font = value;

            if (MainWindow == null) return;

            MainWindow.FontFamily = value;
        }
    }

    public BackgroundMode BackgroundMode
    {
        get => _backgroundMode;
        set
        {
            this.RaiseAndSetIfChanged(ref _backgroundMode, value);

            if (MainWindow == null) return;

            using var config = new Config();

            // switch (value)
            // {
            //     case BackgroundMode.AcrylicBlur:
            //         MainWindow.TransparencyLevelHint = WindowTransparencyLevel.AcrylicBlur;
            //
            //         break;
            //     case BackgroundMode.Mica:
            //         MainWindow.TransparencyLevelHint = WindowTransparencyLevel.Mica;
            //
            //         break;
            //     case BackgroundMode.SolidColor:
            //     default:
            //         MainWindow.TransparencyLevelHint = WindowTransparencyLevel.None;
            //
            //         break;
            // }
        }
    }

    public KnownColors SelectedBackgroundColor
    {
        get => _selectedBackgroundColor;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBackgroundColor, value);

            if (MainWindow?.ViewModel == null) return;

            MainWindow.ViewModel.PanelMaterial = new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = value.ToColor(),
                TintOpacity = 1,
                MaterialOpacity = 0.25
            };

            MainWindow.Background = new SolidColorBrush(value.ToColor());
            MainWindow.ViewModel.RaisePropertyChanged(nameof(MainWindow.ViewModel.PanelMaterial));

            using var config = new Config();
            config.Container.BackgroundColor = value;
        }
    }

    public KnownColors SelectedAccentColor
    {
        get => _selectedAccentColor;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAccentColor, value);

            ColorSetter.SetColor(value.ToColor());

            using var config = new Config();
            config.Container.AccentColor = value;
        }
    }

    public IEnumerable<StartupSong> StartupSongs => Enum.GetValues<StartupSong>();

    public StartupSong SelectedStartupSong
    {
        get => _selectedStartupSong;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedStartupSong, value);

            using var config = new Config();
            config.Container.StartupSong = value;
        }
    }

    public IEnumerable<SortingMode> SortingModes => Enum.GetValues<SortingMode>();

    public SortingMode SelectedSortingMode
    {
        get => _sortingMode.Value;
        set
        {
            _sortingMode.Value = value;
            this.RaisePropertyChanged();

            using var config = new Config();
            config.Container.SortingMode = value;
        }
    }

    public List<IShuffleImpl>? ShuffleAlgorithms => _shuffleServiceProvider?.ShuffleAlgorithms;

    public IShuffleImpl? SelectedShuffleAlgorithm
    {
        get => _selectedShuffleAlgorithm;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedShuffleAlgorithm, value);
            this.RaisePropertyChanged(nameof(SelectedShuffleAlgorithmInfoText));

            _shuffleServiceProvider?.SetShuffleImpl(value);
        }
    }

    public string SelectedShuffleAlgorithmInfoText => $"{SelectedShuffleAlgorithm?.Name} - {SelectedShuffleAlgorithm?.Description}";

    public List<AudioDevice> AvailableAudioDevices { get; }

    public AudioDevice SelectedAudioDevice
    {
        get => _selectedAudioDevice ?? AvailableAudioDevices[0];
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAudioDevice, value);

            using var config = new Config();
            config.Container.SelectedAudioDeviceDriver = value.Driver;

            Player.SetDevice(value);
        }
    }

    public bool BlacklistSkip
    {
        get => _blacklistSkip.Value;
        set
        {
            _blacklistSkip.Value = value;
            this.RaisePropertyChanged();

            using var cfg = new Config();
            cfg.Container.BlacklistSkip = value;
        }
    }

    public bool UseDiscordRpc
    {
        get => _useDiscordRpc;
        set
        {
            this.RaiseAndSetIfChanged(ref _useDiscordRpc, value);

            using var cfg = new Config();
            cfg.Container.UseDiscordRpc = value;
        }
    }

    public bool PlaylistEnableOnPlay
    {
        get => _playlistEnableOnPlay.Value;
        set
        {
            _playlistEnableOnPlay.Value = value;
            this.RaisePropertyChanged();

            using var cfg = new Config();
            cfg.Container.PlaylistEnableOnPlay = value;
        }
    }

    public string SettingsSearchQ
    {
        get => _settingsSearchQ;
        set
        {
            var searchQs = value.Split(' ');

            if (SettingsCategories == default) return;

            foreach (var category in SettingsCategories)
            {
                if (category is not Grid settingsCat) continue;

                var settingsPanel =
                    settingsCat.Children.FirstOrDefault(x => x.Name?.Contains(category.Name) ?? false);

                if (settingsPanel is not StackPanel stackPanel) continue;

                var settings = stackPanel.Children;

                var categoryFound = searchQs.All(x =>
                    category.Name?.Contains(x, StringComparison.OrdinalIgnoreCase) ?? true);

                if (categoryFound)
                {
                    category.IsVisible = true;
                    foreach (var setting in settings) setting.IsVisible = true;

                    foreach (var setting in settings)
                        if (setting is ISettingsDisplayer settingsDisplayer)
                            settingsDisplayer.RefreshCorners();

                    continue;
                }

                var foundAnySettings = false;

                foreach (var setting in settings)
                {
                    setting.IsVisible = searchQs.All(x =>
                        setting.Name?.Contains(x, StringComparison.OrdinalIgnoreCase) ?? false);

                    foundAnySettings = foundAnySettings || setting.IsVisible;
                }

                foreach (var setting in settings)
                    if (setting is ISettingsDisplayer settingsDisplayer)
                        settingsDisplayer.RefreshCorners();

                category.IsVisible = foundAnySettings;
            }

            this.RaiseAndSetIfChanged(ref _settingsSearchQ, value);
        }
    }

    public Controls? SettingsCategories { get; set; }

    public SettingsViewModel(IPlayer player, ISortProvider? sortProvider, IShuffleServiceProvider? shuffleServiceProvider, IProfileManagerService profileManager)
    {
        _faTheme = (Application.Current!.Styles[0] as FluentAvaloniaTheme)!;

        Player = player;

        _shuffleServiceProvider = shuffleServiceProvider;
        _profileManager = profileManager;

        AvailableAudioDevices = Player.AvailableAudioDevices;

        var config = new Config();

        _selectedStartupSong = config.Container.StartupSong;
        _backgroundMode = config.Container.BackgroundMode;
        _selectedReleaseChannel = config.Container.ReleaseChannel;
        _selectedBackgroundColor = config.Container.BackgroundColor;
        _selectedAccentColor = config.Container.AccentColor;
        _selectedFontWeight = config.Container.DefaultFontWeight;
        _selectedFont = config.Container.Font ?? FontManager.Current.DefaultFontFamily.Name;
        _selectedShuffleAlgorithm = ShuffleAlgorithms?.FirstOrDefault(x => x == _shuffleServiceProvider?.ShuffleImpl);
        _selectedAudioDevice = AvailableAudioDevices.FirstOrDefault(x => x.Driver == config.Container.SelectedAudioDeviceDriver);
        _useDiscordRpc = config.Container.UseDiscordRpc;
        _usePitch = config.Container.UsePitch;
        _displayBackgroundImage = config.Container.DisplayBackgroundImage;
        _backgroundBlurRadius = config.Container.BackgroundBlurRadius;
        _enableScrobbling = config.Container.EnableScrobbling;
        _displayUserStats = config.Container.DisplayerUserStats;
        _useLeftNavigationPosition = config.Container.UseLeftNavigationPosition;
        _renderingMode = GetRenderingModeString(config.Container.RenderingMode);

        var lastFmApi = Locator.Current.GetService<ILastFmApiService>();

        _isLastFmAuthorized = lastFmApi.LoadSessionKey();

        if (sortProvider != null)
        {
            _sortingMode.BindTo(sortProvider.SortingModeBindable);
            _sortingMode.BindValueChanged(_ => this.RaisePropertyChanged(nameof(SelectedSortingMode)));
            _sortingMode.Value = config.Container.SortingMode;
        }

        _blacklistSkip.BindTo(Player.BlacklistSkip);
        _blacklistSkip.BindValueChanged(_ => this.RaisePropertyChanged(nameof(BlacklistSkip)));

        _playlistEnableOnPlay.BindTo(Player.PlaylistEnableOnPlay);
        _blacklistSkip.BindValueChanged(_ => this.RaisePropertyChanged(nameof(PlaylistEnableOnPlay)));

        Activator = new ViewModelActivator();
        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        var latestPatchNotes = await GitHub.GetLatestPatchNotes(_selectedReleaseChannel);

        if (string.IsNullOrWhiteSpace(latestPatchNotes)) return;

        Patchnotes = latestPatchNotes;

        Contributors = await GitHub.GetContributers() ?? new List<OsuPlayerContributor>();
    }

    public string CurrentAppTheme
    {
        get => _currentAppTheme;
        set
        {
            if (string.IsNullOrWhiteSpace(this.RaiseAndSetIfChanged(ref _currentAppTheme, value)))
                return;

            var newTheme = GetThemeVariant(value);

            if (newTheme != null)
            {
                if (Application.Current == null)
                {
                    return;
                }

                Application.Current.RequestedThemeVariant = newTheme;
            }

            _faTheme.PreferSystemTheme = value == _system;
        }
    }

    private ThemeVariant? GetThemeVariant(string value)
    {
        return value switch
        {
            _light => ThemeVariant.Light,
            _dark => ThemeVariant.Dark,
            _ => null
        };
    }

    private BitmapInterpolationMode GetRenderingMode(string value)
    {
        return value switch
        {
          _highQuality => BitmapInterpolationMode.HighQuality,
          _mediumQuality => BitmapInterpolationMode.MediumQuality,
          _lowQuality => BitmapInterpolationMode.LowQuality,
          _ => throw new ArgumentOutOfRangeException($"The value {value} is not supported here. How the heck did you even got here?")
        };
    }

    private string GetRenderingModeString(BitmapInterpolationMode value)
    {
        return value switch
        {
            BitmapInterpolationMode.HighQuality => _highQuality,
            BitmapInterpolationMode.MediumQuality => _mediumQuality,
            BitmapInterpolationMode.LowQuality => _lowQuality,
            _ => throw new ArgumentOutOfRangeException($"The value {value} is not supported here. How the heck did you even got here?")
        };
    }
}