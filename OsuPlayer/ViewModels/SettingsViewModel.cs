using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Controls;
using OsuPlayer.IO;
using OsuPlayer.UI_Extensions;
using ReactiveUI;

namespace OsuPlayer.ViewModels;

public class SettingsViewModel : BaseViewModel, IActivatableViewModel
{
    private string _osuLocation;
    public ViewModelActivator Activator { get; }

    public SettingsViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() =>
            {

            }).DisposeWith(disposables);
        });
    }

    public string OsuLocation
    {
        get => $"osu! location: {_osuLocation}";
        set => this.RaiseAndSetIfChanged(ref _osuLocation, value);
    }

    public async Task ImportSongsClick()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select your osu!.db file",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter
                {
                    Extensions = new List<string> {"db"}
                }
            }
        };

        var result = await dialog.ShowAsync(Core.MainWindow);

        if (result == default)
        {
            await MessageBox.ShowDialogAsync(Core.MainWindow, "Did you even selected a file?!");
            return;
        }
        
        var path = result.FirstOrDefault();

        if (Path.GetFileName(path) != "osu!.db")
        {
            await MessageBox.ShowDialogAsync(Core.MainWindow, "You had one job! Just one. Select your osu!.db! Not anything else!");
            return;
        }
        
        var osuFolder = Path.GetDirectoryName(path);

        Core.Config.OsuPath = osuFolder!;
        OsuLocation = osuFolder!;
        
        Core.Config.SaveConfig();
    }
}