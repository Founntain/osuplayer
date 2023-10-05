using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Material.Icons.Avalonia;
using Nein.Base;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.DbReader.Interfaces;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlaylistView : ReactiveControl<PlaylistViewModel>
{
    private MainWindow _mainWindow;

    public PlaylistView()
    {
        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;
        });
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OpenPlaylistEditor_OnClick(object? sender, RoutedEventArgs e)
    {
        _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.PlaylistEditorView;
    }

    private void OpenBlacklistEditor_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow.ViewModel == default) return;

        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.BlacklistEditorView;
    }

    private async void PlaySong(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control {DataContext: IMapEntryBase song}) return;

        if (new Config().Container.PlaylistEnableOnPlay)
        {
            ViewModel.Player.RepeatMode.Value = RepeatMode.Playlist;
            ViewModel.Player.SelectedPlaylist.Value = ViewModel.SelectedPlaylist;
        }

        await ViewModel.Player.TryPlaySongAsync(song);
    }

    private void PlayPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control {DataContext: Playlist playlist}) return;

        if (!playlist.Songs.Any())
        {
            MessageBox.Show("This playlist is empty!");

            return;
        }

        ViewModel.Player.RepeatMode.Value = RepeatMode.Playlist;
        ViewModel.Player.SelectedPlaylist.Value = playlist;
    }

    private void PlaylistItemLoaded(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (sender is not Grid grid) return;

        var icon = (MaterialIcon) grid.Children.FirstOrDefault(x => x is MaterialIcon);

        if (icon == null) return;

        ViewModel.AddIcon(icon);
    }

    private async void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.SelectedPlaylist == null || ViewModel.SelectedSong == null) return;

        await PlaylistManager.RemoveSongFromPlaylist(ViewModel.SelectedPlaylist, ViewModel.SelectedSong);

        ViewModel.Player.TriggerPlaylistChanged(new PropertyChangedEventArgs(ViewModel.SelectedPlaylist.Name));
    }
}