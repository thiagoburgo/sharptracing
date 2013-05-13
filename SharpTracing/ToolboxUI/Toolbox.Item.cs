using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: Item

        /// <summary>
        /// Represents an item in a <see cref="Toolbox"/> control.
        /// </summary>
        [Serializable, ToolboxItem(false), DesignTimeVisible(false), TypeConverter(typeof (ItemConverter))]
        public class Item : IToolboxObject, ICloneable {
            #region Fields

            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Color _backColor = Color.LightGray;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _enabled = true;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Color _foreColor = Color.White;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _highlighted = false;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Image _image = null;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Point _location;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private ITab _owner;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _renaming = false;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _selected = false;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private string _text = string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private string _tooltip = string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Color _transparentColor = Color.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _visible = true;

            #endregion

            #region Static members

            internal static readonly StringFormat _itemTextFormat;

            static Item() {
                _itemTextFormat = new StringFormat();
                _itemTextFormat.LineAlignment = StringAlignment.Center;
                _itemTextFormat.Trimming = StringTrimming.EllipsisWord;
            }

            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class with default values.
            /// </summary>
            public Item() : this(string.Empty) {}

            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class with the specified item text.
            /// </summary>
            /// <param name="text">The text to display for the item.</param>
            public Item(string text) : this(text, null) {}

            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class with the specified item text and image.
            /// </summary>
            /// <param name="text">The text to display for the item.</param>
            /// <param name="image">The image to display for the item.</param>
            public Item(string text, Image image) : this(text, image, Color.Empty) {}

            /// <summary>
            /// Initializes a new instance of the <see cref="Item"/> class with the specified item text, image and image transparency color.
            /// </summary>
            /// <param name="text">The text to display for the item.</param>
            /// <param name="image">The image to display for the item.</param>
            /// <param name="transparentColor">The transparent color for the <paramref name="image"/>.</param>
            public Item(string text, Image image, Color transparentColor) {
                if (string.IsNullOrEmpty(text)) {
                    text = "Item";
                }
                this._text = text;
                this._image = image;
                this._transparentColor = transparentColor;
            }

            #region Public Properties

            /// <summary>
            /// Gets or sets the image displayed for the <see cref="Item"/>.
            /// </summary>
            [DefaultValue(typeof (Image), "null")]
            public virtual Image Image {
                [DebuggerStepThrough] get { return this._image; }
                [DebuggerStepThrough] set { this._image = value; }
            }

            /// <summary>
            /// Gets or sets the color to treat as transparent for the <see cref="Image"/>.
            /// </summary>
            /// <value>One of the <see cref="Color"/> values. The default is <b>null</b>.</value>
            /// <remarks>The transparent color is not rendered when the image is drawn.</remarks>
            public virtual Color TransparentColor {
                [DebuggerStepThrough] get { return this._transparentColor; }
                [DebuggerStepThrough] set { this._transparentColor = value; }
            }

            /// <summary>
            /// Gets or sets the color to display the text of the <see cref="Item"/>.
            /// </summary>
            [DefaultValue(typeof (Color), "White")]
            public virtual Color ForeColor {
                [DebuggerStepThrough] get { return this._foreColor; }
                [DebuggerStepThrough] set { this._foreColor = value; }
            }

            /// <summary>
            /// Gets or sets the color of the <see cref="Item"/> background.
            /// </summary>
            [DefaultValue(typeof (Color), "LightGray")]
            public virtual Color BackColor {
                [DebuggerStepThrough] get { return this._backColor; }
                [DebuggerStepThrough] set { this._backColor = value; }
            }

            /// <summary>
            /// Gets or sets whether the <see cref="Item"/> is selectable.
            /// </summary>
            [DefaultValue(true)]
            public virtual bool Enabled {
                [DebuggerStepThrough] get { return this._enabled; }
                [DebuggerStepThrough] set { this._enabled = value; }
            }

            /// <summary>
            /// Gets or sets the tooltip text of the <see cref="Item"/>.
            /// </summary>
            [DefaultValue("")]
            public virtual string Tooltip {
#if DEBUG
                get { return _text; }
#else
                [DebuggerStepThrough]
                get {
                    if (string.IsNullOrEmpty(this._tooltip)) {
                        this._tooltip = this._text;
                    }
                    return this._tooltip;
                }
#endif
                [DebuggerStepThrough]
                set {
                    if (value != null) {
                        this._tooltip = value;
                    }
                }
            }

            /// <summary>
            /// Gets the value indicating whether the <see cref="Item"/> is currently highlighted.
            /// </summary>
            [Browsable(false), DefaultValue(false)]
            public virtual bool Highlighted {
                [DebuggerStepThrough] get { return this._highlighted; }
                internal set {
                    if (this._highlighted != value && this.Toolbox != null) {
                        this._highlighted = value;
                        this.Toolbox.Invalidate(this.Bounds);
                    }
                }
            }

            /// <summary>
            /// Gets the <see cref="Tab">Category</see> that contains the <see cref="Item"/>.
            /// </summary>
            [Browsable(false)]
            internal Tab Category {
                [DebuggerStepThrough]
                get {
                    Toolbox toolbox = this._owner as Toolbox;
                    if (toolbox != null) {
                        if (toolbox.AllowToolboxItems) {
                            return null;
                        } else {
                            return toolbox.GetToolboxItemsTab(true);
                        }
                    }
                    return this._owner as Tab;
                }
            }

            /// <summary>
            /// Gets the <see cref="Toolbox"/> that contains the <see cref="Item"/>.
            /// </summary>
            [Browsable(false)]
            public Toolbox Toolbox {
                get {
                    Tab tab = this._owner as Tab;
                    if (tab != null) {
                        return tab.Toolbox;
                    }
                    return (Toolbox) this._owner;
                }
            }

            /// <summary>
            /// Gets or sets the text value of the <see cref="Item"/>.
            /// </summary>
            public virtual string Text {
                [DebuggerStepThrough] get { return this._text; }
                [DebuggerStepThrough]
                set {
                    if (this._text != value && !string.IsNullOrEmpty(value)) {
                        this._text = value;
                    }
                }
            }

            /// <summary>
            /// Gets or sets whether the <see cref="Item"/> is visible on the <see cref="Toolbox"/>
            /// </summary>
            [DefaultValue(true)]
            public virtual bool Visible {
                [DebuggerStepThrough] get { return this._visible; }
                [DebuggerStepThrough] set { this._visible = value; }
            }

            /// <summary>
            /// Gets the value indicating whether the <see cref="Item"/> is currently selected.
            /// </summary>
            [Browsable(false), DefaultValue(false)]
            public virtual bool Selected {
                [DebuggerStepThrough] get { return this._selected; }
                internal set {
                    if (this._selected != value && this.Toolbox != null) {
                        this._selected = value;
                        this.Toolbox.Invalidate(this.Bounds);
                    }
                }
            }

            /// <summary>
            /// Gets the <see cref="ITab">current</see> object that contains the <see cref="Item"/>.
            /// </summary>
            [Browsable(false)]
            public ITab Owner {
                [DebuggerStepThrough] get { return this._owner; }
                [DebuggerStepThrough] internal set { this._owner = value; }
            }

            #endregion

            #region Other Properties

            /// <summary>
            /// Gets the location of the left top point where to draw the <see cref="Item"/> on the <see cref="Toolbox"/>.
            /// </summary>
            protected internal Point Location {
                [DebuggerStepThrough] get { return this._location; }
                [DebuggerStepThrough] private set { this._location = value; }
            }

            /// <summary>
            /// Gets the width of the <see cref="Item"/> on the <see cref="Toolbox"/>.
            /// </summary>
            protected int Width {
                get {
                    if (this.Owner == null) {
                        return 0;
                    }
                    Toolbox toolbox = this.Toolbox;
                    if (toolbox == null) {
                        return 0;
                    }
                    int width = this.Owner.Width;
                    if (this.Owner is Toolbox) {
                        width = this.Owner.VisibleRectangle.Width - 2 * Gap_TabBorder;
                    } else if (toolbox.DrawTabLevel && (!(this.Owner is Toolbox) || !toolbox.AllowToolboxItems)) {
                        width -= Gap_TabLevel;
                    }
                    return width;
                }
            }

            /// <summary>
            /// Gets the bounds of the <see cref="Item"/> on the <see cref="Toolbox"/>.
            /// </summary>
            protected internal virtual Rectangle Bounds {
                get {
                    Toolbox toolbox = this.Toolbox;
                    if (toolbox == null) {
                        return Rectangle.Empty;
                    }
                    return new Rectangle(this.Location, new Size(this.Width, toolbox.ItemHeight));
                }
            }

            /// <summary>
            /// Gets or sets the value indicating whether the <see cref="Item"/> is currently renamed.
            /// </summary>
            protected internal bool Renaming {
                [DebuggerStepThrough] get { return this._renaming; }
                set {
                    if (this.Toolbox != null) {
                        this._renaming = value;
                        this.Toolbox.Invalidate(this.Bounds);
                    }
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Selects the <see cref="Item"/> on the <see cref="Toolbox"/>.
            /// </summary>
            public void Select() {
                if (this.Toolbox != null && this.Visible && this.Enabled) {
                    this.EnsureVisible();
                    this.Toolbox.SelectTool(this);
                }
            }

            /// <summary>
            /// Ensures that the <see cref="Item"/> is visible within the <see cref="Toolbox"/>, scrolling the contents of the <see cref="Toolbox"/> if necessary and opening the container <see cref="Tab">Category</see>.
            /// </summary>
            public void EnsureVisible() {
                if (this.Toolbox != null && this.Visible) {
                    if (this.Category != null) {
                        this.Category.Opened = true;
                    }
                    this.Toolbox.EnsureVisible(this.Bounds);
                }
            }

            /// <summary>
            /// Returns a string representation of the <see cref="Item"/>.
            /// </summary>
            /// <returns>A string that states an <see cref="Item"/> type and the its text.</returns>
            public override string ToString() {
                return string.Format("{0}: {1}", this.GetType().Name, this.Text);
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// Invoked when the mouse button is pressed on the <see cref="Item"/>.
            /// </summary>
            protected virtual void OnMouseDown(MouseEventArgs e) {}

            /// <summary>
            /// Invoked when the mouse button is depressed on the <see cref="Item"/>. 
            /// </summary>
            /// <remarks>
            /// If the mouse button is <see cref="MouseButtons.Right"/> the <see cref="IP.Components.Toolbox.ItemMenu"/> is shown if available.
            /// </remarks>
            protected virtual void OnMouseUp(MouseEventArgs e) {
                if (e.Button == MouseButtons.Right) {
                    if (this.Toolbox.ItemMenu != null) {
                        this.Toolbox.ItemMenu.Tag = this;
                        this.Toolbox.ItemMenu.Show(this.Toolbox, e.Location);
                    }
                }
            }

            /// <summary>
            /// Invoked when the user clicks on the <see cref="Item"/>.
            /// </summary>
            protected virtual void OnClick(EventArgs e) {}

            /// <summary>
            /// Invoked when the user double clicks on the <see cref="Item"/>.
            /// </summary>
            protected virtual void OnDoubleClick(EventArgs e) {}

            /// <summary>
            /// Draws the <see cref="Item"/> on the <see cref="Toolbox"/>.
            /// </summary>
            /// <param name="e">A <see cref="PaintEventArgs"/> that contains the paint event data.</param>
            protected virtual void OnPaint(PaintEventArgs e) {
                bool setOwnClip = false;
                Region oldClip = e.Graphics.Clip;
                try {
                    if (e.ClipRectangle.Contains(this.Bounds)) {
                        setOwnClip = true;
                        e.Graphics.Clip = new Region(this.Bounds);
                    }
                    GraphicsPath itemPath = this.GetItemPath();
                    RectangleF itemRect = itemPath.GetBounds();
                    Rectangle itemCorrectRect = Rectangle.Round(itemRect);
                    if (this.Enabled) {
                        // Fill item
                        if (this.Selected && this.Highlighted) {
                            using (Brush selHighBrush = new SolidBrush(this.Toolbox.ItemSelectHoverColor)) {
                                e.Graphics.FillRectangle(selHighBrush, itemRect);
                            }
                        } else if (this.Selected) {
                            using (Brush selBrush = new SolidBrush(this.Toolbox.ItemSelectColor)) {
                                e.Graphics.FillRectangle(selBrush, itemRect);
                            }
                        } else if (this.Highlighted) {
                            using (Brush hoverBrush = new SolidBrush(this.Toolbox.ItemHoverColor)) {
                                e.Graphics.FillRectangle(hoverBrush, itemRect);
                            }
                        }
                        // Draw border
                        if (this.Selected || this.Highlighted) {
                            using (Pen selPen = new Pen(this.Toolbox.ItemSelectBorderColor)) {
                                e.Graphics.DrawRectangle(selPen, itemRect.Left, itemRect.Top, itemRect.Width,
                                                         itemRect.Height);
                            }
                        }
                    }
                    // Draw icon
                    Rectangle imageRectangle = Rectangle.Empty;
                    if (this.Image != null && this.Toolbox.ShowIcons) {
                        Size size = new Size(this.Image.Width, this.Toolbox.ItemHeight);
                        imageRectangle =
                            new Rectangle(new Point(itemCorrectRect.Left + Gap_IconFromText, itemCorrectRect.Top), size);
                        if (imageRectangle.Height > this.Image.Height || imageRectangle.Width > this.Image.Width) {
                            imageRectangle.Offset((int) (imageRectangle.Width - this.Image.Width) / 2,
                                                  (int) (imageRectangle.Height - this.Image.Height) / 2);
                        }
                        using (Bitmap image = new Bitmap(this.Image)) {
                            if (this.TransparentColor != Color.Empty) {
                                image.MakeTransparent(this.TransparentColor);
                            }
                            if (this.Enabled) {
                                e.Graphics.DrawImageUnscaled(image, imageRectangle);
                            } else {
                                ControlPaint.DrawImageDisabled(e.Graphics, image, imageRectangle.Left,
                                                               imageRectangle.Top, this.Toolbox.BackColorGradientStart);
                            }
                        }
                    }
                    // Write text
                    Rectangle itemTextRect = itemCorrectRect;
                    if (this.Toolbox.ShowIcons) {
                        itemTextRect.Offset(imageRectangle.Width + 2 * Gap_IconFromText, 0);
                        itemTextRect.Width -= (imageRectangle.Width + 2 * Gap_IconFromText);
                    }
                    Brush textBrush = new SolidBrush(this.Toolbox.ForeColor);
                    if (this.Enabled) {
                        e.Graphics.DrawString(this.Text, this.Toolbox.Font, textBrush, itemTextRect, _itemTextFormat);
                    } else {
                        ControlPaint.DrawStringDisabled(e.Graphics, this.Text, this.Toolbox.Font, this.Toolbox.BackColor,
                                                        itemTextRect, _itemTextFormat);
                    }
                } finally {
                    if (setOwnClip) {
                        e.Graphics.Clip = oldClip;
                    }
                }
            }

            /// <summary>
            /// Returns the <see cref="GraphicsPath"/> structure that represents the <see cref="Item"/> drawing region.
            /// </summary>
            protected virtual GraphicsPath GetItemPath() {
                Rectangle rect = this.Bounds;
                rect.Width -= 1;
                rect.Height -= 1;
                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(rect);
                return path;
            }

            #endregion

            #region Internal Methods

            internal void InternalPaint(PaintEventArgs e, Point location) {
                this.Location = location;
                this.OnPaint(e);
            }

            internal void InternalMouseDown(MouseEventArgs e) {
                this.OnMouseDown(e);
            }

            internal void InternalMouseUp(MouseEventArgs e) {
                this.OnMouseUp(e);
            }

            internal void InternalClick(EventArgs e) {
                this.OnClick(e);
            }

            internal void InternalDoubleClick(EventArgs e) {
                this.OnDoubleClick(e);
            }

            /// <summary>
            /// Returns a bounds <see cref="Rectangle"/> of the <see cref="Item"/>.
            /// </summary>
            /// <param name="includeImage">Indicates whether to include image rectangle into returned bounds.</param>
            protected internal Rectangle GetBounds(bool includeImage) {
                Rectangle rect = this.Bounds;
                if (!includeImage && this.Image != null && this.Toolbox.ShowIcons) {
                    int offset = this.Image.Width + 2 * Gap_IconFromText;
                    rect.X += offset;
                    rect.Width -= offset;
                }
                return rect;
            }

            #endregion

            #region Private Methods

            private bool ShouldSerializeImage() {
                if (this.Image == null) {
                    return false;
                }
                return true;
            }

            private bool ShouldSerializeTransparentColor() {
                if (this.TransparentColor == Color.Empty) {
                    return false;
                }
                return true;
            }

            private bool ShouldSerializeTab() {
                return false;
            }

            private void ResetImage() {
                this.Image = null;
            }

            #endregion

            #region ICloneable Members

            /// <summary>
            /// Creates a new object that is a copy of the current instance.
            /// </summary>
            object ICloneable.Clone() {
                return this.Clone();
            }

            #endregion

            /// <summary>
            /// Creates an identical copy of the <see cref="Item"/>.
            /// </summary>
            /// <returns>An object that represents an <see cref="Item"/> that has the same text, image, forecolor and backcolor associated with it as the cloned item.</returns>
            public virtual Item Clone() {
                Item item = new Item();
                item._text = this._text;
                item._image = this._image;
                item._foreColor = this._foreColor;
                item._backColor = this._backColor;
                return item;
            }
        }

        #endregion
    }
}