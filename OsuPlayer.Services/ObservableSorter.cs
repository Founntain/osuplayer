using OsuPlayer.IO.DbReader.Interfaces;

namespace OsuPlayer.Services;

public class ObservableSorter : IObservable<IComparer<IMapEntryBase>>
{
    private readonly List<IObserver<IComparer<IMapEntryBase>>> _observers = new();
    private IComparer<IMapEntryBase>? _lastComparer;

    public IDisposable Subscribe(IObserver<IComparer<IMapEntryBase>> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        if (_lastComparer != null)
            observer.OnNext(_lastComparer);

        return new Unsubscribe(_observers, observer);
    }

    public void UpdateComparer(IComparer<IMapEntryBase>? comparer)
    {
        foreach (var observer in _observers)
            if (comparer == null)
                observer.OnError(new NullReferenceException());
            else
                observer.OnNext(comparer);

        _lastComparer = comparer;
    }

    private class Unsubscribe : IDisposable
    {
        private readonly IObserver<IComparer<IMapEntryBase>>? _observer;
        private readonly List<IObserver<IComparer<IMapEntryBase>>> _observers;

        public Unsubscribe(List<IObserver<IComparer<IMapEntryBase>>> observers, IObserver<IComparer<IMapEntryBase>> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}