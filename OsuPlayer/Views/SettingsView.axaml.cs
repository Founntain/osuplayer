using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO.Storage.Config;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
        Core.Instance.MainWindow.ViewModel!.SettingsView.SettingsCategories =
            this.FindControl<WrapPanel>("SettingsGrid").Children;
    }

    private void SettingsView_OnInitialized(object? sender, EventArgs e)
    {
        using var config = new Config();
        ViewModel!.OsuLocation = config.Read().OsuPath!;
    }
}