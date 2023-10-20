using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Services.Interfaces;

namespace OsuPlayer.Services.ShuffleImpl;

/// <summary>
/// This shuffle algorithm performs a full reshuffle of the song list so there aren't any duplicates and biases.
/// </summary>
public class BalancedShuffler : OsuPlayerService, IShuffleImpl
{
    private readonly List<int> _shuffledIndexes = new();
    private int _currentIndex;

    private int _maxRange;

    public string Name => "Balanced Shuffle";
    public string Description => "Shuffles the entire list of songs so there aren't any duplicates and biases.";

    public override string ServiceName => "BALANCED_SHUFFLE_SERVICE";

    public void Init(int maxRange)
    {
        if (_maxRange == maxRange) return;

        _maxRange = maxRange;
        _currentIndex = 0;

        if (_maxRange == 0) return;

        _shuffledIndexes.Clear();
        _shuffledIndexes.Capacity = _maxRange;

        GenerateRandomIndexes();
    }

    public int DoShuffle(int currentIndex, ShuffleDirection direction)
    {
        if (_shuffledIndexes[_currentIndex] != currentIndex) _currentIndex = _shuffledIndexes.IndexOf(currentIndex);

        _currentIndex += (int) direction;

        if (_currentIndex >= _maxRange) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxRange - 1;

        return _shuffledIndexes[_currentIndex];
    }

    private void GenerateRandomIndexes()
    {
        for (var i = 0; i < _maxRange; i++) _shuffledIndexes.Add(i);

        var random = new Random();
        var n = _maxRange - 1;

        while (n > 1)
        {
            var k = random.Next(n--);
            (_shuffledIndexes[n], _shuffledIndexes[k]) = (_shuffledIndexes[k], _shuffledIndexes[n]);
        }
    }

    public override string ToString()
    {
        return Name;
    }
}