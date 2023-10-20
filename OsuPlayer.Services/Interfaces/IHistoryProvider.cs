using Nein.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels.Extensions;
using OsuPlayer.IO.DbReader.Interfaces;

namespace OsuPlayer.Services.Interfaces;

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