// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace OsuPlayer.Extensions.Bindables;

public class ValueChangedEvent<T>
{
    public readonly T NewValue;
    public readonly T OldValue;

    public ValueChangedEvent(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}