using Nein.Extensions.Bindables;
using OsuPlayer.Data.DataModels.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Interfaces.Service;

namespace OsuPlayer.Services;

public class HistoryService : OsuPlayerService, IHistoryProvider
{
    public HistoricalMapEntryComparer Comparer { get; } = new();
    public BindableList<HistoricalMapEntry> History { get; } = new();

    public override string ServiceName => "HISTORY_SERVICE";

    public void AddOrUpdateMapEntry(IMapEntryBase? mapEntry)
    {
        if (mapEntry == default) return;

        var historicalMapEntry = new HistoricalMapEntry(mapEntry);

        var foundEntry = History.FirstOrDefault(entry => Comparer.Equals(entry, historicalMapEntry));

        if (foundEntry != default)
            History.Remove(foundEntry);

        History.Add(historicalMapEntry);

        LogToConsole($"Added or updated map entry {mapEntry.GetSongName()} ({mapEntry.Hash}) to history.");
    }

    public void ClearHistory()
    {
        History.Clear();

        LogToConsole($"Cleared listening history.");
    }
}