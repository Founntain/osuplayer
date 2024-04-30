namespace OsuPlayer.Interfaces.Service;

public interface IJsonService
{
    /// <summary>
    /// Deserializes the JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>An object of type <typeparamref name="T"/>.</returns>
    T Deserialize<T>(string json);

    /// <summary>
    /// Deserializes the JSON string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="jsonStream">The JSON string to deserialize.</param>
    /// <returns>An object of type <typeparamref name="T"/>.</returns>
    T Deserialize<T>(Stream jsonStream);

    /// <summary>
    /// Deserializes the JSON string to an object of type <typeparamref name="T"/> asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A task representing the asynchronous operation that returns an object of type <typeparamref name="T"/>.</returns>
    Task<T> DeserializeAsync<T>(string json);

    /// <summary>
    /// Deserializes the JSON string to an object of type <typeparamref name="T"/> asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="jsonStream">The JSON string to deserialize.</param>
    /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation that returns an object of type <typeparamref name="T"/>.</returns>
    Task<T> DeserializeAsync<T>(Stream jsonStream);

    /// <summary>
    /// Serializes an object to a JSON string asynchronously.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation. The task result contains the JSON string.</returns>
    Task<string> SerializeToJsonStringAsync(object obj);

    /// <summary>
    /// Serializes the object to a JSON file at the specified file path.
    /// </summary>
    /// <param name="filePath">The file path to save the JSON file.</param>
    /// <param name="obj">The object to serialize.</param>
    void SerializeToJsonFile(string filePath, object obj);

    /// <summary>
    /// Serializes an object to JSON and writes it to a file asynchronously.
    /// </summary>
    /// <param name="filePath">The path to the file where the JSON should be written.</param>
    /// <param name="obj">The object to serialize and write to the file.</param>
    /// <returns>A <see cref="Task{T}"/> representing the asynchronous operation.</returns>
    Task SerializeToJsonFileAsync(string filePath, object obj);
}