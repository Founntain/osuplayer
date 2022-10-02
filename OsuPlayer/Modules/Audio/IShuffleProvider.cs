using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Modules.Audio;

public interface IShuffleProvider
{
    /// <summary>
    /// Implements the shuffle logic <seealso cref="SongShuffler.GetNextShuffledIndex" />
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="direction">the <see cref="ShuffleDirection" /> to shuffle to</param>
    /// <param name="rangeMax"></param>
    /// <returns>a random/shuffled <see cref="IMapEntryBase" /></returns>
    int DoShuffle(int currentIndex, ShuffleDirection direction, int rangeMax);
}