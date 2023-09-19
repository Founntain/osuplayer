using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
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

        var successfulSongs = 0;
        var failedSongs = 0;

        Directory.CreateDirectory("export_test");

        var copyTask = Task.Run(async () =>
        {
            for (var index = 0; index < songs.Count; index++)
            {
                var mapEntry = await songs.ElementAt(index).ReadFullEntry();

                if (mapEntry == null) return;
                
                var hashLength = mapEntry.Hash.Length < 8 ? mapEntry.Hash.Length : 8;
                
                var fileName = $"{mapEntry.GetArtist()} - {mapEntry.GetTitle()} ({mapEntry.Hash.Substring(0, hashLength)}).mp3";
                
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

                try
                {
                    #region Copy song to export folder asynchroniously

                    const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
                    const int bufferSize = 81920;

                    await using (var sourceStream = new FileStream(mapEntry.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
                    {
                        await using (var destinationStream = new FileStream($"export_test/{fileName}", FileMode.Create, FileAccess.Write,
                                         FileShare.None, bufferSize, fileOptions))
                        {
                            await sourceStream.CopyToAsync(destinationStream, bufferSize);
                        }
                    }

                    #endregion

                    #region Tag the song with metadata

                    var tFile = TagLib.File.Create($"export_test/{fileName}");

                    tFile.Tag.Title = mapEntry.GetTitle();
                    tFile.Tag.Album = "osu!player";
                    tFile.Tag.Track = (uint) index + 1;
                    tFile.Tag.AlbumArtists = new[]
                    {
                        mapEntry.GetArtist()
                    };

                    #region Get thumbnail from osu! website

                    var findBackground = await mapEntry.FindBackground();

                    if (findBackground != null)
                    {
                        await using (var backgroundStream = new FileStream(findBackground, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
                        {
                            tFile.Tag.Pictures = new IPicture[]
                            {
                                new Picture(ByteVector.FromStream(backgroundStream))
                            };
                        }
                    }

                    #endregion

                    tFile.Tag.DateTagged = DateTime.UtcNow;

                    tFile.Save();

                    #endregion

                    successfulSongs++;
                    
                    Console.WriteLine($"Exported {fileName}");
                }
                catch (Exception _)
                {
                    failedSongs++;

                    File.Delete($"export_test/{fileName}");

                    Console.WriteLine($"ERROR: failed to export {fileName}");
                }

                Dispatcher.UIThread.Post(() =>
                {
                    ViewModel.ExportingSongsProgress = successfulSongs + failedSongs;

                    ViewModel.ExportString = $"💾 Exported {successfulSongs} out of {totalSongCount} songs ({failedSongs} failed) 💾";
                });
            }
        });

        ViewModel.IsExportRunning = true;

        await copyTask;

        ViewModel.IsExportRunning = false;
    }
}