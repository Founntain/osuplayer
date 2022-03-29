namespace OsuPlayer.Extensions.Bindables;

public interface IBindable<T> : IUnbindable
{
    public T Value { get; set; }
    event Action<ValueChangedEvent<T>> ValueChanged;

    public void BindTo(IBindable<T> other);
}