// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace OsuPlayer.Extensions.Bindables;

public interface IBindable : IUnbindable
{
    /// <summary>
    /// Binds ourselves to another bindable such that we receive any value limitations of the bindable we bind width.
    /// </summary>
    /// <param name="other">
    /// The foreign bindable. This should always be the most permanent end of the bind (ie. a
    /// ConfigManager)
    /// </param>
    void BindTo(IBindable other);

    /// <summary>
    /// Retrieve a new bindable instance weakly bound to the configuration backing.
    /// If you are further binding to events of a bindable retrieved using this method, ensure to hold
    /// a local reference.
    /// </summary>
    /// <returns>A weakly bound copy of the specified bindable.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when attempting to instantiate a copy bindable that's not matching
    /// the original's type.
    /// </exception>
    IBindable GetBoundCopy();

    /// <summary>
    /// Creates a new instance of this <see cref="IBindable" /> for use in <see cref="GetBoundCopy" />.
    /// The returned instance must have match the most derived type of the bindable class this method is implemented on.
    /// </summary>
    protected IBindable CreateInstance();

    /// <summary>
    /// Helper method which implements <see cref="GetBoundCopy" /> for use in final classes.
    /// </summary>
    /// <param name="source">The source <see cref="IBindable" />.</param>
    /// <typeparam name="T">The bindable type.</typeparam>
    /// <returns>The bound copy.</returns>
    protected static T GetBoundCopyImplementation<T>(T source)
        where T : IBindable
    {
        var copy = source.CreateInstance();

        if (copy.GetType() != source.GetType())
            throw new InvalidOperationException($"Attempted to create a copy of {source.GetType()}, but the returned instance type was {copy.GetType()}. "
                                                + $"Override {source.GetType()}.{nameof(CreateInstance)}() for {nameof(GetBoundCopy)}() to function properly.");

        copy.BindTo(source);
        return (T) copy;
    }
}

public interface IBindable<T> : IUnbindable
{
    public T Value { get; set; }
    event Action<ValueChangedEvent<T>> ValueChanged;

    public void BindTo(IBindable<T> other);

    /// <summary>
    /// Bind an action to <see cref="ValueChanged" /> with the option of running the bound action once immediately.
    /// </summary>
    /// <param name="onChange">The action to perform when <see cref="Value" /> changes.</param>
    /// <param name="ignoreSource">whether to ignore the trigger source</param>
    /// <param name="runOnceImmediately">
    /// Whether the action provided in <paramref name="onChange" /> should be run once
    /// immediately.
    /// </param>
    void BindValueChanged(Action<ValueChangedEvent<T>> onChange, bool ignoreSource = false, bool runOnceImmediately = false);

    /// <inheritdoc cref="IBindable.GetBoundCopy" />
    IBindable<T> GetBoundCopy();
}