using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Nein.Base;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.DbReader.Interfaces;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlaylistEditorView : ReactiveControl<PlaylistEditorViewModel>
{
    private MainWindow? _mainWindow;

    public PlaylistEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(_ =>
        {
            if (this.GetVisualRoot() is MainWindow mainWindow)
                _mainWindow = mainWindow;
        });

        AvaloniaXamlLoader.Load(this);
    }

    private async void AddToPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSelectedPlaylist == default)
        {
            if (ViewModel.Playlists?.Count > 0)
                ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.ElementAt(0);
            else
                return;
        }

        if (ViewModel.SelectedSongListItems == default || ViewModel.CurrentSelectedPlaylist == default) return;

        foreach (var song in ViewModel.SelectedSongListItems)
        {
            await PlaylistManager.AddSongToPlaylistAsync(ViewModel.CurrentSelectedPlaylist, song);
            ViewModel.CurrentSelectedPlaylist.Songs.Add(song.Hash);
        }

        ViewModel.SelectedSongListItems.Clear();

        ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentSelectedPlaylist));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.SelectedSongListItems));
    }

    private async void RemoveFromPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSelectedPlaylist == default)
        {
            if (ViewModel.Playlists?.Count > 0)
                ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.ElementAt(0);
            else
                return;
        }

        if (ViewModel.SelectedPlaylistItems == null || ViewModel.CurrentSelectedPlaylist == default) return;

        foreach (var song in ViewModel.SelectedPlaylistItems)
        {
            await PlaylistManager.RemoveSongFromPlaylist(ViewModel.CurrentSelectedPlaylist, song);
            ViewModel.CurrentSelectedPlaylist.Songs.Remove(song.Hash);
        }

        ViewModel.SelectedPlaylistItems.Clear();

        ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.CurrentSelectedPlaylist));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.SelectedPlaylistItems));
    }

    private void SongList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        if (sender is not ListBox listBox) return;

        var songs = listBox.SelectedItems.Cast<IMapEntryBase>().ToList();

        _mainWindow.ViewModel.PlaylistEditorView.SelectedSongListItems = songs;
    }

    private void Playlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_mainWindow?.ViewModel == default) return;

        if (sender is not ListBox listBox) return;

        var songs = listBox.SelectedItems.Cast<IMapEntryBase>().ToList();

        _mainWindow.ViewModel.PlaylistEditorView.SelectedPlaylistItems = songs;
    }

    private void CreatePlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.IsNewPlaylistPopupOpen = !ViewModel.IsNewPlaylistPopupOpen;
    }

    private void RenamePlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.IsRenamePlaylistPopupOpen = !ViewModel.IsRenamePlaylistPopupOpen;
    }

    private async void ConfirmNewPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ViewModel.NewPlaylistNameText)) return;

        var playlist = new Playlist
        {
            Name = ViewModel.NewPlaylistNameText
        };

        var playlists = await PlaylistManager.AddPlaylistAsync(playlist);

        ViewModel.IsNewPlaylistPopupOpen = false;

        ViewModel.Playlists = playlists.ToSourceList();

        ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.Last();
    }

    private async void ConfirmRenamePlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSelectedPlaylist == null) return;

        await PlaylistManager.RenamePlaylist(ViewModel.CurrentSelectedPlaylist);

        var playlists = await PlaylistManager.GetAllPlaylistsAsync();

        ViewModel.IsRenamePlaylistPopupOpen = false;

        ViewModel.Playlists = playlists.ToSourceList();

        ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.Last();
    }

    private void DeletePlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.IsDeletePlaylistPopupOpen = !ViewModel.IsDeletePlaylistPopupOpen;
    }

    private async void ConfirmDeletePlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow == default) return;

        if (ViewModel.CurrentSelectedPlaylist == null) return;

        if (ViewModel.CurrentSelectedPlaylist.Name == "Favorites")
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "No you can't delete your favorites! Sorry :(");
            return;
        }

        var playlists = await PlaylistManager.DeletePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.IsDeletePlaylistPopupOpen = false;

        ViewModel.Playlists = playlists.ToSourceList();

        ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.First();
    }

    private void CancelDeletePlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.IsDeletePlaylistPopupOpen = false;
    }

    private async void PlaySong(object? sender, RoutedEventArgs e)
    {
        var tapped = (TappedEventArgs) e;
        var controlSource = (Control) tapped.Pointer.Captured;

        if (controlSource?.DataContext is IMapEntryBase song)
            await ViewModel.Player.TryPlaySongAsync(song);
    }
}