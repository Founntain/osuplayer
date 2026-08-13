using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Threading;
using Nein.Base;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Windows;

namespace OsuPlayer.Views;

public partial class NowPlayingView : ReactiveControl<NowPlayingViewModel>
{
    private readonly DispatcherTimer _rotationTimer;
    private bool _wasPlayingBeforeSeek;
    private bool _isImmersiveTransitioning;

    public NowPlayingView()
    {
        InitializeComponent();

        SongProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed,
            RoutingStrategies.Tunnel);
        SongProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased,
            RoutingStrategies.Tunnel);

        _rotationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(30)
        };
        _rotationTimer.Tick += RotationTimer_OnTick;

        AttachedToVisualTree += (_, _) => _rotationTimer.Start();
        DetachedFromVisualTree += (_, _) => _rotationTimer.Stop();
    }

    private void RotationTimer_OnTick(object? sender, EventArgs e)
    {
        if (ViewModel?.IsPlaying != true) return;

        if (CoverArtwork.RenderTransform is not RotateTransform coverRotation ||
            PlaceholderArtwork.RenderTransform is not RotateTransform placeholderRotation)
            return;

        var nextAngle = (coverRotation.Angle + 0.14) % 360;
        coverRotation.Angle = nextAngle;
        placeholderRotation.Angle = nextAngle;
    }

    private void PlayPause_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel?.Player.PlayPause();
    }

    private async void EnterImmersive_OnClick(object? sender, RoutedEventArgs e)
    {
        await SetImmersiveModeAsync(true);
    }

    private async void ExitImmersive_OnClick(object? sender, RoutedEventArgs e)
    {
        await SetImmersiveModeAsync(false);
    }

    private async Task SetImmersiveModeAsync(bool enabled)
    {
        if (_isImmersiveTransitioning || TopLevel.GetTopLevel(this) is not FluentAppWindow window)
            return;

        _isImmersiveTransitioning = true;
        try
        {
            TransitionCurtain.Opacity = 0.62;
            ImmersiveStage.Opacity = 0.28;
            ImmersiveStage.RenderTransform = TransformOperations.Parse("scale(0.975)");

            await Task.Delay(105);
            window.SetImmersiveMode(enabled);
            await Task.Delay(55);

            ImmersiveStage.RenderTransform = TransformOperations.Parse("scale(1)");
            ImmersiveStage.Opacity = 1;
            TransitionCurtain.Opacity = 0;
            await Task.Delay(280);
        }
        finally
        {
            _isImmersiveTransitioning = false;
        }
    }

    private async void Previous_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
            await ViewModel.Player.NextSong(PlayDirection.Backwards);
    }

    private async void Next_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel != null)
            await ViewModel.Player.NextSong(PlayDirection.Forward);
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel == null) return;

        _wasPlayingBeforeSeek = ViewModel.IsPlaying;
        if (_wasPlayingBeforeSeek)
            ViewModel.Player.Pause();
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (ViewModel == null || !_wasPlayingBeforeSeek) return;

        ViewModel.Player.Play();
        _wasPlayingBeforeSeek = false;
    }
}
