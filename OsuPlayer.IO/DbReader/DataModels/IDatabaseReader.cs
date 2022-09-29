namespace OsuPlayer.IO.DbReader.DataModels;

public interface IDatabaseReader
{
    public Task<List<IMapEntryBase>?> ReadBeatmaps();
    
    public Task<Dictionary<string, int>> GetBeatmapHashes();
    
    public Task<List<Collection>?> GetCollections(string path);
}