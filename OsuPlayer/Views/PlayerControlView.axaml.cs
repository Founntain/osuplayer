using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlayerControlView : ReactivePlayerControl<PlayerControlViewModel>
{
    private MainWindow _mainWindow;

    public PlayerControlView()
    {
        InitializeComponent();
    }

    private Slider ProgressSlider => this.FindControl<Slider>("SongProgressSlider");

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;

            ProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed,
                RoutingStrategies.Tunnel);
            ProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased,
                RoutingStrategies.Tunnel);
        });
        AvaloniaXamlLoader.Load(this);
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.BassEngine.SetChannelPosition(ViewModel!.SongTime);
        ViewModel.Player.Play();
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewModel.Player.Pause();
    }

    private void Volume_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.Player.Mute();
    }

    private void PlaybackSpeedBtn_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void Settings_OnClick(object? sender, RoutedEventArgs e)
    {
        _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.SettingsView;
    }

    private void Blacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        // throw new NotImplementedException();
    }

    private void Favorite_OnClick(object? sender, RoutedEventArgs e)
    {
        // throw new NotImplementedException();
    }

    private void SongControl(object? sender, RoutedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "Repeat":
                ViewModel.Player.Repeat = !ViewModel.Player.Repeat;
                break;
            case "Previous":
                ViewModel.Player.PreviousSong();
                break;
            case "PlayPause":
                ViewModel.Player.PlayPause();
                break;
            case "Next":
                ViewModel.Player.NextSong();
                break;
            case "Shuffle":
                ViewModel.Player.Shuffle = !ViewModel.Player.Shuffle;
                break;
        }
    }

    private void Volume_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Player.Mute();
    }

    private void PlaybackSpeed_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel!.PlaybackSpeed = 0;
    }
}