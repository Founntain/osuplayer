namespace OsuPlayer.Data.OsuPlayer.StorageModels;

/// <summary>
/// A playlist container with a <see cref="Playlists" /> list
/// </summary>
public class PlaylistContainer : IStorableContainer
{
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