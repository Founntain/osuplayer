// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace OsuPlayer.Extensions.Bindables;

public class Cached<T>
{
    private T _value;

    public T Value
    {
        get
        {
            if (!IsValid)
                throw new InvalidOperationException($"May not query {nameof(Value)} of an invalid {nameof(Cached<T>)}.");

            return _value;
        }

        set
        {
            _value = value;
            IsValid = true;
            //FrameStatistics.Increment(StatisticsCounterType.Refreshes);
        }
    }

    public bool IsValid { get; private set; }

    public static implicit operator T(Cached<T> value)
    {
        return value.Value;
    }

    /// <summary>
    /// Invalidate the cache of this object.
    /// </summary>
    /// <returns>True if we invalidated from a valid state.</returns>
    public bool Invalidate()
    {
        if (!IsValid) return false;
        
        IsValid = false;
        //FrameStatistics.Increment(StatisticsCounterType.Invalidations);
        return true;
    }
}