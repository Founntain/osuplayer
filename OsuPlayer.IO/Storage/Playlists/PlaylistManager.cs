using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Storage.Playlists;

/// <summary>
/// A wrapper for the <see cref="PlaylistStorage"/>, containing static methods for creating, deleting and modifying playlists
/// </summary>
public class PlaylistManager
{
    public static Playlist? CurrentPlaylist;

    /// <summary>
    /// Sets a specific playlist as the current playlist globally in the application
    /// </summary>
    /// <param name="playlist">The playlist to set</param>
    public static void SetCurrentPlaylist(Playlist? playlist)
    {
        if (playlist == default) return;

        CurrentPlaylist = playlist;

        using (var storage = new PlaylistStorage())
        {
            storage.Container.LastSelectedPlaylist = CurrentPlaylist.Id;
        }
    }

    /// <summary>
    /// Sets a specific playlist as the current playlist globally in the application asynchronously
    /// </summary>
    /// <param name="playlist">The playlist to set</param>
    public static async Task SetCurrentPlaylistAsync(Playlist? playlist)
    {
        if (playlist == default) return;

        CurrentPlaylist = playlist;

        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            storage.Container.LastSelectedPlaylist = CurrentPlaylist.Id;
        }
    }

    /// <summary>
    /// Gets all stored playlists
    /// </summary>
    /// <returns>an <see cref="IList{T}"/> containing all playlists</returns>
    public static IList<Playlist> GetAllPlaylists()
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
    /// <returns>an <see cref="IList{T}"/> containing all playlists</returns>
    public static async Task<IList<Playlist>> GetAllPlaylistsAsync()
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

            if (storage.Container.Playlists.Any(x => x == playlist))
                return default;

            storage.Container.Playlists.Add(playlist);

            return storage.Container.Playlists;
        }
    }

    /// <summary>
    /// Replace a playlist with the same name
    /// </summary>
    /// <param name="playlist">The playlist to replace</param>
    public static async Task ReplacePlaylistAsync(Playlist? playlist)
    {
        if (playlist == default) return;

        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            var playlistToRemove = storage.Container.Playlists
                .FirstOrDefault(x => x.Name == playlist.Name);

            storage.Container.Playlists.Remove(playlistToRemove);

            storage.Container.Playlists.Add(playlist);
        }
    }

    /// <summary>
    /// Replace a playlist with the same name asynchronously
    /// </summary>
    /// <param name="playlists">The playlist to replace</param>
    public static async Task ReplacePlaylistsAsync(IList<Playlist> playlists)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            storage.Container.Playlists = playlists;
        }
    }

    /// <summary>
    /// Renames a playlist. Gets the stored playlist by its <see cref="Guid"/> and changes the the name of it
    /// </summary>
    /// <param name="playlist">The playlist with the new name</param>
    public static async Task RenamePlaylist(Playlist playlist)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            var p = storage.Container.Playlists.FirstOrDefault(x => x == playlist);

            storage.Container.Playlists.Remove(p);

            p.Name = playlist.Name;

            storage.Container.Playlists.Add(p);
        }
    }

    /// <summary>
    /// Save all playlists on the hard drive
    /// </summary>
    public static async Task SavePlaylistsAsync()
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();
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

            var p = storage.Container.Playlists.First(x => x == playlist);

            storage.Container.Playlists.Remove(p);

            return storage.Container.Playlists;
        }
    }

    /// <summary>
    /// Toggles a song from the playlist asynchronously
    /// <remarks>If the song is not in the playlist, the song gets added, otherwise the song gets removed</remarks>
    /// </summary>
    /// <param name="mapEntry">The song to toggle</param>
    public static async Task ToggleSongOfCurrentPlaylist(IMapEntryBase mapEntry)
    {
        if (CurrentPlaylist.Songs.Contains(mapEntry.BeatmapSetId))
            await RemoveSongToCurrentPlaylist(mapEntry);
        else
            await AddSongToCurrentPlaylistAsync(mapEntry);
    }

    /// <summary>
    /// Adds a song to the playlist asynchronously
    /// </summary>
    /// <param name="mapEntry">The song to add</param>
    public static async Task AddSongToCurrentPlaylistAsync(IMapEntryBase mapEntry)
    {
        CurrentPlaylist.Songs.Add(mapEntry.BeatmapSetId);

        await ReplacePlaylistAsync(CurrentPlaylist);
    }

    /// <summary>
    /// Removes a song to the playlist asynchronously
    /// </summary>
    /// <param name="mapEntry">The song to remove</param>
    public static async Task RemoveSongToCurrentPlaylist(IMapEntryBase mapEntry)
    {
        CurrentPlaylist.Songs.Remove(mapEntry.BeatmapSetId);

        await ReplacePlaylistAsync(CurrentPlaylist);
    }

    /// <summary>
    /// Selects the last used playlist after existing the player to its current playlist globally
    /// </summary>
    public static void SetLastKnownPlaylistAsCurrentPlaylist()
    {
        using (var storage = new PlaylistStorage())
        {
            var container = storage.Read();

            var playlist = container.Playlists?.FirstOrDefault(x => x.Id == container.LastSelectedPlaylist);

            if (playlist == default) return;

            CurrentPlaylist = playlist;
        }
    }

    /// <summary>
    /// Adds a song to a playlist and creates it if it doesn't exist already
    /// </summary>
    /// <param name="playlistName">the name of the playlist to add the song to</param>
    /// <param name="setId">the beatmap set id of the song</param>
    public static async Task AddSongToPlaylistAsync(string playlistName, int setId)
    {
        if (setId < 0) return;

        Playlist playlist;

        if ((playlist = (await GetAllPlaylistsAsync()).FirstOrDefault(x => x.Name == playlistName)) != default)
        {
            if (!playlist.Songs.Contains(setId))
                playlist.Songs.Add(setId);

            await ReplacePlaylistAsync(playlist);
            return;
        }

        playlist = new Playlist
        {
            Name = playlistName
        };
        playlist.Songs.Add(setId);

        await AddPlaylistAsync(playlist);
    }
}