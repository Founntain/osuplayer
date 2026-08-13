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

    public IPlayer Player { get; }

    public string TitleText => _currentSong.Value?.GetTitle() ?? "Nothing playing";
    public string ArtistText => _currentSong.Value?.GetArtist() ?? "Choose a song to begin";
    public bool IsPlaying => _isPlaying.Value;

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
            Dispatcher.UIThread.Post(() => UpdateArtwork(artwork.NewValue));
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
}
