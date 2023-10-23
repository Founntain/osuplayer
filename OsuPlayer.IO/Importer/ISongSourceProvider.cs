using System.Collections.ObjectModel;
using DynamicData;
using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.IO.Importer;

/// <summary>
/// This interface is used as a service provider for accessing imported songs.
/// </summary>
public interface ISongSourceProvider
{
    SourceList<IMapEntryBase> SongSource { get; }

    /// <summary>
    /// An observable which can be used e.g. to put a custom filter on
    /// </summary>
    public IObservable<IChangeSet<IMapEntryBase>>? Songs { get; }

    /// <summary>
    /// An read-only observable collection used to find/access songs
    /// </summary>
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
    /// <param name="hashes">The md5 hashes to find the entries for</param>
    /// <param name="invalidHashes">The md5 hashes that could not be mapped to a map entry</param>
    /// <returns>
    /// A <see cref="List{T}" /> of <see cref="IMapEntryBase" />s which contain all found entries. The list will be
    /// empty of none found
    /// </returns>
    public List<IMapEntryBase> GetMapEntriesFromHash(ICollection<string> hashes, out ICollection<string> invalidHashes);
}