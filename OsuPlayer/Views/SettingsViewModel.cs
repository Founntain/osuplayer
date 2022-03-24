using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Network.Online;
using OsuPlayer.UI_Extensions;
using OsuPlayer.ViewModels;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public class SettingsViewModel : BaseViewModel
{
    private string _osuLocation;
    private StartupSong _selectedStartupSong = new Config().Read().StartupSong;
    private WindowTransparencyLevel _selectedTransparencyLevel = new Config().Read().TransparencyLevelHint;
    private string _settingsSearchQ;

    public SettingsViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public User? CurrentUser => ProfileManager.User;

    public string OsuLocation
    {
        get => $"osu! location: {_osuLocation}";
        set => this.RaiseAndSetIfChanged(ref _osuLocation, value);
    }

    public IEnumerable<WindowTransparencyLevel> WindowTransparencyLevels => Enum.GetValues<WindowTransparencyLevel>();

    public WindowTransparencyLevel SelectedTransparencyLevel
    {
        get => _selectedTransparencyLevel;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTransparencyLevel, value);

            if (Core.Instance.MainWindow == null) return;

            Core.Instance.MainWindow.TransparencyLevelHint = value;
            using var config = new Config();
            config.Read().TransparencyLevelHint = value;
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
            config.Read().StartupSong = value;
        }
    }

    public string SettingsSearchQ
    {
        get => _settingsSearchQ;
        set
        {
            var searchQs = value.Split(' ');

            foreach (var category in SettingsCategories)
            {
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
                            foreach (var setting in settings)
                            {
                                setting.IsVisible = true;
                            }

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
            }

            this.RaiseAndSetIfChanged(ref _settingsSearchQ, value);
        }
    }

    public Avalonia.Controls.Controls SettingsCategories { get; set; }

    public async Task ImportSongsClick()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select your osu!.db file",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Extensions = new List<string> {"db"}
                }
            }
        };

        var result = await dialog.ShowAsync(Core.Instance.MainWindow);

        if (result == default)
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow, "Did you even selected a file?!");
            return;
        }

        var path = result.FirstOrDefault();

        if (Path.GetFileName(path) != "osu!.db")
        {
            await MessageBox.ShowDialogAsync(Core.Instance.MainWindow,
                "You had one job! Just one. Select your osu!.db! Not anything else!");
            return;
        }

        var osuFolder = Path.GetDirectoryName(path);

        using (var config = new Config())
        {
            (await config.ReadAsync()).OsuPath = osuFolder!;
            OsuLocation = osuFolder!;
        }

        await Core.Instance.Player.ImportSongs();
    }

    public async Task Login()
    {
        var loginWindow = new LoginWindow
        {
            ViewModel = new LoginWindowViewModel()
        };

        await loginWindow.ShowDialog(Core.Instance.MainWindow);
    }
}