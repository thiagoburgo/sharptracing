namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: ToolboxItemsTab

        internal class ToolboxItemsTab : Tab {
            private readonly ItemCollection _items;

            public ToolboxItemsTab(ItemCollection items) {
                this._items = items;
                this.Text = "Toolbox Items";
                this.AllowDelete = false;
                this.Owner = items.Owner;
            }

            public override ItemCollection Items {
                get { return this._items; }
            }

            public override bool Visible {
                get {
                    foreach (Item item in this.Items) {
                        if (item.Visible) {
                            return true;
                        }
                    }
                    return false;
                }
                set { }
            }
        }

        #endregion
    }
}