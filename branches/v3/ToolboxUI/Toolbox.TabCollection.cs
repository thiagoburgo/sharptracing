using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: TabCollection

        /// <summary>
        /// Represents a collection of the <see cref="Tab">categories</see> of the <see cref="Toolbox"/> control.
        /// </summary>
        [Serializable]
        public sealed class TabCollection : CollectionBase, ICollection, IEnumerable, ICollection<Tab>, IEnumerable<Tab> {
            private readonly ITab _owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="TabCollection"/> class using the specified <see cref="ITab">owner</see>.
            /// </summary>
            /// <param name="tab">The owner of the collection.</param>
            public TabCollection(ITab tab) {
                this._owner = tab;
            }

            internal Toolbox Toolbox {
                get {
                    Tab tab = this._owner as Tab;
                    if (tab != null) {
                        return tab.Toolbox;
                    }
                    return (Toolbox) this._owner;
                }
            }

            /// <summary>
            /// Gets or sets the <see cref="Tab"/> at the specified index.
            /// </summary>
            /// <param name="index">The zero-based index of the <see cref="Tab"/> to get or set.</param>
            /// <returns>The <see cref="Tab"/> at the specified index.</returns>
            public Tab this[int index] {
                get { return (Tab) this.InnerList[index]; }
                set { this.InnerList[index] = value; }
            }

            /// <summary>
            /// Gets the <see cref="Tab"/> with the specified name.
            /// </summary>
            /// <param name="name">The name of the <see cref="Tab"/> to get.</param>
            /// <returns>The <see cref="Tab"/> with the specified name.</returns>
            public Tab this[string name] {
                get {
                    foreach (Tab tab in this.InnerList) {
                        if (tab.Text == name) {
                            return tab;
                        }
                    }
                    return null;
                }
            }

            #region ICollection<Tab> Members

            /// <summary>
            /// Adds a <paramref name="tab"/> to the collection.
            /// </summary>
            /// <param name="tab">A <see cref="Tab"/> to add.</param>
            public void Add(Tab tab) {
                if (this._owner == tab) {
                    throw new ArgumentException(Resources.ToolboxExceptionTabAddSelf, "tab");
                }
                try {
                    if (this[tab.Text] != null) {
                        throw new DuplicateNameException();
                    }
                    this.List.Add(tab);
                    tab.Owner = this._owner;
                } catch (DuplicateNameException) {
                    throw;
                }
                this._owner.Invalidate();
            }

            /// <summary>
            /// Determines whether a <paramref name="tab"/> is in the collection.
            /// </summary>
            /// <param name="tab">The <see cref="Tab"/> to locate in the collection.</param>
            /// <returns><b>true</b> if tab is found in the collection; otherwise, <b>false</b>.</returns>
            public bool Contains(Tab tab) {
                return this.InnerList.Contains(tab);
            }

            void ICollection<Tab>.CopyTo(Tab[] tabs, int arrayIndex) {
                this.InnerList.CopyTo(tabs, arrayIndex);
            }

            /// <summary>
            /// Removes a <paramref name="tab"/> from the collection.
            /// </summary>
            /// <param name="tab">A <see cref="Tab"/> to remove.</param>
            /// <returns><b>true</b> if tab is found in the collection and successfully removed; otherwise <b>false</b>.</returns>
            public bool Remove(Tab tab) {
                try {
                    int index = this.IndexOf(tab);
                    if (index >= 0) {
                        this.RemoveAt(index);
                    }
                } catch {}
                return tab.Owner == null;
            }

            bool ICollection<Tab>.IsReadOnly {
                get { return false; }
            }

            IEnumerator<Tab> IEnumerable<Tab>.GetEnumerator() {
                return new Enumerator(this);
            }

            #endregion

            #region Overridables

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnInsert(int index, object value) {
                Tab tab = value as Tab;
                if (tab == null) {
                    throw new ArgumentNullException("value");
                }
                if (this[tab.Text] != null) {
                    Toolbox toolbox = this.Toolbox;
                    if (toolbox != null) {
                        tab.Text = GetUnusedCategoryName(this._owner);
                    } else {
                        throw new DuplicateNameException();
                    }
                }
                base.OnInsert(index, value);
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnInsertComplete(int index, object value) {
                Tab tab = value as Tab;
                tab.Owner = this._owner;
                this._owner.Invalidate();
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnRemoveComplete(int index, object value) {
                Tab tab = value as Tab;
                Toolbox toolbox = this.Toolbox;
                if (toolbox != null && (toolbox.Site == null || !toolbox.Site.DesignMode) && !tab.AllowDelete) {
                    throw new Exception();
                }
                tab.Owner = null;
                this._owner.Invalidate();
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnClearComplete() {
                Toolbox toolbox = this._owner as Toolbox;
                if (toolbox != null && toolbox.GeneralCategory != null && toolbox.GeneralCategory.Owner == toolbox) {
                    this.List.Add(toolbox.GeneralCategory);
                }
                this._owner.Invalidate();
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnSetComplete(int index, object oldValue, object newValue) {
                Tab oldTab = oldValue as Tab;
                Tab newTab = newValue as Tab;
                this.Insert(index + 1, oldTab);
            }

            /// <summary>
            /// Overriden.
            /// </summary>
            protected override void OnValidate(object value) {
                if (!(value is Tab)) {
                    throw new InvalidCastException();
                }
            }

            #endregion

            #region Enumerator

            private struct Enumerator : IEnumerator<Tab>, IDisposable, IEnumerator {
                private Tab current;
                private int index;
                private readonly TabCollection list;

                internal Enumerator(TabCollection list) {
                    this.list = list;
                    this.index = 0;
                    this.current = null;
                }

                #region IEnumerator<Tab> Members

                public void Dispose() {}

                public bool MoveNext() {
                    if (this.index < this.list.Count) {
                        this.current = this.list[this.index];
                        this.index++;
                        return true;
                    }
                    this.index = this.list.Count + 1;
                    this.current = null;
                    return false;
                }

                public Tab Current {
                    get { return this.current; }
                }

                object IEnumerator.Current {
                    get {
                        if ((this.index == 0) || (this.index == (this.list.Count + 1))) {
                            throw new ArgumentOutOfRangeException("index");
                        }
                        return this.Current;
                    }
                }

                void IEnumerator.Reset() {
                    this.index = 0;
                    this.current = null;
                }

                #endregion
            }

            #endregion

            /// <summary>
            /// Searches the specified <paramref name="tab"/> in the collection and returns a zero-based index of it.
            /// </summary>
            /// <param name="tab">A <see cref="Tab"/> to search.</param>
            /// <returns></returns>
            public int IndexOf(Tab tab) {
                return this.InnerList.IndexOf(tab);
            }

            /// <summary>
            /// Inserts a <paramref name="tab"/> into the collection at the specified <paramref name="index"/>.
            /// </summary>
            /// <param name="index">The zero-based index.</param>
            /// <param name="tab">A <see cref="Tab"/> to insert.</param>
            public void Insert(int index, Tab tab) {
                try {
                    this.List.Insert(index, tab);
                } catch (DuplicateNameException) {
                    throw;
                }
            }

            /// <summary>
            /// Adds a range of <paramref name="tabs"/> to the collection.
            /// </summary>
            /// <param name="tabs">An array of <see cref="Tab"/> objects to add.</param>
            public void AddRange(Tab[] tabs) {
                foreach (Tab tab in tabs) {
                    try {
                        this.List.Add(tab);
                        tab.Owner = this._owner;
                    } catch (DuplicateNameException) {
                        throw;
                    }
                }
                this._owner.Invalidate();
            }
        }

        #endregion
    }
}