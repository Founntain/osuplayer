﻿using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Nein.Extensions;
using OsuPlayer.UI_Extensions;
using Splat;
using TagLib;
using File = System.IO.File;

namespace OsuPlayer.Windows;

public partial class ExportSongsProcessWindow : ReactiveWindow<ExportSongsProcessWindowViewModel>
{
    private readonly MainWindow? _mainWindow;
    private readonly string _path;
    
    public ExportSongsProcessWindow()
    {
        InitializeComponent();
    }

    public ExportSongsProcessWindow(ICollection<IMapEntryBase> songs, string path)
    {
        InitializeComponent();

        ViewModel = new ExportSongsProcessWindowViewModel(songs);

        _mainWindow = Locator.Current.GetService<MainWindow>();
        _path = path;
        
        var config = new Config();
        
        TransparencyLevelHint = (WindowTransparencyLevel) config.Container.BackgroundMode;
        FontFamily = config.Container.Font ?? FontManager.Current.DefaultFontFamilyName;

    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    internal async Task ExportSongs()
    {
        if (ViewModel == null) return;

        var songs = ViewModel.Songs.ToObservableCollection();
        
        var totalSongCount = songs.Count;
        ViewModel.ExportTotalSongs = totalSongCount;

        var successfulSongs = 0;
        var failedSongs = 0;
        
        var copyTask = Task.Run(async () =>
        {
            for (var index = 0; index < songs.Count; index++)
            {
                var mapEntry = await songs.ElementAt(index).ReadFullEntry();

                if (mapEntry == null) return;
                
                var hashLength = mapEntry.Hash.Length < 8 ? mapEntry.Hash.Length : 8;
                
                var fileName = $"{mapEntry.GetArtist()} - {mapEntry.GetTitle()} ({mapEntry.Hash.Substring(0, hashLength)}).mp3";
                
                fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));

                var exportPath = Path.Combine(_path, fileName);

                try
                {
                    #region Copy song to export folder asynchroniously

                    const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
                    const int bufferSize = 81920;

                    await using (var sourceStream = new FileStream(mapEntry.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions))
                    {
                        await using (var destinationStream = new FileStream(exportPath, FileMode.Create, FileAccess.Write,
                                         FileShare.None, bufferSize, fileOptions))
                        {
                            await sourceStream.CopyToAsync(destinationStream, bufferSize);
                        }
                    }

                    #endregion

                    #region Tag the song with metadata

                    var tFile = TagLib.File.Create(exportPath);

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

                    File.Delete(exportPath);

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

        GeneralExtensions.OpenUrl(_path);

        Close();
    }

    private async void Window_OnActivated(object? sender, EventArgs e)
    {
        await ExportSongs();
    }
}