using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.IO.DbReader.DataModels;

namespace OsuPlayer.IO.Storage.Playlists;

public class PlaylistManager
{
    public static Playlist? CurrentPlaylist;

    public static void SetCurrentPlaylist(Playlist? playlist)
    {
        if (playlist == default) return;

        CurrentPlaylist = playlist;

        using (var storage = new PlaylistStorage())
        {
            storage.Container.LastSelectedPlaylist = CurrentPlaylist.Id;
        }
    }

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

    public static IList<Playlist> GetAllPlaylists()
    {
        using (var storage = new PlaylistStorage())
        {
            storage.Read();

            return storage.Container.Playlists;
        }
    }

    public static async Task<IList<Playlist>> GetAllPlaylistsAsync()
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            return storage.Container.Playlists;
        }
    }

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

    public static async Task ReplacePlaylistsAsync(IList<Playlist> playlists)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();

            storage.Container.Playlists = playlists;
        }
    }

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

    public static async Task SavePlaylistsAsync()
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();
        }
    }

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

    public static async Task ToggleSongOfCurrentPlaylist(IMapEntryBase mapEntry)
    {
        if (CurrentPlaylist.Songs.Contains(mapEntry.BeatmapSetId))
            await RemoveSongToCurrentPlaylist(mapEntry);
        else
            await AddSongToCurrentPlaylist(mapEntry);
    }

    public static async Task AddSongToCurrentPlaylist(IMapEntryBase mapEntry)
    {
        CurrentPlaylist.Songs.Add(mapEntry.BeatmapSetId);

        await ReplacePlaylistAsync(CurrentPlaylist);
    }

    public static async Task RemoveSongToCurrentPlaylist(IMapEntryBase mapEntry)
    {
        CurrentPlaylist.Songs.Remove(mapEntry.BeatmapSetId);

        await ReplacePlaylistAsync(CurrentPlaylist);
    }

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
}