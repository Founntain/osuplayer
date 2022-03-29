using Newtonsoft.Json;
using Realms;

namespace OsuPlayer.IO.Storage.LazerModels.Files;

public interface IHasGuidPrimaryKey
{
    [JsonIgnore] [PrimaryKey] Guid ID { get; }
}