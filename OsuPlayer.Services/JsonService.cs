using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OsuPlayer.Interfaces.Service;

namespace OsuPlayer.Services;

public class JsonService : IJsonService
{
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonService()
    {
        _jsonOptions = new()
        {
            NumberHandling = JsonNumberHandling.Strict,
            IncludeFields = true,
            WriteIndented = true,
            AllowTrailingCommas = true,
            PropertyNameCaseInsensitive = true,
        };
    }

    public JsonService(JsonSerializerOptions jsonOptions)
    {
        _jsonOptions = jsonOptions;
    }

    public T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    public T Deserialize<T>(Stream jsonStream)
    {
        return JsonSerializer.Deserialize<T>(jsonStream, _jsonOptions);
    }

    public async Task<T> DeserializeAsync<T>(string json)
    {
        var data = Encoding.UTF8.GetBytes(json);
        var stream = new MemoryStream(data);

        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions);
    }

    public async Task<T> DeserializeAsync<T>(Stream jsonStream)
    {
        return await JsonSerializer.DeserializeAsync<T>(jsonStream, _jsonOptions);
    }

    public async Task<string> SerializeToJsonStringAsync(object obj)
    {
        using var stream = new MemoryStream();

        await JsonSerializer.SerializeAsync(stream, obj);

        stream.Position = 0;

        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync();
    }

    public void SerializeToJsonFile(string filePath, object obj)
    {
        using var fs = File.Create(filePath);
        JsonSerializer.Serialize(fs, obj);
    }

    public async Task SerializeToJsonFileAsync(string filePath, object obj)
    {
        await using var fs = File.Create(filePath);
        await JsonSerializer.SerializeAsync(fs, obj);
    }
}