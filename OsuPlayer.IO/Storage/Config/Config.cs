using Newtonsoft.Json;

namespace OsuPlayer.IO.Storage.Config;

public class Config : IStorable<ConfigContainer>
{
    public string Path => "data/config.json";

    public ConfigContainer? Container { get; set; }

    public ConfigContainer Read()
    {
        if (!File.Exists(Path) || Container != null)
            return Container ??= new ConfigContainer();

        var data = File.ReadAllText(Path);

        return Container ??= (string.IsNullOrWhiteSpace(data)
            ? new ConfigContainer()
            : JsonConvert.DeserializeObject<ConfigContainer>(data))!;
    }

    public async Task<ConfigContainer> ReadAsync()
    {
        if (!File.Exists(Path) || Container != null)
            return Container ??= new ConfigContainer();

        var data = await File.ReadAllTextAsync(Path);

        return Container ??= (string.IsNullOrWhiteSpace(data)
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
        if (Container != default)
            Save(Container);
    }

    public async ValueTask DisposeAsync()
    {
        if (Container != default)
            await SaveAsync(Container);
    }
}