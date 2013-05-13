namespace TooboxUI.Components {
    partial class Toolbox {
        #region HitArea enum

        /// <summary>
        /// Defines constants that represent areas in a <see cref="Toolbox"/> control. 
        /// </summary>
        public enum HitArea {
            /// <summary>
            /// The specified point is either not on the Toolbox control, or it is in an inactive portion of the control.
            /// </summary>
            None,

            /// <summary>
            /// The specified point is on the <see cref="Item"/> object.
            /// </summary>
            Item,

            /// <summary>
            /// The specified point is on the <see cref="Tab"/> header.
            /// </summary>
            TabHeader,

            /// <summary>
            /// The specified point is withing the body of the <see cref="Tab"/> object but not on the <see cref="Item"/>.
            /// </summary>
            TabBody,

            /// <summary>
            /// The specified point is part of the toolbox's background.
            /// </summary>
            Background
        }

        #endregion
    }
}