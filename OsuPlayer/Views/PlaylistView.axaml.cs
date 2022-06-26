using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.Windows;
using ReactiveUI;

namespace OsuPlayer.Views;

public partial class PlaylistView : ReactivePlayerControl<PlaylistViewModel>
{
    private MainWindow _mainWindow;

    public PlaylistView()
    {
        this.WhenActivated(disposables =>
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
        _mainWindow.ViewModel!.PlaylistEditorView.Playlists = ViewModel.Playlists.ToSourceList();

        _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.PlaylistEditorView;
    }

    private void OpenBlacklistEditor_OnClick(object? sender, RoutedEventArgs e)
    {
        _mainWindow.ViewModel.MainView = _mainWindow.ViewModel.BlacklistEditorView;
    }

    private async void PlaySong(object? sender, RoutedEventArgs e)
    {
        var tapped = (TappedEventArgs) e;
        var controlSource = (Control) tapped.Pointer.Captured;

        if (controlSource?.DataContext is IMapEntryBase song)
            await ViewModel.Player.TryPlaySongAsync(song);
    }

    private async void PlayPlaylist_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button {DataContext: Playlist playlist}) return;

        ViewModel.Player.ActivePlaylistId = playlist.Id;
        ViewModel.Player.Repeat = RepeatMode.Playlist;

        await ViewModel.Player.TryPlaySongAsync(ViewModel.Player.GetMapEntryFromHash(playlist.Songs.First()));
    }
}