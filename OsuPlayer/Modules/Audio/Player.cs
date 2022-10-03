using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Engine;
using OsuPlayer.Network.Discord;

namespace OsuPlayer.Modules.Audio;

/// <summary>
/// This class is a wrapper for our <see cref="BassEngine" />.
/// You can play, pause, stop and etc. from this class. Custom logic should also be implemented here
/// </summary>
public class Player : IPlayer
{
    private readonly IAudioEngine _audioEngine;
    private readonly Stopwatch _currentSongTimer = new();
    private readonly DiscordClient? _discordClient;
    private readonly IShuffleProvider _songShuffler;

    private bool _isMuted;
    private double _oldVolume;

    public Bindable<SourceList<IMapEntryBase>> SongSource { get; } = new();
    public List<IMapEntryBase>? SongSourceList { get; private set; }

    public Bindable<IMapEntry?> CurrentSong { get; } = new();
    public Bindable<Bitmap?> CurrentSongImage { get; } = new();

    public BindableList<ObservableValue> GraphValues { get; } = new();

    public Bindable<bool> IsPlaying { get; } = new();
    public Bindable<bool> IsShuffle { get; } = new();
    public Bindable<bool> BlacklistSkip { get; } = new();
    public Bindable<bool> PlaylistEnableOnPlay { get; } = new();
    public Bindable<RepeatMode> RepeatMode { get; } = new();

    public Bindable<Playlist?> SelectedPlaylist { get; } = new();

    public Bindable<SortingMode> SortingModeBindable { get; } = new();

    public List<AudioDevice> AvailableAudioDevices => _audioEngine.AvailableAudioDevices;
    public BindableArray<decimal> EqGains => _audioEngine.EqGains;
    public Bindable<double> Volume => _audioEngine.Volume;

    public int CurrentIndex { get; private set; }

    public bool IsEqEnabled
    {
        get => _audioEngine.IsEqEnabled;
        set => _audioEngine.IsEqEnabled = value;
    }

    public Playlist? ActivePlaylist { get; private set; }
    private List<IMapEntryBase> ActivePlaylistSongs { get; set; }

    public Bindable<Guid?> ActivePlaylistId { get; } = new();

    public Player(IAudioEngine audioEngine, IShuffleProvider shuffleProvider)
    {
        _audioEngine = audioEngine;

        _audioEngine.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "SongEnd")
                Dispatcher.UIThread.Post(() => NextSong(PlayDirection.Forward));
        };

        _discordClient = new DiscordClient().Initialize();
        _songShuffler = shuffleProvider;

        var config = new Config();

        Volume.Value = config.Container.Volume;

        SortingModeBindable.Value = config.Container.SortingMode;
        BlacklistSkip.Value = config.Container.BlacklistSkip;
        PlaylistEnableOnPlay.Value = config.Container.PlaylistEnableOnPlay;
        RepeatMode.Value = config.Container.RepeatMode;
        IsShuffle.Value = config.Container.IsShuffle;

        IsPlaying.BindTo(_audioEngine.IsPlaying);

        SortingModeBindable.BindValueChanged(d => UpdateSorting(d.NewValue));

        SongSource.BindValueChanged(d => { SongSourceList = d.NewValue.Items.ToList(); }, true);

        CurrentSong.BindValueChanged(d =>
        {
            using var cfg = new Config();

            cfg.Container.LastPlayedSong = d.NewValue?.Hash;

            ApiAsync.SetUserOnlineStatusNonBlock(UserOnlineStatusType.Listening, d.NewValue?.ToString(), d.NewValue?.Hash);

            if (d.NewValue is null) return;

            _discordClient.UpdatePresence(d.NewValue.Title, $"by {d.NewValue.Artist}");

            // _mainWindow.ViewModel!.PlayerControl.CurrentSongImage = Task.Run(value!.FindBackground).Result;
        }, true);

        RepeatMode.BindValueChanged(d =>
        {
            using var cfg = new Config();
            cfg.Container.RepeatMode = d.NewValue;
        }, true);

        ActivePlaylistId.BindValueChanged(d =>
        {
            var playlists = PlaylistManager.GetAllPlaylists();
            ActivePlaylist = playlists?.FirstOrDefault(x => x.Id == d.NewValue);

            if (ActivePlaylist == null) return;

            ActivePlaylistSongs = GetMapEntriesFromHash(ActivePlaylist.Songs);
        }, true);

        SongSource.Value = new SourceList<IMapEntryBase>();
    }

    public Bindable<bool> SongsLoading { get; } = new();

    public async void OnSongImportFinished()
    {
        var config = new Config();
        ActivePlaylistId.Value = config.Container.ActivePlaylistId;

        switch (config.Container.StartupSong)
        {
            case StartupSong.FirstSong:
                await TryPlaySongAsync(SongSourceList?[0]);
                break;
            case StartupSong.LastPlayed:
                await PlayLastPlayedSongAsync(config.Container);
                break;
            case StartupSong.RandomSong:
                await TryPlaySongAsync(SongSourceList?[new Random().Next(SongSourceList.Count)]);
                break;
        }
    }

    public IComparable CustomSorter(IMapEntryBase map, SortingMode sortingMode)
    {
        switch (sortingMode)
        {
            case SortingMode.Title:
                return map.Title;
            case SortingMode.Artist:
                return map.Artist;
            case SortingMode.SetId:
                return map.BeatmapSetId;
            default:
                return null!;
        }
    }

    public event PropertyChangedEventHandler? PlaylistChanged;
    public event PropertyChangedEventHandler? BlacklistChanged;
    public event PropertyChangedEventHandler? UserDataChanged;

    public void SetDevice(AudioDevice audioDevice) => _audioEngine.SetDevice(audioDevice);

    /// <summary>
    /// Plays the last played song read from the <see cref="ConfigContainer" /> and defaults to the
    /// first song in the <see cref="SongSourceList" /> if null
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
            await TryPlaySongAsync(GetMapEntryFromHash(config.LastPlayedSong));
            return;
        }

        await TryPlaySongAsync(SongSourceList?[0]);
    }

    /// <summary>
    /// Updates the <see cref="SongSource" /> according to the <paramref name="sortingMode" />
    /// </summary>
    /// <param name="sortingMode">the <see cref="SortingMode" /> of the song list</param>
    private void UpdateSorting(SortingMode sortingMode = SortingMode.Title)
    {
        SongSource.Value = SongSource.Value.Items.OrderBy(x => CustomSorter(x, sortingMode)).ThenBy(x => x.Title).ToSourceList();
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

    /// <summary>
    /// Updates the user xp on the api
    /// </summary>
    private async void UpdateXp()
    {
        if (ProfileManager.User == default) return;

        var currentTotalXp = ProfileManager.User.TotalXp;

        _currentSongTimer.Stop();

        var time = (double) _currentSongTimer.ElapsedMilliseconds / 1000;

        var response = await ApiAsync.UpdateXpFromCurrentUserAsync(
            CurrentSong.Value?.Hash ?? string.Empty,
            time,
            _audioEngine.ChannelLength.Value);

        if (response == default) return;

        ProfileManager.User = response;

        var xpEarned = response.TotalXp - currentTotalXp;

        GraphValues.Add(new ObservableValue(xpEarned));
    }

    /// <summary>
    /// Updates the songs played count of the user
    /// </summary>
    /// <param name="beatmapSetId">the beatmap set id of the map that was played</param>
    private async void UpdateSongsPlayed(int beatmapSetId)
    {
        if (ProfileManager.User == default) return;

        var response = await ApiAsync.UpdateSongsPlayedForCurrentUserAsync(1, beatmapSetId);

        if (response == default) return;

        ProfileManager.User = response;

        UserDataChanged?.Invoke(this, new PropertyChangedEventArgs("SongsPlayed"));
    }

    public IMapEntryBase? GetMapEntryFromSetId(int setId)
    {
        return SongSourceList!.FirstOrDefault(x => x.BeatmapSetId == setId);
    }

    public IMapEntryBase? GetMapEntryFromHash(string? hash)
    {
        return SongSourceList!.FirstOrDefault(x => x.Hash == hash);
    }

    public List<IMapEntryBase> GetMapEntriesFromSetId(IEnumerable<int> setId)
    {
        return SongSourceList!.Where(x => setId.Contains(x.BeatmapSetId)).ToList();
    }

    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash)
    {
        //return SongSourceList!.FindAll(x => hash.Contains(x.Hash));
        return hash.Select(x => SongSourceList!.Find(map => map.Hash == x)).ToList();
    }

    public void DisposeDiscordClient()
    {
        _discordClient?.DeInitialize();
    }

    public async void PlayPause()
    {
        if (!IsPlaying.Value)
        {
            Play();

            await ApiAsync.SetUserOnlineStatus(UserOnlineStatusType.Listening, CurrentSong.Value?.ToString(), CurrentSong.Value?.Hash);
        }
        else
        {
            Pause();

            await ApiAsync.SetUserOnlineStatus(UserOnlineStatusType.Idle);
        }
    }

    public void Play()
    {
        _audioEngine.Play();
        _currentSongTimer.Start();
    }

    public void Pause()
    {
        _audioEngine.Pause();
        _currentSongTimer.Stop();
    }

    public void Stop() => _audioEngine.Stop();

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
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (playDirection == PlayDirection.Backwards && _audioEngine.ChannelPosition.Value > 3)
        {
            await TryStartSongAsync(CurrentSong.Value ?? SongSourceList[0]);
            return;
        }

        switch (RepeatMode.Value)
        {
            case Data.OsuPlayer.Enums.RepeatMode.NoRepeat:
                await TryPlaySongAsync(GetNextSongToPlay(SongSourceList, CurrentIndex, playDirection), playDirection);
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

    private IMapEntryBase GetNextSongToPlay(IList<IMapEntryBase> songSource, int currentIndex, PlayDirection playDirection)
    {
        IMapEntryBase songToPlay;
        var offset = (int) playDirection;

        currentIndex = songSource.IndexOf(SongSourceList![currentIndex]);

        if (IsShuffle.Value)
        {
            songToPlay = songSource[_songShuffler.DoShuffle(currentIndex, (ShuffleDirection) playDirection, songSource.Count)];
        }
        else
        {
            var x = (currentIndex + offset) % songSource!.Count;
            currentIndex = x < 0 ? x + songSource!.Count : x;

            songToPlay = songSource[currentIndex];
        }

        if (BlacklistSkip.Value && new Blacklist().Container.Songs.Contains(songToPlay.Hash))
        {
            songToPlay = GetNextSongToPlay(songSource, currentIndex, playDirection);
        }

        return songToPlay;
    }

    public async Task TryPlaySongAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal)
    {
        if (SongSourceList == default || !SongSourceList.Any())
            throw new NullReferenceException();

        if (song == default)
        {
            await TryStartSongAsync(SongSourceList[0]);
            return;
        }

        if ((await new Config().ReadAsync()).IgnoreSongsWithSameNameCheckBox && string.Equals(CurrentSong.Value?.SongName, song.SongName, StringComparison.OrdinalIgnoreCase))
        {
            switch (playDirection)
            {
                case PlayDirection.Forward:
                case PlayDirection.Backwards:
                    CurrentIndex += (int)playDirection;
                    NextSong(playDirection);
                    return;
                default:
                    await TryStartSongAsync(song);
                    return;
            }
        }

        await TryStartSongAsync(song);
    }

    /// <summary>
    /// Starts playing a song
    /// </summary>
    /// <param name="song">a <see cref="IMapEntryBase" /> to play next</param>
    /// <returns>a <see cref="Task" /> with the resulting state</returns>
    private async Task TryStartSongAsync(IMapEntryBase song)
    {
        if (SongSourceList == null || !SongSourceList.Any())
        {
            throw new NullReferenceException($"{nameof(SongSourceList)} can't be null or empty");
        }

        var config = new Config();

        await config.ReadAsync();

        var fullMapEntry = await song.ReadFullEntry(config.Container.OsuPath!);

        if (fullMapEntry == default)
        {
            throw new NullReferenceException();
        }

        fullMapEntry.UseUnicode = config.Container.UseSongNameUnicode;
        var findBackgroundTask = fullMapEntry.FindBackground();

        //We put the XP update to an own try catch because if the API fails or is not available,
        //that the whole TryEnqueue does not fail
        try
        {
            if (CurrentSong.Value != default)
                UpdateXp();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update XP error => {e}");
        }

        try
        {
            _audioEngine.OpenFile(fullMapEntry.FullPath!);
            //_bassEngine.SetAllEq(Core.Instance.Config.Eq);
            _audioEngine.Play();

            _currentSongTimer.Restart();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return;
        }

        CurrentSong.Value = fullMapEntry;
        CurrentIndex = SongSourceList.IndexOf(fullMapEntry);

        //Same as update XP mentioned Above
        try
        {
            if (CurrentSong.Value != default)
                UpdateSongsPlayed(fullMapEntry.BeatmapSetId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update Songs Played error => {e}");
        }

        CurrentSongImage.Value = await findBackgroundTask;
    }
}