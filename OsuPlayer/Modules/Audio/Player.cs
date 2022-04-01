using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using LiveChartsCore.Defaults;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;
using ReactiveUI;

namespace OsuPlayer.Modules.Audio;

/// <summary>
/// This class is a wrapper for our <see cref="BassEngine" />.
/// You can play, pause, stop and etc. from this class. Custom logic should also be implemented here
/// </summary>
public class Player
{
    private readonly BassEngine _bassEngine;
    private readonly Stopwatch _currentSongTimer;
    private readonly int?[] _shuffleHistory = new int?[10];
    private int _shuffleHistoryIndex;

    public readonly Bindable<IMapEntry?> CurrentSongBinding = new();

    public readonly Bindable<Bitmap?> CurrentSongImage = new();

    public readonly Bindable<IObservable<Func<IMapEntryBase, bool>>?> Filter = new();

    public readonly Bindable<List<ObservableValue>?> GraphValues = new();

    public readonly Bindable<bool> IsPlaying = new();

    public readonly Bindable<RepeatMode> IsRepeating = new();

    public readonly Bindable<bool> IsShuffle = new();

    public readonly Bindable<bool> SongsLoading = new();
    public readonly Bindable<SourceList<IMapEntryBase>> SongSource = new();
    private bool _isMuted;
    private double _oldVolume;

    private PlayState _playState;

    // private int _shuffleHistoryIndex;

    public ReadOnlyObservableCollection<IMapEntryBase>? FilteredSongEntries;

    public Player(BassEngine bassEngine)
    {
        _bassEngine = bassEngine;

        _bassEngine.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "SongEnd")
                Dispatcher.UIThread.Post(NextSong);
        };

        SongSource.Value = new SourceList<IMapEntryBase>();

        _currentSongTimer = new Stopwatch();
    }

    private IMapEntry? CurrentSong
    {
        get => CurrentSongBinding.Value;
        set
        {
            CurrentSongBinding.Value = value;

            CurrentIndex = SongSourceList!.FindIndex(x => x.BeatmapChecksum == value!.BeatmapChecksum);

            using var config = new Config();

            config.Read().LastPlayedSong = CurrentIndex;

            // _mainWindow.ViewModel!.PlayerControl.CurrentSongImage = Task.Run(value!.FindBackground).Result;
        }
    }

    public List<IMapEntryBase>? SongSourceList => SongSource.Value.Items.ToList();

    private SongImporter Importer => new();

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
        get => IsRepeating.Value;
        set => IsRepeating.Value = value;
    }

    public async Task ImportSongs()
    {
        SongsLoading.Value = true;

        await using var config = new Config();
        var songEntries = await SongImporter.ImportSongs((await config.ReadAsync()).OsuPath!)!;
        if (songEntries == null) return;
        SongSource.Value = songEntries.ToSourceList();

        if (Filter.Value != null)
            SongSource.Value.Connect().Sort(SortExpressionComparer<IMapEntryBase>.Ascending(x => x.Title))
                .Filter(Filter.Value, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
                .Bind(out FilteredSongEntries).Subscribe();

        SongsLoading.Value = false;

        if (SongSourceList == null || !SongSourceList.Any()) return;

        await using var cfg = new Config();
        var configContainer = await cfg.ReadAsync();
        switch (configContainer.StartupSong)
        {
            case StartupSong.FirstSong:
                await PlayAsync(SongSourceList[0]);
                break;
            case StartupSong.LastPlayed:
                if (configContainer.LastPlayedSong < SongSourceList.Count && configContainer.LastPlayedSong >= 0)
                    await PlayAsync(SongSourceList[configContainer.LastPlayedSong]);
                else
                    await PlayAsync(SongSourceList[0]);
                break;
            case StartupSong.RandomSong:
                await PlayAsync(SongSourceList[new Random().Next(SongSourceList.Count)]);
                break;
        }

        _bassEngine.Volume = configContainer.Volume;
    }

    public void SetPlaybackSpeed(double speed)
    {
        _bassEngine.SetSpeed(speed);
    }

    public async Task PlayAsync(IMapEntryBase? song, PlayDirection playDirection = PlayDirection.Forward)
    {
        if (SongSourceList == default || !SongSourceList.Any())
            return;

        if (song == default)
        {
            if ((await TryEnqueueSongAsync(SongSourceList[^1])).IsFaulted)
                await TryEnqueueSongAsync(SongSourceList![0]);
            return;
        }

        if (CurrentSongBinding.Value != null && Repeat != RepeatMode.SingleSong
                                             && (await new Config().ReadAsync()).IgnoreSongsWithSameNameCheckBox
                                             && CurrentSongBinding.Value.BeatmapChecksum == song.BeatmapChecksum)
            if ((await EnqueueSongFromDirectionAsync(playDirection)).IsFaulted)
                await TryEnqueueSongAsync(SongSourceList![^1]);

        if ((await TryEnqueueSongAsync(song)).IsFaulted)
            await TryEnqueueSongAsync(SongSourceList![^1]);
    }

    private async Task<Task> EnqueueSongFromDirectionAsync(PlayDirection direction)
    {
        switch (direction)
        {
            case PlayDirection.Backwards:
            {
                for (var i = CurrentIndex - 1; i < SongSourceList?.Count; i--)
                {
                    if (SongSourceList[i].BeatmapChecksum == CurrentSongBinding.Value!.BeatmapChecksum) continue;

                    return await TryEnqueueSongAsync(SongSourceList[i]);
                }

                break;
            }
            case PlayDirection.Forward:
            {
                for (var i = CurrentIndex + 1; i < SongSourceList?.Count; i++)
                {
                    if (SongSourceList[i].BeatmapChecksum == CurrentSongBinding.Value!.BeatmapChecksum) continue;

                    return await TryEnqueueSongAsync(SongSourceList[i]);
                }

                break;
            }
        }

        return Task.FromException(new InvalidOperationException($"Direction {direction} is not valid"));
    }

    private async Task<Task> TryEnqueueSongAsync(IMapEntryBase song)
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return Task.FromException(new NullReferenceException($"{nameof(SongSourceList)} can't be null or empty"));

        IMapEntry fullMapEntry;
        var path = string.Empty;

        await using (var config = new Config())
        {
            await config.ReadAsync();

            path = config.Container.OsuPath!;
            fullMapEntry = await song.ReadFullEntry(config.Container.OsuPath!);

            if (fullMapEntry == default)
                return Task.FromException(new NullReferenceException());

            fullMapEntry.UseUnicode = config.Container.UseSongNameUnicode;
        }

        //We put the XP update to an own try catch because if the API fails or is not available,
        //that the whole TryEnqueue does not fail
        try
        {
            if (CurrentSongBinding.Value != default)
                await UpdateXp();
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

        CurrentSong = fullMapEntry;

        //Same as update XP mentioned Above
        try
        {
            if (CurrentSongBinding.Value != default)
                await UpdateSongsPlayed(fullMapEntry.BeatmapSetId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update Songs Played error => {e}");
        }

        CurrentSongImage.Value = await fullMapEntry.FindBackground();
        return Task.CompletedTask;
    }

    private async Task UpdateXp()
    {
        if (ProfileManager.User == default) return;

        var currentTotalXp = ProfileManager.User.TotalXp;

        _currentSongTimer.Stop();

        var time = (double)_currentSongTimer.ElapsedMilliseconds / 1000;

        var response = await ApiAsync.UpdateXpFromCurrentUserAsync(
            CurrentSongBinding.Value?.BeatmapChecksum ?? string.Empty,
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

    private async Task UpdateSongsPlayed(int beatmapSetId)
    {
        if (ProfileManager.User == default) return;

        var response = await ApiAsync.UpdateSongsPlayedForCurrentUserAsync(1, beatmapSetId);

        if (response == default) return;

        ProfileManager.User = response;

        UserChanged.Invoke(this, new PropertyChangedEventArgs("SongsPlayed"));
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

    public void PlayPause()
    {
        if (PlayState == PlayState.Paused)
        {
            _bassEngine.Play();
            _currentSongTimer.Start();
            PlayState = PlayState.Playing;
        }
        else
        {
            _bassEngine.Pause();
            _currentSongTimer.Stop();
            PlayState = PlayState.Paused;
        }
    }

    public void Play()
    {
        _bassEngine.Play();
        PlayState = PlayState.Playing;
    }

    public void Pause()
    {
        _bassEngine.Pause();
        PlayState = PlayState.Paused;
    }

    public async void NextSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (Repeat == RepeatMode.SingleSong)
        {
            await PlayAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (IsShuffle.Value)
        {
            await PlayAsync(await DoShuffle(ShuffleDirection.Forward));

            return;
        }

        if (CurrentIndex + 1 == SongSourceList.Count)
        {
            // if (OsuPlayer.Blacklist.IsSongInBlacklist(Songs[0]))
            // {
            //     CurrentIndex++;
            //     await NextSong();
            //     return;
            // }
        }

        if (Repeat == RepeatMode.Playlist)
        {
            if (ActivePlaylist == default || ActivePlaylist.Songs.Count == 0)
            {
                // OsuPlayerMessageBox.Show(
                //    OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.noPlaylistSelected"));
                Repeat = RepeatMode.NoRepeat;

                await PlayAsync(CurrentIndex == SongSourceList.Count - 1 ? SongSourceList[0] : SongSourceList[CurrentIndex + 1],
                    PlayDirection.Forward);

                return;
            }

            var currentPlaylistIndex = ActivePlaylist.Songs.IndexOf(CurrentSong!.BeatmapSetId);

            if (currentPlaylistIndex == ActivePlaylist.Songs.Count - 1)
                await PlayAsync(GetMapEntryFromSetId(ActivePlaylist.Songs[0]));
            else
                await PlayAsync(GetMapEntryFromSetId(ActivePlaylist.Songs[currentPlaylistIndex + 1]));

            return;
        }

        await PlayAsync(CurrentIndex == SongSourceList.Count - 1
            ? SongSourceList[0]
            : SongSourceList[CurrentIndex + 1]);
    }

    public Playlist? ActivePlaylist => ActivePlaylistName != default
        ? PlaylistManager.GetAllPlaylists().First(x => x.Name == ActivePlaylistName)
        : default;

    public string? ActivePlaylistName { get; set; }

    public async void PreviousSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (_bassEngine.ChannelPositionB.Value > 3)
        {
            await TryEnqueueSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (IsShuffle.Value)
        {
            await PlayAsync(await DoShuffle(ShuffleDirection.Backwards), PlayDirection.Backwards);

            return;
        }

        if (CurrentIndex - 1 == -1)
        {
            // if (false) //OsuPlayer.Blacklist.IsSongInBlacklist(Songs[Songs.Count - 1]))
            // {
            //     CurrentIndex--;
            //     PreviousSong();
            //     return;
            // }
        }

        if (Repeat == RepeatMode.Playlist)
        {
            if (ActivePlaylist == default || ActivePlaylist.Songs.Count == 0)
            {
                // OsuPlayerMessageBox.Show(
                //    OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.noPlaylistSelected"));
                Repeat = RepeatMode.NoRepeat;

                await PlayAsync(CurrentIndex <= 0 ? SongSourceList[^1] : SongSourceList[CurrentIndex - 1], PlayDirection.Forward);

                return;
            }

            var currentPlaylistIndex = ActivePlaylist.Songs.IndexOf(CurrentSong!.BeatmapSetId);

            if (currentPlaylistIndex <= 0)
                await PlayAsync(GetMapEntryFromSetId(ActivePlaylist.Songs[^1]));
            else
                await PlayAsync(GetMapEntryFromSetId(ActivePlaylist.Songs[currentPlaylistIndex - 1]));

            return;
        }

        if (SongSourceList == null) return;
        await PlayAsync(CurrentIndex == 0 ? SongSourceList.Last() : SongSourceList[CurrentIndex - 1],
            PlayDirection.Backwards);
    }

    #region Shuffle

    private Task<IMapEntryBase> DoShuffle(ShuffleDirection direction)
    {
        if ((Repeat == RepeatMode.Playlist && ActivePlaylist == default) || CurrentSong == default || SongSourceList == default)
            return Task.FromException<IMapEntryBase>(new NullReferenceException());

        switch (direction)
        {
            case ShuffleDirection.Forward:
            {
                // Next index if not at array end
                if (_shuffleHistoryIndex < _shuffleHistory.Length - 1)
                    GetNextShuffledIndex();
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
                    GetPreviousShuffledIndex();
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
        var shuffleIndex = (int)_shuffleHistory[_shuffleHistoryIndex];

        return Task.FromResult(Repeat == RepeatMode.Playlist
            ? GetMapEntryFromSetId(ActivePlaylist!.Songs[shuffleIndex])
            : SongSourceList![shuffleIndex]);
    }

    private void GetNextShuffledIndex()
    {
        // If there is no "next" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex + 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = Repeat == RepeatMode.Playlist
                ? ActivePlaylist?.Songs.IndexOf(CurrentSong!.BeatmapSetId)
                : CurrentIndex;
            _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "next" song in the history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex + 1] < (Repeat == RepeatMode.Playlist
                    ? ActivePlaylist?.Songs.Count
                    : SongSourceList!.Count))
                _shuffleHistoryIndex++;
            // Generate new shuffled index when not
            else
                _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    private void GetPreviousShuffledIndex()
    {
        // If there is no "prev" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex - 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = Repeat == RepeatMode.Playlist
                ? ActivePlaylist?.Songs.IndexOf(CurrentSong!.BeatmapSetId)
                : CurrentIndex;
            _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "prev" song in history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex - 1] < (Repeat == RepeatMode.Playlist
                    ? ActivePlaylist?.Songs.Count
                    : SongSourceList!.Count))
                _shuffleHistoryIndex--;
            // Generate new shuffled index when not
            else
                _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    private int GenerateShuffledIndex()
    {
        var rdm = new Random();
        var shuffleIndex = rdm.Next(0, Repeat == RepeatMode.Playlist
            ? ActivePlaylist!.Songs.Count
            : SongSourceList!.Count);

        while (shuffleIndex == (Repeat == RepeatMode.Playlist
                   ? ActivePlaylist?.Songs.IndexOf(CurrentSong!.BeatmapSetId)
                   : CurrentIndex)) // || OsuPlayer.Blacklist.IsSongInBlacklist(Songs[shuffleIndex]))
        {
            shuffleIndex = rdm.Next(0, Repeat == RepeatMode.Playlist
                ? ActivePlaylist!.Songs.Count
                : SongSourceList!.Count);
        }

        return shuffleIndex;
    }

    #endregion

    public IMapEntryBase? GetMapEntryFromSetId(int setId)
    {
        return SongSourceList!.FirstOrDefault(x => x.BeatmapSetId == setId);
    }

    public List<IMapEntryBase> GetMapEntriesFromSetId(ICollection<int> setId)
    {
        return SongSourceList!.Where(x => setId.Contains(x.BeatmapSetId)).ToList();
    }
}