using System.ComponentModel;

namespace OsuPlayer.Extensions.Bindings;

public class Bindable<T> : IBindable<T>
{
    public event Action<ValueChangedEvent<T>>? ValueChanged;

    private bool _ignoreSource;

    private T _value;

    public virtual T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;

            SetValue(_value, value, _ignoreSource);
        }
    }

    /// <summary>
    /// Unbinds from all bindings on finalization
    /// </summary>
    ~Bindable()
    {
        UnbindAll();
    }

    /// <summary>
    /// Binds a <see cref="IBindable{T}"/> to this <see cref="Bindable{T}"/> if it's the same type
    /// </summary>
    /// <param name="other">other <see cref="IBindable{T}"/> to bind to</param>
    void IBindable<T>.BindTo(IBindable<T> other)
    {
        if (other is Bindable<T> otherB)
            BindTo(otherB);
    }

    protected List<Bindable<T>>? Bindings { get; private set; }

    /// <summary>
    /// Sets the value with <see cref="TriggerValueChanged"/>
    /// </summary>
    /// <param name="previous">the previous value before value change</param>
    /// <param name="value">the value to change to</param>
    /// <param name="bypassChecks">bool to indicate if checks should be bypassed</param>
    /// <param name="source">invocation source</param>
    internal void SetValue(T previous, T value, bool bypassChecks = false, Bindable<T>? source = null)
    {
        _value = value;
        TriggerValueChanged(previous, source ?? this, true, bypassChecks);
    }

    /// <summary>
    /// Updates the value on all bindings based on <paramref name="propagateToBindings"/> and triggers a <see cref="ValueChangedEvent{T}"/>
    /// </summary>
    /// <param name="previousValue">the previous value before value change</param>
    /// <param name="source">invocation source</param>
    /// <param name="propagateToBindings">bool to indicate if all binding values should be updated</param>
    /// <param name="bypassChecks">bool to indicate if checks should be bypassed</param>
    protected void TriggerValueChanged(T previousValue, Bindable<T> source, bool propagateToBindings = true, bool bypassChecks = false)
    {
        T beforePropagation = _value;

        if (propagateToBindings && Bindings != null)
        {
            foreach (var binding in Bindings)
            {
                if (binding == source) continue;
                
                binding.SetValue(previousValue, _value, bypassChecks, this);
            }
        }

        if (EqualityComparer<T>.Default.Equals(beforePropagation, _value) && (source != this || bypassChecks))
            ValueChanged?.Invoke(new ValueChangedEvent<T>(previousValue, _value));
    }

    /// <summary>
    /// Binds an <paramref name="other"/> <see cref="Bindable{T}"/> to this
    /// </summary>
    /// <param name="other">other <see cref="Bindable{T}"/> to bind to</param>
    /// <exception cref="InvalidOperationException">throws if the <paramref name="other"/> is already bound to this</exception>
    public virtual void BindTo(Bindable<T> other)
    {
        if (Bindings?.Contains(other) == true)
            throw new InvalidOperationException($"Current bindable is already bound to {other}");

        Value = other.Value;

        AddReference(other);
        other.AddReference(this);
    }

    /// <summary>
    /// Unbinds <paramref name="other"/> <see cref="IUnbindable"/> from this and removes references
    /// </summary>
    /// <param name="other">the <see cref="IUnbindable"/> to unbind from</param>
    /// <exception cref="InvalidCastException">throws if types don't match</exception>
    public virtual void UnbindFrom(IUnbindable other)
    {
        if (other is not Bindable<T> otherB)
            throw new InvalidCastException($"Can't unbind type {other.GetType()} from type {GetType()}");

        RemoveReference(otherB);
        otherB.RemoveReference(this);
    }

    /// <summary>
    /// Binds a <see cref="ValueChangedEvent{T}"/> to a <see cref="Bindable{T}"/>
    /// </summary>
    /// <param name="onChange"></param>
    /// <param name="ignoreSource"></param>
    /// <param name="runOnceImmediately"></param>
    public void BindValueChanged(Action<ValueChangedEvent<T>> onChange, bool ignoreSource = false, bool runOnceImmediately = false)
    {
        ValueChanged += onChange;
        _ignoreSource = ignoreSource;
        if (runOnceImmediately)
            onChange(new ValueChangedEvent<T>(Value, Value));
    }

    /// <summary>
    /// Unbinds all bindings
    /// </summary>
    public void UnbindBindings()
    {
        if (Bindings == null)
            return;

        foreach (var binding in Bindings)
        {
            UnbindFrom(binding);
        }
    }

    /// <summary>
    /// Unbinds all <see cref="ValueChangedEvent{T}"/>
    /// </summary>
    public virtual void UnbindEvents()
    {
        ValueChanged = null;
    }

    /// <summary>
    /// Unbinds all
    /// </summary>
    public void UnbindAll() => UnbindAllInternal();

    /// <summary>
    /// Adds a reference to <see cref="Bindings"/>
    /// </summary>
    /// <param name="reference">the <see cref="Bindable{T}"/> to add a reference to</param>
    private void AddReference(Bindable<T> reference)
    {
        Bindings ??= new List<Bindable<T>>();
        Bindings.Add(reference);
    }

    /// <summary>
    /// Removes a reference from <see cref="Bindings"/>
    /// </summary>
    /// <param name="reference">the <see cref="Bindable{T}"/> to remove a reference from</param>
    private void RemoveReference(Bindable<T> reference) => Bindings?.Remove(reference);

    /// <summary>
    /// Calls all unbinds
    /// </summary>
    /// <remarks>for usage in <see cref="UnbindAll"/></remarks>
    internal virtual void UnbindAllInternal()
    {
        UnbindEvents();
        UnbindBindings();
    }
}