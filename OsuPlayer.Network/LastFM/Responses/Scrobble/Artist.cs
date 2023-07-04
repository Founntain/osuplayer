using Newtonsoft.Json;

namespace OsuPlayer.Network.LastFM.Responses.Scrobble;

public class Artist
{
    public string Corrected { get; set; }

    [JsonProperty("#text")]
    public string Text { get; set; }
}