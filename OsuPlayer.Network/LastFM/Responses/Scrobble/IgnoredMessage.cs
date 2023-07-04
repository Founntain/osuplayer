using Newtonsoft.Json;

namespace OsuPlayer.Network.LastFM.Responses.Scrobble;

public class IgnoredMessage
{
    public string Code { get; set; }

    [JsonProperty("#text")]
    public string Text { get; set; }
}