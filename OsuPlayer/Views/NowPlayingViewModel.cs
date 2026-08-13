using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Modules;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Services;
using ReactiveUI;

namespace OsuPlayer.Views;

public class NowPlayingViewModel : BaseViewModel
{
    private readonly Bindable<bool> _isPlaying = new();
    private readonly Bindable<double> _songLength = new();
    private readonly Bindable<double> _songTime = new();
    private readonly Bindable<IMapEntry?> _currentSong = new();
    private readonly ILyricsService _lyricsService;

    private Bitmap? _backgroundImage;
    private Bitmap? _currentSongImage;
    private bool _isImmersive;
    private int _artworkRevision;
    private int _lyricsRevision;
    private int _currentLyricIndex = -2;
    private CancellationTokenSource? _lyricsCancellation;
    private IReadOnlyList<LrcLine> _lyrics = Array.Empty<LrcLine>();
    private string _lyricsStatusText = "Play a song to see lyrics";

    private SolidColorBrush _accentBrush = new(Color.FromRgb(104, 248, 212));
    private SolidColorBrush _accentSoftBrush = new(Color.FromArgb(66, 104, 248, 212));
    private SolidColorBrush _accentForegroundBrush = new(Color.FromRgb(7, 21, 18));

    public IPlayer Player { get; }
    public IAudioEngine AudioEngine { get; }

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
            this.RaisePropertyChanged(nameof(ArtworkSize));
            this.RaisePropertyChanged(nameof(SpectrumSize));
            this.RaisePropertyChanged(nameof(SpectrumMargin));
            this.RaisePropertyChanged(nameof(ArtworkTextWidth));
            this.RaisePropertyChanged(nameof(CenterHoleSize));
            this.RaisePropertyChanged(nameof(LyricsPanelWidth));
            this.RaisePropertyChanged(nameof(CurrentLyricFontSize));
            this.RaisePropertyChanged(nameof(ContextLyricFontSize));
        }
    }

    public bool IsNotImmersive => !IsImmersive;
    public Thickness PageMargin => IsImmersive ? new Thickness(28, 22, 28, 20) : new Thickness(48, 28, 48, 36);
    public double ControlsWidth => IsImmersive ? 860 : 620;
    public double ArtworkSize => IsImmersive ? 370 : 320;
    public double SpectrumSize => IsImmersive ? 520 : 450;
    public Thickness SpectrumMargin => IsImmersive ? new Thickness(0, -58, 0, 0) : new Thickness(0, -42, 0, 0);
    public double ArtworkTextWidth => IsImmersive ? 280 : 238;
    public double CenterHoleSize => IsImmersive ? 76 : 68;
    public double LyricsPanelWidth => IsImmersive ? 720 : 560;
    public double CurrentLyricFontSize => IsImmersive ? 21 : 18;
    public double ContextLyricFontSize => IsImmersive ? 14 : 12;

    public int CurrentLyricIndex => _currentLyricIndex;
    public bool HasSynchronizedLyrics => _lyrics.Count > 0;
    public bool IsLyricsStatusVisible => !HasSynchronizedLyrics;
    public string LyricsStatusText => _lyricsStatusText;
    public string PreviousLyricText => _currentLyricIndex > 0 ? _lyrics[_currentLyricIndex - 1].Text : string.Empty;
    public string CurrentLyricText => _currentLyricIndex >= 0 && _currentLyricIndex < _lyrics.Count
        ? _lyrics[_currentLyricIndex].Text
        : string.Empty;
    public string CurrentLyricTranslation => _currentLyricIndex >= 0 && _currentLyricIndex < _lyrics.Count
        ? _lyrics[_currentLyricIndex].Translation
        : string.Empty;
    public bool HasCurrentLyricTranslation => !string.IsNullOrWhiteSpace(CurrentLyricTranslation);
    public string NextLyricText
    {
        get
        {
            var nextIndex = _currentLyricIndex + 1;
            return nextIndex >= 0 && nextIndex < _lyrics.Count ? _lyrics[nextIndex].Text : string.Empty;
        }
    }

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

    public NowPlayingViewModel(IPlayer player, IAudioEngine audioEngine, ILyricsService lyricsService)
    {
        Player = player;
        AudioEngine = audioEngine;
        _lyricsService = lyricsService;

        _currentSong.BindTo(Player.CurrentSong);
        _currentSong.BindValueChanged(change =>
        {
            this.RaisePropertyChanged(nameof(TitleText));
            this.RaisePropertyChanged(nameof(ArtistText));
            _ = LoadLyricsAsync(change.NewValue);
        }, true);

        _isPlaying.BindTo(Player.IsPlaying);
        _isPlaying.BindValueChanged(_ => this.RaisePropertyChanged(nameof(IsPlaying)));

        _songTime.BindTo(audioEngine.ChannelPosition);
        _songTime.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(SongTime));
            this.RaisePropertyChanged(nameof(CurrentSongTime));
            UpdateCurrentLyric(_songTime.Value);
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

    private async Task LoadLyricsAsync(IMapEntry? song)
    {
        var revision = Interlocked.Increment(ref _lyricsRevision);
        _lyricsCancellation?.Cancel();
        _lyricsCancellation?.Dispose();
        _lyricsCancellation = new CancellationTokenSource();
        var cancellationToken = _lyricsCancellation.Token;

        SetLyricsState(Array.Empty<LrcLine>(), song == null
            ? "Play a song to see lyrics"
            : "Finding synchronized lyrics…");
        if (song == null) return;

        try
        {
            var result = await _lyricsService.GetLyricsAsync(song.GetTitle(), song.GetArtist(), cancellationToken);
            if (revision != _lyricsRevision || cancellationToken.IsCancellationRequested) return;

            var lines = result.Status == LyricsLookupStatus.Found
                ? LrcParser.Parse(result.Lyrics, result.TranslatedLyrics)
                : Array.Empty<LrcLine>();
            var status = result.Status switch
            {
                LyricsLookupStatus.Found when lines.Count == 0 => "Lyrics found, but they have no timestamps",
                LyricsLookupStatus.Instrumental => "Instrumental track · no lyrics",
                LyricsLookupStatus.NotFound => "No synchronized lyrics found",
                LyricsLookupStatus.Unavailable => "Lyrics unavailable · playback continues",
                _ => string.Empty
            };

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (revision != _lyricsRevision) return;
                SetLyricsState(lines, status);
                UpdateCurrentLyric(_songTime.Value, force: true);
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // A newer song has replaced this lookup.
        }
    }

    private void SetLyricsState(IReadOnlyList<LrcLine> lines, string status)
    {
        _lyrics = lines;
        _lyricsStatusText = status;
        _currentLyricIndex = -2;
        this.RaisePropertyChanged(nameof(HasSynchronizedLyrics));
        this.RaisePropertyChanged(nameof(IsLyricsStatusVisible));
        this.RaisePropertyChanged(nameof(LyricsStatusText));
        RaiseLyricLineProperties();
    }

    private void UpdateCurrentLyric(double seconds, bool force = false)
    {
        var index = LrcParser.FindLineIndex(_lyrics, TimeSpan.FromSeconds(Math.Max(0, seconds)));
        if (!force && index == _currentLyricIndex) return;

        _currentLyricIndex = index;
        RaiseLyricLineProperties();
    }

    private void RaiseLyricLineProperties()
    {
        this.RaisePropertyChanged(nameof(CurrentLyricIndex));
        this.RaisePropertyChanged(nameof(PreviousLyricText));
        this.RaisePropertyChanged(nameof(CurrentLyricText));
        this.RaisePropertyChanged(nameof(CurrentLyricTranslation));
        this.RaisePropertyChanged(nameof(HasCurrentLyricTranslation));
        this.RaisePropertyChanged(nameof(NextLyricText));
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
        BackgroundImage = BitmapExtensions.BlurBitmap(path, blurRadius: 10, opacity: 0.96f, quality: 75);
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
            var luminance = (0.2126 * color.R) + (0.7152 * color.G) + (0.0722 * color.B);
            AccentForegroundBrush = new SolidColorBrush(luminance > 150
                ? Color.FromRgb(7, 15, 18)
                : Colors.White);
        });
    }
}
