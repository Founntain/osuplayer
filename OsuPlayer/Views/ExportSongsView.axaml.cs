using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.UI_Extensions;
using OsuPlayer.Windows;
using Splat;

namespace OsuPlayer.Views;

public partial class ExportSongsView : ReactiveUserControl<ExportSongsViewModel>
{
    private readonly FluentAppWindow? _mainWindow;

    public ExportSongsView()
    {
        InitializeComponent();

        _mainWindow = Locator.Current.GetRequiredService<FluentAppWindow>();
    }

    public TopLevel? GetTopLevel() => TopLevel.GetTopLevel(_mainWindow);

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
        var topLevel = GetTopLevel();

        if (_mainWindow == default || ViewModel == default || topLevel == default) return;

        var result = await topLevel.StorageProvider.OpenFolderPickerAsync(new ()
        {
            Title = "Select your folder to export to",
            AllowMultiple = false
        });

        var path = result.First().Path.ToString();

        if (string.IsNullOrWhiteSpace(path))
        {
            await MessageBox.ShowDialogAsync(_mainWindow, "Did you even select a folder?!");

            return;
        }

        var exportWindow = new ExportSongsProcessWindow(songs, path, ViewModel.EmbedBackground);

        exportWindow.Show(_mainWindow);
    }
}