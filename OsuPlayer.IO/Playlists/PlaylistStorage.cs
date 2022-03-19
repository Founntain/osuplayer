namespace OsuPlayer.IO.Playlists;

public class PlaylistStorage
{
    public PlaylistStorage(bool _)
    {
        Playlists = new List<Playlist>
        {
            new()
            {
                Name = "Favorites"
            }
        };
    }
    
    public IList<Playlist> Playlists { get; set; }
}