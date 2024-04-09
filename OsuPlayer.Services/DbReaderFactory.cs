using OsuPlayer.Data.DataModels;
using OsuPlayer.Data.DataModels.Interfaces;
using OsuPlayer.Data.Enums;
using OsuPlayer.Interfaces.Service;
using OsuPlayer.IO.DbReader;
using Splat;

namespace OsuPlayer.Services;

public class DbReaderFactory : OsuPlayerService, IDbReaderFactory
{
    public override string ServiceName => "DBREADER_FACTORY_SERVICE";

    public DbCreationType Type { get; set; }

    public IDatabaseReader? CreateDatabaseReader(string path)
    {
        var loggingService = Locator.Current.GetService<ILoggingService>();

        switch (Type)
        {
            case DbCreationType.OsuDb:
                var dbLoc = Path.Combine(path, "osu!.db");

                if (!File.Exists(dbLoc)) return null;

                var file = File.OpenRead(dbLoc);

                LogToConsole($"Using osu!.db import for path {path}");

                return new OsuDbReader(file, path, this);
            case DbCreationType.Realm:
                LogToConsole($"Using client.realm importer for path {path}");

                return new RealmReader(path, this);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}