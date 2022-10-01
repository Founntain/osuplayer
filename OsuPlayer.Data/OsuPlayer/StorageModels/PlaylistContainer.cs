namespace OsuPlayer.Data.OsuPlayer.StorageModels;

/// <summary>
/// A playlist container with the <see cref="LastSelectedPlaylist" /> and a <see cref="Playlists" /> list
/// </summary>
public class PlaylistContainer : IStorableContainer
{
    public Guid LastSelectedPlaylist { get; set; }

    public IList<Playlist>? Playlists { get; set; }

    /// <summary>
    /// Indicates an init of the <see cref="PlaylistContainer" /> creating a new empty favorites <see cref="Playlist" />
    /// </summary>
    /// <returns>the current <see cref="PlaylistContainer" /> object</returns>
    public IStorableContainer Init()
    {
        Playlists = new List<Playlist>
        {
            new()
            {
                Name = "Favorites"
            }
        };
        return this;
    }
}