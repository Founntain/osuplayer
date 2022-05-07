using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Blacklist;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlayerControlView : ReactivePlayerControl<PlayerControlViewModel>
{
    private MainWindow _mainWindow;

    public PlayerControlView()
    {
        InitializeComponent();
    }

    private Slider ProgressSlider => this.FindControl<Slider>("SongProgressSlider");
    private Button RepeatButton => this.FindControl<Button>("Repeat");

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;

            ProgressSlider.AddHandler(PointerPressedEvent, SongProgressSlider_OnPointerPressed,
                RoutingStrategies.Tunnel);

            ProgressSlider.AddHandler(PointerReleasedEvent, SongProgressSlider_OnPointerReleased,
                RoutingStrategies.Tunnel);

            RepeatButton.AddHandler(PointerReleasedEvent, Repeat_OnPointerReleased, RoutingStrategies.Tunnel);

            PlaylistManager.SetLastKnownPlaylistAsCurrentPlaylist();

            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsAPlaylistSelected));
            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
            ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongOnBlacklist));
        });
        AvaloniaXamlLoader.Load(this);
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

    private void Settings_OnClick(object? sender, RoutedEventArgs e)
    {
        _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.SettingsView;
    }

    private async void Blacklist_OnClick(object? sender, RoutedEventArgs e)
    {
        await using (var blacklist = new Blacklist())
        {
            await blacklist.ReadAsync();

            var currentHash = ViewModel.CurrentSong.Value?.Hash;

            if (blacklist.Container.Songs?.Contains(currentHash) ?? false)
                blacklist.Container.Songs?.Remove(currentHash);
            else
            {
                blacklist.Container.Songs?.Add(currentHash);
                ViewModel.Player.NextSong();
            }
        }

        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongOnBlacklist));
    }

    private async void Favorite_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.Player.CurrentSongBinding.Value != null)
        {
            await PlaylistManager.ToggleSongOfCurrentPlaylist(ViewModel.Player.CurrentSongBinding.Value);
            ViewModel.Player.TriggerPlaylistChanged(new PropertyChangedEventArgs("Fav"));
        }

        ViewModel.RaisePropertyChanged(nameof(ViewModel.IsCurrentSongInPlaylist));
    }

    private void SongControl(object? sender, RoutedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "Repeat":
                ViewModel.Player.Repeat = ViewModel.Player.Repeat.Next();
                break;
            case "Previous":
                ViewModel.Player.PreviousSong();
                break;
            case "PlayPause":
                ViewModel.Player.PlayPause();
                break;
            case "Next":
                ViewModel.Player.NextSong();
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
        ViewModel.Player.ActivePlaylistId = ((Playlist) (sender as ContextMenu)?.SelectedItem)?.Id;
    }
}