namespace OsuPlayer.IO.Storage;

public interface IStorable<T> : IDisposable, IAsyncDisposable where T : IStorableContainer
{
    public string Path { get; }
    public T Container { get; set; }
    public T Read();
    public Task<T> ReadAsync();
    public void Save(T container);
    public Task SaveAsync(T container);
}