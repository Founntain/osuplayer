using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using OsuPlayer.Modules.IO;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class SettingsViewModel : BaseViewModel, IActivatableViewModel
{
    private string _osuLocation;
    private WindowTransparencyLevel _selectedTransparencyLevel;

    public SettingsViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });

        SelectedTransparencyLevel = Core.Instance.Config.TransparencyLevelHint;
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
            Core.Instance.Config.TransparencyLevelHint = value;
            
            Core.Instance.Config.SaveConfig();
        }
    }

    public ViewModelActivator Activator { get; }

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

        Core.Instance.Config.OsuPath = osuFolder!;
        OsuLocation = osuFolder!;
        Core.Instance.Player.ImportSongs();

        Core.Instance.Config.SaveConfig();
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