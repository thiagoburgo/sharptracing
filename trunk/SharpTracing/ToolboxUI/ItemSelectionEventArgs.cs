using System;
using System.Diagnostics;

namespace TooboxUI.Components {
    /// <summary>
    /// Provides data for the <see cref="Toolbox.SelectItem"/> event.
    /// </summary>
    public class ItemSelectionEventArgs : EventArgs {
        private readonly Toolbox.Item _item;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSelectionEventArgs"/> class using the specified <see cref="Toolbox.Item">item</see> object.
        /// </summary>
        /// <param name="item">The <see cref="Toolbox.Item">item</see> that was selected.</param>
        public ItemSelectionEventArgs(Toolbox.Item item) {
            if (item == null) {
                throw new ArgumentNullException("item");
            }
            this._item = item;
        }

        /// <summary>
        /// Gets the <see cref="Toolbox.Item">item</see> that was selected.
        /// </summary>
        public Toolbox.Item Item {
            [DebuggerStepThrough] get { return this._item; }
        }
    }
}