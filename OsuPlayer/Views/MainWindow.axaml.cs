using System;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class MainWindow : ReactiveWindow<MainWindowBaseViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            Core.Init(this);
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        //throw new NotImplementedException();
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        //throw new NotImplementedException();
    }
}