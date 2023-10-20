using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Extensions;
using OsuPlayer.Services.Interfaces;

namespace OsuPlayer.Services.ShuffleImpl;

/// <summary>
/// This shuffle implementation will randomly select a song each time with no further logic.
/// </summary>
[DefaultImplAttr]
public class RngShuffler : OsuPlayerService, IShuffleImpl
{
    private int _maxRange = -1;

    public string Name => "Random Shuffle";
    public string Description => "Randomly select a song each time with no further logic.";

    public override string ServiceName => "RNG_SHUFFLE_SERVICE";

    public void Init(int maxRange) => _maxRange = maxRange;

    public int DoShuffle(int currentIndex, ShuffleDirection direction) => Random.Shared.Next(_maxRange);

    public override string ToString() => Name;
}