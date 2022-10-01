using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Extensions;
using OsuPlayer.IO.Storage.Playlists;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlaylistEditorView : ReactiveControl<PlaylistEditorViewModel>
{
    private MainWindow _mainWindow;

    public PlaylistEditorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.WhenActivated(disposables =>
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
            if (ViewModel.Playlists.Count > 0)
                ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.ElementAt(0);
            else
                return;
        }

        var playlist = ViewModel.CurrentSelectedPlaylist!.Songs;

        if (ViewModel.SelectedSongListItems == default) return;

        foreach (var song in ViewModel.SelectedSongListItems)
        {
            if (playlist.Contains(song.Hash))
                continue;

            playlist.Add(song.Hash);
        }

        ViewModel.SelectedSongListItems = new List<IMapEntryBase>();

        await PlaylistManager.ReplacePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
        ViewModel.RaisePropertyChanged(nameof(ViewModel.SelectedSongListItems));
    }

    private async void RemoveFromPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel.CurrentSelectedPlaylist == default)
        {
            if (ViewModel.Playlists.Count > 0)
                ViewModel.CurrentSelectedPlaylist = ViewModel.Playlists.Items.ElementAt(0);
            else
                return;
        }

        var playlist = ViewModel.CurrentSelectedPlaylist!.Songs;

        if (ViewModel.SelectedPlaylistItems == null) return;

        foreach (var song in ViewModel.SelectedPlaylistItems)
        {
            if (!playlist.Contains(song.Hash))
                continue;

            playlist.Remove(song.Hash);
        }

        ViewModel.SelectedPlaylistItems = new List<IMapEntryBase>();

        await PlaylistManager.ReplacePlaylistAsync(ViewModel.CurrentSelectedPlaylist);

        ViewModel.RaisePropertyChanged(nameof(ViewModel.Playlists));
    }

    private void SongList_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox listBox) return;

        var songs = listBox.SelectedItems.Cast<IMapEntryBase>().ToList();

        _mainWindow.ViewModel.PlaylistEditorView.SelectedSongListItems = songs;
    }

    private void Playlist_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
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
        await PlaylistManager.RenamePlaylist(ViewModel.CurrentSelectedPlaylist);

        await PlaylistManager.SavePlaylistsAsync();

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