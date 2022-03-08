using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using OsuPlayer.Modules.IO;
using ReactiveUI;

namespace OsuPlayer;

public class Player
{
    private SongImporter Importer => new SongImporter();

    public ReadOnlyObservableCollection<SongEntry> FilteredSongEntries;
    private SourceList<SongEntry> SongSource;
    
    public Player()
    {
        SongSource = new SourceList<SongEntry>();
        IObservable<Func<SongEntry, bool>> filter = Core.Instance.MainWindow.ViewModel!.SearchView
            .WhenAnyValue(x => x.FilterText)
            .Throttle(TimeSpan.FromMilliseconds(20))
            .Select(BuildFilter);

        SongSource.Connect().Sort(SortExpressionComparer<SongEntry>.Ascending(x => x.Title))
            .Filter(filter, ListFilterPolicy.ClearAndReplace).ObserveOn(AvaloniaScheduler.Instance)
            .Bind(out FilteredSongEntries).Subscribe();
    }

    private Func<SongEntry, bool> BuildFilter(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return song => true;

        return song => song.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                       song.Artist.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    public async void ImportSongs()
    {
        var songEntries = await Importer.ImportSongs(Core.Instance.Config.OsuPath!);
        foreach (var songEntry in songEntries)
        {
            SongSource.Add(songEntry);
        }
    }
}