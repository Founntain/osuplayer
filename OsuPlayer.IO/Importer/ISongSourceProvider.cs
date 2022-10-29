using System.Collections.ObjectModel;
using DynamicData;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Importer;

/// <summary>
/// This interface is used as a service provider for accessing imported songs.
/// </summary>
public interface ISongSourceProvider
{
    SourceList<IMapEntryBase> SongSource { get; }
    public IObservable<IChangeSet<IMapEntryBase>>? Songs { get; }
    public ReadOnlyObservableCollection<IMapEntryBase>? SongSourceList { get; }

    /// <summary>
    /// Gets the <see cref="IMapEntryBase" /> by the md5 hash
    /// </summary>
    /// <param name="hash">The md5 hash to find the entry for</param>
    /// <returns>The found <see cref="IMapEntryBase" /> or null if not found</returns>
    public IMapEntryBase? GetMapEntryFromHash(string? hash);

    /// <summary>
    /// Gets the <see cref="IMapEntryBase" />s by a md5 hash enumerable
    /// </summary>
    /// <param name="hash">The md5 hashes to find the entries for</param>
    /// <returns>
    /// A <see cref="List{T}" /> of <see cref="IMapEntryBase" />s which contain all found entries. The list will be
    /// empty of none found
    /// </returns>
    public List<IMapEntryBase> GetMapEntriesFromHash(IEnumerable<string> hash);
}