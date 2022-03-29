using System.Collections;

namespace OsuPlayer.Extensions.Lists;

public class LockedWeakList<T> : IWeakList<T>, IEnumerable<T> where T : class
{
    private readonly WeakList<T> _list = new();

    public void Add(T item)
    {
        lock (_list)
            _list.Add(item);
    }

    public void Add(WeakReference<T> weakReference)
    {
        lock (_list)
            _list.Add(weakReference);
    }

    public bool Remove(T item)
    {
        lock (_list)
            return _list.Remove(item);
    }

    public bool Remove(WeakReference<T> weakReference)
    {
        lock (_list)
            return _list.Remove(weakReference);
    }

    public void RemoveAt(int index)
    {
        lock (_list)
            _list.RemoveAt(index);
    }

    public bool Contains(T item)
    {
        lock (_list)
            return _list.Contains(item);
    }

    public bool Contains(WeakReference<T> weakReference)
    {
        lock (_list)
            return _list.Contains(weakReference);
    }

    public void Clear()
    {
        lock (_list)
            _list.Clear();
    }

    public Enumerator GetEnumerator() => new(_list);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public struct Enumerator : IEnumerator<T>
    {
        private readonly WeakList<T> _weakList;

        private WeakList<T>.ValidItemsEnumerator _listEnumerator;

        private readonly bool _lockTaken;

        internal Enumerator(WeakList<T> weakList)
        {
            this._weakList = weakList;

            _lockTaken = false;
            Monitor.Enter(weakList, ref _lockTaken);

            _listEnumerator = weakList.GetEnumerator();
        }

        public bool MoveNext() => _listEnumerator.MoveNext();

        public void Reset() => _listEnumerator.Reset();

        public readonly T Current => _listEnumerator.Current;

        readonly object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (_lockTaken)
                Monitor.Exit(_weakList);
        }
    }
}