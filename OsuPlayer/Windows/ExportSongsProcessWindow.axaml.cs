using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using ManagedBass;
using ManagedBass.Enc;
using Nein.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.Services;
using OsuPlayer.UI_Extensions;
using Splat;
using TagLib;
using File = TagLib.File;

namespace OsuPlayer.Windows;

public partial class ExportSongsProcessWindow : ReactiveWindow<ExportSongsProcessWindowViewModel>
{
    private readonly string _path = string.Empty;
    private readonly bool _embedBackground;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ExportSongsProcessWindow()
    {
        InitializeComponent();
    }

    public ExportSongsProcessWindow(ICollection<IMapEntryBase> songs, string path, bool embedBackground)
    {
        InitializeComponent();

        ViewModel = new ExportSongsProcessWindowViewModel(songs);

        Locator.Current.GetService<FluentAppWindow>();
        // Decode the path, so stuff like %20 are encoded properly
        _path = HttpUtility.UrlDecode(path);
        _embedBackground = embedBackground;

        var config = new Config();

        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };
        FontFamily = config.Container.Font ?? FontManager.Current.DefaultFontFamily;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    internal async Task ExportSongs(CancellationToken token)
    {
        if (ViewModel == null) return;

        var songs = ViewModel.Songs.ToObservableCollection();

        var totalSongCount = songs.Count;
        ViewModel.ExportTotalSongs = totalSongCount;

        var successfulSongs = 0;
        var failedSongs = 0;

        var copyTask = Task.Run(async () =>
        {
            var logger = Locator.Current.GetRequiredService<ILoggingService>();

            for (var index = 0; index < songs.Count; index++)
            {
                if (token.IsCancellationRequested)
                    return;

                var mapEntry = songs.ElementAt(index).ReadFullEntry();

                if (mapEntry == null) return;

                var hashLength = mapEntry.Hash.Length < 8 ? mapEntry.Hash.Length : 8;

                var fileName = $"{mapEntry.Artist} - {mapEntry.Title} ({mapEntry.Hash.Substring(0, hashLength)}).mp3";

                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

                var exportPath = Path.Combine(_path, fileName);

                // Decode the path, so stuff like %20 are encoded properly
                exportPath = HttpUtility.UrlDecode(exportPath);

                logger.Log($"Export path: {exportPath}");

                try
                {
                    const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
                    const int bufferSize = 81920;

                    await TryCopyFile(mapEntry, exportPath, fileName);

                    #region Tag the song with metadata

                    var tFile = File.Create(exportPath);

                    tFile.Tag.Title = mapEntry.GetTitle();
                    tFile.Tag.Album = "osu!player";
                    tFile.Tag.Track = (uint) index + 1;
                    tFile.Tag.AlbumArtists = new[]
                    {
                        mapEntry.GetArtist()
                    };

                    #region Get background image

                    if (_embedBackground)
                    {
                        logger.Log($"Loading and setting background image to audio file {fileName}");

                        var findBackground = await mapEntry.FindBackground();

                        if (findBackground != null)
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

                    logger.Log($"Exported {fileName} successfully");
                }
                catch (Exception)
                {
                    failedSongs++;

                    System.IO.File.Delete(exportPath);

                    logger.Log($"Failed to export {fileName}", LogType.Error);
                }

                Dispatcher.UIThread.Post(() =>
                {
                    ViewModel.ExportingSongsProgress = successfulSongs + failedSongs;

                    ViewModel.ExportString = $"💾 Exported {successfulSongs} out of {totalSongCount} songs ({failedSongs} failed) 💾";
                    Locator.Current.GetRequiredService<ILoggingService>().Log($"💾 Exported {successfulSongs} out of {totalSongCount} songs ({failedSongs} failed) 💾");
                });
            }
        }, token);

        ViewModel.IsExportRunning = true;

        await copyTask;

        ViewModel.IsExportRunning = false;

        try
        {
            GeneralExtensions.OpenUrl(_path);
        }
        catch (Exception)
        {
            await MessageBox.ShowDialogAsync(this, $"Successfully exported {successfulSongs} out of {totalSongCount} songs ({failedSongs} failed)");
        }

        Close();
    }

    private async Task TryCopyFile(IMapEntry mapEntry, string exportPath, string filename)
    {
        var logger = Locator.Current.GetRequiredService<ILoggingService>();

        const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
        const int bufferSize = 81920;

        if (mapEntry.FullPath.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
        {
            // Copy the file directly if it is already a mp3 file

            logger.Log($"Copying {filename} from {mapEntry.FullPath} to {exportPath}");

            await using (var sourceStream = new FileStream(mapEntry.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
            {
                await using (var destinationStream = new FileStream(exportPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, fileOptions))
                {
                    await sourceStream.CopyToAsync(destinationStream, bufferSize);
                }
            }
        }
        else
        {
            // Re-encode the file if it is not already mp3 with quality preset 7 and bitrate 192

            var decodeHandle = Bass.CreateStream(mapEntry.FullPath, 0, 0, BassFlags.Decode | BassFlags.Float);

            if (decodeHandle == 0)
            {
                logger.Log($"Opening {filename} failed with error: {Bass.LastError}", LogType.Error);
            }

            var encodeHandle = BassEnc_Mp3.Start(decodeHandle, "-q7 -b192", EncodeFlags.Default | EncodeFlags.AutoFree, exportPath);

            if (encodeHandle == 0)
            {
                logger.Log($"Encoding {filename} failed with error: {Bass.LastError}", LogType.Error);
            }

            var buf = new byte[bufferSize];

            while (BassEnc.EncodeIsActive(encodeHandle) == PlaybackState.Playing)
            {
                var res = Bass.ChannelGetData(decodeHandle, buf, bufferSize);

                var lastError = Bass.LastError;

                if (res == -1 && lastError == Errors.Ended)
                {
                    BassEnc.EncodeStop(encodeHandle);
                    Bass.StreamFree(decodeHandle);

                    logger.Log($"Encoded {filename} to mp3 successfully", LogType.Success);
                }
                else if ( lastError != Errors.OK )
                {
                    throw new BassException(lastError);
                }
            }
        }
    }

    private async void TopLevel_OnOpened(object? sender, EventArgs e)
    {
        await ExportSongs(_cancellationTokenSource.Token);
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        _cancellationTokenSource.Cancel();
    }
}