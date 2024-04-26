using System.Text.Json;
using Nein.Extensions;
using OsuPlayer.Data.OsuPlayer.StorageModels;
using OsuPlayer.Interfaces.Service;
using Splat;

namespace OsuPlayer.IO.Storage;

/// <summary>
/// This abstract class represents a storage. This storage is used to write or read a given object of type
/// <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">the container which this <see cref="Storable{T}" /> uses</typeparam>
public abstract class Storable<T> : IDisposable, IAsyncDisposable where T : IStorableContainer, new()
{
    private T? _storableContainer;
    private readonly IJsonService _jsonService;

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

    protected Storable()
    {
        _jsonService = Locator.Current.GetRequiredService<IJsonService>();
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
                : _jsonService.Deserialize<T>(data))!;
        }
        catch (JsonException)
        {
            return _storableContainer ??= (T) new T().Init();
        }
    }

    /// <summary>
    /// Reads the corresponding file as set in the <see cref="Path" /> from the disk asynchronously
    /// </summary>
    /// <returns>the read <see cref="IStorableContainer" /> instance</returns>
    public virtual async Task<T> ReadAsync()
    {
        if (!File.Exists(Path) || _storableContainer != null)
            return _storableContainer ??= (T) new T().Init();

        var data = await File.ReadAllTextAsync(Path);

        try
        {
            return _storableContainer ??= (string.IsNullOrWhiteSpace(data)
                ? (T) new T().Init()
                : await _jsonService.DeserializeAsync<T>(data))!;
        }
        catch (JsonException)
        {
            return _storableContainer ??= (T)new T().Init();
        }
    }

    /// <summary>
    /// Saves the file to the disk
    /// </summary>
    /// <param name="container">the <see cref="IStorableContainer" /> to be saved</param>
    /// <remarks>Also happens on dispose of the <see cref="Storable{T}" /> instance</remarks>
    public virtual void Save(T container)
    {
        if (Path == default)
            return;

        Directory.CreateDirectory("data");

        TrySave(container);
    }

    private void TrySave(T container)
    {
        for (var i = 0; i < 3; i++)
            try
            {
                _jsonService.SerializeToJsonFile(Path!, container);
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(50);
            }
    }

    /// <summary>
    /// Saves the file to the disk asynchronously
    /// </summary>
    /// <param name="container">the <see cref="IStorableContainer" /> to be saved</param>
    /// <remarks>Also happens on dispose of the <see cref="Storable{T}" /> instance</remarks>
    public virtual async Task SaveAsync(T container)
    {
        if (Path == default)
            return;

        Directory.CreateDirectory("data");

        await TrySaveAsync(container);
    }

    private async Task TrySaveAsync(T container)
    {
        for (var i = 0; i < 3; i++)
            try
            {
                await _jsonService.SerializeToJsonFileAsync(Path!, container);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(50);
            }
    }
}