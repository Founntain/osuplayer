using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using LiveChartsCore.Defaults;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Interfaces;
using OsuPlayer.Windows;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public partial class PlayerControlView : ReactiveControl<PlayerControlViewModel>
{
    private FluentAppWindow? _mainWindow;

    public PlayerControlView()
    {
        InitializeComponent();

        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is FluentAppWindow mainWindow)
                _mainWindow = mainWindow;

            SongProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed, RoutingStrategies.Tunnel);

            SongProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased, RoutingStrategies.Tunnel);

            Repeat.AddHandler(PointerReleasedEvent, Repeat_OnPointerReleased, RoutingStrategies.Tunnel);

            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsAPlaylistSelected));
            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongOnBlacklist));

            ViewModel.AudioVisualizerUpdateTimer.Interval = TimeSpan.FromMilliseconds(2);
            ViewModel.AudioVisualizerUpdateTimer.Tick += AudioVisualizerUpdateTimer_OnTick;

            ViewModel.AudioVisualizerUpdateTimer.Start();
        });
    }

    private void AudioVisualizerUpdateTimer_OnTick(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            using var config = new Config();

            // Do nothing if audio visualizer is disabled
            if (!config.Container.DisplayAudioVisualizer) return;

            var player = Locator.Current.GetRequiredService<IPlayer>();

            if (ViewModel == default) return;

            if (!player.IsPlaying.Value)
            {
                foreach (var t in ViewModel.SeriesValues.Where(x => x.Value != 0))
                {
                    t.Value = 0;
                }

                return;
            }

            var audioEngine = Locator.Current.GetRequiredService<IAudioEngine>();

            var vData = audioEngine.GetVisualizationData();

            for (var i = 0; i < vData.Length; i++)
            {
                ViewModel.SeriesValues[i].Value = vData[i] * 5;
            }
        });
    }

    private void Repeat_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right)
            ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
    }

    private void SongProgressSlider_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.Player.Play();
    }

    private void SongProgressSlider_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewModel.Player.Pause();
    }

    internal async void Blacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        await using (var blacklist = new Blacklist())
        {
            await blacklist.ReadAsync();

            var currentHash = ViewModel.CurrentSong.Value?.Hash;

            if (blacklist.Contains(ViewModel.CurrentSong.Value))
            {
                blacklist.Container.Songs.Remove(currentHash);
            }
            else
            {
                blacklist.Container.Songs.Add(currentHash);

                if (ViewModel.Player.BlacklistSkip.Value)
                    await ViewModel.Player.NextSong(PlayDirection.Forward);
            }
        }

        ViewModel.Player.TriggerBlacklistChanged(new PropertyChangedEventArgs("Black"));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongOnBlacklist));
    }

    internal async void Favorite_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.Player.CurrentSong.Value != null)
        {
            await PlaylistManager.ToggleSongOfCurrentPlaylist(ViewModel.Player.SelectedPlaylist.Value, ViewModel.Player.CurrentSong.Value);
            ViewModel.Player.TriggerPlaylistChanged(new PropertyChangedEventArgs("Fav"));
        }

        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
    }

    private void SongControl(object? sender, RoutedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "Repeat":
                ViewModel.Player.RepeatMode.Value = ViewModel.Player.RepeatMode.Value.Next();
                break;
            case "Previous":
                ViewModel.Player.NextSong(PlayDirection.Backwards);
                break;
            case "PlayPause":
                ViewModel.Player.PlayPause();
                break;
            case "Next":
                ViewModel.Player.NextSong(PlayDirection.Forward);
                break;
            case "Shuffle":
                ViewModel.IsShuffle = !ViewModel.IsShuffle;
                break;
        }
    }

    private void Volume_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.Player.ToggleMute();
    }

    private void PlaybackSpeed_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel!.PlaybackSpeed = 0;
    }

    private void RepeatContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        ViewModel.Player.SelectedPlaylist.Value = (Playlist) (sender as ContextMenu)?.SelectedItem;
    }

    private void CurrentSongLabel_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        var player = ViewModel.Player;

        if (player.RepeatMode.Value != RepeatMode.Playlist)
        {
            switch (_mainWindow.ViewModel.MainView)
            {
                case SearchViewModel:
                    _mainWindow.ViewModel.SearchView.SelectedSong = player.CurrentSong.Value;
                    _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.SearchView;
                    break;
                default:
                    _mainWindow.ViewModel.HomeView.SelectedSong = player.CurrentSong.Value;
                    _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.HomeView;
                    break;
            }
        }
        else
        {
            _mainWindow.ViewModel.PlaylistView.SelectedPlaylist = player.SelectedPlaylist.Value;
            _mainWindow.ViewModel.PlaylistView.SelectedSong = player.CurrentSong.Value;
            _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.PlaylistView;
        }
    }
}