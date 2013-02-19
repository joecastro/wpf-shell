
namespace Standard
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;

    internal class NotifyingList<T> : IList<T>, INotifyCollectionChanged where T : INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> _list;

        public event PropertyChangedEventHandler ItemPropertyChanged;

        private void _SafeAddPropertyListener(T item)
        {
            if (item != null)
            {
                item.PropertyChanged += _OnItemPropertyChanged;
            }
        }

        private void _SafeRemovePropertyListener(T item)
        {
            if (item != null)
            {
                item.PropertyChanged -= _OnItemPropertyChanged;
            }
        }

        public NotifyingList()
        {
            _list = new ObservableCollection<T>();
        }

        public NotifyingList(IEnumerable<T> collection)
        {
            _list = new ObservableCollection<T>(collection);
            foreach (T item in collection)
            {
                _SafeAddPropertyListener(item);
            }
        }

        private void _OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = ItemPropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
            _SafeAddPropertyListener(item);
        }

        public void RemoveAt(int index)
        {
            T item = _list[index];
            _list.RemoveAt(index);
            _SafeRemovePropertyListener(item);
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set { _list[index] = value; }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            _list.Add(item);
            _SafeAddPropertyListener(item);
        }

        public void Clear()
        {
            T[] items = new T[_list.Count];
            _list.CopyTo(items, 0);
            _list.Clear();
            foreach (T item in items)
            {
                _SafeRemovePropertyListener(item);
            }
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count { get { return _list.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(T item)
        {
            // Don't call through _list.Remove().
            // If equality has been overloaded then we may try removing the handler
            // from a different object and we'll leak it.
            // Ensure reference equality.
            int index = _list.IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region INotifyCollectionChanged Members

        private event NotifyCollectionChangedEventHandler _sourceCollectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                if (_sourceCollectionChanged == null)
                {
                    _list.CollectionChanged += _OnSourceCollectionChanged;
                }
                _sourceCollectionChanged += value;
            }
            remove
            {
                _sourceCollectionChanged -= value;
                if (_sourceCollectionChanged == null)
                {
                    _list.CollectionChanged -= _OnSourceCollectionChanged;
                }
            }
        }

        private void _OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var handler = _sourceCollectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion
    }
}
