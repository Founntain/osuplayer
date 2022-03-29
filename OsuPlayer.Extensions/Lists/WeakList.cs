using System.Collections;

namespace OsuPlayer.Extensions.Lists;

public class WeakList<T> : IWeakList<T>, IEnumerable<T> where T : class
{
    private const int OpportunisticTrimThreshold = 100;

    private readonly List<InvalidatableWeakReference> _list = new();
    private int _listStart; // The inclusive starting index in the list.
    private int _listEnd; // The exclusive ending index in the list.

    private int _countChangesSinceTrim;

    public void Add(T item) => AddInternal(new InvalidatableWeakReference(item));

    public void Add(WeakReference<T> weakReference) => AddInternal(new InvalidatableWeakReference(weakReference));

    private void AddInternal(in InvalidatableWeakReference item)
    {
        if (_countChangesSinceTrim > OpportunisticTrimThreshold)
            Trim();

        if (_listEnd < _list.Count)
        {
            _list[_listEnd] = item;
            _countChangesSinceTrim--;
        }
        else
        {
            _list.Add(item);
            _countChangesSinceTrim++;
        }

        _listEnd++;
    }

    public bool Remove(T item)
    {
        var hashCode = EqualityComparer<T>.Default.GetHashCode(item);

        for (int i = _listStart; i < _listEnd; i++)
        {
            var reference = _list[i].Reference;

            if (reference == null)
                continue;

            if (_list[i].ObjectHashCode != hashCode)
                continue;
            
            if (!reference.TryGetTarget(out var target) || target != item)
                continue;
            
            RemoveAt(i - _listStart);
            return true;
        }

        return false;
    }

    public bool Remove(WeakReference<T> weakReference)
    {
        for (int i = _listStart; i < _listEnd; i++)
        {
            if (_list[i].Reference != weakReference)
                continue;
            
            RemoveAt(i - _listStart);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        index += _listStart;

        if (index < _listStart || index >= _listEnd)
            throw new ArgumentOutOfRangeException(nameof(index));

        _list[index] = default;

        if (index == _listStart)
            _listStart++;
        else if (index == _listEnd - 1)
            _listEnd--;

        _countChangesSinceTrim++;
    }

    public bool Contains(T item)
    {
        var hashCode = EqualityComparer<T>.Default.GetHashCode(item);

        for (int i = _listStart; i < _listEnd; i++)
        {
            var reference = _list[i].Reference;
            
            if (reference == null)
                continue;
            
            if (_list[i].ObjectHashCode != hashCode)
                continue;
            
            if (!reference.TryGetTarget(out var target) || target != item)
                continue;

            return true;
        }

        return false;
    }

    public bool Contains(WeakReference<T> weakReference)
    {
        for (int i = _listStart; i < _listEnd; i++)
        {
            // Check if the object is valid.
            if (_list[i].Reference == weakReference)
                return true;
        }

        return false;
    }

    public void Clear()
    {
        _listStart = _listEnd = 0;
        _countChangesSinceTrim = _list.Count;
    }

    public ValidItemsEnumerator GetEnumerator()
    {
        Trim();
        return new ValidItemsEnumerator(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void Trim()
    {
        _list.RemoveRange(_listEnd, _list.Count - _listEnd);
        _list.RemoveRange(0, _listStart);

        _list.RemoveAll(item => item.Reference == null || !item.Reference.TryGetTarget(out _));

        _listStart = 0;
        _listEnd = _list.Count;
        _countChangesSinceTrim = 0;
    }
    
    public struct ValidItemsEnumerator : IEnumerator<T>
    {
        private readonly WeakList<T> weakList;
        private int currentItemIndex;

        /// <summary>
        /// Creates a new <see cref="ValidItemsEnumerator"/>.
        /// </summary>
        /// <param name="weakList">The <see cref="WeakList{T}"/> to enumerate over.</param>
        internal ValidItemsEnumerator(WeakList<T> weakList)
        {
            this.weakList = weakList;

            currentItemIndex = weakList._listStart - 1; // The first MoveNext() should bring the iterator to the start
            Current = default!;
        }

        public bool MoveNext()
        {
            while (true)
            {
                ++currentItemIndex;

                // Check whether we're still within the valid range of the list.
                if (currentItemIndex >= weakList._listEnd)
                    return false;

                var weakReference = weakList._list[currentItemIndex].Reference;

                // Check whether the reference exists.
                if (weakReference == null || !weakReference.TryGetTarget(out var obj))
                {
                    // If the reference doesn't exist, it must have previously been removed and can be skipped.
                    continue;
                }

                Current = obj;
                return true;
            }
        }

        public void Reset()
        {
            currentItemIndex = weakList._listStart - 1;
            Current = default!;
        }

        public T Current { get; private set; }

        readonly object IEnumerator.Current => Current;

        public void Dispose()
        {
            Current = default!;
        }
    }
    
    private readonly struct InvalidatableWeakReference
    {
        public readonly WeakReference<T>? Reference;

        /// <summary>
        /// Hash code of the target of <see cref="Reference"/>.
        /// </summary>
        public readonly int ObjectHashCode;

        public InvalidatableWeakReference(T reference)
        {
            Reference = new WeakReference<T>(reference);
            ObjectHashCode = EqualityComparer<T>.Default.GetHashCode(reference);
        }

        public InvalidatableWeakReference(WeakReference<T> weakReference)
        {
            Reference = weakReference;
            ObjectHashCode = !weakReference.TryGetTarget(out var target) ? 0 : EqualityComparer<T>.Default.GetHashCode(target);
        }
    }
}