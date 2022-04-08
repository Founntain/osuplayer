using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.IO.Storage.Playlists;

public class PlaylistContainer : IStorableContainer
{
    public PlaylistContainer()
    {
    }

    public PlaylistContainer(bool _)
    {
        Playlists = new List<Playlist>
        {
            new()
            {
                Name = "Favorites"
            }
        };
    }

    public Guid LastSelectedPlaylist { get; set; }
    
    public IList<Playlist>? Playlists { get; set; }
}