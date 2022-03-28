using System.ComponentModel;

namespace OsuPlayer.Extensions.Bindings;

public class Bindable<T> : IBindable<T> where T : IComparable<T>, IConvertible, IEquatable<T>
{
    public event Action<ValueChangedEvent<T>>? ValueChanged;

    private T _value;

    public virtual T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;

            SetValue(_value, value);
        }
    }

    void IBindable<T>.BindTo(IBindable<T> other)
    {
        if (other is Bindable<T> otherB)
            BindTo(otherB);
    }

    protected List<Bindable<T>>? Bindings { get; private set; }

    internal void SetValue(T previous, T value, bool bypassChecks = false, Bindable<T> source = null)
    {
        _value = value;
        TriggerValueChanged(previous, source ?? this, bypassChecks);
    }

    protected void TriggerValueChanged(T previousValue, Bindable<T> source, bool bypassChecks = false)
    {
        if (EqualityComparer<T>.Default.Equals(_value, _value))
            ValueChanged?.Invoke(new ValueChangedEvent<T>(previousValue, _value));
    }

    public virtual void BindTo(Bindable<T> other)
    {
        if (Bindings?.Contains(other) == true)
            throw new InvalidOperationException($"Current bindable is already bound to {other}");

        Value = other.Value;

        AddReference(other);
        other.AddReference(this);
    }

    public virtual void UnbindFrom(IUnbindable other)
    {
        if (other is not Bindable<T> otherB)
            throw new InvalidCastException($"Can't unbind type {other.GetType()} from type {GetType()}");

        RemoveReference(otherB);
        otherB.RemoveReference(this);
    }

    public void BindValueChanged(Action<ValueChangedEvent<T>> onChange, bool runOnceImmediately = false)
    {
        ValueChanged += onChange;
        if (runOnceImmediately)
            onChange(new ValueChangedEvent<T>(Value, Value));
    }

    public void UnbindBindings()
    {
        if (Bindings == null)
            return;

        foreach (var binding in Bindings)
        {
            UnbindFrom(binding);
        }
    }

    public virtual void UnbindEvents()
    {
        ValueChanged = null;
    }

    public void UnbindAll() => UnbindAllInternal();

    private void AddReference(Bindable<T> weakReference)
    {
        Bindings ??= new List<Bindable<T>>();
        Bindings.Add(weakReference);
    }

    private void RemoveReference(Bindable<T> inst) => Bindings?.Remove(inst);

    internal virtual void UnbindAllInternal()
    {
        UnbindEvents();
        UnbindBindings();
    }
}