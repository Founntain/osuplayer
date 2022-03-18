namespace OsuPlayer.IO.Playlists;

public class Playlist
{
    public string Name { get; set; }
    public ICollection<string> Songs { get; set; }
}