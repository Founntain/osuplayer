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
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;
using OsuPlayer.Network.API.ApiEndpoints;
using OsuPlayer.Network.Online;

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

    public readonly Bindable<MapEntry?> CurrentSongBinding = new();

    public readonly Bindable<Bitmap?> CurrentSongImage = new();

    public readonly Bindable<IObservable<Func<MinimalMapEntry, bool>>?> Filter = new();

    public readonly Bindable<List<ObservableValue>?> GraphValues = new();

    public readonly Bindable<bool> IsPlaying = new();

    public readonly Bindable<bool> IsRepeating = new();

    public readonly Bindable<bool> Shuffle = new();

    public readonly Bindable<bool> SongsLoading = new();
    public readonly Bindable<SourceList<MinimalMapEntry>> SongSource = new();
    private bool _isMuted;
    private double _oldVolume;

    private PlayState _playState;

    private bool _repeat;
    // private int _shuffleHistoryIndex;

    public ReadOnlyObservableCollection<MinimalMapEntry>? FilteredSongEntries;

    public Player(BassEngine bassEngine)
    {
        _bassEngine = bassEngine;

        _bassEngine.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == "SongEnd")
                Dispatcher.UIThread.Post(NextSong);
        };

        SongSource.Value = new SourceList<MinimalMapEntry>();

        _currentSongTimer = new Stopwatch();
    }

    private MapEntry? CurrentSong
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

    public List<MinimalMapEntry>? SongSourceList => SongSource.Value.Items.ToList();

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

    public bool Repeat
    {
        get => _repeat;
        set
        {
            _repeat = value;
            IsRepeating.Value = value;
        }
    }

    public async Task ImportSongs()
    {
        SongsLoading.Value = true;

        await using var config = new Config();
        var songEntries = await SongImporter.ImportSongs((await config.ReadAsync()).OsuPath!)!;
        if (songEntries == null) return;
        SongSource.Value = songEntries.ToSourceList();

        if (Filter.Value != null)
            SongSource.Value.Connect().Sort(SortExpressionComparer<MinimalMapEntry>.Ascending(x => x.Title))
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

    public async Task PlayAsync(MinimalMapEntry? song, PlayDirection playDirection = PlayDirection.Forward)
    {
        if (SongSourceList == default || !SongSourceList.Any())
            return;

        if (song == default)
        {
            if ((await TryEnqueueSongAsync(SongSourceList[^1])).IsFaulted)
                await TryEnqueueSongAsync(SongSourceList![0]);
            return;
        }

        if (CurrentSongBinding.Value != null && !Repeat
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

    private async Task<Task> TryEnqueueSongAsync(MinimalMapEntry song)
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return Task.FromException(new NullReferenceException($"{nameof(SongSourceList)} can't be null or empty"));

        MapEntry fullMapEntry;

        await using (var config = new Config())
        {
            await config.ReadAsync();

            fullMapEntry = await DbReader.ReadFullMapEntry(config.Container.OsuPath!, song.DbOffset);

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

        if (Repeat)
        {
            await PlayAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (Shuffle.Value)
        {
            if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
            {
                // var song = DoShuffle(ShuffleDirection.Forward);
                //
                // await Play(song, PlayDirection.Forward);
            }

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

        if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
        {
            // if (OsuPlayer.PlaylistManager.CurrentPlaylist == null)
            // {
            //     OsuPlayerMessageBox.Show(
            //         OsuPlayer.LanguageService.LoadControlLanguageWithKey("message.noPlaylistSelected"));
            //     OsuPlayer.PlaylistManager.IsPlaylistmode = false;
            //
            //     await Play(CurrentIndex == Songs.Count - 1 ? Songs[0] : Songs[CurrentIndex + 1], PlayDirection.Forward);
            //
            //     return;
            // }
            //
            // if (OsuPlayer.PlaylistManager.CurrentPlaylist.GetPlayistIndexOfSong(CurrentSong) ==
            //     OsuPlayer.PlaylistManager.CurrentPlaylist.Songs.Count - 1)
            //     await Play(OsuPlayer.PlaylistManager.CurrentPlaylist.Songs[0]);
            // else
            //     await Play(OsuPlayer.PlaylistManager.CurrentPlaylist.Songs[
            //         OsuPlayer.PlaylistManager.CurrentPlaylist.GetPlayistIndexOfSong(CurrentSong) + 1]);
        }

        await PlayAsync(CurrentIndex == SongSourceList.Count - 1
            ? SongSourceList[0]
            : SongSourceList[CurrentIndex + 1]);
    }

    public async void PreviousSong()
    {
        if (SongSourceList == null || !SongSourceList.Any())
            return;

        if (_bassEngine.ChannelPositionB.Value > 3)
        {
            await TryEnqueueSongAsync(SongSourceList[CurrentIndex]);
            return;
        }

        if (Shuffle.Value)
        {
            // if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
            // {
            //     var song = await DoShuffle(ShuffleDirection.Backwards);
            //
            //     await Play(song);
            // }

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

        if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
        {
            // if (OsuPlayer.PlaylistManager.CurrentPlaylist.GetPlayistIndexOfSong(CurrentSong) == 0)
            //     await Play(OsuPlayer.PlaylistManager.CurrentPlaylist.Songs.Last());
            // else
            //     await Play(OsuPlayer.PlaylistManager.CurrentPlaylist.Songs[
            //         OsuPlayer.PlaylistManager.CurrentPlaylist.GetPlayistIndexOfSong(CurrentSong) - 1]);
        }

        if (SongSourceList == null) return;
        await PlayAsync(CurrentIndex == 0 ? SongSourceList.Last() : SongSourceList[CurrentIndex - 1],
            PlayDirection.Backwards);
    }

    private Task<MinimalMapEntry> DoShuffle(ShuffleDirection direction)
    {
        if (SongSourceList == null)
            return Task.FromException<MinimalMapEntry>(new ArgumentNullException(nameof(FilteredSongEntries)));

        return Task.FromResult(SongSourceList[new Random().Next(SongSourceList.Count)]);
    }

    public MinimalMapEntry? GetMapEntryFromChecksum(string checksum)
    {
        return SongSourceList!.FirstOrDefault(x => x.BeatmapChecksum == checksum);
    }

    public List<MinimalMapEntry> GetMapEntriesFromChecksums(ICollection<string> checksums)
    {
        return SongSourceList!.Where(x => checksums.Contains(x.BeatmapChecksum)).ToList();
    }
}