namespace OsuPlayer.IO.Storage.LazerModels.Files;

public interface IHasRealmFiles
{
    IList<RealmNamedFileUsage> Files { get; }

    string Hash { get; set; }
}