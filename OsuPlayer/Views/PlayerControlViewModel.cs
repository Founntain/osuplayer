using System;
using System.Reactive.Disposables;
using Avalonia.Media.Imaging;
using OsuPlayer.Extensions;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.Modules.Audio;
using OsuPlayer.ViewModels;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlayerControlViewModel : BaseViewModel
{
    private Bindable<MapEntry?> _currentSong = new();
    private Bitmap? _currentSongImage;
    private string _currentSongLength = "00:00";

    private string _currentSongTime = "00:00";

    private Bindable<bool> _isPlaying = new();
    private Bindable<bool> _isRepeating = new();
    private Bindable<bool> _isShuffle = new();

    private double _playbackSpeed;
    private Bindable<double> _songLength = new();
    private Bindable<double> _songTime = new();
    private Bindable<double> _volume = new();

    public BassEngine BassEngine;

    public Player Player;

    public PlayerControlViewModel(Player player, BassEngine bassEngine)
    {
        Player = player;
        BassEngine = bassEngine;

        _songTime.BindTo(BassEngine.ChannelPositionB);
        _songTime.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongTime)));

        _songLength.BindTo(BassEngine.ChannelLengthB);
        _songLength.BindValueChanged(d => this.RaisePropertyChanged(nameof(SongLength)));

        _currentSong.BindTo(Player.CurrentSongBinding);
        _currentSong.BindValueChanged(d =>
        {
            this.RaisePropertyChanged(nameof(TitleText));
            this.RaisePropertyChanged(nameof(ArtistText));
            this.RaisePropertyChanged(nameof(SongText));
        });

        _volume.BindTo(BassEngine.VolumeB);
        _volume.BindValueChanged(d => this.RaisePropertyChanged(nameof(Volume)));

        _isPlaying.BindTo(Player.IsPlaying);
        _isPlaying.BindValueChanged(d => this.RaisePropertyChanged(nameof(IsPlaying)));

        _isRepeating.BindTo(Player.IsRepeating);
        _isRepeating.BindValueChanged(d => this.RaisePropertyChanged(nameof(IsRepeating)));

        _isShuffle.BindTo(Player.Shuffle);
        _isShuffle.BindValueChanged(d => this.RaisePropertyChanged(nameof(IsShuffle)));

        Player.CurrentSongImage.BindValueChanged(d => CurrentSongImage = d.NewValue, true);

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }

    public double Volume
    {
        get => _volume.Value;
        set => _volume.Value = value;
    }

    public bool IsShuffle
    {
        get => _isShuffle.Value;
        set
        {
            _isShuffle.Value = value;
            this.RaisePropertyChanged();
        }
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
        get
        {
            this.RaisePropertyChanged(nameof(CurrentSongTime));
            return _songTime.Value;
        }
        set => _songTime.Value = value;
    }

    public string CurrentSongTime
    {
        get => TimeSpan.FromSeconds(_songTime.Value * (1 - PlaybackSpeed)).FormatTime();
        set => this.RaiseAndSetIfChanged(ref _currentSongTime, value);
    }

    public double SongLength
    {
        get
        {
            this.RaisePropertyChanged(nameof(CurrentSongLength));
            return _songLength.Value;
        }
    }

    public string CurrentSongLength
    {
        get => TimeSpan.FromSeconds(_songLength.Value * (1 - PlaybackSpeed)).FormatTime();
        set => this.RaiseAndSetIfChanged(ref _currentSongLength, value);
    }

    public bool IsPlaying => _isPlaying.Value;

    public bool IsRepeating => _isRepeating.Value;

    public string TitleText => _currentSong.Value?.Title ?? "No song is playing";

    public string ArtistText => _currentSong.Value?.Artist ?? "please select from song list";

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