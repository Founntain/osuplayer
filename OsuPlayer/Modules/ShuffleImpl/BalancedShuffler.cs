﻿using DynamicData;
using OsuPlayer.Data.OsuPlayer.Classes;
using OsuPlayer.Data.OsuPlayer.Enums;

namespace OsuPlayer.Modules.ShuffleImpl;

/// <summary>
/// This shuffle algorithm performs a full reshuffle of the song list so there aren't any duplicates and biases.
/// </summary>
[ImplInfoAttr("Balanced Shuffle", "Shuffles the entire list of songs so there aren't any duplicates and biases.")]
public class BalancedShuffler : IShuffleImpl
{
    private int[] _shuffledIndexes = Array.Empty<int>();
    private int _currentIndex;

    private int _maxRange;

    public void Init(int maxRange)
    {
        if (_maxRange == maxRange) return;

        _maxRange = maxRange;
        _currentIndex = 0;

        if (_maxRange == 0) return;

        _shuffledIndexes = new int[_maxRange];

        GenerateRandomIndexes();
    }

    public int DoShuffle(int currentIndex, ShuffleDirection direction)
    {
        if (_shuffledIndexes[_currentIndex] != currentIndex)
        {
            _currentIndex = _shuffledIndexes.IndexOf(currentIndex);
        }

        _currentIndex += (int) direction;

        if (_currentIndex >= _maxRange) _currentIndex = 0;
        if (_currentIndex < 0) _currentIndex = _maxRange - 1;

        return _shuffledIndexes[_currentIndex];
    }

    private void GenerateRandomIndexes()
    {
        for (var i = 0; i < _shuffledIndexes.Length; i++)
        {
            _shuffledIndexes[i] = i;
        }

        var random = new Random();
        var n = _maxRange - 1;

        while (n > 1)
        {
            var k = random.Next(n--);
            (_shuffledIndexes[n], _shuffledIndexes[k]) = (_shuffledIndexes[k], _shuffledIndexes[n]);
        }
    }
}