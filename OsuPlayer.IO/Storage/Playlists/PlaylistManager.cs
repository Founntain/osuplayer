using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Storage.Playlists;

/// <summary>
/// A wrapper for the <see cref="PlaylistStorage" />, containing static methods for creating, deleting and modifying
/// playlists
/// </summary>
public static class PlaylistManager
{
    /// <summary>
    /// Gets all stored playlists
    /// </summary>
    /// <returns>an <see cref="IList{T}" /> containing all playlists</returns>
    public static IList<Playlist>? GetAllPlaylists()
    {
        using (var storage = new PlaylistStorage())
        {
            storage.Read();

            return storage.Container.Playlists;
        }
    }

    /// <summary>
    /// Gets all stored playlists asynchronously
    /// </summary>
    /// <returns>an <see cref="IList{T}" /> containing all playlists</returns>
    public static async Task<IList<Playlist>?> GetAllPlaylistsAsync()
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            return storage.Container.Playlists;
        }
    }

    /// <summary>
    /// Add a playlist to the storage
    /// </summary>
    /// <param name="playlist">The playlist to add</param>
    /// <returns>all playlists including the newly added one</returns>
    public static async Task<IList<Playlist>?> AddPlaylistAsync(Playlist playlist)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            if (storage.Container.Playlists?.Any(x => x == playlist) ?? true)
                return storage.Container.Playlists;

            storage.Container.Playlists?.Add(playlist);

            return storage.Container.Playlists;
        }
    }

    /// <summary>
    /// Renames a playlist. Gets the stored playlist by its <see cref="Guid" /> and changes the the name of it
    /// </summary>
    /// <param name="playlist">The playlist with the new name</param>
    public static async Task RenamePlaylist(Playlist playlist)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            var p = storage.Container.Playlists?.FirstOrDefault(x => x == playlist);

            if (p == null) return;

            p.Name = playlist.Name;
        }
    }

    /// <summary>
    /// Delete a given playlist and remove it from the storage
    /// </summary>
    /// <param name="playlist">The playlist to remove</param>
    /// <returns>all playlists after deletion</returns>
    public static async Task<IList<Playlist>?> DeletePlaylistAsync(Playlist playlist)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            var p = storage.Container.Playlists?.First(x => x == playlist);

            storage.Container.Playlists?.Remove(p);

            return storage.Container.Playlists;
        }
    }

    /// <summary>
    /// Toggles a song from the playlist asynchronously
    /// <remarks>If the song is not in the playlist, the song gets added, otherwise the song gets removed</remarks>
    /// </summary>
    /// <param name="playlist">The playlist to toggle the song in</param>
    /// <param name="mapEntry">The song to toggle</param>
    public static async Task ToggleSongOfCurrentPlaylist(Playlist? playlist, IMapEntryBase mapEntry)
    {
        if (playlist == default)
            return;

        if (playlist.Songs.Contains(mapEntry.Hash))
            await RemoveSongFromPlaylist(playlist, mapEntry);
        else
            await AddSongToPlaylistAsync(playlist, mapEntry);
    }

    /// <summary>
    /// Adds a song to the playlist asynchronously
    /// </summary>
    /// <param name="playlist">The playlist to add the song to</param>
    /// <param name="mapEntry">The song to add</param>
    public static async Task AddSongToPlaylistAsync(Playlist playlist, IMapEntryBase mapEntry)
    {
        await using var playlistStorage = new PlaylistStorage();

        await playlistStorage.ReadAsync();

        playlistStorage.Container.Playlists?.FirstOrDefault(x => x == playlist)?.Songs.Add(mapEntry.Hash);
        playlist.Songs.Add(mapEntry.Hash);
    }

    /// <summary>
    /// Removes a song from the playlist asynchronously
    /// </summary>
    /// <param name="playlist">The playlist to remove the song from</param>
    /// <param name="mapEntry">The song to remove</param>
    public static async Task RemoveSongFromPlaylist(Playlist playlist, IMapEntryBase mapEntry)
    {
        await using var playlistStorage = new PlaylistStorage();

        await playlistStorage.ReadAsync();

        playlistStorage.Container.Playlists?.FirstOrDefault(x => x == playlist)?.Songs.Remove(mapEntry.Hash);
        playlist.Songs.Remove(mapEntry.Hash);
    }
}