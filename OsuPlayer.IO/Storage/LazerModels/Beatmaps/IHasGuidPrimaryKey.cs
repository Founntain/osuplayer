using Newtonsoft.Json;
using Realms;

namespace OsuPlayer.IO.Storage.LazerModels.Beatmaps;

public interface IHasGuidPrimaryKey
{
    [JsonIgnore]
    [PrimaryKey]
    Guid ID { get; }
}