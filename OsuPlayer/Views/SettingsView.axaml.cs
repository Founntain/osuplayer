using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Controls;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class SettingsView : ReactivePlayerControl<SettingsViewModel>
{
    private MainWindow _mainWindow;

    public SettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
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
        using var config = new Config();
        ViewModel!.OsuLocation = config.Read().OsuPath!;
    }

    public async void ImportSongsClick(object? sender, RoutedEventArgs routedEventArgs)
    {
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

        using (var config = new Config())
        {
            (await config.ReadAsync()).OsuPath = osuFolder!;
            ViewModel.OsuLocation = osuFolder!;
        }

        await Task.Run(ViewModel.Player.ImportSongsAsync);
    }

    public async void ImportCollectionsClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        await Task.Run(ViewModel.Player.ImportCollectionsAsync);
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
        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.EqualizerView;
    }

    private void ReportBug_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/Founntain/osuplayer/issues/new/choose");
    }

    private void JoinDiscord_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://discord.gg/RJQSc5B");
    }

    private void ContactUs_OnClick(object? sender, RoutedEventArgs e)
    {
        GeneralExtensions.OpenUrl("https://github.com/Founntain/osuplayer#contact");
    }
}