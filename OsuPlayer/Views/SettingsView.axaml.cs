using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
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
                this.FindControl<WrapPanel>("SettingsGrid").Children;
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

        await ViewModel.Player.ImportSongs();
    }

    public async void LoginClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        var loginWindow = new LoginWindow
        {
            ViewModel = new LoginWindowViewModel()
        };

        await loginWindow.ShowDialog(_mainWindow);
    }
}