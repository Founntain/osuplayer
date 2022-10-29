using System.Collections.Specialized;
using OsuPlayer.Extensions.Lists;

namespace OsuPlayer.Extensions.Bindables;

public class BindableArray<T> : IBindableArray<T>, IBindable
{
    private readonly int _precision;
    private LockedWeakList<BindableArray<T>>? _bindings;

    private WeakReference<BindableArray<T>> WeakReference => new(this);

    public T this[int index]
    {
        get => Array[index];
        set => SetValue(index, value, this);
    }

    public int Length => Array.Length;

    public T[] Array { get; }

    public BindableArray(int size = 0, int precision = 2)
    {
        Array = new T[size];
        _precision = precision;
    }

    public void BindTo(IBindable other)
    {
        if (!(other is BindableArray<T> otherB))
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

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public void UnbindEvents()
    {
        CollectionChanged = null!;
    }

    public void UnbindBindings()
    {
        if (_bindings == default)
            return;

        foreach (var binding in _bindings)
            UnbindFrom(binding);
    }

    public void UnbindAll()
    {
        UnbindEvents();
        UnbindBindings();
    }

    public void UnbindFrom(IUnbindable other)
    {
        if (!(other is BindableArray<T> otherB))
            throw new InvalidCastException($"Can't unbind a bindable of type {other.GetType()} from a bindable of type {GetType()}.");

        RemoveWeakReference(otherB.WeakReference);
        otherB.RemoveWeakReference(WeakReference);
    }

    void IBindableArray<T>.BindTo(IBindableArray<T> other)
    {
        if (!(other is BindableArray<T> otherB))
            throw new InvalidCastException($"Can't bind to a bindable of type {other.GetType()} from a bindable of type {GetType()}.");

        BindTo(otherB);
    }

    public void BindCollectionChanged(NotifyCollectionChangedEventHandler onChange, bool runOnceImmediately = false)
    {
        CollectionChanged += onChange;
        if (runOnceImmediately)
            onChange(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Array));
    }

    public void Set(T[]? val, BindableArray<T>? source = null)
    {
        if (val == default || val.Length != Array.Length) return;

        source ??= this;

        for (var i = 0; i < val.Length; i++)
        {
            var newVal = Round(val[i]);
            Array[i] = newVal;
        }

        if (_bindings != default)
            foreach (var binding in _bindings)
                if (binding != source)
                    binding.Set(val, this);

        if (source != this)
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    private T Round(T value)
    {
        if (value is double dVal)
        {
            var round = Math.Round(dVal, _precision);
            if (round is T val)
                return val;
        }

        if (value is decimal decVal)
        {
            var round = Math.Round(decVal, _precision);
            if (round is T val)
                return val;
        }

        return value;
    }

    private void SetValue(int index, T value, BindableArray<T> source, bool dontTrigger = false)
    {
        var rValue = Round(value);

        var last = Array[index];

        Array[index] = rValue;

        if (_bindings != default)
            foreach (var binding in _bindings)
                if (binding != source)
                    binding.SetValue(index, value, this, dontTrigger);

        if (!dontTrigger)
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, rValue, last, index));
    }

    private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    private void AddWeakReference(WeakReference<BindableArray<T>> weakReference)
    {
        _bindings ??= new LockedWeakList<BindableArray<T>>();
        _bindings.Add(weakReference);
    }

    private void RemoveWeakReference(WeakReference<BindableArray<T>> weakReference)
    {
        _bindings?.Remove(weakReference);
    }

    public void BindTo(BindableArray<T> other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        if (_bindings?.Contains(WeakReference) == true)
            throw new ArgumentException("An already bound collection can not be bound again.");
        if (other == this)
            throw new ArgumentException("A collection can not be bound to itself");

        AddWeakReference(other.WeakReference);
        other.AddWeakReference(WeakReference);
    }

    /// <inheritdoc cref="IBindable.CreateInstance" />
    protected virtual IBindable CreateInstance()
    {
        return new BindableArray<T>();
    }

    /// <inheritdoc cref="IBindable{T}.GetBoundCopy" />
    public BindableArray<T> GetBoundCopy()
    {
        return IBindable.GetBoundCopyImplementation(this);
    }
}