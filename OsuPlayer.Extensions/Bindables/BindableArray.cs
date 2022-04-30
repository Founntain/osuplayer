using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Styling;
using OsuPlayer.Extensions.Lists;

namespace OsuPlayer.Extensions.Bindables;

public class BindableArray<T> : IBindableArray<T>, IBindable
{
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    private readonly T[] _array;
    
    private bool _ignoreSource;

    private WeakReference<BindableArray<T>> WeakReference => new(this);

    private LockedWeakList<BindableArray<T>>? _bindings;

    public BindableArray(int size = 0)
    {
        _array = new T[size];
    }

    public T this[int index]
    {
        get => _array[index];
        set => SetValue(index, value, this);
    }

    public int Length => _array.Length;

    private void SetValue(int index, T value, BindableArray<T> source)
    {
        T last = _array[index];

        _array[index] = value;

        if (_bindings != default)
        {
            foreach (var binding in _bindings)
            {
                if (binding != source)
                    binding.SetValue(index, value, this);
            }
        }
        
        NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, last, index));
    }
    
    private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);
    
    private void AddWeakReference(WeakReference<BindableArray<T>> weakReference)
    {
        _bindings ??= new LockedWeakList<BindableArray<T>>();
        _bindings.Add(weakReference);
    }
    
    private void RemoveWeakReference(WeakReference<BindableArray<T>> weakReference) => _bindings?.Remove(weakReference);
    
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

    public void BindTo(IBindable other)
    {
        if (!(other is BindableArray<T> otherB))
            throw new InvalidCastException($"Can't bind to a bindable of type {other.GetType()} from a bindable of type {GetType()}.");

        BindTo(otherB);
    }

    void IBindableArray<T>.BindTo(IBindableArray<T> other)
    {
        if (!(other is BindableArray<T> otherB))
            throw new InvalidCastException($"Can't bind to a bindable of type {other.GetType()} from a bindable of type {GetType()}.");

        BindTo(otherB);
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

    public void BindCollectionChanged(NotifyCollectionChangedEventHandler onChange, bool ignoreSource = false, bool runOnceImmediately = false)
    {
        CollectionChanged += onChange;
        _ignoreSource = ignoreSource;
        if (runOnceImmediately)
            onChange(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _array));
    }
}