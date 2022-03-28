using System.ComponentModel;

namespace OsuPlayer.Extensions.Bindings;

public interface IBindable<T> : IUnbindable
{
    event Action<ValueChangedEvent<T>> ValueChanged;

    public T Value { get; set; }

    public void BindTo(IBindable<T> other);
}