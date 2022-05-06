namespace OsuPlayer.Extensions.Lists;

/// <summary>
/// Interface for a list which stores weak references of objects
/// </summary>
/// <typeparam name="T">type of the containing items</typeparam>
public interface IWeakList<T> where T : class
{
    /// <summary>
    /// Adds an <paramref name="item" /> to the list. Item is added as a <see cref="WeakReference{T}" />
    /// </summary>
    /// <param name="item">item to be added</param>
    void Add(T item);

    /// <summary>
    /// Adds an <paramref name="weakReference" /> to the list
    /// </summary>
    /// <param name="weakReference">weak reference to be added</param>
    void Add(WeakReference<T> weakReference);

    /// <summary>
    /// Removes an <paramref name="item" /> from the list
    /// </summary>
    /// <param name="item">item to be removed</param>
    /// <returns>whether the item was removed</returns>
    bool Remove(T item);

    /// <summary>
    /// Removes an <paramref name="weakReference" /> from the list
    /// </summary>
    /// <param name="weakReference">weak reference to be removed</param>
    /// <returns>whether the weak reference was removed</returns>
    bool Remove(WeakReference<T> weakReference);

    /// <summary>
    /// Removes a weak reference at an <paramref name="index" /> from the list
    /// </summary>
    /// <param name="index">index of the item to remove</param>
    void RemoveAt(int index);

    /// <summary>
    /// Searches for a reference to an <paramref name="item" /> in the list
    /// </summary>
    /// <param name="item">item to search for</param>
    /// <returns>whether the list contains a reference</returns>
    bool Contains(T item);

    /// <summary>
    /// Searches for a <paramref name="weakReference" /> in the list
    /// </summary>
    /// <param name="weakReference">weak reference to search for</param>
    /// <returns>whether the list contains a reference</returns>
    bool Contains(WeakReference<T> weakReference);

    /// <summary>
    /// Clears all items from the list
    /// </summary>
    void Clear();
}