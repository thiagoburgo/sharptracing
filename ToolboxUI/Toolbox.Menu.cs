using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components {
    partial class Toolbox {
        private static Bitmap _copyImage;
        private static Bitmap _cutImage;
        private static Bitmap _deleteImage;
        private static Bitmap _pasteImage;

        private Bitmap GetMenuImage(string name) {
            switch (name) {
                case "Cut":
                    if (_cutImage == null) {
                        _cutImage = LoadImage(Resources.Cut);
                    }
                    return _cutImage;
                case "Copy":
                    if (_copyImage == null) {
                        _copyImage = LoadImage(Resources.Copy);
                    }
                    return _copyImage;
                case "Delete":
                    if (_deleteImage == null) {
                        _deleteImage = LoadImage(Resources.Delete);
                    }
                    return _deleteImage;
                case "Paste":
                    if (_pasteImage == null) {
                        _pasteImage = LoadImage(Resources.Paste);
                    }
                    return _pasteImage;
            }
            return null;
        }

        private static Bitmap LoadImage(Bitmap original) {
            Bitmap bitmap = null;
            try {
                bitmap = new Bitmap(original);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));
            } catch (FileNotFoundException) {} catch (ArgumentNullException) {}
            return bitmap;
        }

        /// <summary>
        /// Creates and adds an empty category tab.
        /// </summary>
        /// <remarks>Invoked by the 'Add Tab' menu item.</remarks>
        protected virtual void AddEmptyTab() {
            ITab owner = this.LastSelectedTool as ITab;
            if (owner == null && this.LastSelectedTool != null) {
                owner = this.LastSelectedTool.Owner;
            }
            Tab tab = null;
            if (owner != null) {
                tab = this.CreateNewTab(GetUnusedCategoryName(owner));
                owner.Categories.Add(tab);
            } else {
                tab = this.CreateNewTab(this.GetUnusedCategoryName());
                this.Categories.Add(tab);
            }
            tab.Opened = true;
            this.OnPaint(new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
            this.OnRenameTab(tab, string.Empty);
        }

        /// <summary>
        /// Sorts currently selected category tab items in ascending order.
        /// </summary>
        /// <remarks>Invoked by the 'Sort Alphabetically' menu item.</remarks>
        protected virtual void OnSortAlphabetically() {
            Tab selected = this.LastSelectedTool as Tab;
            if (selected == null && this.LastSelectedTool != null) {
                selected = this.LastSelectedTool.Owner as Tab;
            }
            if (selected != null) {
                selected.SortItems(SortOrder.Ascending);
            }
        }

        /// <summary>
        /// Resets the state of the <see cref="Toolbox"/>.
        /// </summary>
        /// <remarks>Invoked by the 'Reset Toolbox' menu item.</remarks>
        protected virtual void OnResetToolbox() {}

        /// <summary>
        /// Copies to the clipboard and removes an <paramref name="item"/> from the <see cref="Toolbox"/>.
        /// </summary>
        /// <param name="item">An <see cref="Item"/> to cut.</param>
        /// <remarks>Invoked by the 'Cut' menu item.</remarks>
        protected virtual void OnCutItem(Item item) {
            this.OnCopyItem(item);
            this.OnDeleteItem(item);
        }

        /// <summary>
        /// Copies an <paramref name="item"/> to the clipboard.
        /// </summary>
        /// <param name="item">An <see cref="Item"/> to copy.</param>
        /// <remarks>Invoked by the 'Copy' menu item.</remarks>
        protected virtual void OnCopyItem(Item item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            IDataObject dataObject = new DataObject(ClipboardFormat.Item, item.Clone());
            Clipboard.SetDataObject(dataObject, false);
        }

        /// <summary>
        /// Removes an <paramref name="item"/> from the <see cref="Toolbox"/>.
        /// </summary>
        /// <param name="item">An <see cref="Item"/> to remove.</param>
        /// <remarks>Invoked by the 'Delete' menu item.</remarks>
        protected virtual void OnDeleteItem(Item item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (item.Owner == null) {
                throw new ArgumentException(Resources.ToolboxDeleteNonTabbedItem, "item");
            }
            item.Owner.Items.Remove(item);
        }

        /// <summary>
        /// Inserts an <paramref name="item"/> to the <paramref name="category"/> tab.
        /// </summary>
        /// <param name="item">An <see cref="Item"/> to paste.</param>
        /// <param name="tab">A <see cref="Tab"/> to add an item.</param>
        /// <remarks>Invoked by the 'Paste' menu item.</remarks>
        protected virtual void OnPasteItem(Item item, ITab tab) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            if (tab == null) {
                throw new ArgumentNullException("category");
            }
            if (item.Owner != null) {
                if (item.Owner == tab) {
                    return;
                }
                item.Owner.Items.Remove(item);
            }
            tab.Items.Add(item);
        }

        #region Item Menu

        /// <summary>
        /// Creates a default <see cref="ContextMenuStrip">menu</see> to display on <see cref="Item">items</see>.
        /// </summary>
        /// <returns></returns>
        protected virtual ContextMenuStrip CreateItemMenu() {
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Opening += new CancelEventHandler(this.OnItemMenuOpening);
            ToolStripMenuItem item = new ToolStripMenuItem(Resources.ToolboxMenuCut);
            item.Image = this.GetMenuImage("Cut");
            item.Click += new EventHandler(this.OnMenuItemCutItem);
            item.Tag = ContextMenuItems.Cut;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuCopy);
            item.Image = this.GetMenuImage("Copy");
            item.Click += new EventHandler(this.OnMenuItemCopyItem);
            item.Tag = ContextMenuItems.Copy;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuPaste);
            item.Image = this.GetMenuImage("Paste");
            item.Click += new EventHandler(this.OnMenuItemPasteItem);
            item.Tag = ContextMenuItems.Paste;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuDelete);
            item.Image = this.GetMenuImage("Delete");
            item.Click += new EventHandler(this.OnMenuItemDeleteItem);
            item.Tag = ContextMenuItems.Remove;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuRenameItem);
            item.Click += new EventHandler(this.OnMenuItemRenameItem);
            item.Tag = ContextMenuItems.Rename;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuShowAll);
            item.Click += new EventHandler(this.OnMenuItemShowAll);
            item.Tag = ContextMenuItems.ShowAll;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuChooseItems);
            item.Click += new EventHandler(this.OnMenuItemChooseItems);
            item.Tag = ContextMenuItems.ShowAll;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuSortAlphabetically);
            item.Click += new EventHandler(this.OnMenuItemSortAlphabetically);
            item.Tag = ContextMenuItems.SortItems;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuResetToolbox);
            item.Click += new EventHandler(this.OnMenuItemResetToolbox);
            item.Tag = ContextMenuItems.Reset;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuAddTab);
            item.Click += new EventHandler(this.OnMenuItemAddTab);
            item.Tag = ContextMenuItems.AddTab;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuMoveUp);
            item.Click += new EventHandler(this.OnMenuItemMoveUp);
            item.Tag = ContextMenuItems.MoveUp;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuMoveDown);
            item.Click += new EventHandler(this.OnMenuItemMoveDown);
            item.Tag = ContextMenuItems.MoveDown;
            strip.Items.Add(item);
            return strip;
        }

        private void OnItemMenuOpening(object sender, CancelEventArgs e) {
            Item caller = this.ItemMenu.Tag as Item;
            foreach (ToolStripItem stripItem in this.ItemMenu.Items) {
                if (stripItem is ToolStripMenuItem && stripItem.Tag != null && stripItem.Tag is ContextMenuItems) {
                    ToolStripMenuItem item = (ToolStripMenuItem) stripItem;
                    ContextMenuItems context = (ContextMenuItems) item.Tag;
                    switch (context) {
                        case ContextMenuItems.ListView:
                            break;
                        case ContextMenuItems.ShowAll:
                            item.Checked = this.ShowAll;
                            break;
                        case ContextMenuItems.ChooseItems:
                            break;
                        case ContextMenuItems.SortItems:
                            item.Enabled = (caller.Category != null);
                            break;
                        case ContextMenuItems.Reset:
                            break;
                        case ContextMenuItems.AddTab:
                            break;
                        case ContextMenuItems.Rename:
                            item.Enabled = (caller != null);
                            break;
                        case ContextMenuItems.MoveUp:
                            item.Enabled = (caller != null && !this.IsPointerItem(caller) &&
                                            caller.Owner.Items.IndexOf(caller) != 0);
                            break;
                        case ContextMenuItems.MoveDown:
                            item.Enabled = (caller != null && !this.IsPointerItem(caller) &&
                                            caller.Owner.Items.IndexOf(caller) != caller.Owner.Items.Count - 1);
                            break;
                        case ContextMenuItems.Cut:
                            item.Enabled = (caller != null && !this.IsPointerItem(caller));
                            break;
                        case ContextMenuItems.Copy:
                            item.Enabled = (caller != null && !this.IsPointerItem(caller));
                            break;
                        case ContextMenuItems.Paste:
                            item.Enabled = (caller != null && Clipboard.ContainsData(ClipboardFormat.Item));
                            break;
                        case ContextMenuItems.Remove:
                            item.Enabled = (caller != null && !this.IsPointerItem(caller));
                            break;
                    }
                }
            }
        }

        #endregion

        #region Category Menu

        /// <summary>
        /// Creates a default <see cref="ContextMenuStrip">menu</see> to display on <see cref="Tab">categories</see>.
        /// </summary>
        /// <returns></returns>
        protected virtual ContextMenuStrip CreateTabMenu() {
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Opening += new CancelEventHandler(this.OnTabMenuOpening);
            ToolStripMenuItem item = new ToolStripMenuItem(Resources.ToolboxMenuShowAll);
            item.Click += new EventHandler(this.OnMenuItemShowAll);
            item.Tag = ContextMenuItems.ShowAll;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuChooseItems);
            item.Click += new EventHandler(this.OnMenuItemChooseItems);
            item.Tag = ContextMenuItems.ChooseItems;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxTabPasteItem);
            item.Click += new EventHandler(this.OnMenuItemPasteItem);
            item.Tag = ContextMenuItems.Paste;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuSortAlphabetically);
            item.Click += new EventHandler(this.OnMenuItemSortAlphabetically);
            item.Tag = ContextMenuItems.SortItems;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuResetToolbox);
            item.Click += new EventHandler(this.OnMenuItemResetToolbox);
            item.Tag = ContextMenuItems.Reset;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuAddTab);
            item.Click += new EventHandler(this.OnMenuItemAddTab);
            item.Tag = ContextMenuItems.AddTab;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuTabDelete);
            item.Click += new EventHandler(this.OnMenuItemDeleteTab);
            item.Tag = ContextMenuItems.Remove;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuRenameTab);
            item.Click += new EventHandler(this.OnMenuItemRenameTab);
            item.Tag = ContextMenuItems.Rename;
            strip.Items.Add(item);
            strip.Items.Add(new ToolStripSeparator());
            item = new ToolStripMenuItem(Resources.ToolboxMenuMoveUp);
            item.Click += new EventHandler(this.OnMenuItemMoveUp);
            item.Tag = ContextMenuItems.MoveUp;
            strip.Items.Add(item);
            item = new ToolStripMenuItem(Resources.ToolboxMenuMoveDown);
            item.Click += new EventHandler(this.OnMenuItemMoveDown);
            item.Tag = ContextMenuItems.MoveDown;
            strip.Items.Add(item);
            return strip;
        }

        private void OnTabMenuOpening(object sender, CancelEventArgs e) {
            Tab caller = this.TabMenu.Tag as Tab;
            foreach (ToolStripItem stripItem in this.TabMenu.Items) {
                if (stripItem is ToolStripMenuItem && stripItem.Tag != null && stripItem.Tag is ContextMenuItems) {
                    ToolStripMenuItem item = (ToolStripMenuItem) stripItem;
                    ContextMenuItems context = (ContextMenuItems) item.Tag;
                    switch (context) {
                        case ContextMenuItems.ListView:
                            break;
                        case ContextMenuItems.ShowAll:
                            item.Checked = this.ShowAll;
                            break;
                        case ContextMenuItems.ChooseItems:
                            break;
                        case ContextMenuItems.SortItems:
                            item.Enabled = (caller != null);
                            break;
                        case ContextMenuItems.Reset:
                            break;
                        case ContextMenuItems.AddTab:
                            break;
                        case ContextMenuItems.Remove:
                            item.Enabled = (caller != null && caller.AllowDelete);
                            break;
                        case ContextMenuItems.Rename:
                            item.Enabled = (caller != null);
                            break;
                        case ContextMenuItems.MoveUp:
                            item.Enabled = (caller != null && this.Categories.IndexOf(caller) != 0);
                            break;
                        case ContextMenuItems.MoveDown:
                            item.Enabled = (caller != null &&
                                            this.Categories.IndexOf(caller) != this.Categories.Count - 1);
                            break;
                        case ContextMenuItems.Paste:
                            item.Enabled = (caller != null && Clipboard.ContainsData(ClipboardFormat.Item));
                            break;
                    }
                }
            }
        }

        #endregion

        #region MenuItem handlers

        private void OnMenuItemRenameItem(object sender, EventArgs e) {
            Item item = this.ItemMenu.Tag as Item;
            if (item != null) {
                this.OnRenameItem(item);
            }
        }

        private void OnMenuItemDeleteItem(object sender, EventArgs e) {
            this.OnDeleteItem(this.LastSelectedTool as Item);
        }

        private void OnMenuItemPasteItem(object sender, EventArgs e) {
            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(ClipboardFormat.Item)) {
                Item item = (Item) dataObject.GetData(ClipboardFormat.Item, true);
                ITab selTab = this.LastSelectedTool as Tab;
                if (selTab == null && this.LastSelectedTool is Item) {
                    selTab = this.LastSelectedTool.Owner;
                }
                if (selTab != null) {
                    this.OnPasteItem(item, selTab);
                }
            }
        }

        private void OnMenuItemCopyItem(object sender, EventArgs e) {
            this.OnCopyItem(this.LastSelectedTool as Item);
        }

        private void OnMenuItemCutItem(object sender, EventArgs e) {
            this.OnCutItem(this.LastSelectedTool as Item);
        }

        private void OnMenuItemMoveDown(object sender, EventArgs e) {
            Tab selTab = this.LastSelectedTool as Tab;
            if (selTab != null && selTab.Owner != null) {
                TabCollection tabCollection = selTab.Owner.Categories;
                int index = tabCollection.IndexOf(selTab);
                if (index < tabCollection.Count - 1) {
                    bool allowDelete = selTab.AllowDelete;
                    try {
                        selTab.AllowDelete = true;
                        tabCollection.RemoveAt(index);
                        tabCollection.Insert(index + 1, selTab);
                    } finally {
                        selTab.AllowDelete = allowDelete;
                    }
                }
            }
            Item selItem = this.LastSelectedTool as Item;
            if (selItem != null && selItem.Owner != null) {
                ItemCollection itemCollection = selItem.Owner.Items;
                int index = itemCollection.IndexOf(selItem);
                if (index < itemCollection.Count - 1) {
                    itemCollection.RemoveAt(index);
                    itemCollection.Insert(index + 1, selItem);
                }
            }
        }

        private void OnMenuItemMoveUp(object sender, EventArgs e) {
            Tab selTab = this.LastSelectedTool as Tab;
            if (selTab != null && selTab.Owner != null) {
                TabCollection tabCollection = selTab.Owner.Categories;
                int index = tabCollection.IndexOf(selTab);
                if (index > 0) {
                    bool allowDelete = selTab.AllowDelete;
                    try {
                        selTab.AllowDelete = true;
                        tabCollection.RemoveAt(index);
                        tabCollection.Insert(index - 1, selTab);
                    } finally {
                        selTab.AllowDelete = allowDelete;
                    }
                }
            }
            Item selItem = this.LastSelectedTool as Item;
            if (selItem != null && selItem.Owner != null) {
                ItemCollection itemCollection = selItem.Owner.Items;
                int index = itemCollection.IndexOf(selItem);
                if (index > 0) {
                    itemCollection.RemoveAt(index);
                    itemCollection.Insert(index - 1, selItem);
                }
            }
        }

        private void OnMenuItemRenameTab(object sender, EventArgs e) {
            Tab caller = this.LastSelectedTool as Tab;
            if (caller != null) {
                this.OnRenameTab(caller, caller.Text);
            }
        }

        private void OnMenuItemDeleteTab(object sender, EventArgs e) {
            Tab selected = this.LastSelectedTool as Tab;
            if (selected != null && selected.Owner != null && selected.AllowDelete) {
                selected.Owner.Categories.Remove(selected);
            }
        }

        private void OnMenuItemAddTab(object sender, EventArgs e) {
            this.AddEmptyTab();
        }

        private void OnMenuItemResetToolbox(object sender, EventArgs e) {
            this.OnResetToolbox();
        }

        private void OnMenuItemSortAlphabetically(object sender, EventArgs e) {
            this.OnSortAlphabetically();
        }

        private void OnMenuItemShowAll(object sender, EventArgs e) {
            this.ShowAll = !this.ShowAll;
        }

        private void OnMenuItemChooseItems(object sender, EventArgs e) {
            ChooseToolboxItemsDialog cti = new ChooseToolboxItemsDialog();
            if (cti.ShowDialog() == DialogResult.OK) {
                if (cti.SelectedTypes != null) {
                    foreach (Type type in cti.SelectedTypes) {
                        Tab owner = null;
                        if (this.LastSelectedTool != null && this.LastSelectedTool is Item) {
                            owner = ((Item) this.LastSelectedTool).Owner as Tab;
                        } else {
                            owner = this.LastSelectedTool as Tab;
                        }
                        if (owner != null) {
                            this.AddToolboxItem(type, owner.Text);
                        } else {
                            this.AddToolboxItem(type, this.GeneralCategory.Text);
                        }
                    }
                }
            }
        }

        #endregion

        #region Nested type: ContextMenuItems

        /// <summary>
        /// Enumerates the default context menu items of the <see cref="Toolbox"/>.
        /// </summary>
        protected enum ContextMenuItems {
            /// <summary>
            /// No item.
            /// </summary>
            None = 0,

            /// <summary>
            /// A 'List View' item. Not implemented in default context menus.
            /// </summary>
            ListView,

            /// <summary>
            /// A 'Show All' item.
            /// </summary>
            ShowAll,

            /// <summary>
            /// A 'Choose Items...' item. Not implemented in default context menus.
            /// </summary>
            ChooseItems,

            /// <summary>
            /// A 'Sort alphabetically' menu item.
            /// </summary>
            SortItems,

            /// <summary>
            /// A 'Reset Toolbox' menu item. Not implemented in default context menus.
            /// </summary>
            Reset,

            /// <summary>
            /// An 'Add Tab' menu item.
            /// </summary>
            AddTab,

            /// <summary>
            /// A 'Remove Tab (Item)' menu item.
            /// </summary>
            Remove,

            /// <summary>
            /// A 'Rename Tab (Item)' menu item.
            /// </summary>
            Rename,

            /// <summary>
            /// A 'Move Up' menu item.
            /// </summary>
            MoveUp,

            /// <summary>
            /// A 'Move Down' menu item.
            /// </summary>
            MoveDown,

            /// <summary>
            /// A 'Cut' menu item.
            /// </summary>
            Cut,

            /// <summary>
            /// A 'Copy' menu item.
            /// </summary>
            Copy,

            /// <summary>
            /// A 'Paste' menu item.
            /// </summary>
            Paste
        }

        #endregion
    }
}