using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.IO.DbReader;

namespace OsuPlayer.Services;

public class DbReaderFactory : IDbReaderFactory
{
    public IDbReaderFactory.CreationType Type { get; set; }

    public IDatabaseReader? CreateDatabaseReader(string path)
    {
        switch (Type)
        {
            case IDbReaderFactory.CreationType.OsuDb:
                var dbLoc = Path.Combine(path, "osu!.db");

                if (!File.Exists(dbLoc)) return null;

                var file = File.OpenRead(dbLoc);

                return new OsuDbReader(file, path, this);
            case IDbReaderFactory.CreationType.Realm:
                return new RealmReader(path, this);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}