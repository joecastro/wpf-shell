namespace Standard
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    internal interface IMergeable<TKey, TItem> : IEquatable<TItem> where TKey : IEquatable<TKey>
    {
        /// <summary>Immutable foreign key for the object </summary>
        TKey FKID { get; }

        /// <summary>Merge the properties of another object of like type with this one.</summary>
        /// <param name="other">The object to merge</param>
        void Merge(TItem other);
    }

    internal class MergeableCollection<TKey, TItem> : IList<TItem>, INotifyCollectionChanged where TItem : class where TKey : IEquatable<TKey>
    {
        private class _Comparer : IComparer<TItem>
        {
            private bool _dontCompare = false;
            private Comparison<TItem> _defaultComparison;
            private Comparison<TItem> _customComparison;
            private Comparison<TItem> _trueComparison;

            public _Comparer()
            {
                if (Utility.IsInterfaceImplemented(typeof(TItem), typeof(IComparable<TItem>)))
                {
                    _defaultComparison = (left, right) =>
                    {
                        if (object.ReferenceEquals(left, right))
                        {
                            return 0;
                        }

                        var comparableLeft = (IComparable<TItem>)left;
                        if (comparableLeft == null)
                        {
                            return -1;
                        }

                        return comparableLeft.CompareTo(right);
                    };
                }

                _trueComparison = _defaultComparison;
            }

            public Comparison<TItem> Comparison
            {
                get { return _trueComparison; }
                set
                {
                    Assert.IsFalse(_dontCompare);
                    if (!_dontCompare)
                    {
                        _customComparison = value;
                        _trueComparison = _customComparison ?? _defaultComparison;
                    }
                }
            }

            public bool CanCompare
            { 
                get { return _trueComparison != null; }
            }

            public void StopComparisons()
            {
                _trueComparison = null;
                _dontCompare = true;
            }

            public int Compare(TItem x, TItem y)
            {
                Assert.IsTrue(CanCompare);
                return _trueComparison(x, y);
            }

            public IEnumerable<TItem> OrderedList(IEnumerable<TItem> original)
            {
                Assert.IsNotNull(original);

                if (!CanCompare)
                {
                    return original;
                }

                return original.OrderBy(x => x, this);
            }
        }

        private readonly bool _areItemsMergable;
        private readonly bool _areItemsNotifiable;
        private readonly ObservableCollection<TItem> _items;
        private readonly Dictionary<TKey, TItem> _fkidLookup;
        private _Comparer _itemComparer = new _Comparer();

        // Block reentrancy that would cause the collection to be reordered.
        // If while we're doing a merge we get change notifications that the item has changed, ignore it.
        private IMergeable<TKey, TItem> _suspendNotificationsForMergeableObject;

        public object SyncRoot { get; private set; }

        public MergeableCollection()
            : this(null, true)
        {}

        public MergeableCollection(bool sort)
            : this(null, sort)
        {}

        public MergeableCollection(IEnumerable<TItem> dataObjects)
            : this(dataObjects, true)
        {}

        public MergeableCollection(IEnumerable<TItem> dataObjects, bool sort)
        {
            SyncRoot = new object();

            if (!sort)
            {
                _itemComparer.StopComparisons();
            }

            // We don't really want to constrain based on the type being IMergeable or comparable
            // This is a very specific check.  We want to ensure that this type supports IMergeable<T>, not IMergeable<SomethingElse>
            _areItemsMergable = Utility.IsInterfaceImplemented(typeof(TItem), typeof(IMergeable<TKey, TItem>));
            _areItemsNotifiable = Utility.IsInterfaceImplemented(typeof(TItem), typeof(INotifyPropertyChanged));

            if (dataObjects == null)
            {
                _items = new ObservableCollection<TItem>();
            }
            else
            {
                _items = new ObservableCollection<TItem>(_itemComparer.OrderedList(dataObjects));
                if (_areItemsNotifiable)
                {
                    lock (SyncRoot)
                    {
                        foreach (INotifyPropertyChanged item in _items)
                        {
                            item.PropertyChanged += _OnItemChanged;
                        }
                    }
                }
            }

            if (_areItemsMergable)
            {
                _fkidLookup = new Dictionary<TKey, TItem>();
                foreach (IMergeable<TKey, TItem> item in _items)
                {
                    //Assert.IsNotNull(item.FKID);
                    _fkidLookup.Add(item.FKID, (TItem)item);
                }
            }
        }

        public Comparison<TItem> CustomComparison
        {
            get { return _itemComparer.Comparison; }
            set
            {
                if (_itemComparer.Comparison != value)
                {
                    _itemComparer.Comparison = value;
                    RefreshSort();
                }
            }
        }

        public void RefreshSort()
        {
            if (_itemComparer.CanCompare)
            {
                lock (SyncRoot)
                {
                    if (!_AreItemsSortedUpToIndex(_items.Count))
                    {
                        var copyList = new List<TItem>(_items);
                        // Clear the list first so we don't bubble-sort just to reorder.
                        _Merge(null, false, null);
                        _Merge(copyList, false, null);
                    }
                }
            }
        }

        /// <summary>
        /// Merges data from another collection.
        /// </summary>
        /// <param name="newCollection">The data object collection that contains new data.</param>
        /// <param name="add">
        /// If true, combine the new collection with existing content, otherwise replace the list.
        /// </param>
        public void Merge(IEnumerable<TItem> newCollection, bool add)
        {
            _Merge(newCollection, add, null);
        }

        public void Merge(IEnumerable<TItem> newCollection, bool add, int? maxCount)
        {
            _Merge(newCollection, add, maxCount);
        }

        private void _Merge(IEnumerable<TItem> newCollection, bool add, int? maxCount)
        {
            // These should never get out of sync.
            Assert.Implies(_areItemsMergable, () => _items.Count == _fkidLookup.Count);

            lock (SyncRoot)
            {
                // Go-go partial template specialization!
                if (_areItemsMergable)
                {
                    _RichMerge(newCollection, add, maxCount);
                }
                else
                {
                    _SimpleMerge(newCollection, add, maxCount);
                }
            }
        }

        public TItem FindFKID(TKey id)
        {
            lock (SyncRoot)
            {
                if (!_areItemsMergable)
                {
                    throw new InvalidOperationException("This can only be used on collections with Mergeable items.");
                }

                TItem ret;
                if (_fkidLookup.TryGetValue(id, out ret))
                {
                    return ret;
                }
                return null;
            }
        }

        private int _FindIndex(int startIndex, Predicate<IMergeable<TKey, TItem>> match)
        {
            int count = Count - startIndex;
            for (int i = startIndex; i < startIndex + count; ++i)
            {
                if (match((IMergeable<TKey, TItem>)this[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private bool _VerifyInsertionPoint(int index)
        {
            lock (SyncRoot)
            {
                if (_itemComparer.Comparison == null)
                {
                    // We don't have any way of determining a correct order.  Whatever we have is fine.
                    return true;
                }

                // Make sure that the item at index is not less than the one before it
                // and not greater than the one after it.
                // If this fails, we need to update the list.

                if (index != 0)
                {
                    if (_itemComparer.Compare(_items[index - 1], _items[index]) > 0)
                    {
                        return false;
                    }
                }

                if (index < _items.Count - 1)
                {
                    if (_itemComparer.Compare(_items[index], _items[index+1]) > 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private int _FindInsertionPoint(TItem item)
        {
            Assert.IsNotNull(item);

            if (_itemComparer.Comparison != null)
            {
                for (int i = 0; i < _items.Count; ++i)
                {
                    if (_itemComparer.Compare(item, _items[i]) <= 0)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Safe version of Clear that removes references to this from the items being removed.
        /// </summary>
        private void _MergeClear()
        {
            if (_areItemsNotifiable)
            {
                foreach (var item in _items)
                {
                    _SafeRemoveNotifyAndLookup(item);
                }
            }
            _items.Clear();

            if (_fkidLookup != null)
            {
                _fkidLookup.Clear();
            }
        }

        private void _RichMerge(IEnumerable<TItem> newCollection, bool additive, int? maxCount)
        {
            lock (SyncRoot)
            {
                if (newCollection == null)
                {
                    _MergeClear();
                    return;
                }

                if (!additive)
                {
                    int index = -1;
                    foreach (TItem newItem in _itemComparer.OrderedList(newCollection).Sublist(0, maxCount))
                    {
                        var mergeableItem = newItem as IMergeable<TKey, TItem>;

                        ++index;
                        //Assert.IsNotNull(mergeableItem.FKID);
                        int oldIndex = _FindIndex(index, p => mergeableItem.FKID.Equals(p.FKID));
                        if (oldIndex == -1)
                        {
                            _items.Insert(index, newItem);
                            _SafeAddNotifyAndLookup(newItem);
                            continue;
                        }
                        else if (oldIndex != index)
                        {
                            _items.Move(oldIndex, index);
                        }

                        _suspendNotificationsForMergeableObject = (IMergeable<TKey, TItem>)this[index];
                        _suspendNotificationsForMergeableObject.Merge(newItem);
                        _suspendNotificationsForMergeableObject = null;

                        Assert.IsTrue<object>(unused => _AreItemsSortedUpToIndex(index), null);
                    }

                    if (index != -1)
                    {
                        ++index;
                        _RemoveRange(index);
                    }
                    else
                    {
                        _MergeClear();
                    }
                }
                else
                {
                    foreach (var item in newCollection)
                    {
                        var mergableItem = (IMergeable<TKey, TItem>)item;

                        int index = _FindIndex(0, p => p.FKID.Equals(mergableItem.FKID));
                        if (index == -1)
                        {
                            index = _FindInsertionPoint(item);

                            if (-1 == index)
                            {
                                _items.Add(item);
                            }
                            else
                            {
                                _items.Insert(index, item);
                            }
                            _SafeAddNotifyAndLookup(item);
                        }
                        else
                        {
                            _suspendNotificationsForMergeableObject = (IMergeable<TKey, TItem>)this[index];
                            _suspendNotificationsForMergeableObject.Merge(item);
                            _suspendNotificationsForMergeableObject = null;
                        }
                    }
                }

                if (maxCount != null && _items.Count > maxCount.Value)
                {
                    _RemoveRange(maxCount.Value);
                }

                //Assert.Implies(_areItemsComparable || _customComparison != null, () => _items.AreSorted(_customComparison));
            }
        }

        private bool _AreItemsSortedUpToIndex(int index)
        {
            if (_itemComparer.CanCompare)
            {
                for (int i = 1; i < index; ++i)
                {
                    if (_itemComparer.Comparison(_items[i - 1], _items[i]) > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void _RemoveRange(int index)
        {
            while (index < _items.Count)
            {
                _SafeRemoveNotifyAndLookup(_items[index]);
                _items.RemoveAt(index);
            }
        }

        private void _SimpleMerge(IEnumerable<TItem> newCollection, bool add, int? maxCount)
        {
            lock (SyncRoot)
            {
                if (!add)
                {
                    // This just replaces the entire collection.
                    _MergeClear();

                    if (newCollection == null)
                    {
                        return;
                    }

                    foreach (TItem item in _itemComparer.OrderedList(newCollection).Sublist(0, maxCount))
                    {
                        _items.Add(item);
                        _SafeAddNotifyAndLookup(item);
                    }
                }
                else
                {
                    foreach (var item in newCollection)
                    {
                        if (!_items.Contains(item))
                        {
                            int index = _FindInsertionPoint(item);

                            if (-1 == index)
                            {
                                _items.Add(item);
                            }
                            else
                            {
                                _items.Insert(index, item);
                            }
                            _SafeAddNotifyAndLookup(item);
                        }
                    }

                    if (maxCount != null && _items.Count > maxCount.Value)
                    {
                        _RemoveRange(maxCount.Value);
                    }
                }
                //Assert.Implies(_areItemsComparable || _customComparison != null, () => _items.AreSorted(_customComparison));
            }
        }

        private void _SafeAddNotifyAndLookup(TItem item)
        {
            Assert.IsNotNull(item);
            if (_areItemsNotifiable)
            {
                ((INotifyPropertyChanged)item).PropertyChanged += _OnItemChanged;
            }

            if (_areItemsMergable)
            {
                //Assert.IsNotNull(((IMergeable<TKey, TItem>)item).FKID);
                _fkidLookup.Add(((IMergeable<TKey, TItem>)item).FKID, item);
            }
        }

        private void _SafeRemoveNotifyAndLookup(TItem item)
        {
            Assert.IsNotNull(item);
            if (_areItemsNotifiable)
            {
                ((INotifyPropertyChanged)item).PropertyChanged -= _OnItemChanged;
            }

            if (_areItemsMergable)
            {
                //Assert.IsNotNull(((IMergeable<TKey, TItem>)item).FKID);
                _fkidLookup.Remove(((IMergeable<TKey, TItem>)item).FKID);
            }
        }

        private void _OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as TItem;
            // If we're doing a merge of this item then we expect that properties may change.
            // Don't reorder because of this.  We expect the merge is putting the item in the right place.
            if (item != null && item != _suspendNotificationsForMergeableObject)
            {
                lock (SyncRoot)
                {
                    int currentIndex = _items.IndexOf(item);
                    if (-1 != currentIndex)
                    {
                        if (!_VerifyInsertionPoint(currentIndex))
                        {
                            _items.RemoveAt(currentIndex);
                            int newIndex = _FindInsertionPoint(item);
                            if (newIndex == -1 || newIndex == _items.Count)
                            {
                                _items.Add(item);
                            }
                            else
                            {
                                _items.Insert(newIndex, item);
                            }
                        }
                    }

                    //Assert.IsTrue(_items.AreSorted(_customComparison));
                }
            }
        }

        #region IList<T> Members

        public int IndexOf(TItem item) { return _items.IndexOf(item); }

        public TItem this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException(); }
        }

        #region Unsupported Mutable IList<T> Members
        void IList<TItem>.Insert(int index, TItem item) { throw new NotSupportedException(); }
        void IList<TItem>.RemoveAt(int index) { throw new NotSupportedException(); }
        #endregion

        #endregion

        #region ICollection<T> Members

        public bool Contains(TItem item) { return _items.Contains(item); }
        public void CopyTo(TItem[] array, int arrayIndex) { _items.CopyTo(array, arrayIndex); }
        public int Count { get { return _items.Count; } }
        public bool IsReadOnly { get { return true; } }

        public void Clear() { Merge(null, false); }
        
        public void Add(TItem item)
        { 
            Verify.IsNotNull(item, "item");
            Merge(new [] { item }, true);
        }

        public bool Remove(TItem item)
        { 
            Verify.IsNotNull(item, "item");

            lock (SyncRoot)
            {
                if (_items.Remove(item))
                {
                    _SafeRemoveNotifyAndLookup(item);
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<TItem> GetEnumerator() { return _items.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _items.CollectionChanged += value; }
            remove { _items.CollectionChanged -= value; }
        }

        #endregion
    }
}
