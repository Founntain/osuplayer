using System.Reactive.Disposables;
using Avalonia.Media.Imaging;
using OsuPlayer.Base.ViewModels;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlayerControlViewModel : BaseViewModel
{
    private readonly Bindable<bool> _isPlaying = new();
    private readonly Bindable<RepeatMode> _isRepeating = new();
    private readonly Bindable<bool> _isShuffle = new();
    private readonly Bindable<double> _songLength = new();
    private readonly Bindable<double> _songTime = new();
    private readonly Bindable<double> _volume = new();
    public readonly Bindable<IMapEntry?> CurrentSong = new();

    public readonly IPlayer Player;
    private Bitmap? _currentSongImage;
    private string _currentSongLength = "00:00";

    private string _currentSongTime = "00:00";

    private double _playbackSpeed;

    public FontWeights SmallerFont
    {
        get
        {
            using var config = new Config();

            return config.Container.GetSmallerFont();
        }
    }

    public bool IsCurrentSongInPlaylist => CurrentSong.Value != null
                                           && Player.SelectedPlaylist.Value != null
                                           && Player.SelectedPlaylist.Value.Songs.Contains(CurrentSong.Value.Hash);

    public bool IsAPlaylistSelected => Player.SelectedPlaylist.Value != default;

    public bool IsCurrentSongOnBlacklist => new Blacklist().Contains(CurrentSong.Value);

    public double Volume
    {
        get => _volume.Value;
        set
        {
            _volume.Value = value;
            this.RaisePropertyChanged();
        }
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

    public string TitleText => CurrentSong.Value?.Title ?? "No song is playing";

    public RepeatMode IsRepeating
    {
        get => _isRepeating.Value;
        set
        {
            _isRepeating.Value = value;
            this.RaisePropertyChanged();
        }
    }

    public string ArtistText => CurrentSong.Value?.Artist ?? "please select from song list";

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

    public IEnumerable<Playlist>? Playlists => PlaylistManager.GetAllPlaylists()?.Where(x => x.Songs.Count > 0);

    public string ActivePlaylist => $"Active playlist: {Player.SelectedPlaylist.Value?.Name ?? "none"}";

    public PlayerControlViewModel(IPlayer player, IAudioEngine bassEngine)
    {
        Player = player;

        _songTime.BindTo(bassEngine.ChannelPosition);
        _songTime.BindValueChanged(_ => this.RaisePropertyChanged(nameof(SongTime)));

        _songLength.BindTo(bassEngine.ChannelLength);
        _songLength.BindValueChanged(_ => this.RaisePropertyChanged(nameof(SongLength)));

        CurrentSong.BindTo(Player.CurrentSong);
        CurrentSong.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(TitleText));
            this.RaisePropertyChanged(nameof(ArtistText));
            this.RaisePropertyChanged(nameof(SongText));
            this.RaisePropertyChanged(nameof(IsCurrentSongInPlaylist));
            this.RaisePropertyChanged(nameof(IsCurrentSongOnBlacklist));
        });

        _volume.BindTo(Player.Volume);
        _volume.BindValueChanged(_ => this.RaisePropertyChanged(nameof(Volume)));

        _isPlaying.BindTo(Player.IsPlaying);
        _isPlaying.BindValueChanged(_ => this.RaisePropertyChanged(nameof(IsPlaying)));

        _isRepeating.BindTo(Player.RepeatMode);
        _isRepeating.BindValueChanged(_ => { this.RaisePropertyChanged(nameof(IsRepeating)); });

        _isShuffle.BindTo(Player.IsShuffle);
        _isShuffle.BindValueChanged(_ => this.RaisePropertyChanged(nameof(IsShuffle)));

        Player.CurrentSongImage.BindValueChanged(d =>
        {
            CurrentSongImage?.Dispose();
            if (!string.IsNullOrEmpty(d.NewValue) && File.Exists(d.NewValue))
            {
                CurrentSongImage = new Bitmap(d.NewValue);
                return;
            }

            CurrentSongImage = null;
        }, true);

        Player.SelectedPlaylist.BindValueChanged(_ =>
        {
            this.RaisePropertyChanged(nameof(IsAPlaylistSelected));
            this.RaisePropertyChanged(nameof(IsCurrentSongInPlaylist));
        }, true);

        Activator = new ViewModelActivator();
        this.WhenActivated(disposables => { Disposable.Create(() => { }).DisposeWith(disposables); });
    }
}