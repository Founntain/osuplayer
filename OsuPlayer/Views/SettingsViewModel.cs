using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Media;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
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
    public readonly Player Player;
    private string _osuLocation;
    private string _patchnotes;
    private KnownColors _selectedBackgroundColors;
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

    public IEnumerable<KnownColors> WindowBackgroundColors => Enum.GetValues<KnownColors>();

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

    public KnownColors SelectedBackgroundColors
    {
        get => _selectedBackgroundColors;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedBackgroundColors, value);

            if (MainWindow == null) return;

            MainWindow.ViewModel.PanelMaterial = new ExperimentalAcrylicMaterial
            {
                BackgroundSource = AcrylicBackgroundSource.Digger,
                TintColor = value.ToColor(),
                TintOpacity = 1,
                MaterialOpacity = 0.25
            };

            ColorSetter.SetColor(value.ToColor());
            MainWindow.ViewModel.RaisePropertyChanged(nameof(MainWindow.ViewModel.PanelMaterial));

            using var config = new Config();
            config.Container.BackgroundColor = value;
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

    public SettingsViewModel(Player player)
    {
        var config = new Config();

        _selectedStartupSong = config.Container.StartupSong;
        _selectedTransparencyLevel = config.Container.TransparencyLevelHint;
        _selectedReleaseChannel = config.Container.ReleaseChannel;
        _selectedBackgroundColors = config.Container.BackgroundColor ?? KnownColors.Black;

        Player = player;

        _sortingMode.BindTo(Player.SortingModeBindable);
        _sortingMode.BindValueChanged(d => this.RaisePropertyChanged(nameof(SelectedSortingMode)));

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