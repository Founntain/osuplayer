using Nein.Base;
using OsuPlayer.IO.DbReader.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class ExportSongsProcessWindowViewModel : BaseWindowViewModel
{
    private int _exportingSongsProgress;
    private int _exportTotalSongs;
    private string _exportString = string.Empty;
    private bool _isExportRunning;
    private ICollection<IMapEntryBase> _songs = new List<IMapEntryBase>();

    public ICollection<IMapEntryBase> Songs
    {
        get => _songs;
        set => this.RaiseAndSetIfChanged(ref _songs, value);
    }

    public int ExportTotalSongs
    {
        get => _exportTotalSongs;
        set => this.RaiseAndSetIfChanged(ref _exportTotalSongs, value);
    }

    public int ExportingSongsProgress
    {
        get => _exportingSongsProgress;
        set => this.RaiseAndSetIfChanged(ref _exportingSongsProgress, value);
    }

    public string ExportString
    {
        get => _exportString;
        set => this.RaiseAndSetIfChanged(ref _exportString, value);
    }

    public bool IsExportRunning
    {
        get => _isExportRunning;
        set => this.RaiseAndSetIfChanged(ref _isExportRunning, value);
    }

    public ExportSongsProcessWindowViewModel()
    {
    }

    public ExportSongsProcessWindowViewModel(ICollection<IMapEntryBase> songs)
    {
        Songs = songs;
    }
}