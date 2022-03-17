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
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO;
using OsuPlayer.IO.DbReader;
using ReactiveUI;

namespace OsuPlayer.Modules.Audio;

public class Player
{
    private readonly int?[] _shuffleHistory = new int?[10];

    private MapEntry? _currentSong;
    private IObservable<Func<MapEntry, bool>>? _filter;

    public ReadOnlyObservableCollection<MapEntry>? FilteredSongEntries;

    private PlayState _playState;

    private bool _shuffle;
    private bool _isMuted;
    private double _oldVolume;
    private int _shuffleHistoryIndex;
    private readonly SourceList<MapEntry> _songSource;
    public List<MapEntry>? SongSource => _songSource.Items.ToList();

    public Player()
    {
        _songSource = new SourceList<MapEntry>();
    }

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

    private bool _repeat;

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
        get => Core.Instance.Config.Volume;
        set
        {
            Core.Instance.Config.Volume = value;
            Core.Instance.Engine.Volume = (float) value / 100;
            Core.Instance.MainWindow.ViewModel!.PlayerControl.RaisePropertyChanged();
            if (value > 0)
                Mute(true);
        }
    }

    private Func<MapEntry, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return _ => true;

        var searchQs = searchText.Split(' ');

        return song =>
        {
            return searchQs.All(x => song.SongName.Contains(x, StringComparison.OrdinalIgnoreCase));
        };
    }

    public async Task ImportSongs()
    {
        _filter = Core.Instance.MainWindow.ViewModel!.SearchView
            .WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);
        
        var songEntries = await Importer.ImportSongs(Core.Instance.Config.OsuPath!)!;
        if (songEntries == null) return;
        _songSource.AddRange(songEntries);
        
        _songSource.Connect().Sort(SortExpressionComparer<MapEntry>.Ascending(x => x.Title))
            .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out FilteredSongEntries).Subscribe();
    }

    public async Task Play(MapEntry? song, PlayDirection playDirection = PlayDirection.Forward)
    {
        if (SongSource == null || !SongSource.Any())
            return;

        if (song == null)
        {
            await TryEnqueueSong(SongSource[^1]);
            return;
        }

        if (_currentSong != null && !Repeat && //Core.Instance.Config.IgnoreSongsWithSameNameCheckBox &&
            _currentSong.SongName == song.SongName)
            switch (playDirection)
            {
                case PlayDirection.Backwards:
                {
                    for (var i = CurrentIndex - 1; i < SongSource.Count; i--)
                    {
                        if (SongSource[i].SongName == _currentSong.SongName) continue;

                        await TryEnqueueSong(SongSource[i]);
                        return;
                    }

                    break;
                }
                case PlayDirection.Forward:
                {
                    for (var i = CurrentIndex + 1; i < SongSource.Count; i++)
                    {
                        if (SongSource[i].SongName == _currentSong.SongName) continue;

                        await TryEnqueueSong(SongSource[i]);
                        return;
                    }

                    break;
                }
            }

        await TryEnqueueSong(song);
    }

    private Task TryEnqueueSong(MapEntry song)
    {
        if (SongSource == null || !SongSource.Any())
            return Task.FromException(new NullReferenceException($"{nameof(SongSource)} can't be null or empty"));
        
        try
        {
            Core.Instance.Engine.OpenFile(song.Fullpath);
            //Core.Instance.Engine.SetAllEq(Core.Instance.Config.Eq);
            Core.Instance.Engine.Volume = (float) Core.Instance.MainWindow.ViewModel!.PlayerControl.Volume / 100;
            Core.Instance.Engine.Play();
            PlayState = PlayState.Playing;
            Core.Instance.MainWindow.ViewModel.PlayerControl.CurrentSong = song;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return Task.FromException(ex);
        }

        CurrentIndex = SongSource.IndexOf(song);

        _currentSong = song;
        return Task.CompletedTask;
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
            //songTimeStamp.Start();
            PlayState = PlayState.Playing;
        }
        else
        {
            Core.Instance.Engine.Pause();
            //songTimeStamp.Stop();
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
            if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
            {
                var song = await DoShuffle(ShuffleDirection.Backwards);

                await Play(song);
            }

            await Play(await DoShuffle(ShuffleDirection.Backwards), PlayDirection.Backwards);

            return;
        }

        if (CurrentIndex - 1 == -1)
        {
            if (false) //OsuPlayer.Blacklist.IsSongInBlacklist(Songs[Songs.Count - 1]))
            {
                CurrentIndex--;
                PreviousSong();
                return;
            }
        }
        else
        {
            if (false) //OsuPlayer.Blacklist.IsSongInBlacklist(Songs[CurrentIndex - 1]))
            {
                CurrentIndex--;
                PreviousSong();
                return;
            }
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

    private Task<MapEntry> DoShuffle(ShuffleDirection direction)
    {
        if (SongSource == null) return Task.FromException<MapEntry>(new ArgumentNullException(nameof(FilteredSongEntries)));
        
        return Task.FromResult(SongSource[new Random().Next(SongSource.Count)]);
    }
}