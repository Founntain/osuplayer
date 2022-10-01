using System.Collections;
using System.Collections.Specialized;
using OsuPlayer.Extensions.Lists;

namespace OsuPlayer.Extensions.Bindables;


    public class BindableList<T> : IBindableList<T>, IBindable, IParseable, IList<T>, IList
    {
        /// <summary>
        /// An event which is raised when this <see cref="BindableList{T}"/> changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// An event which is raised when <see cref="Disabled"/>'s state has changed (or manually via <see cref="TriggerDisabledChange"/>).
        /// </summary>
        public event Action<bool> DisabledChanged;

        private readonly List<T> _collection = new();

        private readonly Cached<WeakReference<BindableList<T>>> _weakReferenceCache = new();

        private WeakReference<BindableList<T>> WeakReference => _weakReferenceCache.IsValid ? _weakReferenceCache.Value : _weakReferenceCache.Value = new WeakReference<BindableList<T>>(this);

        private LockedWeakList<BindableList<T>>? _bindings;

        /// <summary>
        /// Creates a new <see cref="BindableList{T}"/>, optionally adding the items of the given collection.
        /// </summary>
        /// <param name="items">The items that are going to be contained in the newly created <see cref="BindableList{T}"/>.</param>
        public BindableList(IEnumerable<T> items = null)
        {
            if (items != null)
                _collection.AddRange(items);
        }

        #region IList<T>

        /// <summary>
        /// Gets or sets the item at an index in this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <exception cref="InvalidOperationException">Thrown when setting a value while this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public T this[int index]
        {
            get => _collection[index];
            set => SetIndex(index, value, null);
        }

        private void SetIndex(int index, T item, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            T lastItem = _collection[index];

            _collection[index] = item;

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.SetIndex(index, item, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, lastItem, index));
        }

        /// <summary>
        /// Adds a single item to this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <exception cref="InvalidOperationException">Thrown when this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public void Add(T item)
            => Add(item, null);

        private void Add(T item, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            _collection.Add(item);

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.Add(item, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, _collection.Count - 1));
        }

        /// <summary>
        /// Retrieves the index of an item in this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="item">The item to retrieve the index of.</param>
        /// <returns>The index of the item, or -1 if the item isn't in this <see cref="BindableList{T}"/>.</returns>
        public int IndexOf(T item) => _collection.IndexOf(item);

        /// <summary>
        /// Inserts an item at the specified index in this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="index">The index to insert at.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="InvalidOperationException">Thrown when this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public void Insert(int index, T item)
            => Insert(index, item, null);

        private void Insert(int index, T item, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            _collection.Insert(index, item);

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.Insert(index, item, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        /// <summary>
        /// Clears the contents of this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public void Clear()
            => Clear(null);

        private void Clear(BindableList<T> caller)
        {
            EnsureMutationAllowed();

            if (_collection.Count <= 0)
                return;

            // Preserve items for subscribers
            var clearedItems = _collection.ToList();

            _collection.Clear();

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.Clear(this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, clearedItems, 0));
        }

        /// <summary>
        /// Determines if an item is in this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="item">The item to locate in this <see cref="BindableList{T}"/>.</param>
        /// <returns><code>true</code> if this <see cref="BindableList{T}"/> contains the given item.</returns>
        public bool Contains(T item)
            => _collection.Contains(item);

        /// <summary>
        /// Removes an item from this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="item">The item to remove from this <see cref="BindableList{T}"/>.</param>
        /// <returns><code>true</code> if the removal was successful.</returns>
        /// <exception cref="InvalidOperationException">Thrown if this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public bool Remove(T item)
            => Remove(item, null);

        private bool Remove(T item, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            int index = _collection.IndexOf(item);

            if (index < 0)
                return false;

            // Removal may have come from an equality comparison.
            // Always return the original reference from the list to other bindings and events.
            var listItem = _collection[index];

            _collection.RemoveAt(index);

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-removing from the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.Remove(listItem, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, listItem, index));

            return true;
        }

        /// <summary>
        /// Removes <paramref name="count"/> items starting from <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index to start removing from.</param>
        /// <param name="count">The count of items to be removed.</param>
        public void RemoveRange(int index, int count)
        {
            RemoveRange(index, count, null);
        }

        private void RemoveRange(int index, int count, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            var removedItems = _collection.GetRange(index, count);

            _collection.RemoveRange(index, count);

            if (removedItems.Count == 0)
                return;

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // Prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.RemoveRange(index, count, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItems, index));
        }

        /// <summary>
        /// Removes an item at the specified index from this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        /// <exception cref="InvalidOperationException">Thrown if this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public void RemoveAt(int index)
            => RemoveAt(index, null);

        private void RemoveAt(int index, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            T item = _collection[index];

            _collection.RemoveAt(index);

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.RemoveAt(index, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        /// <summary>
        /// Removes all items from this <see cref="BindableList{T}"/> that match a predicate.
        /// </summary>
        /// <param name="match">The predicate.</param>
        public int RemoveAll(Predicate<T> match)
            => RemoveAll(match, null);

        private int RemoveAll(Predicate<T> match, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            var removed = _collection.FindAll(match);

            if (removed.Count == 0) return removed.Count;

            // RemoveAll is internally optimised
            _collection.RemoveAll(match);

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.RemoveAll(match, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));

            return removed.Count;
        }

        /// <summary>
        /// Copies the contents of this <see cref="BindableList{T}"/> to the given array, starting at the given index.
        /// </summary>
        /// <param name="array">The array that is the destination of the items copied from this <see cref="BindableList{T}"/>.</param>
        /// <param name="arrayIndex">The index at which the copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
            => _collection.CopyTo(array, arrayIndex);

        /// <summary>
        /// Copies the contents of this <see cref="BindableList{T}"/> to the given array, starting at the given index.
        /// </summary>
        /// <param name="array">The array that is the destination of the items copied from this <see cref="BindableList{T}"/>.</param>
        /// <param name="index">The index at which the copying begins.</param>
        public void CopyTo(Array array, int index)
            => ((ICollection)_collection).CopyTo(array, index);

        public int BinarySearch(T item) => _collection.BinarySearch(item);

        public int Count => _collection.Count;
        public bool IsSynchronized => ((ICollection)_collection).IsSynchronized;
        public object SyncRoot => ((ICollection)_collection).SyncRoot;
        public bool IsReadOnly => Disabled;

        #endregion

        #region IList

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return Count - 1;
        }

        bool IList.Contains(object value) => Contains((T)value);

        int IList.IndexOf(object value) => IndexOf((T)value);

        void IList.Insert(int index, object value) => Insert(index, (T)value);

        void IList.Remove(object value) => Remove((T)value);

        bool IList.IsFixedSize => false;

        #endregion

        #region IParseable

        /// <summary>
        /// Parse an object into this instance.
        /// A collection holding items of type <typeparamref name="T"/> can be parsed. Null results in an empty <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="input">The input which is to be parsed.</param>
        /// <exception cref="InvalidOperationException">Thrown if this <see cref="BindableList{T}"/> is <see cref="Disabled"/>.</exception>
        public void Parse(object input)
        {
            EnsureMutationAllowed();

            switch (input)
            {
                case null:
                    Clear();
                    break;

                case IEnumerable<T> enumerable:
                    // enumerate once locally before proceeding.
                    var newItems = enumerable.ToList();

                    if (this.SequenceEqual(newItems))
                        return;

                    Clear();
                    AddRange(newItems);
                    break;

                default:
                    throw new ArgumentException($@"Could not parse provided {input.GetType()} ({input}) to {typeof(T)}.");
            }
        }

        #endregion

        #region ICanBeDisabled

        private bool _disabled;

        /// <summary>
        /// Whether this <see cref="BindableList{T}"/> has been disabled. When disabled, attempting to change the contents of this <see cref="BindableList{T}"/> will result in an <see cref="InvalidOperationException"/>.
        /// </summary>
        public bool Disabled
        {
            get => _disabled;
            set
            {
                if (value == _disabled)
                    return;

                _disabled = value;

                TriggerDisabledChange();
            }
        }

        public void BindDisabledChanged(Action<bool> onChange, bool runOnceImmediately = false)
        {
            DisabledChanged += onChange;
            if (runOnceImmediately)
                onChange(Disabled);
        }

        private void TriggerDisabledChange(bool propagateToBindings = true)
        {
            // check a bound bindable hasn't changed the value again (it will fire its own event)
            bool beforePropagation = _disabled;

            if (propagateToBindings && _bindings != null)
            {
                foreach (var b in _bindings)
                    b.Disabled = _disabled;
            }

            if (beforePropagation == _disabled)
                DisabledChanged?.Invoke(_disabled);
        }

        #endregion ICanBeDisabled

        #region IUnbindable

        public virtual void UnbindEvents()
        {
            CollectionChanged = null;
            DisabledChanged = null;
        }

        public void UnbindBindings()
        {
            if (_bindings == null)
                return;

            foreach (var b in _bindings)
                UnbindFrom(b);
        }

        public void UnbindAll()
        {
            UnbindEvents();
            UnbindBindings();
        }

        public virtual void UnbindFrom(IUnbindable them)
        {
            if (!(them is BindableList<T> tThem))
                throw new InvalidCastException($"Can't unbind a bindable of type {them.GetType()} from a bindable of type {GetType()}.");

            RemoveWeakReference(tThem.WeakReference);
            tThem.RemoveWeakReference(WeakReference);
        }

        #endregion IUnbindable

        #region IHasDescription

        public string Description { get; set; }

        #endregion IHasDescription

        #region IBindableCollection

        /// <summary>
        /// Adds a collection of items to this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="items">The collection whose items should be added to this collection.</param>
        /// <exception cref="InvalidOperationException">Thrown if this collection is <see cref="Disabled"/></exception>
        public void AddRange(IEnumerable<T> items)
            => AddRange(items as IList ?? items.ToArray(), null);

        private void AddRange(IList items, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            _collection.AddRange(items.Cast<T>());

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.AddRange(items, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, _collection.Count - items.Count));
        }

        /// <summary>
        /// Moves an item in this collection.
        /// </summary>
        /// <param name="oldIndex">The index of the item to move.</param>
        /// <param name="newIndex">The index specifying the new location of the item.</param>
        public void Move(int oldIndex, int newIndex)
            => Move(oldIndex, newIndex, null);

        private void Move(int oldIndex, int newIndex, BindableList<T> caller)
        {
            EnsureMutationAllowed();

            T item = _collection[oldIndex];

            _collection.RemoveAt(oldIndex);
            _collection.Insert(newIndex, item);

            if (_bindings != null)
            {
                foreach (var b in _bindings)
                {
                    // prevent re-adding the item back to the callee.
                    // That would result in a <see cref="StackOverflowException"/>.
                    if (b != caller)
                        b.Move(oldIndex, newIndex, this);
                }
            }

            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
        }

        void IBindable.BindTo(IBindable them)
        {
            if (!(them is BindableList<T> tThem))
                throw new InvalidCastException($"Can't bind to a bindable of type {them.GetType()} from a bindable of type {GetType()}.");

            BindTo(tThem);
        }

        void IBindableList<T>.BindTo(IBindableList<T> them)
        {
            if (!(them is BindableList<T> tThem))
                throw new InvalidCastException($"Can't bind to a bindable of type {them.GetType()} from a bindable of type {GetType()}.");

            BindTo(tThem);
        }

        /// <summary>
        /// An alias of <see cref="BindTo"/> provided for use in object initializer scenarios.
        /// Passes the provided value as the foreign (more permanent) bindable.
        /// </summary>
        public IBindableList<T> BindTarget
        {
            set => ((IBindableList<T>)this).BindTo(value);
        }

        /// <summary>
        /// Binds this <see cref="BindableList{T}"/> to another.
        /// </summary>
        /// <param name="them">The <see cref="BindableList{T}"/> to be bound to.</param>
        public void BindTo(BindableList<T> them)
        {
            if (them == null)
                throw new ArgumentNullException(nameof(them));
            if (_bindings?.Contains(WeakReference) == true)
                throw new ArgumentException("An already bound collection can not be bound again.");
            if (them == this)
                throw new ArgumentException("A collection can not be bound to itself");

            // copy state and content over
            Parse(them);
            Disabled = them.Disabled;

            AddWeakReference(them.WeakReference);
            them.AddWeakReference(WeakReference);
        }

        /// <summary>
        /// Bind an action to <see cref="CollectionChanged"/> with the option of running the bound action once immediately
        /// with an <see cref="NotifyCollectionChangedAction.Add"/> event for the entire contents of this <see cref="BindableList{T}"/>.
        /// </summary>
        /// <param name="onChange">The action to perform when this <see cref="BindableList{T}"/> changes.</param>
        /// <param name="runOnceImmediately">Whether the action provided in <paramref name="onChange"/> should be run once immediately.</param>
        public void BindCollectionChanged(NotifyCollectionChangedEventHandler onChange, bool runOnceImmediately = false)
        {
            CollectionChanged += onChange;
            if (runOnceImmediately)
                onChange(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _collection));
        }

        private void AddWeakReference(WeakReference<BindableList<T>> weakReference)
        {
            _bindings ??= new LockedWeakList<BindableList<T>>();
            _bindings.Add(weakReference);
        }

        private void RemoveWeakReference(WeakReference<BindableList<T>> weakReference) => _bindings?.Remove(weakReference);

        IBindable IBindable.CreateInstance() => CreateInstance();

        /// <inheritdoc cref="IBindable.CreateInstance"/>
        protected virtual BindableList<T> CreateInstance() => new BindableList<T>();

        IBindable IBindable.GetBoundCopy() => GetBoundCopy();

        IBindableList<T> IBindableList<T>.GetBoundCopy() => GetBoundCopy();

        /// <inheritdoc cref="IBindableList{T}.GetBoundCopy"/>
        public BindableList<T> GetBoundCopy() => IBindable.GetBoundCopyImplementation(this);

        #endregion IBindableCollection

        #region IEnumerable

        public List<T>.Enumerator GetEnumerator() => _collection.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion IEnumerable

        private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);

        private void EnsureMutationAllowed()
        {
            if (Disabled)
                throw new InvalidOperationException($"Cannot mutate the {nameof(BindableList<T>)} while it is disabled.");
        }

        public bool IsDefault => Count == 0;
    }