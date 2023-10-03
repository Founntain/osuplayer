using OsuPlayer.IO.DbReader.DataModels.Extensions;
using OsuPlayer.IO.DbReader.Interfaces;
using OsuPlayer.Modules.Audio.Interfaces;

namespace OsuPlayer.Modules.Services;

public class HistoryProvider : IHistoryProvider
{
    public HistoricalMapEntryComparer Comparer { get; }
    public BindableList<HistoricalMapEntry> History { get; }
    
    public HistoryProvider()
    {
        Comparer = new HistoricalMapEntryComparer();
        History = new BindableList<HistoricalMapEntry>();
    }

    public void AddOrUpdateMapEntry(IMapEntryBase mapEntry)
    {
        if (mapEntry == default) return;
        
        var historicalMapEntry = new HistoricalMapEntry(mapEntry);
            
        var foundEntry = History.FirstOrDefault(entry => Comparer.Equals(entry, historicalMapEntry));

        if (foundEntry != default) 
            History.Remove(foundEntry);

        History.Add(historicalMapEntry);
    }
    
    public void ClearHistory()
    {
        History.Clear();
    }
}