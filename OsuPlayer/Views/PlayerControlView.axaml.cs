using System;
using Avalonia;
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
        throw new System.NotImplementedException();
    }

    private void PlaybackSpeedBtn_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void FavoriteBtn_OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void BlacklistBtn_OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void NavigationSettingsBtn_OnMouseLeftButtonUp(object? sender, PointerReleasedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void SongControl(object? sender, PointerReleasedEventArgs e)
    {
        switch ((sender as Border)?.Name)
        {
            case "Repeat":
                break;
            case "Previous":
                break;
            case "PlayPause":
                break;
            case "Next":
                break;
            case "Shuffle":
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