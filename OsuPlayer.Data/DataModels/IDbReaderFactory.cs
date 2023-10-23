using OsuPlayer.Data.DataModels.Interfaces;

namespace OsuPlayer.Data.DataModels;

public interface IDbReaderFactory
{
    public CreationType Type { get; set; }
    
    public IDatabaseReader CreateDatabaseReader(string path);
    
    public enum CreationType
    {
        OsuDb,
        Realm
    }
}