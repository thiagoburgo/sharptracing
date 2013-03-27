using System;
using System.Diagnostics;

namespace TooboxUI.Components {
    /// <summary>
    /// Provides data for the <see cref="Toolbox.Tab.Toggle">Toggle</see> and <see cref="Toolbox.Tab.SelectedChanged">SelectedChanged</see> events on the <see cref="Toolbox.Tab"/>.
    /// </summary>
    public class TabEventArgs : EventArgs {
        private readonly Toolbox.Tab _tab;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabEventArgs"/> class.
        /// </summary>
        /// <param name="tab">A <see cref="Toolbox.Tab">Tab</see> object on which an event is raised.</param>
        public TabEventArgs(Toolbox.Tab tab) {
            if (tab == null) {
                throw new ArgumentNullException("tab");
            }
            this._tab = tab;
        }

        /// <summary>
        /// Gets a <see cref="Toolbox.Tab">Tab</see> object on which an event is raised.
        /// </summary>
        public Toolbox.Tab Tab {
            [DebuggerStepThrough] get { return this._tab; }
        }
    }
}