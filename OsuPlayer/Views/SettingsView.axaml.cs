using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using OsuPlayer.IO;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void SettingsView_OnInitialized(object? sender, EventArgs e)
    {
        var viewmodel = (SettingsViewModel) DataContext!;

        var config = Config.GetConfigInstance();

        viewmodel.OsuLocation = config.OsuPath;
    }
}