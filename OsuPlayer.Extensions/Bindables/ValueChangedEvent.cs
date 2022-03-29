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