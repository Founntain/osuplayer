using DynamicData;
using Nein.Extensions.Bindables;
using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Interfaces.Service;

public interface ISortProvider
{
    public IObservable<IChangeSet<IMapEntryBase>>? SortedSongs { get; set; }
    public Bindable<SortingMode> SortingModeBindable { get; }
    public ObservableSorter SortingModeObservable { get; }

    public IComparable CustomSorter(IMapEntryBase map, SortingMode sortingMode);
}