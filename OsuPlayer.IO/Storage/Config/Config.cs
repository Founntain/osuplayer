using Newtonsoft.Json;

namespace OsuPlayer.IO.Storage.Config;

public class Config : IStorable<ConfigContainer>
{
    public string Path => "data/config.json";

    private ConfigContainer? _configContainer;

    public ConfigContainer Container
    {
        get => _configContainer ?? Read();
        set => _configContainer = value;
    }

    public ConfigContainer Read()
    {
        if (!File.Exists(Path) || _configContainer != null)
            return _configContainer ??= new ConfigContainer();

        var data = File.ReadAllText(Path);

        return _configContainer ??= (string.IsNullOrWhiteSpace(data)
            ? new ConfigContainer()
            : JsonConvert.DeserializeObject<ConfigContainer>(data))!;
    }

    public async Task<ConfigContainer> ReadAsync()
    {
        if (!File.Exists(Path) || _configContainer != null)
            return _configContainer ??= new ConfigContainer();

        var data = await File.ReadAllTextAsync(Path);

        return _configContainer ??= (string.IsNullOrWhiteSpace(data)
            ? new ConfigContainer()
            : JsonConvert.DeserializeObject<ConfigContainer>(data))!;
    }

    public void Save(ConfigContainer container)
    {
        Directory.CreateDirectory("data");

        File.WriteAllText(Path, JsonConvert.SerializeObject(container));
    }

    public async Task SaveAsync(ConfigContainer container)
    {
        Directory.CreateDirectory("data");

        await File.WriteAllTextAsync(Path, JsonConvert.SerializeObject(container));
    }

    public void Dispose()
    {
        if (_configContainer != default)
            Save(_configContainer);
    }

    public async ValueTask DisposeAsync()
    {
        if (_configContainer != default)
            await SaveAsync(_configContainer);
    }
}