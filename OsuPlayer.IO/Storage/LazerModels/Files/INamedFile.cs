namespace OsuPlayer.IO.Storage.LazerModels.Files;

public interface INamedFile
{
    string Filename { get; set; }

    RealmFile File { get; set; }
}