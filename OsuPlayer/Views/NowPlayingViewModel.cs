using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Modules;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Views;

public class NowPlayingViewModel : BaseViewModel
{
    private readonly Bindable<bool> _isPlaying = new();
    private readonly Bindable<double> _songLength = new();
    private readonly Bindable<double> _songTime = new();
    private readonly Bindable<IMapEntry?> _currentSong = new();

    private Bitmap? _backgroundImage;
    private Bitmap? _currentSongImage;
    private bool _isImmersive;
    private int _artworkRevision;

    private SolidColorBrush _accentBrush = new(Color.FromRgb(104, 248, 212));
    private SolidColorBrush _accentSoftBrush = new(Color.FromArgb(66, 104, 248, 212));
    private SolidColorBrush _accentGlowBrush = new(Color.FromArgb(30, 104, 248, 212));
    private SolidColorBrush _accentForegroundBrush = new(Color.FromRgb(7, 21, 18));

    public IPlayer Player { get; }

    public string TitleText => _currentSong.Value?.GetTitle() ?? "Nothing playing";
    public string ArtistText => _currentSong.Value?.GetArtist() ?? "Choose a song to begin";
    public bool IsPlaying => _isPlaying.Value;

    public bool IsImmersive
    {
        get => _isImmersive;
        set
        {
            this.RaiseAndSetIfChanged(ref _isImmersive, value);
            this.RaisePropertyChanged(nameof(IsNotImmersive));
            this.RaisePropertyChanged(nameof(PageMargin));
            this.RaisePropertyChanged(nameof(ControlsWidth));
            this.RaisePropertyChanged(nameof(ArtworkOuterSize));
            this.RaisePropertyChanged(nameof(ArtworkMiddleSize));
            this.RaisePropertyChanged(nameof(ArtworkSize));
            this.RaisePropertyChanged(nameof(CenterHoleSize));
        }
    }

    public bool IsNotImmersive => !IsImmersive;
    public Thickness PageMargin => IsImmersive ? new Thickness(28, 22, 28, 20) : new Thickness(48, 28, 48, 36);
    public double ControlsWidth => IsImmersive ? 860 : 620;
    public double ArtworkOuterSize => IsImmersive ? 436 : 356;
    public double ArtworkMiddleSize => IsImmersive ? 416 : 336;
    public double ArtworkSize => IsImmersive ? 400 : 320;
    public double CenterHoleSize => IsImmersive ? 76 : 68;

    public SolidColorBrush AccentBrush
    {
        get => _accentBrush;
        private set => this.RaiseAndSetIfChanged(ref _accentBrush, value);
    }

    public SolidColorBrush AccentSoftBrush
    {
        get => _accentSoftBrush;
        private set => this.RaiseAndSetIfChanged(ref _accentSoftBrush, value);
    }

    public SolidColorBrush AccentGlowBrush
    {
        get => _accentGlowBrush;
        private set => this.RaiseAndSetIfChanged(ref _accentGlowBrush, value);
    }

    public SolidColorBrush AccentForegroundBrush
    {
        get => _accentForegroundBrush;
        private set => this.RaiseAndSetIfChanged(ref _accentForegroundBrush, value);
    }

    public double SongTime
    {
        get => _songTime.Value;
        set => _songTime.Value = value;
    }

    public double SongLength => _songLength.Value;

    public string CurrentSongTime => TimeSpan.FromSeconds(_songTime.Value).FormatTime();
    public string CurrentSongLength => TimeSpan.FromSeconds(_songLength.Value).FormatTime();

    public Bitmap? CurrentSongImage
    {
        get => _currentSongImage;
        private set
        {
            _currentSongImage?.Dispose();
            this.RaiseAndSetIfChanged(ref _currentSongImage, value);
        }
    }

    public Bitmap? BackgroundImage
    {
        get => _backgroundImage;
        private set
        {
            _backgroundImage?.Dispose();
            this.RaiseAndSetIfChanged(ref _backgroundImage, value);
        }
    }

    public NowPlayingViewModel(IPlayer player, IAudioEngine audioEngine)
    {
        Player = player;

        _currentSong.BindTo(Player.CurrentSong);
        _currentSong.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(TitleText));
            this.RaisePropertyChanged(nameof(ArtistText));
        });

        _isPlaying.BindTo(Player.IsPlaying);
        _isPlaying.BindValueChanged(_ => this.RaisePropertyChanged(nameof(IsPlaying)));

        _songTime.BindTo(audioEngine.ChannelPosition);
        _songTime.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(SongTime));
            this.RaisePropertyChanged(nameof(CurrentSongTime));
        });

        _songLength.BindTo(audioEngine.ChannelLength);
        _songLength.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(SongLength));
            this.RaisePropertyChanged(nameof(CurrentSongLength));
        });

        Player.CurrentSongImage.BindValueChanged(artwork =>
        {
            var revision = Interlocked.Increment(ref _artworkRevision);
            Dispatcher.UIThread.Post(() => UpdateArtwork(artwork.NewValue));
            _ = UpdateAccentColorAsync(artwork.NewValue, revision);
        }, true);

        Activator = new ViewModelActivator();
    }

    private void UpdateArtwork(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            CurrentSongImage = null;
            BackgroundImage = null;
            return;
        }

        CurrentSongImage = new Bitmap(path);
        BackgroundImage = BitmapExtensions.BlurBitmap(path, blurRadius: 30, opacity: 0.9f, quality: 65);
    }

    private async Task UpdateAccentColorAsync(string? path, int revision)
    {
        var color = string.IsNullOrWhiteSpace(path) || !File.Exists(path)
            ? Color.FromRgb(104, 248, 212)
            : await Task.Run(() => BitmapExtensions.GetDominantColor(path));

        if (revision != _artworkRevision) return;

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (revision != _artworkRevision) return;

            AccentBrush = new SolidColorBrush(color);
            AccentSoftBrush = new SolidColorBrush(Color.FromArgb(66, color.R, color.G, color.B));
            AccentGlowBrush = new SolidColorBrush(Color.FromArgb(30, color.R, color.G, color.B));

            var luminance = (0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B);
            AccentForegroundBrush = new SolidColorBrush(luminance > 150
                ? Color.FromRgb(7, 15, 18)
                : Colors.White);
        });
    }
}
