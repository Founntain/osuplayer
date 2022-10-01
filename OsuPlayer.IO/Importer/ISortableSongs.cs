using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

public interface ISortableSongs
{
    public Bindable<SortingMode> SortingModeBindable { get; }

    /// <summary>
    /// Picks the <see cref="IMapEntryBase" /> property to sort maps on
    /// </summary>
    /// <param name="map">the <see cref="IMapEntryBase" /> to be sorted</param>
    /// <param name="sortingMode">the <see cref="SortingMode" /> to decide how to sort</param>
    /// <returns>an <see cref="IComparable" /> containing the property to compare on</returns>
    public IComparable CustomSorter(IMapEntryBase map, SortingMode sortingMode);
}