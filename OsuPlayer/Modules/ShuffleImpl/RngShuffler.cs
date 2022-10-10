using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;

namespace OsuPlayer.Modules.ShuffleImpl;

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