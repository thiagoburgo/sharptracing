using System;
using System.Collections;
using System.Collections.Generic;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: ItemCollection

        /// <summary>
        /// Represents a collection of the <see cref="Item">items</see> in the <see cref="Toolbox"/>.
        /// </summary>
        [Serializable]
        public class ItemCollection : CollectionBase, ICollection, ICollection<Item>, IEnumerable<Item> {
            private readonly ITab _owner;
            private bool _suspendInvalidate = false;

            /// <summary>
            /// Initializes a new instance of the <see cref="ItemCollection"/> class using the specified <see cref="ITab">owner</see>.
            /// </summary>
            /// <param name="owner">The owner of the collection</param>
            public ItemCollection(ITab owner) {
                this._owner = owner;
            }

            internal Tab Tab {
                get { return this._owner as Tab; }
            }

            #region Overridables

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnInsertComplete(int index, object value) {
                Item item = value as Item;
                if (item == null) {
                    throw new Exception();
                }
                item.Owner = this._owner;
                this.OnCountChanged(EventArgs.Empty);
                if (this.Tab != null && !this.Tab.PointerItem.Enabled) {
                    this.Tab.PointerItem.Enabled = true;
                }
                if (!this._suspendInvalidate) {
                    this._owner.Invalidate();
                }
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnRemoveComplete(int index, object value) {
                Item item = value as Item;
                item.Owner = null;
                this.OnCountChanged(EventArgs.Empty);
                if (this.Count == 0) {
                    this.OnEmptied(EventArgs.Empty);
                    if (this.Tab != null) {
                        this.Tab.PointerItem.Enabled = false;
                    }
                }
                this._owner.Invalidate();
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnClear() {
                foreach (Item item in this.List) {
                    item.Owner = null;
                }
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnClearComplete() {
                this.OnCountChanged(EventArgs.Empty);
                this.OnEmptied(EventArgs.Empty);
                this._owner.Invalidate();
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                Item value = newValue as Item;
                if (value == null) {
                    throw new ArgumentException();
                }
                if (value.Owner != null) {
                    throw new ArgumentException(Resources.ToolboxExceptionItemHasOtherOwner, "newValue");
                }
                value.Owner = this._owner;
                ((Item) oldValue).Owner = null;
                this._owner.Invalidate();
            }

            #endregion

            #region ICollection<Item> Members

            /// <summary>
            /// Adds an <paramref name="item"/> to the collection.
            /// </summary>
            /// <param name="item">An <see cref="Item"/> to add.</param>
            public void Add(Item item) {
                if (item == null) {
                    throw new ArgumentNullException("item");
                }
                if (this.Contains(item)) {
                    throw new ArgumentException(Resources.ToolboxExceptionItemIsInCollection, "item");
                }
                if (item.Owner != null) {
                    throw new ArgumentException(Resources.ToolboxExceptionItemHasOtherOwner, "item");
                }
                item.Owner = this._owner;
                this.List.Add(item);
            }

            /// <summary>
            /// Determines whether an <paramref name="item"/> is in the collection.
            /// </summary>
            /// <param name="item">The <see cref="Item"/> to locate in the collection.</param>
            /// <returns><b>true</b> if item is found in the collection; otherwise, <b>false</b>.</returns>
            public bool Contains(Item item) {
                return this.InnerList.Contains(item);
            }

            void ICollection<Item>.CopyTo(Item[] array, int arrayIndex) {
                this.InnerList.CopyTo(array, arrayIndex);
            }

            bool ICollection<Item>.IsReadOnly {
                get { return this.InnerList.IsReadOnly; }
            }

            /// <summary>
            /// Removes an <paramref name="item"/> from the collection.
            /// </summary>
            /// <param name="item">An <see cref="Item"/> to remove.</param>
            /// <returns><b>true</b> if item is found in the collection and successfully removed; otherwise <b>false</b>.</returns>
            public bool Remove(Item item) {
                if (item == null) {
                    throw new ArgumentNullException("item");
                }
                if (this.Contains(item)) {
                    this.List.Remove(item);
                    item.Owner = null;
                    this._owner.Invalidate();
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns an <see cref="IEnumerator"/> for the <see cref="ItemCollection"/>.
            /// </summary>
            public new IEnumerator<Item> GetEnumerator() {
                return new ItemEnumerator(this.List);
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Searches the specified <paramref name="item"/> in the collection and returns a zero-based index of it.
            /// </summary>
            /// <param name="item">An <see cref="Item"/> to search.</param>
            /// <returns></returns>
            public int IndexOf(Item item) {
                if (item == null) {
                    throw new ArgumentNullException("item");
                }
                return this.InnerList.IndexOf(item);
            }

            /// <summary>
            /// Inserts an <paramref name="item"/> into the collection at the specified <paramref name="index"/>.
            /// </summary>
            /// <param name="index">The zero-based index.</param>
            /// <param name="item">An <see cref="Item"/> to insert.</param>
            public void Insert(int index, Item item) {
                if (this.Contains(item)) {
                    throw new ArgumentException(Resources.ToolboxExceptionItemIsInCollection, "item");
                }
                this.List.Insert(index, item);
            }

            /// <summary>
            /// Inserts an <paramref name="item"/> into the collection just after the specified item.
            /// </summary>
            /// <param name="after">An <see cref="Item"/> after which to insert new item.</param>
            /// <param name="item">An <see cref="Item"/> to insert.</param>
            public void InsertAfter(Item after, Item item) {
                if (after.Owner != this._owner) {
                    throw new ArgumentException(Resources.ToolboxExceptionItemHasOtherOwner, "after");
                }
                if (this.Tab != null && after == this.Tab.PointerItem) {
                    this.Insert(0, item);
                } else {
                    int index = this.IndexOf(after);
                    this.Insert(index + 1, item);
                }
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets an <see cref="Item"/> on the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the <see cref="Item"/> to get or set.</param>
            /// <returns>An <see cref="Item"/> on the specified index.</returns>
            public Item this[int index] {
                get { return (Item) this.List[index]; }
                set { this.List[index] = value; }
            }

            /// <summary>
            /// Gets the <see cref="ITab">object</see> that contains the <see cref="Item"/>.
            /// </summary>
            public ITab Owner {
                get { return this._owner; }
            }

            #endregion

            #region Events

            /// <summary>
            /// Occurs when the <see cref="ItemCollection"/> is emptied.
            /// </summary>
            public event EventHandler Emptied;

            /// <summary>
            /// Occurs when the <see cref="ItemCollection"/> is changed.
            /// </summary>
            public event EventHandler CountChanged;

            /// <summary>
            /// Raises an <see cref="Emptied"/> event.
            /// </summary>
            protected virtual void OnEmptied(EventArgs e) {
                if (this.Emptied != null) {
                    this.Emptied(this.Owner, e);
                }
            }

            /// <summary>
            /// Raises a <see cref="CountChanged"/> event.
            /// </summary>
            /// <param name="e"></param>
            protected virtual void OnCountChanged(EventArgs e) {
                if (this.CountChanged != null) {
                    this.CountChanged(this.Owner, e);
                }
            }

            #endregion

            /// <summary>
            /// Adds a range of <paramref name="items"/> to the collection.
            /// </summary>
            /// <param name="items">An array of <see cref="Item"/> objects to add.</param>
            public void AddRange(Item[] items) {
                if (items == null) {
                    throw new ArgumentNullException("items");
                }
                this._suspendInvalidate = true;
                try {
                    foreach (Item item in items) {
                        this.Add(item);
                    }
                } finally {
                    this._suspendInvalidate = false;
                }
            }

            /// <summary>
            /// Sorts the items in the collection using the default comparer.
            /// </summary>
            public void Sort() {
                this.Sort(0, this.Count, null);
            }

            /// <summary>
            /// Sorts the items in the collection using the specified <paramref name="comparer"/>.
            /// </summary>
            /// <param name="comparer"></param>
            public void Sort(IComparer<Item> comparer) {
                this.Sort(0, this.Count, comparer);
            }

            /// <summary>
            /// Sorts the items in the collection using the specified <paramref name="comparison"/> delegate.
            /// </summary>
            /// <param name="comparison"></param>
            public void Sort(Comparison<Item> comparison) {
                if (comparison == null) {
                    throw new ArgumentNullException("comparison");
                }
                if (this.Count > 0) {
                    Item[] items = new Item[this.Count];
                    this.InnerList.CopyTo(items, 0);
                    Array.Sort<Item>(items, comparison);
                    this.InnerList.Clear();
                    this.InnerList.AddRange(items);
                    this._owner.Invalidate();
                }
            }

            /// <summary>
            /// Sorts the specified range of items in the collection using the specified <paramref name="comparer"/>.
            /// </summary>
            /// <param name="index"></param>
            /// <param name="count"></param>
            /// <param name="comparer"></param>
            public void Sort(int index, int count, IComparer<Item> comparer) {
                if ((index < 0) || (count < 0)) {
                    throw new ArgumentOutOfRangeException();
                }
                if ((this.Count - index) < count) {
                    throw new ArgumentException();
                }
                Item[] items = new Item[this.Count];
                this.InnerList.CopyTo(items, 0);
                Array.Sort<Item>(items, index, count, comparer);
                this.InnerList.Clear();
                this.InnerList.AddRange(items);
                this._owner.Invalidate();
            }

            #region Nested type: ItemEnumerator

            private class ItemEnumerator : IEnumerator<Item> {
                private int _currentIndex = -1;
                private Item _currentItem = null;
                private readonly IList _items;

                public ItemEnumerator(IList items) {
                    this._items = items;
                }

                #region IEnumerator<Item> Members

                public Item Current {
                    get {
                        if (this._currentItem != null) {
                            return this._currentItem;
                        }
                        throw new InvalidOperationException();
                    }
                }

                public void Dispose() {}

                object IEnumerator.Current {
                    get { return this.Current; }
                }

                public bool MoveNext() {
                    if (this._currentIndex < this._items.Count - 1) {
                        this._currentIndex++;
                        this._currentItem = (Item) this._items[this._currentIndex];
                        return true;
                    }
                    this._currentItem = null;
                    this._currentIndex = this._items.Count;
                    return false;
                }

                public void Reset() {
                    this._currentItem = null;
                    this._currentIndex = -1;
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}