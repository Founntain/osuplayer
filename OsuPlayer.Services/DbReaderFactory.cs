using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.Enums;
using OsuPlayer.IO.DbReader;

namespace OsuPlayer.Services;

public class DbReaderFactory : OsuPlayerService, IDbReaderFactory
{
    public override string ServiceName => "DBREADER_FACTORY_SERVICE";

    public DbCreationType Type { get; set; }

    public IDatabaseReader? CreateDatabaseReader(string path)
    {
        switch (Type)
        {
            case DbCreationType.OsuDb:
                var dbLoc = Path.Combine(path, "osu!.db");

                if (!File.Exists(dbLoc)) return null;

                var file = File.OpenRead(dbLoc);

                return new OsuDbReader(file, path, this);
            case DbCreationType.Realm:
                return new RealmReader(path, this);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}