namespace OsuPlayer.IO.DbReader.DataModels;

/// <summary>
/// Interface used by the database readers used to read the osu databases.
/// </summary>
public interface IDatabaseReader : IDisposable
{
    /// <summary>
    /// Reads all beatmaps from the database.
    /// </summary>
    /// <returns>a list of <see cref="IMapEntryBase"/>s</returns>
    public Task<List<IMapEntryBase>?> ReadBeatmaps();
    
    /// <summary>
    /// Get all beatmap hashes from the database.
    /// </summary>
    /// <remarks>This is primarily used for the collection import. </remarks>
    /// <returns>a dictionary with the hashes as the key and the beatmap set id as value</returns>
    public Dictionary<string, int> GetBeatmapHashes();
    
    /// <summary>
    /// Reads the osu! collections from the database.
    /// </summary>
    /// <param name="path">the osu! path</param>
    /// <returns>a list of <see cref="Collection"/>s</returns>
    public Task<List<Collection>?> GetCollections(string path);
}