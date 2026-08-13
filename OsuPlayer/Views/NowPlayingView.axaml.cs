using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Nein.Base;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Views;

public partial class NowPlayingView : ReactiveControl<NowPlayingViewModel>
{
    private readonly DispatcherTimer _rotationTimer;
    private bool _wasPlayingBeforeSeek;

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
