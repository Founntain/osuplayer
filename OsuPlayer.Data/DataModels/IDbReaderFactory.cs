using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.Enums;

namespace OsuPlayer.Data.DataModels;

public interface IDbReaderFactory
{
    public DbCreationType Type { get; set; }

    public IDatabaseReader CreateDatabaseReader(string path);
}