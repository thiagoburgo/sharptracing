namespace TooboxUI.Components
{
    /// <summary>
    /// Identifies an <see cref="Toolbox.Item"/> or a <see cref="Toolbox.Tab"/> object withing <see cref="Toolbox"/> class.
    /// </summary>
    public interface IToolboxObject
    {
        /// <summary>
        /// Gets or sets the text to display for the <see cref="IToolboxObject"/>.
        /// </summary>
        string Text { get; set; }
        /// <summary>
        /// Indicates whether the <see cref="IToolboxObject"/> is selected on the <see cref="Toolbox"/>.
        /// </summary>
        bool Selected { get; }
        /// <summary>
        /// Indicates whether the <see cref="IToolboxObject"/> is visible on the <see cref="Toolbox"/>.
        /// </summary>
        bool Visible { get; }
        /// <summary>
        /// Gets the <see cref="ITab">owner</see> of the object.
        /// </summary>
        ITab Owner { get; }
        /// <summary>
        /// Ensures that the <see cref="IToolboxObject"/> is visible within the <see cref="Toolbox"/>, scrolling the contents of the <see cref="Toolbox"/> if necessary.
        /// </summary>
        void EnsureVisible();
        /// <summary>
        /// Selects the <see cref="IToolboxObject"/> on the <see cref="Toolbox"/>.
        /// </summary>
        void Select();
    }
}