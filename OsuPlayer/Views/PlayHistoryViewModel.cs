using Nein.Base;
using Nein.Extensions;
using OsuPlayer.IO.DbReader.DataModels.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;

namespace OsuPlayer.Views;

public class PlayHistoryViewModel : BaseViewModel
{
    public readonly ISongSourceProvider SongSourceProvider;

    private ObservableCollection<HistoricalMapEntry>? _history;

    private HistoricalMapEntry? _selectedHistoricalMapEntry;

    public HistoricalMapEntry? SelectedHistoricalMapEntry
    {
        get => _selectedHistoricalMapEntry;
        set => this.RaiseAndSetIfChanged(ref _selectedHistoricalMapEntry, value);
    }

    public ObservableCollection<HistoricalMapEntry>? History
    {
        get => _history;
        set => this.RaiseAndSetIfChanged(ref _history, value);
    }

    public IPlayer Player { get; set; }

    public PlayHistoryViewModel(IPlayer player, IHistoryProvider? historyProvider, ISongSourceProvider songSourceProvider)
    {
        Player = player;
        SongSourceProvider = songSourceProvider;

        historyProvider?.History.BindCollectionChanged((_, _) =>
        {
            // We first sort them descending by time played, then by song name alphabetically
            History = historyProvider.History.OrderByDescending(x => x.TimePlayed).ThenBy(x => x.MapEntry.SongName).ToObservableCollection();
        });

        Activator = new ViewModelActivator();
    }
}