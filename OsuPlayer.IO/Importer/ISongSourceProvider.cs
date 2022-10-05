using DynamicData;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

public interface ISongSourceProvider
{
    SourceList<IMapEntryBase> SongSource { get; }
    public IObservable<IChangeSet<IMapEntryBase>>? Songs { get; }
    public List<IMapEntryBase>? SongSourceList { get; }

    public IMapEntryBase? GetMapEntryFromHash(string? hash);
    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash);
}