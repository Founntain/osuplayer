using DynamicData;
using Nein.Extensions.Bindables;
using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.IO.DbReader.Interfaces;
using OsuPlayer.Services.Interfaces;

namespace OsuPlayer.Services;

public class SortService : OsuPlayerService, ISortProvider
{
    public IObservable<IChangeSet<IMapEntryBase>>? SortedSongs { get; set; }
    public Bindable<SortingMode> SortingModeBindable { get; } = new();
    public ObservableSorter SortingModeObservable { get; } = new();

    public override string ServiceName => "SORT_SERVICE";

    public SortService()
    {
        SortingModeBindable.BindValueChanged(d => SortingModeObservable.UpdateComparer(new MapSorter(d.NewValue)), true, true);
    }

    public IComparable CustomSorter(IMapEntryBase map, SortingMode sortingMode)
    {
        return sortingMode switch
        {
            SortingMode.Title => map.Title,
            SortingMode.Artist => map.Artist,
            SortingMode.SetId => map.BeatmapSetId,
            _ => ""
        };
    }

    private class MapSorter : IComparer<IMapEntryBase>
    {
        private readonly SortingMode _sortingMode;

        public MapSorter(SortingMode sortingMode)
        {
            _sortingMode = sortingMode;
        }

        public int Compare(IMapEntryBase? x, IMapEntryBase? y)
        {
            if (x == null || y == null)
                return 0;

            return _sortingMode switch
            {
                SortingMode.Artist => string.Compare(x.Artist, y.Artist, StringComparison.InvariantCulture),
                SortingMode.Title => string.Compare(x.Title, y.Title, StringComparison.InvariantCulture),
                SortingMode.SetId => x.BeatmapSetId.CompareTo(y.BeatmapSetId),
                _ => 0
            };
        }
    }
}