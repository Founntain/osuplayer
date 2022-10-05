using DynamicData;
using OsuPlayer.IO.Importer;

namespace OsuPlayer.Modules.Services;

public class OsuSongSourceProvider : ISongSourceProvider
{
    public SourceList<IMapEntryBase> SongSource { get; } = new();
    public IObservable<IChangeSet<IMapEntryBase>>? Songs { get; }
    public List<IMapEntryBase>? SongSourceList { get; private set; }

    public OsuSongSourceProvider(ISortProvider? sortProvider = null)
    {
        if (sortProvider != null)
        {
            sortProvider.SortedSongs = SongSource.Connect().Sort(sortProvider.SortingModeObservable);

            Songs = sortProvider.SortedSongs;
            
            sortProvider.SortedSongs.Bind(out var sortedSongs).Subscribe(next => SongSourceList = sortedSongs.ToList());
        }
        else
        {
            Songs = SongSource.Connect();
            
            Songs.Subscribe(next => SongSourceList = SongSource.Items.ToList());
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