using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using JetBrains.Annotations;
using OsuPlayer.Data;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    private Slider ProgressSlider => this.FindControl<Slider>("SongProgressSlider");
    
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            Core.Instance.SetMainWindow(this);
            Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
            ProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed, RoutingStrategies.Tunnel);
            ProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased, RoutingStrategies.Tunnel);
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Core.Instance.Engine.ChannelPosition = Core.Instance.MainWindow.ViewModel!.PlayerControl.SongTime;
        Core.Instance.Player.Play();
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Core.Instance.Player.Pause();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Core.Instance.Config.SaveConfig();
        base.OnClosing(e);
    }
}