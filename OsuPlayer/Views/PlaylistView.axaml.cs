using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using OsuPlayer.Extensions;
using OsuPlayer.IO.DbReader.DataModels;
using OsuPlayer.IO.Storage.Playlists;
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

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var playlists = await PlaylistManager.GetAllPlaylistsAsync();

        _mainWindow.ViewModel!.PlaylistEditorViewModel.Playlists = playlists.ToSourceList();

        _mainWindow.ViewModel!.MainView = _mainWindow.ViewModel.PlaylistEditorViewModel;
    }

    private async void PlaySong(object? sender, RoutedEventArgs e)
    {
        var tapped = (TappedEventArgs) e;
        var controlSource = (Control) tapped.Pointer.Captured;

        if (controlSource?.DataContext is IMapEntryBase song)
            await ViewModel.Player.PlayAsync(song);
    }
}