using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Threading;
using DrawEngine.Renderer.Collections.Design;

namespace DrawEngine.Renderer.Collections
{
    public delegate void NotifyCollectionChangedEventHandler<T>(object sender, NotifyCollectionChangedEventArgs<T> e);

    public enum NotifyCollectionChangedAction
    {
        Add,
        Remove,
        Clear,
        Replace
    }

    public class NotifyCollectionChangedEventArgs<T> : EventArgs
    {
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action) : this(action, null, null) {}
        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T[] newItems, T[] oldItems)
        {
            this.Action = action;
            this.NewItems = newItems;
            this.OldItems = oldItems;
        }
        public NotifyCollectionChangedAction Action { get; set; }
        public T[] NewItems { get; private set; }
        public T[] OldItems { get; private set; }
    }

    public interface INotify
    {
        bool NotificationsEnabled { get; set; }
    }

    [Editor(typeof(CustomCollectionEditor), typeof(UITypeEditor))]
    public class NotifyList<T> : IList, IList<T>, INotify
    {
        private readonly List<T> internalList;
        private bool notificationsEnabled = true;
        private object syncRoot = new object();
        public NotifyList()
        {
            this.internalList = new List<T>();
        }
        public NotifyList(int capacity)
        {
            this.internalList = new List<T>(capacity);
        }
        public NotifyList(IEnumerable<T> list)
        {
            this.internalList = new List<T>(list);
        }

        #region IList Members
        public void RemoveAt(int index)
        {
            this.internalList.RemoveAt(index);
            if(this.CollectionChanged != null && this.NotificationsEnabled){
                NotifyCollectionChangedEventArgs<T> action =
                        new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove);
                this.CollectionChanged(this, action);
            }
        }
        public void Clear()
        {
            this.internalList.Clear();
            if(this.CollectionChanged != null && this.NotificationsEnabled){
                NotifyCollectionChangedEventArgs<T> action =
                        new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Clear);
                this.CollectionChanged(this, action);
            }
        }
        public int Count
        {
            get { return this.internalList.Count; }
        }
        public bool IsReadOnly
        {
            get { return false; }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }
        int IList.Add(object value)
        {
            this.Add((T)value);
            return this.internalList.Count - 1;
        }
        bool IList.Contains(object value)
        {
            return this.Contains((T)value);
        }
        int IList.IndexOf(object value)
        {
            return this.IndexOf((T)value);
        }
        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T)value);
        }
        bool IList.IsFixedSize
        {
            get { return false; }
        }
        void IList.Remove(object value)
        {
            this.Remove((T)value);
        }
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }
        public void CopyTo(Array array, int index)
        {
            this.internalList.CopyTo(array as T[], index);
        }
        public bool IsSynchronized
        {
            get { return false; }
        }
        public object SyncRoot
        {
            get
            {
                if(this.syncRoot == null){
                    Interlocked.CompareExchange(ref this.syncRoot, new object(), null);
                }
                return this.syncRoot;
            }
        }
        #endregion

        #region IList<T> Members
        public int IndexOf(T item)
        {
            return this.internalList.IndexOf(item);
        }
        public virtual void Insert(int index, T item)
        {
            this.internalList.Insert(index, item);
            if(this.CollectionChanged != null && this.NotificationsEnabled){
                NotifyCollectionChangedEventArgs<T> action =
                        new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, new T[]{item}, null);
                this.CollectionChanged(this, action);
            }
        }
        public virtual T this[int index]
        {
            get { return this.internalList[index]; }
            set
            {
                T oldItem = this.internalList[index];
                this.internalList[index] = value;
                if(this.CollectionChanged != null && this.NotificationsEnabled){
                    NotifyCollectionChangedEventArgs<T> action =
                            new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Replace,
                                                                    new T[]{this.internalList[index]}, new T[]{oldItem});
                    this.CollectionChanged(this, action);
                }
            }
        }
        public virtual void Add(T item)
        {
            this.internalList.Add(item);
            if(this.CollectionChanged != null && this.NotificationsEnabled){
                NotifyCollectionChangedEventArgs<T> action =
                        new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Add, new T[]{item}, null);
                this.CollectionChanged(this, action);
            }
        }
        public bool Contains(T item)
        {
            return this.internalList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.internalList.CopyTo(array, arrayIndex);
        }
        public bool Remove(T item)
        {
            bool removed = this.internalList.Remove(item);
            if(this.CollectionChanged != null && this.NotificationsEnabled && removed){
                NotifyCollectionChangedEventArgs<T> action =
                        new NotifyCollectionChangedEventArgs<T>(NotifyCollectionChangedAction.Remove);
                this.CollectionChanged(this, action);
            }
            return removed;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return this.internalList.GetEnumerator();
        }
        #endregion

        #region INotify Members
        public bool NotificationsEnabled
        {
            get { return this.notificationsEnabled; }
            set { this.notificationsEnabled = value; }
        }
        #endregion

        public event NotifyCollectionChangedEventHandler<T> CollectionChanged;
        public override string ToString()
        {
            return "List of " + typeof(T).Name;
        }
    }
}