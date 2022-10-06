using DynamicData;
using OsuPlayer.IO.Importer;

namespace OsuPlayer.Modules.Services;

public class OsuSongSourceProvider : ISongSourceProvider
{
    private readonly ReadOnlyObservableCollection<IMapEntryBase>? _songSourceList;

    public SourceList<IMapEntryBase> SongSource { get; } = new();
    public IObservable<IChangeSet<IMapEntryBase>>? Songs { get; }
    public ReadOnlyObservableCollection<IMapEntryBase>? SongSourceList => _songSourceList;

    public OsuSongSourceProvider(ISortProvider? sortProvider = null)
    {
        if (sortProvider != null)
        {
            sortProvider.SortedSongs = SongSource.Connect().Sort(sortProvider.SortingModeObservable);

            Songs = sortProvider.SortedSongs;

            Songs.Bind(out _songSourceList).Subscribe();
        }
        else
        {
            Songs = SongSource.Connect();

            Songs.Bind(out _songSourceList).Subscribe();
        }
    }

    public IMapEntryBase? GetMapEntryFromHash(string? hash)
    {
        return SongSourceList!.FirstOrDefault(x => x.Hash == hash);
    }

    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash)
    {
        //return SongSourceList!.FindAll(x => hash.Contains(x.Hash));
        return hash.Select(x => SongSourceList!.FirstOrDefault(map => map.Hash == x)).ToList();
    }
}