namespace OsuPlayer.IO.Storage;

/// <summary>
/// This interface represents a storage. This storage is used to write or read a given object of type T.
/// The object needs to implement the <see cref="IStorableContainer" /> interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStorable<T> : IDisposable, IAsyncDisposable where T : IStorableContainer
{
    /// <summary>
    /// The path in which the <see cref="IStorableContainer" /> is to be stored
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The container of the <see cref="IStorable{T}" /> type for external access
    /// </summary>
    public T Container { get; set; }

    /// <summary>
    /// Reads the corresponding file as set in the <see cref="Path" /> from the disk
    /// </summary>
    /// <returns>the read <see cref="IStorableContainer" /> instance</returns>
    public T Read();

    /// <summary>
    /// Reads the corresponding file as set in the <see cref="Path" /> from the disk asynchronously
    /// </summary>
    /// <returns>the read <see cref="IStorableContainer" /> instance</returns>
    public Task<T> ReadAsync();

    /// <summary>
    /// Saves the file to the disk
    /// </summary>
    /// <param name="container">the <see cref="IStorableContainer" /> to be saved</param>
    /// <remarks>Also happens on dispose of the <see cref="IStorable{T}" /> instance</remarks>
    public void Save(T container);

    /// <summary>
    /// Saves the file to the disk asynchronously
    /// </summary>
    /// <param name="container">the <see cref="IStorableContainer" /> to be saved</param>
    /// <remarks>Also happens on dispose of the <see cref="IStorable{T}" /> instance</remarks>
    public Task SaveAsync(T container);
}