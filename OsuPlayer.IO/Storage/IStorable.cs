using OsuPlayer.Extensions.Storage;

namespace OsuPlayer.IO.Storage;

public interface IStorable<T> : IDisposable where T : IStorableContainer
{
    public string Path { get; }
    public T Container { get; set; }
    public T Read();
    public Task<T> ReadAsync();
    public void Save(T storableContainer);
    public Task SaveAsync(T storableContainer);
}