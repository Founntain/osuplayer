namespace OsuPlayer.Data.OsuPlayer.Classes;

public class Playlist
{
    public string Name { get; set; }
    public ICollection<string> Songs { get; set; }
}