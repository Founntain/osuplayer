using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using FluentAvalonia.UI.Windowing;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions.EnumExtensions;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Windows;

public partial class Miniplayer : FluentReactiveWindow<MiniplayerViewModel>
{
    private readonly FluentAppWindow? _mainWindow;

    public Miniplayer()
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        LoadSettings();
    }

    public Miniplayer(IPlayer player, IAudioEngine engine)
    {
        InitializeComponent();

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        _mainWindow = Locator.GetLocator().GetRequiredService<FluentAppWindow>();

        DataContext = new MiniplayerViewModel(player, engine);

        LoadSettings();

        ViewModel.AudioVisualizerUpdateTimer.Interval = TimeSpan.FromMilliseconds(2);
        ViewModel.AudioVisualizerUpdateTimer.Tick += AudioVisualizerUpdateTimer_OnTick;

        ViewModel.AudioVisualizerUpdateTimer.Start();
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

    private void LoadSettings()
    {
        using var config = new Config();

        Background = new SolidColorBrush(config.Container.BackgroundColor.ToColor());
    }

    private void SongControl(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

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

    private void RepeatContextMenu_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (ViewModel == default) return;

        ViewModel.Player.SelectedPlaylist.Value = (Playlist) (sender as ContextMenu)?.SelectedItem;
    }

    private void TopBarGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
        e.Handled = false;
    }

    private void Volume_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == default) return;

        ViewModel.Player.ToggleMute();
    }

    private void PlaybackSpeed_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel!.PlaybackSpeed = 0;
    }

    private void Miniplayer_OnClosed(object? sender, EventArgs e)
    {
        if (_mainWindow == default) return;

        _mainWindow.Miniplayer = null;

        _mainWindow.WindowState = WindowState.Normal;
    }

    private async void Blacklist_OnClick(object? sender, RoutedEventArgs e)
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
        //ViewModel.RaisePropertyChanged(nameof(_mainWindow.ViewModel.PlayerControl.IsCurrentSongOnBlacklist));
    }

    private async void Favorite_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.Player.CurrentSong.Value != null)
        {
            await PlaylistManager.ToggleSongOfCurrentPlaylist(ViewModel.Player.SelectedPlaylist.Value, ViewModel.Player.CurrentSong.Value);
            ViewModel.Player.TriggerPlaylistChanged(new PropertyChangedEventArgs("Fav"));
        }

        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
        //ViewModel.RaisePropertyChanged(nameof(_mainWindow.ViewModel.PlayerControl.IsCurrentSongInPlaylist));
    }
}