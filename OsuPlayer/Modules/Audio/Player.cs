using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Network.Discord;
using OsuPlayer.UI_Extensions;

namespace OsuPlayer.Modules.Audio;

/// <summary>
/// This class is a wrapper for our <see cref="BassEngine" />.
/// You can play, pause, stop and etc. from this class. Custom logic should also be implemented here
/// </summary>
public class Player : IPlayer
{
    private readonly BassEngine _bassEngine;
    private readonly Stopwatch _currentSongTimer;
    private readonly DiscordClient? _discordClient;
    private readonly SongShuffler _songShuffler;

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

    public BindableArray<decimal> EqGains => _bassEngine.EqGains;

    public int CurrentIndex { get; set; }

    public bool IsEqEnabled
    {
        get => _bassEngine.IsEqEnabled;
        set => _bassEngine.IsEqEnabled = value;
    }

    public Playlist? ActivePlaylist => ActivePlaylistId != default
        ? PlaylistManager.GetAllPlaylists()?.First(x => x.Id == ActivePlaylistId)
        : default;

    public Guid? ActivePlaylistId { get; set; }

    // private int _shuffleHistoryIndex;

    public Player(BassEngine bassEngine)
    {
        _bassEngine = bassEngine;

        _bassEngine.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "SongEnd")
                Dispatcher.UIThread.Post(NextSong);
        };

        _discordClient = new DiscordClient().Initialize();
        _songShuffler = new SongShuffler(this);

        var config = new Config();

        _bassEngine.Volume = config.Container.Volume;

        SortingModeBindable.Value = config.Container.SortingMode;
        BlacklistSkip.Value = config.Container.BlacklistSkip;
        PlaylistEnableOnPlay.Value = config.Container.PlaylistEnableOnPlay;
        RepeatMode.Value = config.Container.RepeatMode;
        IsShuffle.Value = config.Container.IsShuffle;
        ActivePlaylistId = config.Container.ActivePlaylistId;

        SortingModeBindable.BindValueChanged(d => UpdateSorting(d.NewValue));

        SongSource.BindValueChanged(d => { SongSourceList = d.NewValue.Items.ToList(); }, true);

        CurrentSong.BindValueChanged(d =>
        {
            CurrentIndex = SongSourceList!.FindIndex(x => x.Hash == d.NewValue!.Hash);

            using var cfg = new Config();

            cfg.Container.LastPlayedSong = d.NewValue?.Hash;

            ApiAsync.SetUserOnlineStatusNonBlock(UserOnlineStatusType.Listening, d.NewValue?.ToString(), d.NewValue?.Hash);

            if (_discordClient is null || d.NewValue is null) return;

            _discordClient.UpdatePresence(d.NewValue.Title, $"by {d.NewValue.Artist}");

            // _mainWindow.ViewModel!.PlayerControl.CurrentSongImage = Task.Run(value!.FindBackground).Result;
        }, true);
        
        RepeatMode.BindValueChanged(d =>
        {
            using var cfg = new Config();
            cfg.Container.RepeatMode = d.NewValue;
        }, true);

        SongSource.Value = new SourceList<IMapEntryBase>();

        _currentSongTimer = new Stopwatch();
    }

    public Bindable<bool> SongsLoading { get; } = new();

    public async void OnSongImportFinished()
    {
        var config = new Config();

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
        _bassEngine.SetPlaybackSpeed(speed);
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
            _bassEngine.ChannelLengthB.Value);

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

    public IMapEntryBase? GetMapEntryFromHash(string hash)
    {
        return SongSourceList!.FirstOrDefault(x => x.Hash == hash);
    }

    public List<IMapEntryBase> GetMapEntriesFromSetId(IEnumerable<int> setId)
    {
        return SongSourceList!.Where(x => setId.Contains(x.BeatmapSetId)).ToList();
    }

    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash)
    {
        return hash.Select(x => SongSourceList!.Find(y => y.Hash == x)).ToList();
    }

    public void DisposeDiscordClient()
    {
        _discordClient?.DeInitialize();
    }

    public async void PlayPause()
    {
        if (!IsPlaying.Value)
        {
            _bassEngine.Play();
            _currentSongTimer.Start();
            IsPlaying.Value = true;

            await ApiAsync.SetUserOnlineStatus(UserOnlineStatusType.Listening, CurrentSong.Value?.ToString(), CurrentSong.Value?.Hash);
        }
        else
        {
            _bassEngine.Pause();
            _currentSongTimer.Stop();
            IsPlaying.Value = false;

            await ApiAsync.SetUserOnlineStatus(UserOnlineStatusType.Idle);
        }
    }

    public void Play()
    {
        _bassEngine.Play();
        IsPlaying.Value = true;
    }

    public void Pause()
    {
        _bassEngine.Pause();
        IsPlaying.Value = false;
    }

    public void ToggleMute()
    {
        if (!_isMuted)
        {
            _oldVolume = _bassEngine.Volume;
            _bassEngine.Volume = 0;
            _isMuted = true;
        }
        else
        {
            _bassEngine.Volume = _oldVolume;
            _isMuted = false;
        }
    }

    public async void PreviousSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (_bassEngine.ChannelPositionB.Value > 3)
        {
            await TryStartSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (RepeatMode.Value == Data.OsuPlayer.Enums.RepeatMode.SingleSong)
        {
            await TryStartSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (IsShuffle.Value)
        {
            await TryPlaySongAsync(_songShuffler.DoShuffle(ShuffleDirection.Backwards), PlayDirection.Backwards);
            return;
        }

        if (RepeatMode.Value == Data.OsuPlayer.Enums.RepeatMode.Playlist)
        {
            await EnqueuePlaylistAsync(PlayDirection.Backwards);
            return;
        }

        if (BlacklistSkip.Value)
        {
            await EnqueueBlacklistAsync(PlayDirection.Backwards);
            return;
        }

        var prevIndex = (CurrentIndex - 1) % SongSourceList!.Count;
        await TryPlaySongAsync(SongSourceList[prevIndex < 0 ? prevIndex + SongSourceList!.Count : prevIndex], PlayDirection.Backwards);
    }

    public async void NextSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (RepeatMode.Value == Data.OsuPlayer.Enums.RepeatMode.SingleSong)
        {
            await TryStartSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (IsShuffle.Value)
        {
            await TryPlaySongAsync(_songShuffler.DoShuffle(ShuffleDirection.Forward));
            return;
        }

        if (RepeatMode.Value == Data.OsuPlayer.Enums.RepeatMode.Playlist)
        {
            await EnqueuePlaylistAsync(PlayDirection.Forward);
            return;
        }

        if (BlacklistSkip.Value)
        {
            await EnqueueBlacklistAsync(PlayDirection.Forward);
            return;
        }

        var nextIndex = (CurrentIndex + 1) % SongSourceList!.Count;
        await TryPlaySongAsync(SongSourceList[nextIndex < 0 ? nextIndex + SongSourceList!.Count : nextIndex], PlayDirection.Forward);
    }

    /// <summary>
    /// Enqueues a song in the current active <see cref="Playlist" />.
    /// Called only if the current <see cref="RepeatMode" /> is <see cref="Data.OsuPlayer.Enums.RepeatMode.Playlist" />
    /// </summary>
    /// <param name="direction">in what direction the next song should be</param>
    private async Task EnqueuePlaylistAsync(PlayDirection direction = PlayDirection.Normal)
    {
        var offset = direction switch
        {
            PlayDirection.Backwards => -1,
            PlayDirection.Forward => 1,
            _ => 0
        };

        if (offset == 0)
            throw new InvalidOperationException();

        if (ActivePlaylist == default || ActivePlaylist.Songs.Count == 0)
        {
            // OsuPlayerMessageBox.Show(
            //    OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.noPlaylistSelected"));
            RepeatMode.Value = Data.OsuPlayer.Enums.RepeatMode.NoRepeat;

            if (SongSourceList!.IsInBounds(CurrentIndex + offset))
                await TryPlaySongAsync(SongSourceList[CurrentIndex + offset], direction);
            else
                await TryPlaySongAsync(direction == PlayDirection.Forward ? SongSourceList.First() : SongSourceList.Last(), direction);

            return;
        }

        var currentPlaylistIndex = ActivePlaylist.Songs.IndexOf(CurrentSong.Value!.Hash);

        if (ActivePlaylist.Songs.IsInBounds(currentPlaylistIndex + offset))
            await TryPlaySongAsync(GetMapEntryFromHash(ActivePlaylist.Songs[currentPlaylistIndex + offset]), direction);
        else
            await TryPlaySongAsync(GetMapEntryFromHash(direction == PlayDirection.Forward ? ActivePlaylist.Songs.First() : ActivePlaylist.Songs.Last()));
    }

    /// <summary>
    /// Enqueues a song ignoring all songs in the <see cref="Blacklist" />.
    /// Called only if the <see cref="BlacklistSkip" /> is true
    /// </summary>
    /// <param name="direction">in what direction the next song should be</param>
    private async Task EnqueueBlacklistAsync(PlayDirection direction = PlayDirection.Normal)
    {
        var offset = direction switch
        {
            PlayDirection.Backwards => -1,
            PlayDirection.Forward => 1,
            _ => 0
        };

        if (offset == 0)
            throw new InvalidOperationException();

        var blacklist = new Blacklist();
        for (var i = CurrentIndex + offset; i != CurrentIndex - offset; i += offset)
        {
            var x = i % SongSourceList!.Count;
            i = x < 0 ? x + SongSourceList!.Count : x;

            if (blacklist.Contains(SongSourceList[i])) continue;

            await TryPlaySongAsync(SongSourceList[i], direction);
            return;
        }

        MessageBox.Show("There is no song to play!");
        _bassEngine.Stop();
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
                    CurrentIndex++;
                    NextSong();
                    return;
                case PlayDirection.Backwards:
                    CurrentIndex--;
                    PreviousSong();
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
            _bassEngine.OpenFile(fullMapEntry.FullPath!);
            //_bassEngine.SetAllEq(Core.Instance.Config.Eq);
            _bassEngine.Play();
            IsPlaying.Value = true;

            _currentSongTimer.Restart();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return;
        }

        CurrentSong.Value = fullMapEntry;

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