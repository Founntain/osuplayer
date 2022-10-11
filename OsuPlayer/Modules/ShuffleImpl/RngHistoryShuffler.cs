using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Modules.ShuffleImpl;

/// <summary>
/// This shuffle implementation is mostly the same as <see cref="RngShuffler"/> but adds a 10 depth history buffer so one can go back to the previous song.<br/>
/// This also has a security check to prevent the same song from being played twice in a row.
/// </summary>
public class RngHistoryShuffler : IShuffleImpl
{
    private int _shuffleHistoryIndex;
    private readonly int?[] _shuffleHistory = new int?[10];

    private int _maxRange = -1;
    private int _currentIndex;

    public void Init(int maxRange)
    {
        _maxRange = maxRange;
    }

    public int DoShuffle(int currentIndex, ShuffleDirection direction)
    {
        if (_maxRange == -1) return -1;

        _currentIndex = currentIndex;

        switch (direction)
        {
            case ShuffleDirection.Forward:
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

                break;
            }
            case ShuffleDirection.Backwards:
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

                break;
            }
        }

        var shuffledIndex = _shuffleHistory[_shuffleHistoryIndex];

        return shuffledIndex ?? 0;
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
}