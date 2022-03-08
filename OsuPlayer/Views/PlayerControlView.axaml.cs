using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using OsuPlayer.ViewModels;

namespace OsuPlayer.Views;

public partial class PlayerControlView : ReactiveUserControl<PlayerControlViewModel>
{
    public PlayerControlView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Volume_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void PlaybackSpeedBtn_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void FavoriteBtn_OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void BlacklistBtn_OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void NavigationSettingsBtn_OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        Core.Instance.MainWindow.ViewModel!.MainView = Core.Instance.MainWindow.ViewModel.SettingsView;
    }

    private void SongControl(object? sender, PointerReleasedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "Repeat":
                Core.Instance.Player.Repeat = !Core.Instance.Player.Repeat;
                break;
            case "Previous":
                Core.Instance.Player.PreviousSong();
                break;
            case "PlayPause":
                Core.Instance.Player.PlayPause();
                break;
            case "Next":
                Core.Instance.Player.NextSong();
                break;
            case "Shuffle":
                Core.Instance.Player.Shuffle = !Core.Instance.Player.Shuffle;
                break;
        }
    }

    private void SongProgressSlider_PreviewMouseLeftButtonUp(object? sender, PointerPressedEventArgs e)
    {
        //throw new System.NotImplementedException();
    }

    private void SongProgressSlider_PreviewMouseLeftButtonDown(object? sender, PointerReleasedEventArgs e)
    {
        //throw new System.NotImplementedException();
    }
}