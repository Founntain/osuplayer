using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Nein.Extensions;
using OsuPlayer.Network;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using ReactiveUI;
using TagLib;
using File = System.IO.File;

namespace OsuPlayer.Views;

public partial class ExportSongsView : ReactiveUserControl<ExportSongsViewModel>
{
    private MainWindow? _mainWindow;

    public ExportSongsView()
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

    private async void Export_OnClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel == null) return;

        var songs = ViewModel.SelectedPlaylistSongs;

        if (songs.Count > 0) await ExportSongs(songs);
    }

    private async void ExportAll_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_mainWindow?.ViewModel == null) return;

        var songs = _mainWindow.ViewModel.Player.SongSourceProvider.SongSourceList;

        if (songs?.Count > 0) await ExportSongs(songs);
    }

    private async Task ExportSongs(ICollection<IMapEntryBase> songs)
    {
        if (_mainWindow == default) return;
        
        var dialog = new OpenFolderDialog()
        {
            Title = "Select your folder to export to",
        };

        var path = await dialog.ShowAsync(_mainWindow);

        if (path == default || string.IsNullOrWhiteSpace(path))
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Did you even selected a folder?!");
            
            return;
        }
        
        var exportWindow = new ExportSongsProcessWindow(songs, path);

        await exportWindow.ShowDialog(_mainWindow);
    }
}