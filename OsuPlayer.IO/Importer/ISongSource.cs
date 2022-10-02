using DynamicData;
using OsuPlayer.Extensions.Bindables;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

public interface ISongSource
{
    public Bindable<SourceList<IMapEntryBase>> SongSource { get; }
    public List<IMapEntryBase>? SongSourceList { get; }

    /// <summary>
    /// Gets the map entry from the beatmap set id
    /// </summary>
    /// <param name="setId">the beatmap set id to get the map from</param>
    /// <returns>an <see cref="IMapEntryBase" /> of the found map or null if it doesn't exist</returns>
    public IMapEntryBase? GetMapEntryFromSetId(int setId);

    /// <summary>
    /// Gets the map entry from the beatmap hash
    /// </summary>
    /// <param name="hash">the beatmap hash to get the map from</param>
    /// <returns>the found <see cref="IMapEntryBase" /> or null if it doesn't exist</returns>
    public IMapEntryBase? GetMapEntryFromHash(string? hash);


    /// <summary>
    /// Gets all Songs from a specific beatmap set ID
    /// </summary>
    /// <param name="setId">The beatmap set ID</param>
    /// <returns>A list of <see cref="IMapEntryBase" /></returns>
    public List<IMapEntryBase> GetMapEntriesFromSetId(IEnumerable<int> setId);

    /// <summary>
    /// Gets all Songs from a specific beatmap hash
    /// </summary>
    /// <param name="hash">The beatmap hash</param>
    /// <returns>a list of <see cref="IMapEntryBase" />s</returns>
    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash);
}