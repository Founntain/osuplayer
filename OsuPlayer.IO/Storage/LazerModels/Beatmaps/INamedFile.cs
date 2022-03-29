namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public interface INamedFile
{
    string Filename { get; set; }

    RealmFile File { get; set; }
}