using System.Text;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.VisualTree;
using OsuPlayer.Network;
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
        if (ViewModel == null) return;
        
        var totalSongCount = songs.Count;
        ViewModel.ExportTotalSongs = totalSongCount;

        var progress = new bool?[totalSongCount];

        var tasks = await Task.Run(() =>
        {
            return songs.Select(async (mapEntryBase, index) =>
            {
                var mapEntry = await mapEntryBase.ReadFullEntry();

                if (mapEntry == null) return;

                try
                {
                    #region Copy song to export folder asynchroniously

                    Directory.CreateDirectory("export_test");
                    
                    const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
                    const int bufferSize = 81920;

                    await using (var sourceStream = new FileStream(mapEntry.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
                    {
                        await using (var destinationStream = new FileStream($"export_test/{mapEntry.Hash}.mp3", FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, fileOptions))
                        {
                            await sourceStream.CopyToAsync(destinationStream, bufferSize);
                        }
                    }

                    #endregion

                    #region Tag the song with metadata

                    var tFile = TagLib.File.Create($"export_test/{mapEntry.Hash}.mp3");

                    tFile.Tag.Title = mapEntry.GetTitle();
                    tFile.Tag.AlbumArtists = new[]
                    {
                        mapEntry.GetArtist()
                    };
                    tFile.Tag.Album = "osu!player";
                    tFile.Tag.Track = (uint) index + 1;

                    #region Get thumbnail from osu! website

                    var url = $"https://assets.ppy.sh/beatmaps/{mapEntry.BeatmapSetId}/covers/list.jpg";

                    // Discord can't accept URLs bigger than 256 bytes and throws an exception, so we check for that here
                    if (Encoding.UTF8.GetByteCount(url) <= 256)
                    {
                        var osuApi = new WebRequestBase(url);

                        var thumbnailResponse = await osuApi.GetRequestWithHttpResponseMessage(string.Empty);

                        if (thumbnailResponse.IsSuccessStatusCode)
                        {
                            tFile.Tag.Pictures = new IPicture[]
                            {
                                new Picture(ByteVector.FromStream(await thumbnailResponse.Content.ReadAsStreamAsync()))
                            };
                        }
                    }

                    #endregion

                    tFile.Tag.DateTagged = DateTime.UtcNow;

                    tFile.Save();

                    #endregion

                    progress[index] = true;
                }
                catch (Exception _)
                {
                    progress[index] = false;

                    File.Delete($"export_test/{mapEntry.Hash}.mp3");
                }

                // Report progress
                int successedSongs = progress.Count(p => p == true);
                int failedSongs = progress.Count(p => p == false);

                ViewModel.CurrentExportedSongs = successedSongs;

                ViewModel.ExportString = $"Exported {successedSongs} songs out of {totalSongCount} ({failedSongs} failed)";
            });
        });

        ViewModel.IsExportRunning = true;

        await Task.WhenAll(tasks);

        ViewModel.IsExportRunning = false;
    }
}