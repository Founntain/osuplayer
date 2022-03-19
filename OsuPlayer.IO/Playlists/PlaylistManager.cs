using Newtonsoft.Json;

namespace OsuPlayer.IO.Playlists;

public static class PlaylistManager
{
    public static IList<Playlist> GetAllPlaylists()
    {
        var ps = GetPlaylistStorage();

        return ps.Playlists;
    }
    
    public static async Task<IList<Playlist>> GetAllPlaylistsAsync()
    {
        var ps = await GetPlaylistStorageAsync();

        return ps.Playlists;
    }

    public static async Task SavePlaylistStorageAsync(PlaylistStorage playlistStorage)
    {
        var data = JsonConvert.SerializeObject(playlistStorage);

        await File.WriteAllTextAsync("data/playlists.json", data);
    }

    public static PlaylistStorage GetPlaylistStorage()
    {
        if (!File.Exists("data/playlists.json")) return new(true);
        
        var data = File.ReadAllText("data/playlists.json");

        try
        {
            return JsonConvert.DeserializeObject<PlaylistStorage>(data) ?? new(true);
        }
        catch (Exception)
        {
            return new(true);
        }
    }
    
    public static async Task<PlaylistStorage> GetPlaylistStorageAsync()
    {
        if (!File.Exists("data/playlists.json")) return new(true);
        
        var data = await File.ReadAllTextAsync("data/playlists.json");

        try
        {
            return JsonConvert.DeserializeObject<PlaylistStorage>(data) ?? new(true);
        }
        catch (Exception)
        {
            return new(true);
        }
    }

    public static async void AddPlaylistAsync(Playlist playlist)
    {
        var ps = await GetPlaylistStorageAsync();

        if (ps.Playlists.Any(x => x.Name == playlist.Name))
            return;
        
        ps.Playlists.Add(playlist);

        await SavePlaylistStorageAsync(ps);
    }
    
    public static void AddPlaylistToStorage(ref PlaylistStorage ps, Playlist playlist)
    {
        if (ps.Playlists.Any(x => x.Name == playlist.Name))
            return;
        
        ps.Playlists.Add(playlist);
    }

    public static async Task ReplacePlaylistAsync(Playlist? playlist)
    {
        if (playlist == default) return;
        
        var ps = await GetPlaylistStorageAsync();

        var p = ps.Playlists.FirstOrDefault(x => x.Name == playlist.Name);

        ps.Playlists.Remove(p);
        
        ps.Playlists.Add(playlist);

        await SavePlaylistStorageAsync(ps);
    }
    
    public static void ReplacePlaylistOnStorage(ref PlaylistStorage ps, Playlist? playlist)
    {
        if (playlist == default) return;
        
        var p = ps.Playlists.FirstOrDefault(x => x.Name == playlist.Name);

        ps.Playlists.Remove(p);
        
        ps.Playlists.Add(playlist);
    }
    
    public static async void ReplacePlaylistsAsync(IList<Playlist> playlists)
    {
        var ps = await GetPlaylistStorageAsync();

        ps.Playlists = playlists;

        await SavePlaylistStorageAsync(ps);
    }
    
    public static void ReplacePlaylistsOnStorage(ref PlaylistStorage ps, IList<Playlist> playlists)
    {
        ps.Playlists = playlists;
    }
}