using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;

namespace OsuPlayer.Modules.ShuffleImpl;

/// <summary>
/// This shuffle implementation will randomly select a song each time with no further logic.
/// </summary>
[DefaultImplAttr]
public class RngShuffler : IShuffleImpl
{
    private int _maxRange = -1;

    public void Init(int maxRange)
    {
        _maxRange = maxRange;
    }

    public int DoShuffle(int currentIndex, ShuffleDirection direction)
    {
        return Random.Shared.Next(_maxRange);
    }
}