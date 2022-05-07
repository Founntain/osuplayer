using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace OsuPlayer.IO.Storage.Config;

public class Config : IStorable<ConfigContainer>
{
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        Formatting = Formatting.Indented
    };

    private ConfigContainer? _configContainer;

    public string Path => System.IO.Path.Combine("data", "config.json");

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

        try
        {
            return _configContainer ??= (string.IsNullOrWhiteSpace(data)
                ? new ConfigContainer()
                : JsonConvert.DeserializeObject<ConfigContainer>(data, _serializerSettings))!;
        }
        catch (JsonReaderException e)
        {
            var rawJson = JObject.Parse(data);
            rawJson.Remove(e.Path ?? string.Empty);
            return _configContainer = JsonConvert.DeserializeObject<ConfigContainer>(rawJson.ToString(), _serializerSettings)!;
        }
    }

    public async Task<ConfigContainer> ReadAsync()
    {
        if (!File.Exists(Path) || _configContainer != null)
            return _configContainer ??= new ConfigContainer();

        var data = await File.ReadAllTextAsync(Path);

        try
        {
            return _configContainer ??= (string.IsNullOrWhiteSpace(data)
                ? new ConfigContainer()
                : JsonConvert.DeserializeObject<ConfigContainer>(data, _serializerSettings))!;
        }
        catch (JsonReaderException e)
        {
            var rawJson = JObject.Parse(data);
            rawJson.Remove(e.Path ?? string.Empty);
            return _configContainer = JsonConvert.DeserializeObject<ConfigContainer>(rawJson.ToString(), _serializerSettings)!;
        }
    }

    public void Save(ConfigContainer container)
    {
        Directory.CreateDirectory("data");

        File.WriteAllText(Path, JsonConvert.SerializeObject(container, _serializerSettings));
    }

    public async Task SaveAsync(ConfigContainer container)
    {
        Directory.CreateDirectory("data");

        await File.WriteAllTextAsync(Path, JsonConvert.SerializeObject(container, _serializerSettings));
    }

    public void Dispose()
    {
        if (_configContainer != default)
            Save(_configContainer);

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_configContainer != default)
            await SaveAsync(_configContainer);

        GC.SuppressFinalize(this);
    }
}