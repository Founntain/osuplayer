// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.
// THIS CODE WAS REFACTORED, TO RESOLVE ISSUES WITH PROJECT WARNINGS!

using OsuPlayer.Extensions.Lists;

namespace OsuPlayer.Extensions.Bindables;

/// <summary>
/// Generic implementation of <see cref="IBindable{T}" />
/// </summary>
/// <typeparam name="T">the type of the stored <see cref="Value" /></typeparam>
public class Bindable<T> : IBindable<T>, IBindable
{
    private bool _ignoreSource;

    private T _value;

    private WeakReference<Bindable<T>>? _weakReferenceInstance;

    private WeakReference<Bindable<T>> WeakReference => _weakReferenceInstance ??= new WeakReference<Bindable<T>>(this);

    protected LockedWeakList<Bindable<T>>? Bindings { get; private set; }

    public virtual T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;

            SetValue(_value, value, _ignoreSource);
        }
    }

    void IBindable.BindTo(IBindable other)
    {
        if (other is not Bindable<T> otherB)
            throw new InvalidCastException($"Can't bind to a bindable of type {other.GetType()} from a bindable of type {GetType()}.");

        BindTo(otherB);
    }

    IBindable IBindable.CreateInstance()
    {
        return CreateInstance();
    }

    IBindable IBindable.GetBoundCopy()
    {
        return GetBoundCopy();
    }

    /// <summary>
    /// An event raised when <see cref="Value" /> has changed
    /// </summary>
    public event Action<ValueChangedEvent<T>>? ValueChanged;

    /// <summary>
    /// Binds a <see cref="IBindable{T}" /> to this <see cref="Bindable{T}" /> if it's the same type
    /// </summary>
    /// <param name="other">other <see cref="IBindable{T}" /> to bind to</param>
    void IBindable<T>.BindTo(IBindable<T> other)
    {
        if (other is Bindable<T> otherB)
            BindTo(otherB);
    }

    /// <summary>
    /// Unbinds all bindings
    /// </summary>
    public void UnbindBindings()
    {
        if (Bindings == null)
            return;

        foreach (var binding in Bindings) UnbindFrom(binding);
    }

    /// <summary>
    /// Unbinds all <see cref="ValueChangedEvent{T}" />
    /// </summary>
    public virtual void UnbindEvents()
    {
        ValueChanged = null;
    }

    /// <summary>
    /// Unbinds all
    /// </summary>
    public void UnbindAll()
    {
        UnbindAllInternal();
    }

    /// <summary>
    /// Unbinds <paramref name="other" /> <see cref="IUnbindable" /> from this and removes references
    /// </summary>
    /// <param name="other">the <see cref="IUnbindable" /> to unbind from</param>
    /// <exception cref="InvalidCastException">throws if types don't match</exception>
    public virtual void UnbindFrom(IUnbindable other)
    {
        if (other is not Bindable<T> otherB)
            throw new InvalidCastException($"Can't unbind type {other.GetType()} from type {GetType()}");

        RemoveReference(otherB.WeakReference);
        otherB.RemoveReference(WeakReference);
    }

    /// <summary>
    /// Binds a <see cref="ValueChangedEvent{T}" /> to a <see cref="Bindable{T}" />
    /// </summary>
    /// <param name="onChange">the <see cref="Action{T}" /> that should be executed</param>
    /// <param name="ignoreSource">whether to ignore the trigger source</param>
    /// <param name="runOnceImmediately">whether to run the <paramref name="onChange" /> action once immediately</param>
    public void BindValueChanged(Action<ValueChangedEvent<T>> onChange, bool ignoreSource = false, bool runOnceImmediately = false)
    {
        ValueChanged += onChange;
        
        _ignoreSource = ignoreSource;
        
        if (runOnceImmediately)
            onChange(new ValueChangedEvent<T>(Value, Value));
    }

    IBindable<T> IBindable<T>.GetBoundCopy()
    {
        return GetBoundCopy();
    }

    /// <summary>
    /// Unbinds from all bindings on finalization
    /// </summary>
    ~Bindable()
    {
        UnbindAll();
    }

    /// <summary>
    /// Sets the value with <see cref="TriggerValueChanged" />
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
    /// Updates the value on all bindings based on <paramref name="propagateToBindings" /> and triggers a
    /// <see cref="ValueChangedEvent{T}" />
    /// </summary>
    /// <param name="previousValue">the previous value before value change</param>
    /// <param name="source">invocation source</param>
    /// <param name="propagateToBindings">bool to indicate if all binding values should be updated</param>
    /// <param name="bypassChecks">bool to indicate if checks should be bypassed</param>
    protected void TriggerValueChanged(T previousValue, Bindable<T> source, bool propagateToBindings = true, bool bypassChecks = false)
    {
        var beforePropagation = _value;

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
    /// Binds an <paramref name="other" /> <see cref="Bindable{T}" /> to this
    /// </summary>
    /// <param name="other">other <see cref="Bindable{T}" /> to bind to</param>
    /// <exception cref="InvalidOperationException">throws if the <paramref name="other" /> is already bound to this</exception>
    public virtual void BindTo(Bindable<T> other)
    {
        if (Bindings?.Contains(other) == true)
            throw new InvalidOperationException($"Current bindable is already bound to {other}");

        Value = other.Value;

        AddReference(other.WeakReference);
        other.AddReference(WeakReference);
    }

    /// <summary>
    /// Adds a reference to <see cref="Bindings" />
    /// </summary>
    /// <param name="reference">the <see cref="Bindable{T}" /> to add a reference to</param>
    private void AddReference(WeakReference<Bindable<T>> reference)
    {
        Bindings ??= new LockedWeakList<Bindable<T>>();
        Bindings.Add(reference);
    }

    /// <summary>
    /// Removes a reference from <see cref="Bindings" />
    /// </summary>
    /// <param name="reference">the <see cref="Bindable{T}" /> to remove a reference from</param>
    private void RemoveReference(WeakReference<Bindable<T>> reference)
    {
        Bindings?.Remove(reference);
    }

    /// <summary>
    /// Calls all unbinds
    /// </summary>
    /// <remarks>for usage in <see cref="UnbindAll" /></remarks>
    internal virtual void UnbindAllInternal()
    {
        UnbindEvents();
        UnbindBindings();
    }

    /// <inheritdoc cref="IBindable.CreateInstance" />
    protected virtual Bindable<T> CreateInstance()
    {
        return new();
    }

    /// <inheritdoc cref="IBindable{T}.GetBoundCopy" />
    public Bindable<T> GetBoundCopy()
    {
        return IBindable.GetBoundCopyImplementation(this);
    }
}