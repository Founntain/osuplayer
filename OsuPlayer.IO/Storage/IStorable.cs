namespace OsuPlayer.IO.Storage;

/// <summary>
/// This interface represents a storage. This storage is used to write or read a given object of type T.
/// The object needs to implement the <see cref="IStorableContainer" /> interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStorable<T> : IDisposable, IAsyncDisposable where T : IStorableContainer
{
    public string Path { get; }
    public T Container { get; set; }
    public T Read();
    public Task<T> ReadAsync();
    public void Save(T container);
    public Task SaveAsync(T container);
}