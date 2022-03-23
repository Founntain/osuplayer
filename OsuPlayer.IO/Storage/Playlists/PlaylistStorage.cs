using Newtonsoft.Json;

namespace OsuPlayer.IO.Storage.Playlists;

/// <summary>
///     The PlaylistStorage reads and writes playlist data to the playlists.json file located in the data folder.
/// </summary>
public class PlaylistStorage : IStorable<PlaylistContainer>
{
    public string Path => System.IO.Path.Combine("data", "playlists.json");

    public PlaylistContainer? Container { get; set; }

    public PlaylistContainer Read()
    {
        if (!File.Exists(Path) || Container != null)
            return Container ??= new(false);

        var data = File.ReadAllText(Path);

        return Container ??= (string.IsNullOrWhiteSpace(data)
            ? new(false)
            : JsonConvert.DeserializeObject<PlaylistContainer>(data))!;
    }

    public async Task<PlaylistContainer> ReadAsync()
    {
        if (!File.Exists(Path) || Container != null)
            return Container ??= new(false);

        var data = await File.ReadAllTextAsync(Path);

        return Container ??= (string.IsNullOrWhiteSpace(data)
            ? new(false)
            : JsonConvert.DeserializeObject<PlaylistContainer>(data))!;
    }

    public void Save(PlaylistContainer container)
    {
        Directory.CreateDirectory("data");

        File.WriteAllText(Path, JsonConvert.SerializeObject(container));
    }

    public async Task SaveAsync(PlaylistContainer container)
    {
        await File.WriteAllTextAsync(Path, JsonConvert.SerializeObject(container));
    }

    public void Dispose()
    {
        if (Container != null)
            Save(Container);
    }

    public async ValueTask DisposeAsync()
    {
        if (Container != default)
            await SaveAsync(Container);
    }
}