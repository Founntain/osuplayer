using DynamicData;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.Modules.Services;

public interface ISongSourceProvider
{
    public SourceList<IMapEntryBase> SongSource { get; }
    public List<IMapEntryBase>? SongSourceList { get; }

    public IMapEntryBase? GetMapEntryFromHash(string? hash);
    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash);
}