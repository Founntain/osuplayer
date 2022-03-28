using Newtonsoft.Json;

namespace OsuPlayer.IO.Storage.Playlists;

/// <summary>
/// The PlaylistStorage reads and writes playlist data to the playlists.json file located in the data folder.
/// </summary>
public class PlaylistStorage : IStorable<PlaylistContainer>
{
    private PlaylistContainer? _container;
    public string Path => System.IO.Path.Combine("data", "playlists.json");

    public PlaylistContainer Container
    {
        get => _container ?? Read();
        set => _container = value;
    }

    public PlaylistContainer Read()
    {
        if (!File.Exists(Path) || _container != null)
            return _container ??= new PlaylistContainer(false);

        var data = File.ReadAllText(Path);

        return _container ??= (string.IsNullOrWhiteSpace(data)
            ? new PlaylistContainer(false)
            : JsonConvert.DeserializeObject<PlaylistContainer>(data))!;
    }

    public async Task<PlaylistContainer> ReadAsync()
    {
        if (!File.Exists(Path) || _container != null)
            return _container ??= new PlaylistContainer(false);

        var data = await File.ReadAllTextAsync(Path);

        return _container ??= (string.IsNullOrWhiteSpace(data)
            ? new PlaylistContainer(false)
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
        if (_container != null)
            Save(_container);
    }

    public async ValueTask DisposeAsync()
    {
        if (_container != default)
            await SaveAsync(_container);
    }
}