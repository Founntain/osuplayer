using Newtonsoft.Json;

namespace OsuPlayer.Network.LastFM.Responses.Scrobble;

public class Scrobbles
{
    public Scrobble Scrobble { get; set; }

    [JsonProperty("@attr")]
    public Attr Attr { get; set; }
}