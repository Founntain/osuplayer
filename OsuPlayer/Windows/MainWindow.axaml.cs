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
            Core.Instance.SetupCore(this);
            
            
            Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.HomeView;
            ProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed,
                RoutingStrategies.Tunnel);
            ProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased,
                RoutingStrategies.Tunnel);
            ProgressSlider.AddHandler(KeyDownEvent, KeyDownHandler, RoutingStrategies.Tunnel);
            ProgressSlider.AddHandler(KeyUpEvent, KeyUpHandler, RoutingStrategies.Tunnel);
        });
        
        AvaloniaXamlLoader.Load(this);
    }

    private void KeyDownHandler(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Right:
                //Core.Instance.Player.Pause();
                break;
            case Key.Left:
                //Core.Instance.Player.Pause();
                break;
            case Key.Space:
                Core.Instance.Player.PlayPause();
                break;
        }
        e.Handled = true;
    }
    private void KeyUpHandler(object? sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Right:
                //Core.Instance.Player.Play();
                break;
            case Key.Left:
                //Core.Instance.Player.Play();
                break;
            case Key.Space:
                break;
        }
        e.Handled = true;
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