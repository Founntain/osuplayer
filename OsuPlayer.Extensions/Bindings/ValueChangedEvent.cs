namespace OsuPlayer.Extensions.Bindings;

public class ValueChangedEvent<T>
{
    public readonly T OldValue;

    public readonly T NewValue;

    public ValueChangedEvent(T oldValue, T newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}