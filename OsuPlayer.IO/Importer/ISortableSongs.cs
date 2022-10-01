using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

public interface ISortableSongs
{
    public Bindable<SortingMode> SortingModeBindable { get; }

    public IComparable CustomSorter(IMapEntryBase map, SortingMode sortingMode);
}