using OsuPlayer.Data.OsuPlayer.Enums;
using OsuPlayer.Services.Interfaces;

namespace OsuPlayer.Services.ShuffleImpl;

/// <summary>
/// This shuffle implementation is mostly the same as <see cref="RngShuffler" /> but adds a 10 depth history buffer so one
/// can go back to the previous song.<br />
/// This also has a security check to prevent the same song from being played twice in a row.
/// </summary>
public class RngHistoryShuffler : OsuPlayerService, IShuffleImpl
{
    private readonly int?[] _shuffleHistory = new int?[10];
    private int _currentIndex;

    private int _maxRange = -1;
    private int _shuffleHistoryIndex;

    public string Name => "Random Shuffle (with history)";
    public string Description => "Randomly picks a song from the list with a history buffer of 10 songs.";

    public override string ServiceName => "RNG_HISTORY_SHUFFLE_SERVICE";

    public void Init(int maxRange) => _maxRange = maxRange;

    public int DoShuffle(int currentIndex, ShuffleDirection direction)
    {
        if (_maxRange == -1) return -1;

        if (currentIndex < 0)
            currentIndex = GenerateShuffledIndex();

        _currentIndex = currentIndex;

        switch (direction)
        {
            case ShuffleDirection.Forward:
            {
                ForwardShuffle();

                break;
            }
            case ShuffleDirection.Backwards:
            {
                BackwardShuffle();

                break;
            }
        }

        var shuffledIndex = _shuffleHistory[_shuffleHistoryIndex];

        return shuffledIndex ?? 0;
    }

    /// <summary>
    /// Does a forward shuffle operation
    /// </summary>
    private void ForwardShuffle()
    {
        // Next index if not at array end
        if (_shuffleHistoryIndex < _shuffleHistory.Length - 1)
        {
            GetNextShuffledIndex();
        }
        // Move array one down if at the top of the array
        else
        {
            Array.Copy(_shuffleHistory, 1, _shuffleHistory, 0, _shuffleHistory.Length - 1);

            _shuffleHistory[_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Does a backward shuffle operation
    /// </summary>
    private void BackwardShuffle()
    {
        // Prev index if index greater than zero
        if (_shuffleHistoryIndex > 0)
        {
            GetPreviousShuffledIndex();
        }
        // Move array one up if at the start of the array
        else
        {
            Array.Copy(_shuffleHistory, 0, _shuffleHistory, 1, _shuffleHistory.Length - 1);

            _shuffleHistory[_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Generates the next shuffled index in <see cref="_shuffleHistory" />
    /// <seealso cref="GenerateShuffledIndex" />
    /// </summary>
    private void GetNextShuffledIndex()
    {
        // If there is no "next" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex + 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = _currentIndex;
            _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "next" song in the history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex + 1] < _maxRange)
                _shuffleHistoryIndex++;
            // Generate new shuffled index when not
            else
                _shuffleHistory[++_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    /// <summary>
    /// Generates the previous shuffled index in <see cref="_shuffleHistory" />
    /// <seealso cref="GenerateShuffledIndex" />
    /// </summary>
    private void GetPreviousShuffledIndex()
    {
        // If there is no "prev" song generate new shuffled index
        if (_shuffleHistory[_shuffleHistoryIndex - 1] == null)
        {
            _shuffleHistory[_shuffleHistoryIndex] = _currentIndex;
            _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
        // There is a "prev" song in history
        else
        {
            // Check if next song index is in allowed boundary
            if (_shuffleHistory[_shuffleHistoryIndex - 1] < _maxRange)
                _shuffleHistoryIndex--;
            // Generate new shuffled index when not
            else
                _shuffleHistory[--_shuffleHistoryIndex] = GenerateShuffledIndex();
        }
    }

    private int GenerateShuffledIndex()
    {
        var rdm = new Random();

        int shuffleIndex;

        do
        {
            shuffleIndex = rdm.Next(0, _maxRange);
        } while (shuffleIndex == _currentIndex);

        return shuffleIndex;
    }

    public override string ToString()
    {
        return Name;
    }
}