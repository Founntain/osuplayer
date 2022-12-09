// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Specialized;

namespace OsuPlayer.Extensions.Bindables;

public interface IBindableArray<T> : IUnbindable, INotifyCollectionChanged
{
    /// <summary>
    /// Binds self to another bindable such that we receive any values and value limitations of the bindable we bind width.
    /// </summary>
    /// <param name="other">
    /// The foreign bindable. This should always be the most permanent end of the bind (ie. a
    /// ConfigManager)
    /// </param>
    void BindTo(IBindableArray<T> other);

    /// <summary>
    /// Bind an action to <see cref="INotifyCollectionChanged.CollectionChanged" /> with the option of running the bound action
    /// once immediately
    /// with an <see cref="NotifyCollectionChangedAction.Add" /> event for the entire contents of this
    /// <see cref="BindableArray{T}" />.
    /// </summary>
    /// <param name="onChange">The action to perform when this <see cref="BindableArray{T}" /> changes.</param>
    /// <param name="ignoreSource">Whether to ignore the source of the event trigger</param>
    /// <param name="runOnceImmediately">
    /// Whether the action provided in <paramref name="onChange" /> should be run once
    /// immediately.
    /// </param>
    void BindCollectionChanged(NotifyCollectionChangedEventHandler onChange, bool runOnceImmediately = false);
}