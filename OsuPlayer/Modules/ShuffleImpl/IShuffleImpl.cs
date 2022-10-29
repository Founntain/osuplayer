using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Modules.ShuffleImpl;

/// <summary>
/// This interface is used as a base for all shuffle algorithm implementations.
/// </summary>
public interface IShuffleImpl
{
    public string Name { get; }
    public string Description { get; }

    /// <summary>
    /// Inits the shuffler with the given parameters. This must be called before <see cref="DoShuffle" /> each time to ensure
    /// the shuffler is initialized with valid params.
    /// </summary>
    /// <param name="maxRange">The max index range the shuffler will return with <see cref="DoShuffle" /></param>
    void Init(int maxRange);

    /// <summary>
    /// Provides a method to generate a shuffled index based on simple parameters.
    /// </summary>
    /// <param name="currentIndex">The current song index the shuffle algorithm is based on</param>
    /// <param name="direction">The <see cref="ShuffleDirection" /> to shuffle to</param>
    /// <returns>a random generated (shuffled) index</returns>
    int DoShuffle(int currentIndex, ShuffleDirection direction);
}