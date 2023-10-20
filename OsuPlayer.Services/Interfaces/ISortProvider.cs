using DynamicData;
using Nein.Extensions.Bindables;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.DbReader.Interfaces;

namespace OsuPlayer.Services.Interfaces;

public interface ISortProvider
{
    public IObservable<IChangeSet<IMapEntryBase>>? SortedSongs { get; set; }
    public Bindable<SortingMode> SortingModeBindable { get; }
    public ObservableSorter SortingModeObservable { get; }

    public IComparable CustomSorter(IMapEntryBase map, SortingMode sortingMode);
}