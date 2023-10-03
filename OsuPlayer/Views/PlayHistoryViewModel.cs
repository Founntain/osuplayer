using Nein.Base;
using Nein.Extensions;
using OsuPlayer.IO.DbReader.DataModels.Extensions;
using OsuPlayer.IO.Importer;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class PlayHistoryViewModel : BaseViewModel
{
    private readonly IHistoryProvider _historyProvider;
    public readonly ISongSourceProvider SongSourceProvider;

    private ObservableCollection<HistoricalMapEntry>? _history;

    private HistoricalMapEntry _selectedHistoricalMapEntry;

    public HistoricalMapEntry SelectedHistoricalMapEntry
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

    public PlayHistoryViewModel() : this(Locator.Current.GetService<IPlayer>(), Locator.Current.GetService<IHistoryProvider>(), Locator.Current.GetService<ISongSourceProvider>())
    {
    }

    public PlayHistoryViewModel(IPlayer player, IHistoryProvider historyProvider, ISongSourceProvider songSourceProvider)
    {
        Player = player;
        _historyProvider = historyProvider;
        SongSourceProvider = songSourceProvider;

        historyProvider.History.BindCollectionChanged((_, _) =>
        {
            // We first sort them descending by time played, then by song name alphabetically
            History = _historyProvider.History.OrderByDescending(x => x.TimePlayed).ThenBy(x => x.MapEntry.SongName).ToObservableCollection();
        });

        Activator = new ViewModelActivator();
    }
}