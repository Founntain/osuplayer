using Nein.Base;
using Nein.Extensions;
using OsuPlayer.IO.DbReader.DataModels.Extensions;
using OsuPlayer.Modules.Audio.Interfaces;
using ReactiveUI;
using Splat;

namespace OsuPlayer.Views;

public class PlayHistoryViewModel : BaseViewModel
{
    protected internal IPlayer Player;
    private readonly IHistoryProvider _historyProvider;
    
    private HistoricalMapEntry _selectedHistoricalMapEntry;

    public HistoricalMapEntry SelectedHistoricalMapEntry
    {
        get => _selectedHistoricalMapEntry;
        set => this.RaiseAndSetIfChanged(ref _selectedHistoricalMapEntry, value);
    }
    
    private ObservableCollection<HistoricalMapEntry>? _history;

    public ObservableCollection<HistoricalMapEntry>? History
    {
        get => _history;
        set => this.RaiseAndSetIfChanged(ref _history, value);
    }

    public PlayHistoryViewModel() : this(Locator.Current.GetService<IHistoryProvider>())
    {
    } 

    public PlayHistoryViewModel(IHistoryProvider historyProvider)
    {
        _historyProvider = historyProvider;
        
        historyProvider.History.BindCollectionChanged((_, _) =>
        {
            // We first sort them descending by time played, then by song name alphabetically
            History = _historyProvider.History.OrderByDescending(x => x.TimePlayed).ThenBy(x => x.MapEntry.SongName).ToObservableCollection();
        });
        
        Activator = new ViewModelActivator();
    }
}
    