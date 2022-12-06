using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Engine;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Modules.Services;
using OsuPlayer.Network.Discord;

namespace OsuPlayer.Modules.Audio;

/// <summary>
/// This class is a wrapper for our <see cref="BassEngine" />.
/// You can play, pause, stop and etc. from this class. Custom logic should also be implemented here
/// </summary>
public class Player : IPlayer, IImportNotifications
{
    private readonly IAudioEngine _audioEngine;
    private readonly Stopwatch _currentSongTimer = new();
    private readonly DiscordClient? _discordClient;
    private readonly IShuffleServiceProvider? _shuffleProvider;
    private readonly IStatisticsProvider? _statisticsProvider;
    private readonly WindowsMediaTransportControls? _winMediaControls;

    private bool _isMuted;
    private double _oldVolume;

    public ISongSourceProvider SongSourceProvider { get; }
    public Bindable<bool> SongsLoading { get; } = new();

    public Bindable<IMapEntry?> CurrentSong { get; } = new();
    public Bindable<string?> CurrentSongImage { get; } = new();

    public Bindable<bool> IsPlaying { get; } = new();
    public Bindable<bool> IsShuffle { get; } = new();
    public Bindable<bool> BlacklistSkip { get; } = new();
    public Bindable<bool> PlaylistEnableOnPlay { get; } = new();
    public Bindable<RepeatMode> RepeatMode { get; } = new();

    public List<AudioDevice> AvailableAudioDevices => _audioEngine.AvailableAudioDevices;
    public BindableArray<decimal> EqGains => _audioEngine.EqGains;
    public Bindable<double> Volume => _audioEngine.Volume;

    public int CurrentIndex { get; private set; }

    public bool IsEqEnabled
    {
        get => _audioEngine.IsEqEnabled;
        set => _audioEngine.IsEqEnabled = value;
    }

    public Bindable<Playlist?> SelectedPlaylist { get; } = new();
    private List<IMapEntryBase> ActivePlaylistSongs { get; set; }

    public Player(IAudioEngine audioEngine, ISongSourceProvider songSourceProvider, IShuffleServiceProvider? shuffleProvider = null, IStatisticsProvider? statisticsProvider = null, ISortProvider? sortProvider = null)
    {
        _audioEngine = audioEngine;

        var runtimePlatform = AvaloniaLocator.Current.GetRequiredService<IRuntimePlatform>();

        if (runtimePlatform.GetRuntimeInfo().OperatingSystem == OperatingSystemType.WinNT)
            try
            {
                _winMediaControls = new WindowsMediaTransportControls(this);
            }
            catch
            {
                _winMediaControls = null;
            }

        _audioEngine.ChannelReachedEnd = () => NextSong(PlayDirection.Forward);

        var config = new Config();

        _discordClient = config.Container.UseDiscordRpc ? new DiscordClient().Initialize() : null;

        SongSourceProvider = songSourceProvider;
        _shuffleProvider = shuffleProvider;
        _statisticsProvider = statisticsProvider;

        IsPlaying.BindTo(_audioEngine.IsPlaying);

        Volume.Value = config.Container.Volume;

        BlacklistSkip.Value = config.Container.BlacklistSkip;
        PlaylistEnableOnPlay.Value = config.Container.PlaylistEnableOnPlay;
        RepeatMode.Value = config.Container.RepeatMode;
        IsShuffle.Value = config.Container.IsShuffle;

        songSourceProvider.Songs?.Subscribe(next => CurrentIndex = songSourceProvider.SongSourceList?.IndexOf(CurrentSong.Value) ?? -1);

        CurrentSong.BindValueChanged(d =>
        {
            using var cfg = new Config();

            cfg.Container.LastPlayedSong = d.NewValue?.Hash;

            _statisticsProvider?.UpdateOnlineStatus(UserOnlineStatusType.Listening, d.NewValue?.ToString(), d.NewValue?.Hash);

            if (d.NewValue is null) return;

            _discordClient?.UpdatePresence(d.NewValue.Title, $"by {d.NewValue.Artist}");
        }, true);

        RepeatMode.BindValueChanged(d =>
        {
            using var cfg = new Config();
            cfg.Container.RepeatMode = d.NewValue;
        }, true);

        IsShuffle.BindValueChanged(d => _shuffleProvider?.ShuffleImpl?.Init(0));

        SelectedPlaylist.BindValueChanged(d =>
        {
            using var cfg = new Config();
            cfg.Container.SelectedPlaylist = d.NewValue?.Id;

            if (d.NewValue == null) return;

            ActivePlaylistSongs = SongSourceProvider.GetMapEntriesFromHash(d.NewValue.Songs, out var invalidHashes);

            if (invalidHashes.Any())
            {
                using var playlists = new PlaylistStorage();

                var playlist = playlists.Container.Playlists?.First(x => x.Id == d.NewValue.Id);

                playlist?.Songs.RemoveWhere(song => invalidHashes.Contains(song));
            }

            if (RepeatMode.Value != Data.OsuPlayer.Enums.RepeatMode.Playlist || CurrentSong.Value == null) return;

            if (!ActivePlaylistSongs.Contains(CurrentSong.Value))
                NextSong(PlayDirection.Forward);
        }, true);
    }

    public void OnImportStarted()
    {
        SongsLoading.Value = true;
    }

    public async void OnImportFinished()
    {
        SongsLoading.Value = false;

        var config = new Config();
        var playlists = new PlaylistStorage();

        SelectedPlaylist.Value = playlists.Container.Playlists?.FirstOrDefault(x => x.Id == config.Container.SelectedPlaylist) ?? playlists.Container.Playlists?.First(y => y.Name == "Favorites");

        switch (config.Container.StartupSong)
        {
            case StartupSong.FirstSong:
                await TryPlaySongAsync(SongSourceProvider.SongSourceList?[0]);
                break;
            case StartupSong.LastPlayed:
                await PlayLastPlayedSongAsync(config.Container);
                break;
            case StartupSong.RandomSong:
                await TryPlaySongAsync(SongSourceProvider.SongSourceList?[new Random().Next(SongSourceProvider.SongSourceList.Count)]);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Startup song type {config.Container.StartupSong} is not supported!");
        }
    }

    public event PropertyChangedEventHandler? PlaylistChanged;
    public event PropertyChangedEventHandler? BlacklistChanged;

    public void SetDevice(AudioDevice audioDevice)
    {
        _audioEngine.SetDevice(audioDevice);
    }

    public void TriggerPlaylistChanged(PropertyChangedEventArgs e)
    {
        PlaylistChanged?.Invoke(this, e);
    }

    public void TriggerBlacklistChanged(PropertyChangedEventArgs e)
    {
        BlacklistChanged?.Invoke(this, e);
    }

    public void SetPlaybackSpeed(double speed)
    {
        _audioEngine.SetPlaybackSpeed(speed);
    }

    public void DisposeDiscordClient()
    {
        _discordClient?.DeInitialize();
    }

    public void PlayPause()
    {
        if (!IsPlaying.Value)
        {
            Play();

            _statisticsProvider?.UpdateOnlineStatus(UserOnlineStatusType.Listening, CurrentSong.Value?.ToString(), CurrentSong.Value?.Hash);
        }
        else
        {
            Pause();

            _statisticsProvider?.UpdateOnlineStatus(UserOnlineStatusType.Idle);
        }
    }

    public void Play()
    {
        _audioEngine.Play();
        _currentSongTimer.Start();

        _winMediaControls?.UpdatePlayingStatus(true);
    }

    public void Pause()
    {
        _audioEngine.Pause();
        _currentSongTimer.Stop();

        _winMediaControls?.UpdatePlayingStatus(false);
    }

    public void Stop()
    {
        _audioEngine.Stop();
    }

    public void ToggleMute()
    {
        if (!_isMuted)
        {
            _oldVolume = Volume.Value;
            _audioEngine.Volume.Value = 0;
            _isMuted = true;
        }
        else
        {
            Volume.Value = _oldVolume;
            _isMuted = false;
        }
    }

    public async void NextSong(PlayDirection playDirection)
    {
        if (SongSourceProvider.SongSourceList == null || !SongSourceProvider.SongSourceList.Any())
            return;

        if (playDirection == PlayDirection.Backwards && _audioEngine.ChannelPosition.Value > 3)
        {
            await TryStartSongAsync(CurrentSong.Value ?? SongSourceProvider.SongSourceList[0]);
            return;
        }

        switch (RepeatMode.Value)
        {
            case Data.OsuPlayer.Enums.RepeatMode.NoRepeat:
                await TryPlaySongAsync(GetNextSongToPlay(SongSourceProvider.SongSourceList, CurrentIndex, playDirection), playDirection);
                return;
            case Data.OsuPlayer.Enums.RepeatMode.Playlist:
                await TryPlaySongAsync(GetNextSongToPlay(ActivePlaylistSongs, CurrentIndex, playDirection), playDirection);
                return;
            case Data.OsuPlayer.Enums.RepeatMode.SingleSong:
                await TryStartSongAsync(CurrentSong.Value!);
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task TryPlaySongAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal)
    {
        if (SongSourceProvider.SongSourceList == default || !SongSourceProvider.SongSourceList.Any())
            throw new NullReferenceException();

        if (song == default)
        {
            await TryStartSongAsync(SongSourceProvider.SongSourceList[0]);
            return;
        }

        if ((await new Config().ReadAsync()).IgnoreSongsWithSameNameCheckBox && string.Equals(CurrentSong.Value?.SongName, song.SongName, StringComparison.OrdinalIgnoreCase))
            switch (playDirection)
            {
                case PlayDirection.Forward:
                case PlayDirection.Backwards:
                    CurrentIndex += (int) playDirection;
                    NextSong(playDirection);
                    return;
                default:
                    await TryStartSongAsync(song);
                    return;
            }

        await TryStartSongAsync(song);
    }

    /// <summary>
    /// Plays the last played song read from the <see cref="ConfigContainer" /> and defaults to the
    /// first song in the <see cref="ISongSourceProvider.SongSourceList" /> if null
    /// </summary>
    /// <param name="config">optional parameter defaults to null. Used to avoid duplications of config instances</param>
    private async Task PlayLastPlayedSongAsync(ConfigContainer? config = null)
    {
        config ??= new Config().Container;

        if (config.LastPlayedSong == null)
        {
            await TryPlaySongAsync(null);
            return;
        }

        if (!string.IsNullOrWhiteSpace(config.LastPlayedSong))
        {
            await TryPlaySongAsync(SongSourceProvider.GetMapEntryFromHash(config.LastPlayedSong));
            return;
        }

        await TryPlaySongAsync(SongSourceProvider.SongSourceList?[0]);
    }

    private IMapEntryBase GetNextSongToPlay(IList<IMapEntryBase> songSource, int currentIndex, PlayDirection playDirection)
    {
        IMapEntryBase songToPlay;
        var offset = (int) playDirection;

        if (!SongSourceProvider.SongSourceList?.Any() ?? true) 
            throw new NullReferenceException("SongSourceList is null or empty");

        if (!SongSourceProvider.SongSourceList.IsInBounds(currentIndex))
            currentIndex = 0;

        currentIndex = songSource.IndexOf(SongSourceProvider.SongSourceList[currentIndex]);

        if (IsShuffle.Value && _shuffleProvider?.ShuffleImpl != null)
        {
            _shuffleProvider.ShuffleImpl.Init(songSource.Count);
            songToPlay = songSource[_shuffleProvider.ShuffleImpl.DoShuffle(currentIndex, (ShuffleDirection) playDirection)];
        }
        else
        {
            var x = (currentIndex + offset) % songSource!.Count;
            currentIndex = x < 0 ? x + songSource!.Count : x;

            songToPlay = songSource[currentIndex];
        }

        if (BlacklistSkip.Value && new Blacklist().Container.Songs.Contains(songToPlay.Hash)) songToPlay = GetNextSongToPlay(songSource, currentIndex, playDirection);

        return songToPlay;
    }

    /// <summary>
    /// Starts playing a song
    /// </summary>
    /// <param name="song">a <see cref="IMapEntryBase" /> to play next</param>
    /// <returns>a <see cref="Task" /> with the resulting state</returns>
    private async Task TryStartSongAsync(IMapEntryBase song)
    {
        if (SongSourceProvider.SongSourceList == null || !SongSourceProvider.SongSourceList.Any()) throw new NullReferenceException($"{nameof(SongSourceProvider.SongSourceList)} can't be null or empty");

        var config = new Config();

        await config.ReadAsync();

        var fullMapEntry = await song.ReadFullEntry();

        if (fullMapEntry == default) throw new NullReferenceException();

        fullMapEntry.UseUnicode = config.Container.UseSongNameUnicode;
        var findBackgroundTask = fullMapEntry.FindBackground();

        _currentSongTimer.Stop();

        //We put the XP update to an own try catch because if the API fails or is not available,
        //that the whole TryEnqueue does not fail
        try
        {
            if (CurrentSong.Value != default)
                _statisticsProvider?.UpdateXp(fullMapEntry.Hash, _currentSongTimer.ElapsedMilliseconds, _audioEngine.ChannelLength.Value);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update XP error => {e}");
        }

        CurrentSongImage.Value = await findBackgroundTask;

        try
        {
            _audioEngine.OpenFile(fullMapEntry.FullPath!);
            _audioEngine.Play();

            _winMediaControls?.UpdatePlayingStatus(true);
            _winMediaControls?.SetMetadata(fullMapEntry);

            _currentSongTimer.Restart();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return;
        }

        CurrentSong.Value = fullMapEntry;
        CurrentIndex = SongSourceProvider.SongSourceList.IndexOf(fullMapEntry);

        //Same as update XP mentioned Above
        try
        {
            if (CurrentSong.Value != default)
                _statisticsProvider?.UpdateSongsPlayed(fullMapEntry.BeatmapSetId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update Songs Played error => {e}");
        }
    }
}