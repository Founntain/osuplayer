using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Windows;

public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private Slider ProgressSlider => this.FindControl<Slider>("SongProgressSlider");

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            Core.Instance.SetMainWindow(this);
            
            
            Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
            ProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed,
                RoutingStrategies.Tunnel);
            ProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased,
                RoutingStrategies.Tunnel);
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