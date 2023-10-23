namespace OsuPlayer.Interfaces.Service;

/// <summary>
/// This interface provides a service provider for shuffle implementations.
/// </summary>
public interface IShuffleServiceProvider
{
    public List<IShuffleImpl> ShuffleAlgorithms { get; }
    public IShuffleImpl? ShuffleImpl { get; }

    /// <summary>
    /// Sets the shuffle algorithm.
    /// </summary>
    /// <param name="algorithm">The <see cref="IShuffleImpl" /> to set.</param>
    public void SetShuffleImpl(IShuffleImpl? algorithm);
}