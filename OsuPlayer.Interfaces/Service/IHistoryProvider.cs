using Nein.Extensions.Bindables;
using OsuPlayer.Data.DataModels.Extensions;
using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.Interfaces.Service;

/// <summary>
/// This interface provides historic capabilities
/// </summary>
public interface IHistoryProvider
{
    public HistoricalMapEntryComparer Comparer { get; }
    public BindableList<HistoricalMapEntry> History { get; }

    void AddOrUpdateMapEntry(IMapEntryBase mapEntry);
    void ClearHistory();
}