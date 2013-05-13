using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace TooboxUI.Components {
    /// <summary>
    /// Represents a Toolbox control for designers.
    /// </summary>
    [TypeConverter(typeof (ToolboxConverter))]
    public partial class HostToolbox : Toolbox {
        private static readonly Type _hostItemType = typeof (HostItem);
        private readonly Dictionary<ToolboxItem, ToolCursor> _cursors = new Dictionary<ToolboxItem, ToolCursor>();
        private IToolboxService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="HostToolbox"/> class.
        /// </summary>
        public HostToolbox() : this(true) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="HostToolbox"/> class and creating a 'General' category tab if needed.
        /// </summary>
        /// <param name="createGeneral"></param>
        public HostToolbox(bool createGeneral) : base(createGeneral) {}

        //#region Area para facilitar a adicao de items e a mudanca de estado
        //public void AddToolboxItem(Type itemType, string tabName) {
        //    AddToolboxItem(new ToolboxItem(itemType), tabName);
        //}
        //public void AddToolboxItem(ToolboxItem item, string tabName) {
        //    Toolbox.Tab tab = this.Categories[tabName];
        //    if (tab == null) {
        //        tab = new Tab(tabName);                
        //        this.Categories.Add(tab);
        //    }
        //    tab.Opened = true;
        //    tab.Items.Add(new HostToolbox.HostItem(item));
        //}
        //public void DisableItems() {
        //    ChangeItemsState(false);
        //}
        //public void EnableItems() {
        //    ChangeItemsState(true);
        //}
        //public void HideTabs() {
        //    ChangeTabsState(false);
        //}
        //public void ShowTabs() {
        //    ChangeTabsState(true);
        //}
        //private void ChangeItemsState(bool enable) {
        //    foreach (Tab tab in Categories) {
        //        foreach (Item item in tab.Items)
        //            item.Enabled = enable;
        //        tab.PointerItem.Enabled = enable;
        //    }
        //    Invalidate();
        //}
        //private void ChangeTabsState(bool show) {
        //    foreach (Tab tab in Categories) {
        //        if (tab != GeneralCategory) {
        //            tab.Visible = show;
        //        }
        //    }
        //}
        //#endregion
        /// <summary>
        /// Gets an <see cref="IToolboxService"/> service.
        /// </summary>
        [Browsable(false)]
        public IToolboxService Service {
            get {
                if (this._service == null) {
                    this._service = new TBService(this);
                }
                return this._service;
            }
        }

        /// <summary>
        /// Gets or sets the currently selected category.
        /// </summary>
        [Browsable(false)]
        public string SelectedCategory {
            get {
                if (this.LastSelectedTool is Tab) {
                    return ((Tab) this.LastSelectedTool).Text;
                }
                Item item = this.LastSelectedTool as Item;
                if (item != null && item.Category != null) {
                    return item.Category.Text;
                }
                return string.Empty;
            }
            set {
                Tab tab = this.Categories[value];
                if (tab != null) {
                    tab.Select();
                }
            }
        }

        /// <summary>
        /// Returns currently selected <see cref="ToolboxItem"/> on the <see cref="HostToolbox"/>.
        /// </summary>
        /// <returns></returns>
        public virtual ToolboxItem GetSelectedToolboxItem() {
            HostItem hostItem = this.LastSelectedItem as HostItem;
            if (hostItem != null) {
                return hostItem.ToolboxItem;
            }
            return null;
        }

        /// <summary>
        /// Sets currently selected <see cref="ToolboxItem"/> on the <see cref="HostToolbox"/>.
        /// </summary>
        /// <param name="toolboxItem">A <see cref="ToolboxItem"/> to select.</param>
        public virtual void SetSelectedToolboxItem(ToolboxItem toolboxItem) {
            if (toolboxItem == null) {
                HostItem hostItem = this.LastSelectedItem as HostItem;
                if (hostItem != null && hostItem.Category != null && hostItem.Category.PointerItem != null) {
                    hostItem.Category.PointerItem.Select();
                }
            } else {
                Dictionary<HostItem, ToolboxItem> dict = this.GetToolboxItemsDictionary(true);
                if (dict.ContainsValue(toolboxItem)) {
                    foreach (KeyValuePair<HostItem, ToolboxItem> pair in dict) {
                        if (pair.Value == toolboxItem) {
                            pair.Key.Select();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets current cursor displayed an icon of the currenly selected <see cref="ToolboxItem"/> over the design surface.
        /// </summary>
        /// <returns><b>true</b> if the <see cref="ToolboxItem"/> is selected and cursor is set; otherwise <b>false</b>.</returns>
        public virtual bool SetCursor() {
            ToolboxItem selectedItem = this.GetSelectedToolboxItem();
            if (selectedItem != null) {
                ToolCursor cursor = null;
                if (!this._cursors.TryGetValue(selectedItem, out cursor)) {
                    try {
                        cursor = new ToolCursor(selectedItem);
                        this._cursors.Add(selectedItem, cursor);
                    } catch (Exception) {}
                }
                if (cursor != null) {
                    Cursor.Current = cursor.Cursor;
                } else {
                    Cursor.Current = Cursors.Cross;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all <see cref="HostToolbox.HostItem">items</see> associated with the specified <paramref name="toolboxItem"/> from the specified <paramref name="category"/>.
        /// </summary>
        /// <param name="toolboxItem">The <see cref="ToolboxItem"/> object to remove.</param>
        /// <param name="category">The name of the category to search for.</param>
        public virtual void RemoveToolboxItem(ToolboxItem toolboxItem, string category) {
            Dictionary<HostItem, ToolboxItem> dict;
            if (!string.IsNullOrEmpty(category)) {
                dict = GetToolboxItemsDictionary(category);
            } else {
                dict = this.GetToolboxItemsDictionary(true);
            }
            if (dict.ContainsValue(toolboxItem)) {
                List<HostItem> listRemove = new List<HostItem>();
                foreach (KeyValuePair<HostItem, ToolboxItem> pair in dict) {
                    if (pair.Value == toolboxItem) {
                        listRemove.Add(pair.Key);
                    }
                }
                foreach (HostItem item in listRemove) {
                    item.Owner.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// Removes all <see cref="HostToolbox.HostItem">items</see> associated with the specified <paramref name="toolboxItem"/> from the <see cref="HostToolbox"/>.
        /// </summary>
        /// <param name="toolboxItem">The <see cref="ToolboxItem"/> object to remove.</param>
        public void RemoveToolboxItem(ToolboxItem toolboxItem) {
            this.RemoveToolboxItem(toolboxItem, null);
        }

        /// <summary>
        /// Retrieves an <see cref="IToolboxObject"/> object from the specified format-independent data storage.
        /// </summary>
        /// <param name="dragged">An <see cref="IDataObject"/> object that contains drag'n'drop data.</param>
        protected override IToolboxObject GetDragDropTool(IDataObject dragged) {
            if (dragged.GetDataPresent(_hostItemType)) {
                return (HostItem) dragged.GetData(_hostItemType);
            }
            return base.GetDragDropTool(dragged);
        }

        #region Private Methods

        /// <summary>
        /// Returns a <see cref="ToolboxItemCollection"/> of items.
        /// </summary>
        /// <param name="visibleOnly">Indicates whether to return only visible items.</param>
        protected ToolboxItemCollection GetToolboxItems(bool visibleOnly) {
            Dictionary<HostItem, ToolboxItem> dict = GetToolboxItemsDictionary(visibleOnly);
            ToolboxItem[] items = new ToolboxItem[dict.Count];
            dict.Values.CopyTo(items, 0);
            return new ToolboxItemCollection(items);
        }

        /// <summary>
        /// Returns a one-to-one relation between <see cref="HostItem"/> and <see cref="ToolboxItem"/> in the <see cref="HostToolbox"/>.
        /// </summary>
        /// <param name="visibleOnly">Indicates whether to enumerate only visible items.</param>
        protected Dictionary<HostItem, ToolboxItem> GetToolboxItemsDictionary(bool visibleOnly) {
            Dictionary<HostItem, ToolboxItem> dict = new Dictionary<HostItem, ToolboxItem>();
            foreach (Tab tab in this.Categories) {
                if (!visibleOnly || tab.Visible) {
                    foreach (Item item in tab.Items) {
                        if (!visibleOnly || item.Visible) {
                            HostItem hostItem = item as HostItem;
                            if (hostItem != null) {
                                dict.Add(hostItem, hostItem.ToolboxItem);
                            }
                        }
                    }
                }
            }
            return dict;
        }

        private Dictionary<HostItem, ToolboxItem> GetToolboxItemsDictionary(string category) {
            Dictionary<HostItem, ToolboxItem> dict = new Dictionary<HostItem, ToolboxItem>();
            Tab tab = this.Categories[category];
            if (tab != null) {
                foreach (Item item in tab.Items) {
                    if (item.Visible) {
                        HostItem hostItem = item as HostItem;
                        if (hostItem != null) {
                            dict.Add(hostItem, hostItem.ToolboxItem);
                        }
                    }
                }
            }
            return dict;
        }

        #endregion
    }
}