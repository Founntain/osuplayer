using Realms;

namespace OsuPlayer.IO.Storage.LazerModels.Files;

[MapTo("File")]
public class RealmFile : RealmObject, IFileInfo
{
    [PrimaryKey] public string Hash { get; set; } = string.Empty;
}