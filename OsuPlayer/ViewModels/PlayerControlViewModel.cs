using System;
using System.Reactive.Disposables;
using OsuPlayer.Extensions;
using OsuPlayer.IO.DbReader;
using ReactiveUI;
using Avalonia.Media.Imaging;

namespace OsuPlayer.ViewModels;

public class PlayerControlViewModel : BaseViewModel, IActivatableViewModel
{
    private string _currentSongLength = "00:00";

    private string _currentSongTime = "00:00";

    private bool _isPlaying;

    private bool _isShuffle;

    private double _playbackSpeed;

    private double _songLength;

    private double _songTime;
    private bool _isRepeating;
    private MapEntry? _currentSong;
    private bool _isVolumeVisible;
    private bool _isSpeedVisible;
    private bool _volumePointerOver;
    private Bitmap? _currentSongImage;

    public PlayerControlViewModel()
    {
        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double Volume
    {
        get => Core.Instance.Player.Volume;
        set { Core.Instance.Player.Volume = value; }
    }

    public bool IsShuffle
    {
        get => _isShuffle;
        set => this.RaiseAndSetIfChanged(ref _isShuffle, value);
    }

    public double PlaybackSpeed
    {
        get => _playbackSpeed;
        set => this.RaiseAndSetIfChanged(ref _playbackSpeed, value);
    }

    public double SongTime
    {
        get => _songTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _songTime, value);
            CurrentSongTime = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    public string CurrentSongTime
    {
        get => _currentSongTime;
        set => this.RaiseAndSetIfChanged(ref _currentSongTime, value);
    }

    public double SongLength
    {
        get => _songLength;
        set
        {
            this.RaiseAndSetIfChanged(ref _songLength, value);
            CurrentSongLength = TimeSpan.FromSeconds(value).FormatTime();
        }
    }

    public string CurrentSongLength
    {
        get => _currentSongLength;
        set => this.RaiseAndSetIfChanged(ref _currentSongLength, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

    public ViewModelActivator Activator { get; }

    public bool IsRepeating
    {
        get => _isRepeating;
        set => this.RaiseAndSetIfChanged(ref _isRepeating, value);
    }

    public MapEntry? CurrentSong
    {
        get => _currentSong;
        set
        {
            _currentSong = value;
            this.RaisePropertyChanged(nameof(TitleText));
            this.RaisePropertyChanged(nameof(ArtistText));
            this.RaisePropertyChanged(nameof(SongText));
        }
    }

    public string TitleText => _currentSong?.Title ?? "No song is playing";

    public string ArtistText => _currentSong?.Artist ?? "please select from song list";

    public string SongText => $"{ArtistText} - {TitleText}";

    public bool IsVolumeVisible
    {
        get => _isVolumeVisible;
        set => this.RaiseAndSetIfChanged(ref _isVolumeVisible, value);
    }

    public bool IsSpeedVisible
    {
        get => _isSpeedVisible;
        set => this.RaiseAndSetIfChanged(ref _isSpeedVisible, value);
    }

    public bool VolumePointerOver { get; set; }
    public bool VolumePopupPointerOver { get; set; }
    public bool SpeedPointerOver { get; set; }
    public bool SpeedPopupPointerOver { get; set; }

    public Bitmap? CurrentSongImage
    {
        get => _currentSongImage;
        set
        {
            _currentSongImage?.Dispose();
            this.RaiseAndSetIfChanged(ref _currentSongImage, value);
        }
    }
}