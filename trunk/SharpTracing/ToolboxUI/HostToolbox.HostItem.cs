using System;
using System.Drawing;
using System.Drawing.Design;

namespace TooboxUI.Components {
    partial class HostToolbox {
        #region Nested type: HostItem

        /// <summary>
        /// Represents an <see cref="Toolbox.Item"/> associated with the <see cref="ToolboxItem"/> object.
        /// </summary>
        [Serializable]
        public class HostItem : Item {
            #region Fields

            private readonly ToolboxItem _item;
            private readonly bool _projectSpecific;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="HostItem"/> using the specified <see cref="ToolboxItem"/> object.
            /// </summary>
            /// <param name="item">A <see cref="ToolboxItem"/> object to associate with an item.</param>
            public HostItem(ToolboxItem item) {
                this._item = item;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="HostItem"/> using the specified <see cref="ToolboxItem"/> object and a value indicating whether this item is specific for the project/design surface.
            /// </summary>
            /// <param name="item">A <see cref="ToolboxItem"/> object to associate with an item.</param>
            /// <param name="projectSpecific"></param>
            public HostItem(ToolboxItem item, bool projectSpecific) : this(item) {
                this._projectSpecific = projectSpecific;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets or sets the text value of the <see cref="HostItem"/>.
            /// </summary>
            public override string Text {
                get { return this._item.DisplayName; }
                set { this._item.DisplayName = value; }
            }

            /// <summary>
            /// Gets or sets the image displayed for the <see cref="HostItem"/>.
            /// </summary>
            public override Image Image {
                get { return this._item.Bitmap; }
                set { this._item.Bitmap = new Bitmap(value); }
            }

            /// <summary>
            /// Gets or set the tooltip text of the <see cref="HostItem"/>.
            /// </summary>
            public override string Tooltip {
                get { return this._item.Description; }
                set { this._item.Description = value; }
            }

            /// <summary>
            /// Gets a <see cref="ToolboxItem"/> associated with the <see cref="HostItem"/>.
            /// </summary>
            public ToolboxItem ToolboxItem {
                get { return this._item; }
            }

            /// <summary>
            /// Indicates whether the <see cref="ToolboxItem"/> is specific for the project/design surface.
            /// </summary>
            public bool ProjectSpecific {
                get { return this._projectSpecific; }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Creates an identical copy of the <see cref="HostItem"/>.
            /// </summary>
            /// <returns>An object that represents an <see cref="HostItem"/> that has the same <see cref="ToolboxItem"/>, forecolor and backcolor associated with it as the cloned item.</returns>
            public override Item Clone() {
                HostItem hostItem = new HostItem(this._item, this._projectSpecific);
                hostItem.BackColor = this.BackColor;
                hostItem.ForeColor = this.ForeColor;
                return hostItem;
            }

            #endregion
        }

        #endregion
    }
}