using Newtonsoft.Json;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace OsuPlayer.IO.Storage.Equalizer;

public class EqStorage : IStorable<EqContainer>
{
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        Formatting = Formatting.Indented
    };

    private EqContainer? _container;

    public string Path => System.IO.Path.Combine("data", "eqPresets.json");

    public EqContainer Container
    {
        get => _container ?? Read();
        set => _container = value;
    }

    public EqContainer Read()
    {
        if (!File.Exists(Path) || _container != null)
            return _container ??= new EqContainer().Init();

        var data = File.ReadAllText(Path);

        return _container ??= (string.IsNullOrWhiteSpace(data)
            ? new EqContainer().Init()
            : JsonConvert.DeserializeObject<EqContainer>(data, _serializerSettings))!;
    }

    public async Task<EqContainer> ReadAsync()
    {
        if (!File.Exists(Path) || _container != null)
            return _container ??= new EqContainer().Init();

        var data = await File.ReadAllTextAsync(Path);

        return _container ??= (string.IsNullOrWhiteSpace(data)
            ? new EqContainer().Init()
            : JsonConvert.DeserializeObject<EqContainer>(data, _serializerSettings))!;
    }

    public void Save(EqContainer container)
    {
        Directory.CreateDirectory("data");

        File.WriteAllText(Path, JsonConvert.SerializeObject(container, _serializerSettings));
    }

    public async Task SaveAsync(EqContainer container)
    {
        Directory.CreateDirectory("data");

        await File.WriteAllTextAsync(Path, JsonConvert.SerializeObject(container, _serializerSettings));
    }

    public void Dispose()
    {
        if (_container != default)
            Save(_container);
    }

    public async ValueTask DisposeAsync()
    {
        if (_container != default)
            await SaveAsync(_container);
    }
}