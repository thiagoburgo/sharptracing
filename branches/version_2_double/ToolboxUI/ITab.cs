using System.Drawing;

namespace TooboxUI.Components
{
    /// <summary>
    /// Represents an object that can contain <see cref="Toolbox.Tab">tabs</see> and <see cref="Toolbox.Item">items</see>.
    /// </summary>
    public interface ITab
    {
        /// <summary>
        /// Gets the <see cref="ITab">owner</see> of the current object.
        /// </summary>
        ITab Owner { get; }
        /// <summary>
        /// Gets the visible <see cref="Rectangle">rectangle</see> of the object on the <see cref="Toolbox"/>.
        /// </summary>
        Rectangle VisibleRectangle { get; }
        /// <summary>
        /// Gets the height of the object on the <see cref="Toolbox"/>.
        /// </summary>
        int Height { get; }
        /// <summary>
        /// Gets the width of the object on the <see cref="Toolbox"/>.
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Gets the collection of <see cref="Toolbox.Item">items</see>.
        /// </summary>
        Toolbox.ItemCollection Items { get; }
        /// <summary>
        /// Gets the collection of <see cref="Toolbox.Tab">categories</see>.
        /// </summary>
        Toolbox.TabCollection Categories { get; }
        /// <summary>
        /// Invalidates the object on the <see cref="Toolbox"/>.
        /// </summary>
        void Invalidate();
    }
}