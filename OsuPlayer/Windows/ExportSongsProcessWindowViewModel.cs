using Nein.Base;
using ReactiveUI;

namespace OsuPlayer.Windows;

public class ExportSongsProcessWindowViewModel : BaseWindowViewModel
{
    private ICollection<IMapEntryBase> _songs;

    public ICollection<IMapEntryBase> Songs
    {
        get => _songs;
        set => this.RaiseAndSetIfChanged(ref _songs, value);
    }
    
    private int _exportTotalSongs;

    public int ExportTotalSongs
    {
        get => _exportTotalSongs;
        set => this.RaiseAndSetIfChanged(ref _exportTotalSongs, value);
    }

    private int _exportingSongsProgress;

    public int ExportingSongsProgress
    {
        get => _exportingSongsProgress;
        set => this.RaiseAndSetIfChanged(ref _exportingSongsProgress, value);
    }

    private string _exportString;

    public string ExportString
    {
        get => _exportString;
        set => this.RaiseAndSetIfChanged(ref _exportString, value);
    }

    private bool _isExportRunning;

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