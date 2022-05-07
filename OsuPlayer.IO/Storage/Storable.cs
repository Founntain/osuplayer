using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OsuPlayer.IO.Storage;

/// <summary>
/// This abstract class represents a storage. This storage is used to write or read a given object of type
/// <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">the container which this <see cref="Storable{T}" /> uses</typeparam>
public abstract class Storable<T> : IDisposable, IAsyncDisposable where T : IStorableContainer, new()
{
    private T? _storableContainer;

    protected virtual JsonSerializerSettings SerializerSettings { get; } = new()
    {
        Formatting = Formatting.Indented
    };

    /// <summary>
    /// The path in which the <see cref="IStorableContainer" /> is to be stored
    /// </summary>
    public virtual string? Path => null;

    /// <summary>
    /// The container of the <see cref="Storable{T}" /> type for external access
    /// </summary>
    /// <remarks>
    /// If accessed before a <see cref="Read" /> or <see cref="ReadAsync" /> it calls a <see cref="Read" />
    /// to ensure the container is not null for easier handling
    /// </remarks>
    public T Container
    {
        get => _storableContainer ?? Read();
        set => _storableContainer = value;
    }

    public async ValueTask DisposeAsync()
    {
        if (_storableContainer != null)
            await SaveAsync(_storableContainer);

        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        if (_storableContainer != null)
            Save(_storableContainer);

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Reads the corresponding file as set in the <see cref="Path" /> from the disk
    /// </summary>
    /// <returns>the read <see cref="IStorableContainer" /> instance</returns>
    public virtual T Read()
    {
        if (!File.Exists(Path) || _storableContainer != null)
            return _storableContainer ??= (T) new T().Init();

        var data = File.ReadAllText(Path);

        try
        {
            return _storableContainer ??= (string.IsNullOrWhiteSpace(data)
                ? (T) new T().Init()
                : JsonConvert.DeserializeObject<T>(data, SerializerSettings))!;
        }
        catch (JsonReaderException e)
        {
            var rawJson = JObject.Parse(data);
            rawJson.Remove(e.Path ?? string.Empty);
            return _storableContainer = JsonConvert.DeserializeObject<T>(rawJson.ToString(), SerializerSettings)!;
        }
    }

    /// <summary>
    /// Reads the corresponding file as set in the <see cref="Path" /> from the disk asynchronously
    /// </summary>
    /// <returns>the read <see cref="StorableContainer" /> instance</returns>
    public virtual async Task<T> ReadAsync()
    {
        if (!File.Exists(Path) || _storableContainer != null)
            return _storableContainer ??= (T) new T().Init();

        var data = await File.ReadAllTextAsync(Path);

        try
        {
            return _storableContainer ??= (string.IsNullOrWhiteSpace(data)
                ? (T) new T().Init()
                : JsonConvert.DeserializeObject<T>(data, SerializerSettings))!;
        }
        catch (JsonReaderException e)
        {
            var rawJson = JObject.Parse(data);
            rawJson.Remove(e.Path ?? string.Empty);
            return _storableContainer = JsonConvert.DeserializeObject<T>(rawJson.ToString(), SerializerSettings)!;
        }
    }

    /// <summary>
    /// Saves the file to the disk
    /// </summary>
    /// <param name="container">the <see cref="StorableContainer" /> to be saved</param>
    /// <remarks>Also happens on dispose of the <see cref="Storable{T}" /> instance</remarks>
    public virtual void Save(T container)
    {
        if (Path == default)
            return;

        Directory.CreateDirectory("data");

        File.WriteAllText(Path, JsonConvert.SerializeObject(container, SerializerSettings));
    }

    /// <summary>
    /// Saves the file to the disk asynchronously
    /// </summary>
    /// <param name="container">the <see cref="StorableContainer" /> to be saved</param>
    /// <remarks>Also happens on dispose of the <see cref="Storable{T}" /> instance</remarks>
    public virtual async Task SaveAsync(T container)
    {
        if (Path == default)
            return;

        Directory.CreateDirectory("data");

        await File.WriteAllTextAsync(Path, JsonConvert.SerializeObject(container, SerializerSettings));
    }
}