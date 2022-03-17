namespace OsuPlayer.IO.Database.Entities;

public class Song : BaseEntity
{
    public string Songchecksum { get; set; }
    
    public virtual HashSet<Playlist> Playlists { get; set; }
}