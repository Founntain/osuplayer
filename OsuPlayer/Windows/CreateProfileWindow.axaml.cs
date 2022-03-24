using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Windows;

public partial class CreateProfileWindow : ReactiveWindow<CreateProfileWindowViewModel>
{
    public CreateProfileWindow()
    {
        InitializeComponent();
        
        using var config = new Config();
        TransparencyLevelHint = config.Read().TransparencyLevelHint;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}