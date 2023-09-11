using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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

public partial class Miniplayer : ReactiveWindow<MiniplayerViewModel>
{
    private readonly MainWindow? _mainWindow;

    public Miniplayer()
    {
        InitializeComponent();

        LoadSettings();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    public Miniplayer(IPlayer player, IAudioEngine engine)
    {
        InitializeComponent();

        _mainWindow = Locator.GetLocator().GetRequiredService<MainWindow>();

        DataContext = new MiniplayerViewModel(player, engine);

#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void LoadSettings()
    {
        using var config = new Config();

        Background = new SolidColorBrush(config.Container.BackgroundColor.ToColor());
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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
                    ViewModel.Player.NextSong(PlayDirection.Forward);
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