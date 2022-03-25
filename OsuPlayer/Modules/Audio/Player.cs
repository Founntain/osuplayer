using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using ManagedBass;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.IO;
using OsuPlayer.IO.DbReader;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Config;
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
    private readonly Stopwatch _currentSongTimer;
    private readonly int?[] _shuffleHistory = new int?[10];
    private readonly SourceList<MinimalMapEntry> _songSource;

    private MapEntry? _currentSong;

    private IObservable<Func<MinimalMapEntry, bool>>? _filter;
    private bool _isMuted;
    private double _oldVolume;

    private PlayState _playState;

    private bool _repeat;

    private bool _shuffle;
    // private int _shuffleHistoryIndex;

    private double _volume = new Config().Read().Volume;

    public ReadOnlyObservableCollection<MinimalMapEntry>? FilteredSongEntries;

    public Player()
    {
        _songSource = new SourceList<MinimalMapEntry>();

        _currentSongTimer = new();
    }

    private MapEntry? CurrentSong
    {
        get => _currentSong;
        set
        {
            _currentSong = value;

            CurrentIndex = SongSource!.FindIndex(x => x.BeatmapChecksum == value!.BeatmapChecksum);

            using var config = new Config();

            config.Read().LastPlayedSong = CurrentIndex;

            Core.Instance.MainWindow.ViewModel!.PlayerControl.CurrentSong = value;

            // Core.Instance.MainWindow.ViewModel!.PlayerControl.CurrentSongImage = Task.Run(value!.FindBackground).Result;
        }
    }

    public List<MinimalMapEntry>? SongSource => _songSource.Items.ToList();

    private SongImporter Importer => new();

    private PlayState PlayState
    {
        get => _playState;
        set
        {
            Core.Instance.MainWindow.ViewModel!.PlayerControl.IsPlaying = value == PlayState.Playing;
            _playState = value;
        }
    }

    private int CurrentIndex { get; set; }

    public bool Shuffle
    {
        get => _shuffle;
        set
        {
            _shuffle = value;
            Core.Instance.MainWindow.ViewModel!.PlayerControl.IsShuffle = value;
        }
    }

    public bool Repeat
    {
        get => _repeat;
        set
        {
            _repeat = value;
            Core.Instance.MainWindow.ViewModel!.PlayerControl.IsRepeating = value;
        }
    }

    public double Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            Core.Instance.Engine.Volume = (float) value / 100;
            Core.Instance.MainWindow.ViewModel!.PlayerControl.RaisePropertyChanged();
            if (value > 0)
                Mute(true);
        }
    }

    private Func<MinimalMapEntry, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return _ => true;

        var searchQs = searchText.Split(' ');

        return song =>
        {
            return searchQs.All(x =>
                song.Title.Contains(x, StringComparison.OrdinalIgnoreCase) ||
                song.Artist.Contains(x, StringComparison.OrdinalIgnoreCase));
        };
    }

    public async Task ImportSongs()
    {
        Core.Instance.MainWindow.ViewModel!.HomeView.SongsLoading = true;

        _filter = Core.Instance.MainWindow.ViewModel!.SearchView
            .WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        await using var config = new Config();
        var songEntries = await SongImporter.ImportSongs((await config.ReadAsync()).OsuPath!)!;
        if (songEntries == null) return;
        _songSource.AddRange(songEntries);

        _songSource.Connect().Sort(SortExpressionComparer<MinimalMapEntry>.Ascending(x => x.Title))
            .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out FilteredSongEntries).Subscribe();

        Core.Instance.MainWindow.ViewModel.HomeView.RaisePropertyChanged(nameof(Core.Instance.MainWindow.ViewModel
            .HomeView.SongEntries));
        Core.Instance.MainWindow.ViewModel!.HomeView.SongsLoading = false;

        if (SongSource == null || !SongSource.Any()) return;

        await using var cfg = new Config();
        var configContainer = await cfg.ReadAsync();
        switch (configContainer.StartupSong)
        {
            case StartupSong.FirstSong:
                await Play(SongSource[0]);
                break;
            case StartupSong.LastPlayed:
                if (configContainer.LastPlayedSong < SongSource.Count && configContainer.LastPlayedSong >= 0)
                    await Play(SongSource[configContainer.LastPlayedSong]);
                else
                    await Play(SongSource[0]);
                break;
            case StartupSong.RandomSong:
                await Play(SongSource[new Random().Next(SongSource.Count)]);
                break;
        }
    }

    public void SetPlaybackSpeed(double speed)
    {
        Bass.ChannelSetAttribute(Core.Instance.Engine.FxStream, ChannelAttribute.TempoFrequency,
            Core.Instance.Engine.SampleFrequency * (1 + speed));
    }

    public async Task Play(MinimalMapEntry? song, PlayDirection playDirection = PlayDirection.Forward)
    {
        if (SongSource == default || !SongSource.Any())
            return;

        if (song == default)
        {
            await TryEnqueueSong(SongSource[^1]);
            return;
        }

        if (CurrentSong != null && !Repeat
                                && (await new Config().ReadAsync()).IgnoreSongsWithSameNameCheckBox
                                && CurrentSong.BeatmapChecksum == song.BeatmapChecksum)
            await EnqueueSongFromDirection(playDirection);

        await TryEnqueueSong(song);
    }

    private async Task EnqueueSongFromDirection(PlayDirection direction)
    {
        switch (direction)
        {
            case PlayDirection.Backwards:
            {
                for (var i = CurrentIndex - 1; i < SongSource?.Count; i--)
                {
                    if (SongSource[i].BeatmapChecksum == _currentSong!.BeatmapChecksum) continue;

                    await TryEnqueueSong(SongSource[i]);

                    return;
                }

                break;
            }
            case PlayDirection.Forward:
            {
                for (var i = CurrentIndex + 1; i < SongSource?.Count; i++)
                {
                    if (SongSource[i].BeatmapChecksum == _currentSong!.BeatmapChecksum) continue;

                    await TryEnqueueSong(SongSource[i]);

                    return;
                }

                break;
            }
        }
    }

    private async Task<Task> TryEnqueueSong(MinimalMapEntry song)
    {
        if (SongSource == null || !SongSource.Any())
            return Task.FromException(new NullReferenceException($"{nameof(SongSource)} can't be null or empty"));

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
            if (CurrentSong != default)
                await UpdateXp();
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update XP error => {e}");
        }

        try
        {
            Core.Instance.Engine.OpenFile(fullMapEntry.FullPath!);
            //Core.Instance.Engine.SetAllEq(Core.Instance.Config.Eq);
            Core.Instance.Engine.Volume = (float) Core.Instance.MainWindow.ViewModel!.PlayerControl.Volume / 100;
            Core.Instance.Engine.Play();
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
            if (CurrentSong != default)
                await UpdateSongsPlayed(fullMapEntry.BeatmapSetId);
        }
        catch (Exception e)
        {
            Debug.WriteLine($"Could not update Songs Played error => {e}");
        }

        Core.Instance.MainWindow.ViewModel.PlayerControl.CurrentSongImage = await fullMapEntry.FindBackground();
        return Task.CompletedTask;
    }

    private async Task UpdateXp()
    {
        if (ProfileManager.User == default) return;

        var currentTotalXp = ProfileManager.User?.TotalXp ?? 0;

        _currentSongTimer.Stop();

        var time = (double) _currentSongTimer.ElapsedMilliseconds / 1000;

        var response = await ApiAsync.UpdateXpFromCurrentUserAsync(
            CurrentSong?.BeatmapChecksum ?? string.Empty,
            time,
            Core.Instance.Engine.ChannelLength);

        if (response == default) return;

        ProfileManager.User = response;

        var xpEarned = response.TotalXp - currentTotalXp;

        var values = Core.Instance.MainWindow.ViewModel!.HomeView.GraphValues.ToList();

        values.Add(new(xpEarned));

        Core.Instance.MainWindow.ViewModel!.HomeView.GraphValues = values.ToObservableCollection();

        Core.Instance.MainWindow.ViewModel!.HomeView.RaisePropertyChanged(nameof(Core.Instance.MainWindow.ViewModel
            .HomeView.CurrentUser));
        Core.Instance.MainWindow.ViewModel!.HomeView.RaisePropertyChanged(nameof(Core.Instance.MainWindow.ViewModel
            .HomeView.Series));
        Core.Instance.MainWindow.ViewModel!.HomeView.RaisePropertyChanged(nameof(Core.Instance.MainWindow.ViewModel
            .HomeView.GraphValues));
    }

    private async Task UpdateSongsPlayed(int beatmapSetId)
    {
        if (ProfileManager.User == default) return;

        var response = await ApiAsync.UpdateSongsPlayedForCurrentUserAsync(1, beatmapSetId);

        if (response == default) return;

        ProfileManager.User = response;

        Core.Instance.MainWindow.ViewModel!.HomeView.RaisePropertyChanged(nameof(Core.Instance.MainWindow.ViewModel
            .HomeView.CurrentUser));
    }

    public void Mute(bool force = false)
    {
        if (force)
        {
            _isMuted = false;
            return;
        }

        if (!_isMuted)
        {
            _oldVolume = Volume;
            Volume = 0;
            _isMuted = true;
        }
        else
        {
            Volume = _oldVolume;
        }
    }

    public void PlayPause()
    {
        if (PlayState == PlayState.Paused)
        {
            Core.Instance.Engine.Play();
            _currentSongTimer.Start();
            PlayState = PlayState.Playing;
        }
        else
        {
            Core.Instance.Engine.Pause();
            _currentSongTimer.Stop();
            PlayState = PlayState.Paused;
        }
    }

    public void Play()
    {
        Core.Instance.Engine.Play();
        PlayState = PlayState.Playing;
    }

    public void Pause()
    {
        Core.Instance.Engine.Pause();
        PlayState = PlayState.Paused;
    }

    public async void NextSong()
    {
        if (SongSource == null || !SongSource.Any())
            return;

        if (Repeat)
        {
            await Play(SongSource[CurrentIndex]);
            return;
        }

        if (Shuffle)
        {
            if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
            {
                // var song = DoShuffle(ShuffleDirection.Forward);
                //
                // await Play(song, PlayDirection.Forward);
            }

            await Play(await DoShuffle(ShuffleDirection.Forward));

            return;
        }

        if (CurrentIndex + 1 == SongSource.Count)
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

        await Play(CurrentIndex == SongSource.Count - 1
            ? SongSource[0]
            : SongSource[CurrentIndex + 1]);
    }

    public async void PreviousSong()
    {
        if (SongSource == null || !SongSource.Any())
            return;

        if (Core.Instance.Engine.ChannelPosition > 3)
        {
            await TryEnqueueSong(SongSource[CurrentIndex]);
            return;
        }

        if (Shuffle)
        {
            // if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
            // {
            //     var song = await DoShuffle(ShuffleDirection.Backwards);
            //
            //     await Play(song);
            // }

            await Play(await DoShuffle(ShuffleDirection.Backwards), PlayDirection.Backwards);

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

        if (SongSource == null) return;
        await Play(CurrentIndex == 0 ? SongSource.Last() : SongSource[CurrentIndex - 1],
            PlayDirection.Backwards);
    }

    private Task<MinimalMapEntry> DoShuffle(ShuffleDirection direction)
    {
        if (SongSource == null)
            return Task.FromException<MinimalMapEntry>(new ArgumentNullException(nameof(FilteredSongEntries)));

        return Task.FromResult(SongSource[new Random().Next(SongSource.Count)]);
    }

    public MinimalMapEntry? GetMapEntryFromChecksum(string checksum)
    {
        return SongSource!.FirstOrDefault(x => x.BeatmapChecksum == checksum);
    }

    public List<MinimalMapEntry> GetMapEntriesFromChecksums(ICollection<string> checksums)
    {
        return SongSource!.Where(x => checksums.Contains(x.BeatmapChecksum)).ToList();
    }
}