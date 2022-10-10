using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Modules.Audio.Interfaces;

namespace OsuPlayer.Modules.Audio;

public class AltSongShuffler : IShuffleProvider
{
    private List<int> _shuffledIndexes;
    private int _maxRange;

    public void Init(int maxRange)
    {
        if (_maxRange == maxRange) return;

        _maxRange = maxRange;
        _shuffledIndexes.Clear();
        _shuffledIndexes.Capacity = _maxRange;
    }

    public int DoShuffle(int currentIndex, ShuffleDirection direction)
    {
        return _shuffledIndexes[currentIndex + 1];
    }

    private void GenerateRandomIndexes()
    {
        
    }
}