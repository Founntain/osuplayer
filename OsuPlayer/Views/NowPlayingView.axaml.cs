using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
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
    private readonly Stopwatch _rotationClock = new();
    private bool _wasPlayingBeforeSeek;
    private bool _isImmersiveTransitioning;
    private bool _isParticleDrawerVisible;
    private INotifyPropertyChanged? _observedViewModel;
    private CancellationTokenSource? _lyricAnimationCancellation;

    public NowPlayingView()
    {
        InitializeComponent();

        SongProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed,
            RoutingStrategies.Tunnel);
        SongProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased,
            RoutingStrategies.Tunnel);

        _rotationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _rotationTimer.Tick += RotationTimer_OnTick;

        AttachedToVisualTree += (_, _) =>
        {
            _rotationClock.Restart();
            _rotationTimer.Start();
        };
        AttachedToVisualTree += (_, _) => ObserveViewModel();
        DetachedFromVisualTree += (_, _) =>
        {
            _rotationTimer.Stop();
            _rotationClock.Reset();
            StopObservingViewModel();
        };
        DataContextChanged += (_, _) => ObserveViewModel();
    }

    private void RotationTimer_OnTick(object? sender, EventArgs e)
    {
        var elapsedSeconds = _rotationClock.Elapsed.TotalSeconds;
        _rotationClock.Restart();
        if (ViewModel?.IsPlaying != true) return;

        if (CoverArtwork.RenderTransform is not RotateTransform coverRotation ||
            PlaceholderArtwork.RenderTransform is not RotateTransform placeholderRotation)
            return;

        var nextAngle = (coverRotation.Angle + (elapsedSeconds * 4.7)) % 360;
        coverRotation.Angle = nextAngle;
        placeholderRotation.Angle = nextAngle;
    }

    private void NowPlayingRoot_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetPosition(NowPlayingRoot);
        var nearRightEdge = NowPlayingRoot.Bounds.Width - point.X <= 88;
        SetParticleDrawerVisible(nearRightEdge || ParticleDensityDrawer.IsPointerOver);
    }

    private void NowPlayingRoot_OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (!ParticleDensityDrawer.IsPointerOver)
            SetParticleDrawerVisible(false);
    }

    private void SetParticleDrawerVisible(bool visible)
    {
        if (_isParticleDrawerVisible == visible) return;

        _isParticleDrawerVisible = visible;
        ParticleDensityDrawer.Opacity = visible ? 1 : 0;
        ParticleDensityDrawer.RenderTransform = TransformOperations.Parse(
            visible ? "translate(0px, 0px)" : "translate(86px, 0px)");
    }

    private void ObserveViewModel()
    {
        if (ReferenceEquals(_observedViewModel, DataContext)) return;

        StopObservingViewModel();
        _observedViewModel = DataContext as INotifyPropertyChanged;
        if (_observedViewModel != null)
            _observedViewModel.PropertyChanged += ViewModel_OnPropertyChanged;
    }

    private void StopObservingViewModel()
    {
        if (_observedViewModel != null)
            _observedViewModel.PropertyChanged -= ViewModel_OnPropertyChanged;
        _observedViewModel = null;
        _lyricAnimationCancellation?.Cancel();
        _lyricAnimationCancellation?.Dispose();
        _lyricAnimationCancellation = null;
    }

    private async void ViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(NowPlayingViewModel.CurrentLyricIndex)) return;

        _lyricAnimationCancellation?.Cancel();
        _lyricAnimationCancellation?.Dispose();
        _lyricAnimationCancellation = new CancellationTokenSource();
        var cancellationToken = _lyricAnimationCancellation.Token;

        LyricsCarousel.Opacity = 0.32;
        LyricsCarousel.RenderTransform = TransformOperations.Parse("translate(0px, 7px)");
        try
        {
            await Task.Delay(55, cancellationToken);
            LyricsCarousel.Opacity = 1;
            LyricsCarousel.RenderTransform = TransformOperations.Parse("translate(0px, 0px)");
        }
        catch (OperationCanceledException)
        {
            // A seek or track change has superseded this transition.
        }
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
