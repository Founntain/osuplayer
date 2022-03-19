using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO;
using OsuPlayer.IO.Storage;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class SettingsViewModel : BaseViewModel, IActivatableViewModel
{
    private string _osuLocation;
    private WindowTransparencyLevel _selectedTransparencyLevel = new Config().Read().TransparencyLevelHint;
    private StartupSong _selectedStartupSong = new Config().Read().StartupSong;

    public SettingsViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

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

    public ViewModelActivator Activator { get; }

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

        using var config = new Config();
        (await config.ReadAsync()).OsuPath = osuFolder!;
        OsuLocation = osuFolder!;
        await Core.Instance.Player.ImportSongs();
    }

    public async Task Login()
    {
        var loginWindow = new LoginWindow()
        {
            ViewModel = new LoginWindowViewModel()
        };

        await loginWindow.ShowDialog(Core.Instance.MainWindow);
    }
}