using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

namespace TooboxUI.Components
{
    partial class HostToolbox
    {
        #region Nested type: TBService
        /// <summary>
        /// Provides an implementation of the <see cref="IToolboxService"/> based on the Microsoft's <see cref="ToolboxService"/>.
        /// </summary>
        public class TBService : ToolboxService
        {
            private static Type _itemType = typeof(Item);
            private HostToolbox _toolbox;
            /// <summary>
            /// Initializes a new instance of the <see cref="TBService"/> with the specified <paramref name="toolbox"/> reference.
            /// </summary>
            /// <param name="toolbox">A <see cref="HostToolbox"/> component to associate.</param>
            public TBService(HostToolbox toolbox)
            {
                this._toolbox = toolbox;
            }
            /// <summary>
            /// Gets a collection of strings depicting available categories of the toolbox.
            /// </summary>
            protected override CategoryNameCollection CategoryNames
            {
                get
                {
                    List<string> names = new List<string>();
                    foreach(Tab tab in this._toolbox.Categories){
                        if(tab.Visible){
                            names.Add(tab.Text);
                        }
                    }
                    return new CategoryNameCollection(names.ToArray());
                }
            }
            /// <summary>
            /// Gets or sets the name of the currently selected category.
            /// </summary>
            protected override string SelectedCategory
            {
                get { return this._toolbox.SelectedCategory; }
                set { this._toolbox.SelectedCategory = value; }
            }
            /// <summary>
            /// Gets or sets the currently selected item container.
            /// </summary>
            protected override ToolboxItemContainer SelectedItemContainer
            {
                get
                {
                    ToolboxItem item = this._toolbox.GetSelectedToolboxItem();
                    if(item == null){
                        return null;
                    }
                    return new ToolboxItemContainer(item);
                }
                set { this._toolbox.SetSelectedToolboxItem((value == null) ? null : value.GetToolboxItem(null)); }
            }
            /// <summary>
            /// Returns an <see cref="IList"/> containing all items in a given category.
            /// </summary>
            /// <param name="categoryName">The category for which to retrieve the item container list.</param>
            /// <returns>An <see cref="IList"/> containing all items in the category specified by <paramref name="categoryName"/>.</returns>
            protected override IList GetItemContainers(string categoryName)
            {
                Tab tab = this._toolbox.Categories[categoryName];
                if(tab == null){
                    throw new ArgumentException("categoryName");
                }
                return new ToolboxItemList(tab);
            }
            /// <summary>
            /// Returns an <see cref="IList"/> containing all items on the toolbox.
            /// </summary>
            /// <returns>An <see cref="IList"/> containing all items on the toolbox.</returns>
            protected override IList GetItemContainers()
            {
                return new ToolboxItemList(this._toolbox);
            }
            /// <summary>
            /// Refreshes the state of the toolbox items.
            /// </summary>
            protected override void Refresh()
            {
                this._toolbox.Refresh();
            }
            /// <summary>
            /// Sets the current application's cursor to a cursor that represents the currently selected tool.
            /// </summary>
            /// <returns><b>true</b> if there is an item selected; otherwise, <b>false</b>.</returns>
            protected override bool SetCursor()
            {
                return this._toolbox.SetCursor();
            }
            /// <summary>
            /// Creates a new toolbox item container from a saved data object.
            /// </summary>
            /// <param name="dataObject">A data object containing saved toolbox data.</param>
            /// <returns>A new toolbox item container.</returns>
            protected override ToolboxItemContainer CreateItemContainer(IDataObject dataObject)
            {
                HostItem item = this._toolbox.GetDragDropTool(dataObject) as HostItem;
                if(item != null){
                    ToolboxItemContainer container = this.CreateItemContainer(item.ToolboxItem, null);
                    return container;
                }
                return base.CreateItemContainer(dataObject);
            }
            /// <summary>
            /// Returns a value indicating whether the given data object represents an item container.
            /// </summary>
            /// <param name="dataObject">The data object to examine for the presence of a toolbox item container.</param>
            /// <param name="host">An optional designer host. This parameter can be a null reference.</param>
            /// <returns><b>true</b> if the given data object represents an item container; otherwise, <b>false</b>.</returns>
            protected override bool IsItemContainer(IDataObject dataObject, IDesignerHost host)
            {
                return base.IsItemContainer(dataObject, host);
            }
            /// <summary>
            /// Receives a call from the toolbox service when a user reports that a selected toolbox item has been used.
            /// </summary>
            protected override void SelectedItemContainerUsed()
            {
                if((ModifierKeys & Keys.Control) != Keys.None){
                    return;
                }
                base.SelectedItemContainerUsed();
            }
        }
        #endregion
    }
}