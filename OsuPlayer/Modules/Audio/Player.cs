using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.API.Enums;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Network.Discord;
using OsuPlayer.UI_Extensions;

namespace OsuPlayer.Modules.Audio;

/// <summary>
/// This class is a wrapper for our <see cref="BassEngine" />.
/// You can play, pause, stop and etc. from this class. Custom logic should also be implemented here
/// </summary>
public class Player : ICanImportSongs
{
    private readonly BassEngine _bassEngine;
    private readonly Stopwatch _currentSongTimer;
    private readonly DiscordClient? _discordClient;
    private readonly int?[] _shuffleHistory = new int?[10];

    private bool _isMuted;
    private double _oldVolume;

    private PlayState _playState;
    private int _shuffleHistoryIndex;

    public Bindable<bool> BlacklistSkip { get; } = new();

    public Bindable<IMapEntry?> CurrentSong { get; } = new();

    public Bindable<Bitmap?> CurrentSongImage { get; } = new();

    public Bindable<List<ObservableValue>?> GraphValues { get; } = new();

    public Bindable<bool> IsPlaying { get; } = new();

    public Bindable<bool> IsShuffle { get; } = new();
    public Bindable<bool> PlaylistEnableOnPlay { get; } = new();

    public Bindable<RepeatMode> RepeatMode { get; } = new();

    public Bindable<Playlist?> SelectedPlaylist { get; } = new();

    public Bindable<SortingMode> SortingModeBindable { get; } = new();

    public BindableArray<decimal> EqGains => _bassEngine.EqGains;

    private PlayState PlayState
    {
        get => _playState;
        set
        {
            IsPlaying.Value = value == PlayState.Playing;
            _playState = value;
        }
    }

    private int CurrentIndex { get; set; }

    public RepeatMode Repeat
    {
        get => RepeatMode.Value;
        set => RepeatMode.Value = value;
    }

    public bool IsEqEnabled
    {
        get => _bassEngine.IsEqEnabled;
        set => _bassEngine.IsEqEnabled = value;
    }

    public Playlist? ActivePlaylist => ActivePlaylistId != default
        ? PlaylistManager.GetAllPlaylists().First(x => x.Id == ActivePlaylistId)
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

        var config = new Config();

        _bassEngine.Volume = config.Container.Volume;

        SortingModeBindable.Value = config.Container.SortingMode;
        BlacklistSkip.Value = config.Container.BlacklistSkip;
        PlaylistEnableOnPlay.Value = config.Container.PlaylistEnableOnPlay;
        Repeat = config.Container.RepeatMode;
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

        SongSource.Value = new SourceList<IMapEntryBase>();

        _currentSongTimer = new Stopwatch();
    }

    public Bindable<SourceList<IMapEntryBase>> SongSource { get; } = new();
    public List<IMapEntryBase>? SongSourceList { get; private set; }

    public Bindable<bool> SongsLoading { get; } = new();

    public async void OnSongImportFinished()
    {
        await using var config = new Config();

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

    /// <summary>
    /// Picks the <see cref="IMapEntryBase" /> property to sort maps on
    /// </summary>
    /// <param name="map">the <see cref="IMapEntryBase" /> to be sorted</param>
    /// <param name="sortingMode">the <see cref="SortingMode" /> to decide how to sort</param>
    /// <returns>an <see cref="IComparable" /> containing the property to compare on</returns>
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

    /// <summary>
    /// Triggers if the playlist got changed
    /// </summary>
    /// <param name="e"></param>
    public void TriggerPlaylistChanged(PropertyChangedEventArgs e)
    {
        PlaylistChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Triggers if the blacklist got changed
    /// </summary>
    /// <param name="e"></param>
    public void TriggerBlacklistChanged(PropertyChangedEventArgs e)
    {
        BlacklistChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Sets the playback speed globally (including pitch)
    /// </summary>
    /// <param name="speed">The speed to set</param>
    public void SetPlaybackSpeed(double speed)
    {
        _bassEngine.SetSpeed(speed);
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

        var values = GraphValues.Value?.ToList() ?? new List<ObservableValue>();

        values.Add(new ObservableValue(xpEarned));

        GraphValues.Value = values;
    }

    public event PropertyChangedEventHandler UserChanged;

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

        UserChanged.Invoke(this, new PropertyChangedEventArgs("SongsPlayed"));
    }

    /// <summary>
    /// Gets the map entry from the beatmap set id
    /// </summary>
    /// <param name="setId">the beatmap set id to get the map from</param>
    /// <returns>an <see cref="IMapEntryBase" /> of the found map or null if it doesn't exist</returns>
    private IMapEntryBase? GetMapEntryFromSetId(int setId)
    {
        return SongSourceList!.FirstOrDefault(x => x.BeatmapSetId == setId);
    }

    /// <summary>
    /// Gets the map entry from the beatmap hash
    /// </summary>
    /// <param name="hash">the beatmap hash to get the map from</param>
    /// <returns>an <see cref="IMapEntryBase" /> of the found map or null if it doesn't exist</returns>
    public IMapEntryBase? GetMapEntryFromHash(string hash)
    {
        return SongSourceList!.FirstOrDefault(x => x.Hash == hash);
    }

    /// <summary>
    /// Gets all Songs from a specific beatmap set ID
    /// </summary>
    /// <param name="setId">The beatmap set ID</param>
    /// <returns>A list of <see cref="IMapEntryBase" /></returns>
    public List<IMapEntryBase> GetMapEntriesFromSetId(ICollection<int> setId)
    {
        return SongSourceList!.Where(x => setId.Contains(x.BeatmapSetId)).ToList();
    }

    /// <summary>
    /// Gets all Songs from a specific beatmap hash
    /// </summary>
    /// <param name="hash">The beatmap hash</param>
    /// <returns>A list of <see cref="IMapEntryBase" /></returns>
    public List<IMapEntryBase> GetMapEntriesFromHash(ICollection<string> hash)
    {
        return hash.Select(x => SongSourceList!.Find(y => y.Hash == x)).ToList();
    }

    public void DisposeDiscordClient()
    {
        _discordClient?.DeInitialize();
    }

    #region Player

    /// <summary>
    /// Toggles mute of the volume
    /// </summary>
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

    /// <summary>
    /// Pauses the current song if playing or plays again if paused
    /// </summary>
    public async void PlayPause()
    {
        if (PlayState == PlayState.Paused)
        {
            _bassEngine.Play();
            _currentSongTimer.Start();
            PlayState = PlayState.Playing;

            await ApiAsync.SetUserOnlineStatus(UserOnlineStatusType.Listening, CurrentSong.Value?.ToString(), CurrentSong.Value?.Hash);
        }
        else
        {
            _bassEngine.Pause();
            _currentSongTimer.Stop();
            PlayState = PlayState.Paused;

            await ApiAsync.SetUserOnlineStatus(UserOnlineStatusType.Idle);
        }
    }

    /// <summary>
    /// Sets the playing state to Playing
    /// </summary>
    public void Play()
    {
        _bassEngine.Play();
        PlayState = PlayState.Playing;
    }

    /// <summary>
    /// Sets the playing state to Pause
    /// </summary>
    public void Pause()
    {
        _bassEngine.Pause();
        PlayState = PlayState.Paused;
    }

    /// <summary>
    /// Plays the previous song or the last song if we are the beginning
    /// </summary>
    public async void PreviousSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (_bassEngine.ChannelPositionB.Value > 3)
        {
            await TryStartSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (Repeat == Data.OsuPlayer.Enums.RepeatMode.SingleSong)
        {
            await TryStartSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (IsShuffle.Value)
        {
            await TryPlaySongAsync(DoShuffle(ShuffleDirection.Backwards), PlayDirection.Backwards);
            return;
        }

        if (Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist)
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
        await TryEnqueueAsync(SongSourceList[prevIndex < 0 ? prevIndex + SongSourceList!.Count : prevIndex], PlayDirection.Backwards);
    }

    /// <summary>
    /// Plays the next song in the list. Or the first if we are at the end
    /// </summary>
    public async void NextSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (Repeat == Data.OsuPlayer.Enums.RepeatMode.SingleSong)
        {
            await TryStartSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (IsShuffle.Value)
        {
            await TryPlaySongAsync(DoShuffle(ShuffleDirection.Forward));
            return;
        }

        if (Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist)
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
        await TryEnqueueAsync(SongSourceList[nextIndex < 0 ? nextIndex + SongSourceList!.Count : nextIndex], PlayDirection.Forward);
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
            Repeat = Data.OsuPlayer.Enums.RepeatMode.NoRepeat;

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

    /// <summary>
    /// Enqueues a song with a given <paramref name="direction" /> and ignores all songs with the same name as the currently
    /// playing song
    /// </summary>
    /// <param name="song">
    /// the song that should be played when the <paramref name="direction" /> is
    /// <see cref="PlayDirection.Normal" />
    /// </param>
    /// <param name="direction">a <see cref="PlayDirection" /> to indicate in which direction the next song should be</param>
    /// <returns>a <see cref="Task" /> from the enqueue try <seealso cref="TryStartSongAsync" /></returns>
    private async Task<Task> TryEnqueueIgnoreSameNameAsync(IMapEntryBase song, PlayDirection direction)
    {
        if (CurrentSong.Value == default)
            return Task.FromException(new NullReferenceException());

        if (RepeatMode.Value == Data.OsuPlayer.Enums.RepeatMode.Playlist)
            return TryStartSongAsync(song);

        var offset = direction switch
        {
            PlayDirection.Backwards => -1,
            PlayDirection.Forward => 1,
            _ => 0
        };

        if (offset == 0)
            return await TryStartSongAsync(song);

        if (CurrentSong.Value.SongName == song.SongName)
        {
            CurrentSong.Value = await song.ReadFullEntry(new Config().Container.OsuPath!);
            switch (direction)
            {
                case PlayDirection.Forward:
                    NextSong();
                    return Task.CompletedTask;
                case PlayDirection.Backwards:
                    PreviousSong();
                    return Task.CompletedTask;
            }
        }

        for (var i = CurrentIndex + offset; i < SongSourceList?.Count; i += offset)
        {
            if (SongSourceList[i].SongName == CurrentSong.Value.SongName) continue;

            return await TryStartSongAsync(SongSourceList[i]);
        }

        return Task.FromException(new InvalidOperationException("There is no song with a different name"));
    }

    /// <summary>
    /// Enqueues a specific song to play
    /// </summary>
    /// <param name="song">The song to enqueue</param>
    /// <param name="playDirection">The direction we went in the playlist. Mostly used by the Next and Prev method</param>
    public async Task TryPlaySongAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal)
    {
        if ((await TryEnqueueAsync(song, playDirection)).IsFaulted)
        {
            if (!SongSourceList?.Any() ?? true)
                return;

            await TryStartSongAsync(SongSourceList[0]);
        }
    }

    /// <summary>
    /// Enqueues a specific song to play
    /// </summary>
    /// <param name="song">The song to enqueue</param>
    /// <param name="playDirection">The direction we went in the playlist. Mostly used by the Next and Prev method</param>
    private async Task<Task> TryEnqueueAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Normal)
    {
        if (SongSourceList == default || !SongSourceList.Any())
            return Task.FromException(new NullReferenceException());

        if (song == default)
            return await TryStartSongAsync(SongSourceList[^1]);

        if ((await new Config().ReadAsync()).IgnoreSongsWithSameNameCheckBox)
            return await TryEnqueueIgnoreSameNameAsync(song, playDirection);

        return await TryStartSongAsync(song);
    }

    /// <summary>
    /// Starts playing a song
    /// </summary>
    /// <param name="song">a <see cref="IMapEntryBase" /> to play next</param>
    /// <returns>a <see cref="Task" /> with the resulting state</returns>
    private async Task<Task> TryStartSongAsync(IMapEntryBase song)
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return Task.FromException(new NullReferenceException($"{nameof(SongSourceList)} can't be null or empty"));

        var config = new Config();

        await config.ReadAsync();

        var fullMapEntry = await song.ReadFullEntry(config.Container.OsuPath!);

        if (fullMapEntry == default)
            return Task.FromException(new NullReferenceException());

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
            PlayState = PlayState.Playing;

            _currentSongTimer.Restart();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return Task.FromException(ex);
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

        return Task.CompletedTask;
    }

    #endregion

    #region Shuffle

    /// <summary>
    /// Implements the shuffle logic <seealso cref="GetNextShuffledIndex" />
    /// </summary>
    /// <param name="direction">the <see cref="ShuffleDirection" /> to shuffle to</param>
    /// <returns>a random/shuffled <see cref="IMapEntryBase" /></returns>
    private IMapEntryBase? DoShuffle(ShuffleDirection direction)
    {
        if (CurrentSong.Value == default || SongSourceList == default)
            throw new NullReferenceException();

        if (Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist && ActivePlaylist == default) ActivePlaylistId = (PlaylistManager.GetAllPlaylists() as List<Playlist>)?.Find(x => x.Name == "Favorites")?.Id;

        switch (direction)
        {
            case ShuffleDirection.Forward:
            {
                // Next index if not at array end
                if (_shuffleHistoryIndex < _shuffleHistory.Length - 1)
                {
                    GetNextShuffledIndex();
                }
                // Move array one down if at the top of the array
                else
                {
                    Array.Copy(_shuffleHistory, 1, _shuffleHistory, 0, _shuffleHistory.Length - 1);

                    _shuffleHistory[_shuffleHistoryIndex] = GenerateShuffledIndex();
                }

                break;
            }
            case ShuffleDirection.Backwards:
            {
                // Prev index if index greater than zero
                if (_shuffleHistoryIndex > 0)
                {
                    GetPreviousShuffledIndex();
                }
                // Move array one up if at the start of the array
                else
                {
                    Array.Copy(_shuffleHistory, 0, _shuffleHistory, 1, _shuffleHistory.Length - 1);

                    _shuffleHistory[_shuffleHistoryIndex] = GenerateShuffledIndex();
                }

                break;
            }
        }

        Debug.WriteLine("ShuffleHistory: " + _shuffleHistoryIndex);

        // ReSharper disable once PossibleInvalidOperationException
        var shuffleIndex = (int) _shuffleHistory[_shuffleHistoryIndex];

        return Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist && ActivePlaylist != default
            ? GetMapEntryFromHash(ActivePlaylist!.Songs[shuffleIndex])
            : SongSourceList![shuffleIndex];
    }

    /// <summary>
    /// Generates the next shuffled index in <see cref="_shuffleHistory" />
    /// <seealso cref="GenerateShuffledIndex" />
    /// </summary>
    private void GetNextShuffledIndex()
    {
        // If there is no "next" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex + 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
                ? ActivePlaylist?.Songs.IndexOf(CurrentSong.Value!.Hash)
                : CurrentIndex;
            _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "next" song in the history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex + 1] < (Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
                    ? ActivePlaylist?.Songs.Count
                    : SongSourceList!.Count))
                _shuffleHistoryIndex++;
            // Generate new shuffled index when not
            else
                _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Generates the previous shuffled index in <see cref="_shuffleHistory" />
    /// <seealso cref="GenerateShuffledIndex" />
    /// </summary>
    private void GetPreviousShuffledIndex()
    {
        // If there is no "prev" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex - 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
                ? ActivePlaylist?.Songs.IndexOf(CurrentSong.Value!.Hash)
                : CurrentIndex;
            _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "prev" song in history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex - 1] < (Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
                    ? ActivePlaylist?.Songs.Count
                    : SongSourceList!.Count))
                _shuffleHistoryIndex--;
            // Generate new shuffled index when not
            else
                _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Generates a new random/shuffled index of available songs in either the <see cref="SongSourceList" /> or
    /// <see cref="ActivePlaylist" /> songs
    /// </summary>
    /// <returns>the index of the new shuffled index</returns>
    private int GenerateShuffledIndex()
    {
        var rdm = new Random();
        var shuffleIndex = rdm.Next(0, Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
            ? ActivePlaylist!.Songs.Count
            : SongSourceList!.Count);

        while (shuffleIndex == (Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
                   ? ActivePlaylist?.Songs.IndexOf(CurrentSong.Value!.Hash)
                   : CurrentIndex)) // || OsuPlayer.Blacklist.IsSongInBlacklist(Songs[shuffleIndex]))
            shuffleIndex = rdm.Next(0, Repeat == Data.OsuPlayer.Enums.RepeatMode.Playlist
                ? ActivePlaylist!.Songs.Count
                : SongSourceList!.Count);

        return shuffleIndex;
    }

    #endregion
}