using OsuPlayer.Data.OsuPlayer.Classes;

namespace OsuPlayer.IO.Storage.Playlists;

public class PlaylistManager
{
    public static IList<Playlist> GetAllPlaylists()
    {
        using (var storage = new PlaylistStorage())
        {
            storage.Read();
            
            return storage.Container.Playlists;
        }
    }
    
    public async static Task<IList<Playlist>> GetAllPlaylistsAsync()
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();
            
            return storage.Container.Playlists;
        }
    }

    public static async void AddPlaylistAsync(Playlist playlist)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();
            
            if (storage.Container.Playlists.Any(x => x == playlist))
                return;

            storage.Container.Playlists.Add(playlist);
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

    public static async void ReplacePlaylistsAsync(IList<Playlist> playlists)
    {
        await using (var storage = new PlaylistStorage())
        {
            await storage.ReadAsync();
            
            storage.Container.Playlists = playlists;
        }
    }
}