namespace OsuPlayer.Extensions.Bindables;

public interface IBindable : IUnbindable
{
    /// <summary>
    /// Binds ourselves to another bindable such that we receive any value limitations of the bindable we bind width.
    /// </summary>
    /// <param name="other">The foreign bindable. This should always be the most permanent end of the bind (ie. a ConfigManager)</param>
    void BindTo(IBindable other);
}

public interface IBindable<T> : IUnbindable
{
    public T Value { get; set; }
    event Action<ValueChangedEvent<T>> ValueChanged;

    public void BindTo(IBindable<T> other);
}