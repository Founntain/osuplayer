using System;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.Modules.IO;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
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
        var config = Config.GetConfigInstance();

        ViewModel!.OsuLocation = config.OsuPath;
    }
}