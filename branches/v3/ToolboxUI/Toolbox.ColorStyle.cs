namespace TooboxUI.Components {
    partial class Toolbox {
        #region ColorStyle enum

        /// <summary>
        /// Defines constants that represent drawing style of <see cref="Tab"/> objects on the <see cref="Toolbox"/>.
        /// </summary>
        /// <seealso cref="Toolbox.BackColorGradientStart"/>
        /// <seealso cref="Toolbox.BackColorGradientEnd"/>
        public enum ColorStyle {
            /// <summary>
            /// Draw background using the standard colors
            /// </summary>
            Standard = 0,

            /// <summary>
            /// Draw gradient background using lightened colors of the <see cref="Toolbox"/> background.
            /// </summary>
            Lighter = 1,

            /// <summary>
            /// Draw gradient background using darkened colors of the <see cref="Toolbox"/> background.
            /// </summary>
            Darker = 2,

            /// <summary>
            /// Do not draw background
            /// </summary>
            None = 3,
        }

        #endregion
    }
}