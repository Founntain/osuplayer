using DynamicData;
using OsuPlayer.IO.DbReader.Interfaces;
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

    public List<IMapEntryBase> GetMapEntriesFromHash(ICollection<string> hashes, out ICollection<string> invalidHashes)
    {
        var maps = hashes.Select(x => SongSourceList!.FirstOrDefault(map => map.Hash == x)).ToArray();

        invalidHashes = new List<string>();

        for (var i = 0; i < maps.Length; i++)
            if (maps[i] == null)
                invalidHashes.Add(hashes.ElementAt(i));

        return maps.Where(map => map != null).ToList();
    }
}