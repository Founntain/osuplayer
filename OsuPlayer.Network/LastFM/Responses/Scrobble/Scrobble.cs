namespace OsuPlayer.Network.LastFM.Responses.Scrobble;

public class Scrobble
{
    public Artist Artist { get; set; }
    public Album Album { get; set; }
    public Track Track { get; set; }
    public IgnoredMessage IgnoredMessage { get; set; }
    public AlbumArtist AlbumArtist { get; set; }
    public string Timestamp { get; set; }
}