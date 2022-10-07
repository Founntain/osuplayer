using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Modules.Services;
using OsuPlayer.Network;
using OsuPlayer.Styles;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public class SettingsViewModel : BaseViewModel
{
    private readonly Bindable<bool> _blacklistSkip = new();
    private readonly Bindable<bool> _playlistEnableOnPlay = new();
    private readonly Bindable<SortingMode> _sortingMode = new();
    public readonly IPlayer Player;
    private string _osuLocation;
    private string _patchnotes;
    private KnownColors _selectedAccentColor;
    private KnownColors _selectedBackgroundColor;
    private string? _selectedFont;
    private FontWeights _selectedFontWeight;
    private ReleaseChannels _selectedReleaseChannel;
    private StartupSong _selectedStartupSong;
    private WindowTransparencyLevel _selectedTransparencyLevel;
    private string _settingsSearchQ;

    public MainWindow? MainWindow;

    public string Patchnotes
    {
        get => _patchnotes;
        set => this.RaiseAndSetIfChanged(ref _patchnotes, value);
    }

    public User? CurrentUser => ProfileManager.User;

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

    public IEnumerable<WindowTransparencyLevel> WindowTransparencyLevels => Enum.GetValues<WindowTransparencyLevel>();

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

            Application.Current!.Resources["SmallerFontWeight"] = config.Container.GetSmallerFont().ToFontWeight();
            Application.Current!.Resources["DefaultFontWeight"] = value.ToFontWeight();
            Application.Current!.Resources["BiggerFontWeight"] = config.Container.GetBiggerFont().ToFontWeight();

            Debug.WriteLine("SMALLER FONT: " + config.Container.GetSmallerFont().ToFontWeight().ToString());
            Debug.WriteLine("NORMAL FONT: " + value.ToFontWeight().ToString());
            Debug.WriteLine("BIGGER FONT: " + config.Container.GetBiggerFont().ToFontWeight().ToString());
        }
    }

    public IEnumerable<string> Fonts => FontManager.Current.GetInstalledFontFamilyNames();

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

    public WindowTransparencyLevel SelectedTransparencyLevel
    {
        get => _selectedTransparencyLevel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTransparencyLevel, value);

            if (MainWindow == null) return;

            MainWindow.TransparencyLevelHint = value;
            using var config = new Config();
            config.Container.TransparencyLevelHint = value;
        }
    }

    public KnownColors SelectedBackgroundColor
    {
        get => _selectedBackgroundColor;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBackgroundColor, value);

            if (MainWindow == null) return;

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

            foreach (var category in SettingsCategories)
                if (category is Grid settingsCat)
                {
                    var settingsPanel =
                        settingsCat.Children.FirstOrDefault(x => x.Name?.Contains(category.Name) ?? false);

                    if (settingsPanel is StackPanel stackPanel)
                    {
                        var settings = stackPanel.Children;

                        var categoryFound = searchQs.All(x =>
                            category.Name?.Contains(x, StringComparison.OrdinalIgnoreCase) ?? true);

                        if (categoryFound)
                        {
                            category.IsVisible = true;
                            foreach (var setting in settings) setting.IsVisible = true;

                            continue;
                        }

                        var foundAnySettings = false;
                        foreach (var setting in settings)
                        {
                            setting.IsVisible = searchQs.All(x =>
                                setting.Name?.Contains(x, StringComparison.OrdinalIgnoreCase) ?? false);
                            foundAnySettings = foundAnySettings || setting.IsVisible;
                        }

                        category.IsVisible = foundAnySettings;
                    }
                }

            this.RaiseAndSetIfChanged(ref _settingsSearchQ, value);
        }
    }

    public Avalonia.Controls.Controls SettingsCategories { get; set; }

    public ObservableCollection<AudioDevice> OutputDeviceComboboxItems { get; set; }

    public SettingsViewModel(IPlayer player, ISortProvider? sortProvider)
    {
        Player = player;

        var config = new Config();

        _selectedStartupSong = config.Container.StartupSong;
        _selectedTransparencyLevel = config.Container.TransparencyLevelHint;
        _selectedReleaseChannel = config.Container.ReleaseChannel;
        _selectedBackgroundColor = config.Container.BackgroundColor;
        _selectedAccentColor = config.Container.AccentColor;
        _selectedFontWeight = config.Container.DefaultFontWeight;
        _selectedFont = config.Container.Font ?? FontManager.Current.DefaultFontFamilyName;

        if (sortProvider != null)
        {
            _sortingMode.BindTo(sortProvider.SortingModeBindable);
            _sortingMode.BindValueChanged(d => this.RaisePropertyChanged(nameof(SelectedSortingMode)));
            _sortingMode.Value = config.Container.SortingMode;
        }

        _blacklistSkip.BindTo(Player.BlacklistSkip);
        _blacklistSkip.BindValueChanged(d => this.RaisePropertyChanged(nameof(BlacklistSkip)));

        _playlistEnableOnPlay.BindTo(Player.PlaylistEnableOnPlay);
        _blacklistSkip.BindValueChanged(d => this.RaisePropertyChanged(nameof(PlaylistEnableOnPlay)));

        Activator = new ViewModelActivator();
        this.WhenActivated(Block);
    }

    private async void Block(CompositeDisposable disposables)
    {
        Disposable.Create(() => { }).DisposeWith(disposables);

        var latestPatchNotes = await GitHubUpdater.GetLatestPatchNotes(_selectedReleaseChannel);

        if (string.IsNullOrWhiteSpace(latestPatchNotes)) return;

        Patchnotes = latestPatchNotes;
    }
}