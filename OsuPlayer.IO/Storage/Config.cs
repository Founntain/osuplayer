using Newtonsoft.Json;
using OsuPlayer.Extensions.Storage;

namespace OsuPlayer.IO.Storage;

public class Config : IStorable<ConfigContainer>
{
    public string Path => "data/config.json";
    
    private ConfigContainer? _container;
    public ConfigContainer Container
    {
        get => _container!;
        set => _container = value;
    }

    public ConfigContainer Read()
    {
        if (!File.Exists(Path) || _container != null)
            return _container ??= new ConfigContainer();
        
        var data = File.ReadAllText(Path);

        return _container ??= (string.IsNullOrWhiteSpace(data)
            ? new ConfigContainer()
            : JsonConvert.DeserializeObject<ConfigContainer>(data))!;

    }
    
    public async Task<ConfigContainer> ReadAsync()
    {
        if (!File.Exists(Path) || _container != null)
            return _container ??= new ConfigContainer();
        
        var data = await File.ReadAllTextAsync(Path);

        return _container ??= (string.IsNullOrWhiteSpace(data)
            ? new ConfigContainer()
            : JsonConvert.DeserializeObject<ConfigContainer>(data))!;

    }

    public void Save(ConfigContainer config)
    {
        File.WriteAllText(Path, JsonConvert.SerializeObject(config));
    }

    public async Task SaveAsync(ConfigContainer config)
    {
        await File.WriteAllTextAsync(Path, JsonConvert.SerializeObject(config));
    }
    
    public void Dispose()
    {
        if (_container != null) 
            Save(_container);
    }
}