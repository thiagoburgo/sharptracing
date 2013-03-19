using System;
using System.Collections;
using System.Drawing.Design;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components
{
    partial class HostToolbox
    {
        #region Nested type: ToolboxItemList
        private class ToolboxItemList : IList
        {
            // Used only Add, Remove, Count, GetEnumerator
            private Tab _tab;
            private HostToolbox _toolbox;
            public ToolboxItemList(Tab tab)
            {
                this._tab = tab;
                this._toolbox = this._tab.Toolbox as HostToolbox;
            }
            public ToolboxItemList(HostToolbox toolbox)
            {
                this._toolbox = toolbox;
            }

            #region IList Members
            public int Add(object value)
            {
                ToolboxItemContainer item = (ToolboxItemContainer)value;
                if(item != null){
                    HostItem hostItem = new HostItem(item.GetToolboxItem(null));
                    if(this._tab != null){
                        this._tab.Items.Add(hostItem);
                    } else if(this._toolbox.GeneralCategory != null){
                        this._toolbox.GeneralCategory.Items.Add(hostItem);
                    } else{
                        if(this._toolbox.Categories.Count == 0){
                            this._toolbox.Categories.Add(this._toolbox.CreateNewTab(Resources.ToolboxTabDefaultName));
                        }
                        this._toolbox.Categories[this._toolbox.Categories.Count - 1].Items.Add(hostItem);
                    }
                }
                return -1;
            }
            public void Clear() {}
            public bool Contains(object value)
            {
                return false;
            }
            public int IndexOf(object value)
            {
                return -1;
            }
            public void Insert(int index, object value) {}
            public bool IsFixedSize
            {
                get { return false; }
            }
            public void Remove(object value)
            {
                ToolboxItemContainer item = (ToolboxItemContainer)value;
                if(item != null){
                    ToolboxItem toolboxItem = item.GetToolboxItem(null);
                    this._toolbox.RemoveToolboxItem(toolboxItem, (this._tab != null) ? this._tab.Text : null);
                }
            }
            public void RemoveAt(int index) {}
            object IList.this[int index]
            {
                get { return null; }
                set { }
            }
            public bool IsReadOnly
            {
                get { return false; }
            }
            public void CopyTo(Array array, int index) {}
            public int Count
            {
                get
                {
                    if(this._tab != null){
                        return this._tab.Items.Count;
                    } else{
                        int count = 0;
                        foreach(Tab tab in this._toolbox.Categories){
                            count += tab.Items.Count;
                        }
                        return count;
                    }
                }
            }
            public bool IsSynchronized
            {
                get { return false; }
            }
            public object SyncRoot
            {
                get { return this; }
            }
            public IEnumerator GetEnumerator()
            {
                if(this._tab != null){
                    foreach(Item item in this._tab.Items){
                        HostItem hostItem = item as HostItem;
                        if(hostItem != null){
                            yield return new ToolboxItemContainer(hostItem.ToolboxItem);
                        }
                    }
                } else{
                    foreach(Tab tab in this._toolbox.Categories){
                        foreach(Item item in tab.Items){
                            HostItem hostItem = item as HostItem;
                            if(hostItem != null){
                                yield return new ToolboxItemContainer(hostItem.ToolboxItem);
                            }
                        }
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}