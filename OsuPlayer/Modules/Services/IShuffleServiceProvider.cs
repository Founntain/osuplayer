using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Modules.ShuffleImpl;

namespace OsuPlayer.Modules.Services;

/// <summary>
/// This interface provides a service provider for shuffle implementations.
/// </summary>
public interface IShuffleServiceProvider
{
    public List<ShuffleAlgorithm> ShuffleAlgorithms { get; }
    public IShuffleImpl? ShuffleImpl { get; }

    /// <summary>
    /// Sets the shuffle algorithm.
    /// </summary>
    /// <param name="algorithm">The <see cref="ShuffleAlgorithm"/> to set.</param>
    public void SetShuffleImpl(ShuffleAlgorithm? algorithm);
}