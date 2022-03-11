using System;
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
using ReactiveUI;

namespace OsuPlayer.Modules.Audio;

public class Player
{
    private readonly int?[] _shuffleHistory = new int?[10];

    public SongEntry CurrentSong;
    private readonly IObservable<Func<SongEntry, bool>> _filter;

    public ReadOnlyObservableCollection<SongEntry> FilteredSongEntries;

    private Playstate _playstate;

    private bool _shuffle;
    private int _shuffleHistoryIndex;
    private readonly SourceList<SongEntry> _songSource;

    public Player()
    {
        _songSource = new SourceList<SongEntry>();
        _filter = Core.Instance.MainWindow.ViewModel!.SearchView
            .WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);
        Core.Instance.MainWindow.ViewModel.PlayerControl.Volume = Core.Instance.Config.Volume;
    }

    private SongImporter Importer => new();

    public Playstate Playstate
    {
        get => _playstate;
        set
        {
            Core.Instance.MainWindow.ViewModel!.PlayerControl.IsPlaying = value == Playstate.Playing;
            _playstate = value;
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

    public bool Repeat { get; set; }

    private Func<SongEntry, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return song => true;

        return song => song.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                       song.Artist.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    public async void ImportSongs()
    {
        var songEntries = Importer.ImportSongs(Core.Instance.Config.OsuPath!);
        foreach (var songEntry in songEntries) _songSource.Add(songEntry);

        _songSource.Connect().Sort(SortExpressionComparer<SongEntry>.Ascending(x => x.Title))
            .Filter(_filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out FilteredSongEntries).Subscribe();
    }

    public async Task Play(SongEntry song, PlayDirection playDirection = PlayDirection.Forward)
    {
        if (!FilteredSongEntries.Any())
            return;

        if (song == null)
        {
            await EnqueueSong(FilteredSongEntries[^1]);
            return;
        }

        if (CurrentSong != null && !Repeat && //Core.Instance.Config.IgnoreSongsWithSameNameCheckBox &&
            CurrentSong.SongName == song.SongName)
            switch (playDirection)
            {
                case PlayDirection.Backwards:
                {
                    for (var i = CurrentIndex - 1; i < FilteredSongEntries.Count; i--)
                    {
                        if (FilteredSongEntries[i].SongName == CurrentSong.SongName) continue;

                        await EnqueueSong(FilteredSongEntries[i]);
                        return;
                    }

                    break;
                }
                case PlayDirection.Forward:
                {
                    for (var i = CurrentIndex + 1; i < FilteredSongEntries.Count; i++)
                    {
                        if (FilteredSongEntries[i].SongName == CurrentSong.SongName) continue;

                        await EnqueueSong(FilteredSongEntries[i]);
                        return;
                    }

                    break;
                }
            }

        await EnqueueSong(song);
    }

    private Task EnqueueSong(SongEntry song)
    {
        try
        {
            Core.Instance.Engine.OpenFile(song.Fullpath);
            //Core.Instance.Engine.SetAllEq(Core.Instance.Config.Eq);
            Core.Instance.Engine.SetVolume((float) Core.Instance.MainWindow.ViewModel!.PlayerControl.Volume / 100);
            Core.Instance.Engine.Play();
            Playstate = Playstate.Playing;
            Core.Instance.MainWindow.ViewModel.TopBar.CurrentSongText = $"{song.Artist} - {song.Title}";
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
        }

        CurrentIndex = FilteredSongEntries.IndexOf(song);

        CurrentSong = song;
        return Task.CompletedTask;
    }

    public void PlayPause()
    {
        if (Playstate == Playstate.Paused)
        {
            Core.Instance.Engine.Play();
            //songTimeStamp.Start();
            Playstate = Playstate.Playing;
        }
        else
        {
            Core.Instance.Engine.Pause();
            //songTimeStamp.Stop();
            Playstate = Playstate.Paused;
        }
    }

    public void Play()
    {
        Core.Instance.Engine.Play();
        Playstate = Playstate.Playing;
    }

    public void Pause()
    {
        Core.Instance.Engine.Pause();
        Playstate = Playstate.Paused;
    }

    public async void NextSong()
    {
        if (!FilteredSongEntries.Any())
            return;

        if (Repeat)
        {
            await Play(FilteredSongEntries[CurrentIndex]);
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

            await Play(DoShuffle(ShuffleDirection.Forward));

            return;
        }

        if (CurrentIndex + 1 == FilteredSongEntries.Count)
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

        await Play(CurrentIndex == FilteredSongEntries.Count - 1
            ? FilteredSongEntries[0]
            : FilteredSongEntries[CurrentIndex + 1]);
    }

    public async void PreviousSong()
    {
        if (Core.Instance.Engine.ChannelPosition > 3)
        {
            await EnqueueSong(FilteredSongEntries[CurrentIndex]);
            return;
        }

        if (Shuffle)
        {
            if (false) //OsuPlayer.PlaylistManager.IsPlaylistmode)
            {
                var song = DoShuffle(ShuffleDirection.Backwards);

                await Play(song);
            }

            await Play(DoShuffle(ShuffleDirection.Backwards), PlayDirection.Backwards);

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

        await Play(CurrentIndex == 0 ? FilteredSongEntries.Last() : FilteredSongEntries[CurrentIndex - 1],
            PlayDirection.Backwards);
    }

    public SongEntry DoShuffle(ShuffleDirection direction)
    {
        return FilteredSongEntries[new Random().Next(FilteredSongEntries.Count)];
    }
}