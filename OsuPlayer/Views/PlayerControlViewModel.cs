using System;
using System.Reactive.Disposables;
using Avalonia.Media.Imaging;
using OsuPlayer.Extensions;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlayerControlViewModel : BaseViewModel
{
    private MapEntry? _currentSong;
    private Bitmap? _currentSongImage;
    private string _currentSongLength = "00:00";

    private string _currentSongTime = "00:00";

    private bool _isPlaying;
    private bool _isRepeating;

    private bool _isShuffle;

    private double _playbackSpeed;

    private double _songLength;
    private double _songTime;
    public BassEngine BassEngine;

    public Player Player;

    public PlayerControlViewModel(Player player, BassEngine bassEngine)
    {
        Player = player;
        BassEngine = bassEngine;
        BassEngine.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "ChannelPosition")
            {
                _songTime = BassEngine.ChannelPosition;
                this.RaisePropertyChanged(nameof(SongTime));
                this.RaisePropertyChanged(nameof(CurrentSongTime));
            }

            if (args.PropertyName == "ChannelLength")
            {
                _songLength = BassEngine.ChannelLength;
                this.RaisePropertyChanged(nameof(SongLength));
                this.RaisePropertyChanged(nameof(CurrentSongLength));
            }
        };

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double Volume
    {
        get => Player.Volume;
        set => Player.Volume = value;
    }

    public bool IsShuffle
    {
        get => _isShuffle;
        set => this.RaiseAndSetIfChanged(ref _isShuffle, value);
    }

    public double PlaybackSpeed
    {
        get => _playbackSpeed;
        set
        {
            Player.SetPlaybackSpeed(value);
            this.RaiseAndSetIfChanged(ref _playbackSpeed, value);
            this.RaisePropertyChanged(nameof(CurrentSongLength));
        }
    }

    public double SongTime
    {
        get => _songTime;
        set => this.RaiseAndSetIfChanged(ref _songTime, value);
    }

    public string CurrentSongTime
    {
        get
        {
            _currentSongTime = TimeSpan.FromSeconds(_songTime * (1 - PlaybackSpeed)).FormatTime();
            return _currentSongTime;
        }
        set => this.RaiseAndSetIfChanged(ref _currentSongTime, value);
    }

    public double SongLength
    {
        get => _songLength;
        set => this.RaiseAndSetIfChanged(ref _songLength, value);
    }

    public string CurrentSongLength
    {
        get
        {
            _currentSongLength = TimeSpan.FromSeconds(_songLength * (1 - PlaybackSpeed)).FormatTime();
            return _currentSongLength;
        }
        set => this.RaiseAndSetIfChanged(ref _currentSongLength, value);
    }

    public bool IsPlaying
    {
        get => _isPlaying;
        set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

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