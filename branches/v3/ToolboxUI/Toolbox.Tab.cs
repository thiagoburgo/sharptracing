using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components {
    partial class Toolbox {
        #region Nested type: Tab

        /// <summary>
        /// Represents a category in a <see cref="Toolbox"/> control.
        /// </summary>
        [Serializable, ToolboxItem(false), DesignTimeVisible(false), TypeConverter(typeof (TabConverter))]
        public class Tab : IToolboxObject, ITab {
            #region Fields

            private static readonly Bitmap _collapsed;
            private static readonly Bitmap _expanded;
            private static readonly Bitmap _pointer;
            private static readonly StringFormat _tabCaptionFormat;
            private static readonly StringFormat _tabEmptyTextFormat;

            /// <summary>
            /// Represents the readonly Pointer <see cref="Item"/> of the <see cref="Tab"/>.
            /// </summary>
            public readonly Item PointerItem;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _allowDelete = true;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private int _height;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly ItemCollection _items;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private Point _location;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _opened;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private ITab _owner;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _renaming = false;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _selected;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private readonly TabCollection _tabs;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private string _text;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)] private bool _visible = true;

            /// <summary>
            /// Occurs when the <see cref="Tab"/> is opened or closed on the <see cref="Toolbox"/>.
            /// </summary>
            public event EventHandler<TabEventArgs> Toggle;

            /// <summary>
            /// Occurs when the <see cref="Tab"/> is selected/deselected on the <see cref="Toolbox"/>.
            /// </summary>
            public event EventHandler<TabEventArgs> SelectedChanged;

            #endregion

            #region Static constructor

            [DebuggerHidden]
            static Tab() {
                try {
                    _collapsed = new Bitmap(Resources.Collapsed);
                    _collapsed.MakeTransparent(_collapsed.GetPixel(0, 0));
                } catch {}
                try {
                    _expanded = new Bitmap(Resources.Expanded);
                    _expanded.MakeTransparent(_expanded.GetPixel(0, 0));
                } catch {}
                try {
                    _pointer = new Bitmap(Resources.PointerBlack);
                    _pointer.MakeTransparent(_pointer.GetPixel(0, 0));
                } catch {}
                _tabCaptionFormat = new StringFormat();
                _tabCaptionFormat.FormatFlags = StringFormatFlags.NoWrap;
                _tabCaptionFormat.Trimming = StringTrimming.EllipsisWord;
                _tabEmptyTextFormat = new StringFormat();
                _tabEmptyTextFormat.Alignment = StringAlignment.Center;
                _tabEmptyTextFormat.LineAlignment = StringAlignment.Center;
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="Tab"/> class with the default values.
            /// </summary>
            public Tab() : this("") {}

            /// <summary>
            /// Initializes a new instance of the <see cref="Tab"/> class with the specified category text.
            /// </summary>
            /// <param name="text">The text to display for the tab.</param>
            public Tab(string text) {
                if (string.IsNullOrEmpty(text)) {
                    text = Resources.ToolboxTabDefaultName;
                }
                this._text = text;
                this._items = new ItemCollection(this);
                this._tabs = new TabCollection(this);
                this.PointerItem = this.CreatePointerItem(Resources.ToolboxPointerItem);
                this.PointerItem.Owner = this;
                this.PointerItem.Image = _pointer;
                this.PointerItem.Enabled = false;
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Gets the <see cref="Toolbox"/> object that contains the <see cref="Tab"/>.
            /// </summary>
            [Browsable(false)]
            public Toolbox Toolbox {
                [DebuggerStepThrough]
                get {
                    ITab owner = this.Owner;
                    while (owner != null && !(owner is Toolbox)) {
                        owner = owner.Owner;
                    }
                    return (Toolbox) owner;
                }
            }

            /// <summary>
            /// Gets or sets the value indicating whether the <see cref="Tab"/> is opened on the <see cref="Toolbox"/>.
            /// </summary>
            [DefaultValue(false)]
            public virtual bool Opened {
                [DebuggerStepThrough] get { return this._opened; }
                set {
                    if (this._opened != value) {
                        this._opened = value;
                        this.OnToggleTab();
                        if (this.Owner != null) {
                            this.Owner.Invalidate();
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the <see cref="Tab"/> is allowed to remove from the <see cref="Toolbox"/>.
            /// </summary>
            [DefaultValue(true)]
            public bool AllowDelete {
                [DebuggerStepThrough] get { return this._allowDelete; }
                [DebuggerStepThrough]
                set {
                    if (this._allowDelete != value) {
                        this._allowDelete = value;
                    }
                }
            }

            /// <summary>
            /// Gets the location of the left top point where to draw the <see cref="Tab"/> on the <see cref="Toolbox"/>.
            /// </summary>
            [Browsable(false)]
            public Point Location {
                [DebuggerStepThrough] get { return this._location; }
            }

            #region ITab Members

            /// <summary>
            /// Gets the bounds of the <see cref="Tab"/> on the <see cref="Toolbox"/>.
            /// </summary>
            [Browsable(false)]
            public virtual Rectangle VisibleRectangle {
                get {
                    Toolbox toolbox = this.Toolbox;
                    if (toolbox == null) {
                        return Rectangle.Empty;
                    }
                    return new Rectangle(this.Location, new Size(this.Width, this.Height));
                }
            }

            #endregion

            #region IToolboxObject Members

            /// <summary>
            /// Gets or sets the text to display for the <see cref="Tab"/>.
            /// </summary>
            public virtual string Text {
                [DebuggerStepThrough] get { return this._text; }
                set {
                    this._text = value;
                    Toolbox toolbox = this.Toolbox;
                    if (toolbox != null) {
                        toolbox.Invalidate(this.GetCaptionRectangle(false));
                    }
                }
            }

            /// <summary>
            /// Gets or sets the value indicating whether the <see cref="Tab"/> is visible on the <see cref="Toolbox"/>.
            /// </summary>
            [DefaultValue(true)]
            public virtual bool Visible {
                [DebuggerStepThrough] get { return this._visible; }
                set {
                    if (this._visible != value) {
                        this._visible = value;
                        if (this.Owner != null) {
                            this.Owner.Invalidate();
                        }
                    }
                }
            }

            /// <summary>
            /// Indicates whether the <see cref="Tab"/> is currently selected on the <see cref="Toolbox"/>.
            /// </summary>
            [Browsable(false), DefaultValue(false)]
            public bool Selected {
                [DebuggerStepThrough] get { return this._selected; }
                internal set {
                    if (this._selected != value) {
                        Toolbox toolbox = this.Toolbox;
                        if (toolbox != null) {
                            this._selected = value;
                            Rectangle invalidate = this.GetCaptionRectangle(true);
                            invalidate.Inflate(1, 1);
                            toolbox.Invalidate(invalidate);
                            this.OnSelectedChanged();
                        }
                    }
                }
            }

            #endregion

            #endregion

            #region Other Properties

            /// <summary>
            /// Gets or sets the value indicating whether the <see cref="Tab"/> is currently renamed.
            /// </summary>
            protected internal bool Renaming {
                [DebuggerStepThrough] get { return this._renaming; }
                set {
                    Toolbox toolbox = this.Toolbox;
                    if (toolbox != null) {
                        this._renaming = value;
                        toolbox.Invalidate(this.GetCaptionRectangle(true));
                    }
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Ensures that the <see cref="Tab"/> is visible within the <see cref="Toolbox"/>, scrolling the contents of the <see cref="Toolbox"/> if necessary.
            /// </summary>
            public void EnsureVisible() {
                Toolbox toolbox = this.Toolbox;
                if (toolbox != null && this.Visible) {
                    toolbox.EnsureVisible(this.VisibleRectangle);
                }
            }

            /// <summary>
            /// Selects the <see cref="Tab"/> on the <see cref="Toolbox"/>.
            /// </summary>
            public void Select() {
                Toolbox toolbox = this.Toolbox;
                if (toolbox != null && this.Visible) {
                    this.EnsureVisible();
                    toolbox.SelectTool(this);
                }
            }

            /// <summary>
            /// Sorts items contained in the <see cref="Tab"/> using the specified order.
            /// </summary>
            /// <param name="order">One of the <see cref="SortOrder"/> values indicating how to sort items.</param>
            public void SortItems(SortOrder order) {
                if (order == SortOrder.None) {
                    return;
                }
                Comparison<Item> compare = delegate(Item first, Item second) {
                                               if (first == second) {
                                                   return 0;
                                               }
                                               if (order == SortOrder.Ascending) {
                                                   return string.Compare(first.Text, second.Text);
                                               } else {
                                                   return string.Compare(second.Text, first.Text);
                                               }
                                           };
                this.Items.Sort(compare);
            }

            /// <summary>
            /// Returns a string representation of the <see cref="Tab"/>.
            /// </summary>
            /// <returns>A string that states an <see cref="Tab"/> type and the its text.</returns>
            public override string ToString() {
                return string.Format("{0}: {1}", this.GetType().Name, this.Text);
            }

            #endregion

            #region Protected Methods

            /// <summary>
            /// Creates a Pointer <see cref="Item"/> for the <see cref="Tab"/>.
            /// </summary>
            /// <param name="text">The default text of the Pointer Item.</param>
            /// <returns>An <see cref="Item"/> object representing the Pointer Item of the <see cref="Tab"/>.</returns>
            protected virtual Item CreatePointerItem(string text) {
                return new Item(text);
            }

            /// <summary>
            /// Notifies the <see cref="Toolbox"/> object that the state of the <see cref="Tab"/> is changed and invalidation is required.
            /// </summary>
            protected internal void NotifyInvalidate() {
                Toolbox toolbox = this.Toolbox;
                if (toolbox != null && this.Opened && this.Visible) {
                    toolbox.Invalidate(this.VisibleRectangle);
                }
            }

            /// <summary>
            /// Returns an object with information on which portion of a toolbox control is at a location specified by a <see cref="Point"/>.
            /// </summary>
            /// <param name="location">A <see cref="Point"/> containing the <see cref="Point.X"/> and <see cref="Point.Y"/> coordinates of the point to be hit tested.</param>
            /// <returns>A <see cref="HitTestInfo"/> that contains information about the specified point on the <see cref="Toolbox"/>.</returns>
            protected internal virtual HitTestInfo HitTest(Point location) {
                Toolbox toolbox = this.Toolbox;
                if (toolbox == null) {
                    return null;
                }
                Rectangle tabRect = this.VisibleRectangle;
                Rectangle captionRect = this.GetCaptionRectangle(true);
                HitTestInfo info = new HitTestInfo(location);
                if (tabRect.Contains(location)) {
                    if (captionRect.Contains(location)) {
                        info.HitArea = HitArea.TabHeader;
                    } else {
                        if (toolbox.AllowNestedTabs) {
                            foreach (Tab tab in this.Categories) {
                                if ((toolbox.ShowAll || tab.Visible) && tab.VisibleRectangle.Contains(location)) {
                                    return tab.HitTest(location);
                                }
                            }
                        }
                        if (toolbox.ShowPointer && (this.PointerItem.Visible || toolbox.ShowAll) &&
                            this.PointerItem.Bounds.Contains(location)) {
                            info.HitArea = HitArea.Item;
                            info.Tool = this.PointerItem;
                        } else {
                            foreach (Item item in this.Items) {
                                if ((item.Visible || toolbox.ShowAll) && item.Bounds.Contains(location)) {
                                    info.HitArea = HitArea.Item;
                                    info.Tool = item;
                                    break;
                                }
                            }
                        }
                    }
                    if (info.HitArea == HitArea.None) {
                        info.HitArea = HitArea.TabBody;
                    }
                    if (info.Tool == null) {
                        info.Tool = this;
                    }
                }
                return info;
            }

            /// <summary>
            /// Returns the <see cref="GraphicsPath"/> structure that represents the <see cref="Tab"/> drawing region.
            /// </summary>
            protected virtual GraphicsPath GetTabPath() {
                Rectangle rect = this.GetCaptionRectangle(true);
                GraphicsPath path = new GraphicsPath();
                path.AddRectangle(rect);
                return path;
            }

            /// <summary>
            /// Draws the <see cref="Tab"/> on the <see cref="Toolbox"/>.
            /// </summary>
            /// <param name="e">A <see cref="PaintEventArgs"/> that contains the paint event data.</param>
            protected virtual void OnPaint(PaintEventArgs e) {
                Toolbox toolbox = this.Toolbox;
                if (toolbox == null) {
                    return;
                }
                GraphicsPath tabPath = this.GetTabPath();
                RectangleF tabRectangle = tabPath.GetBounds();
                // Draw tab
                if (this.Selected) {
                    using (Brush selBrush = new SolidBrush(toolbox.TabSelectColor)) {
                        e.Graphics.FillPath(selBrush, tabPath);
                    }
                    using (Pen selPen = new Pen(toolbox.TabSelectBorderColor)) {
                        e.Graphics.DrawRectangle(selPen, tabRectangle.Left, tabRectangle.Top, tabRectangle.Width - 1,
                                                 tabRectangle.Height - 1);
                    }
                } else if (toolbox.TabColorStyle != ColorStyle.None) {
                    Color backColorStart;
                    Color backColorEnd;
                    switch (toolbox.TabColorStyle) {
                        case ColorStyle.Lighter:
                            backColorStart = ControlPaint.Light(toolbox.BackColorGradientStart, 0.001f);
                            backColorEnd = ControlPaint.Light(toolbox.BackColorGradientEnd, 0.001f);
                            break;
                        case ColorStyle.Darker:
                            backColorStart = ControlPaint.Dark(toolbox.BackColorGradientStart, 0.001f);
                            backColorEnd = ControlPaint.Dark(toolbox.BackColorGradientEnd, 0.001f);
                            break;
                        default:
                            backColorStart = Color.LightGray;
                            backColorEnd = Color.Gray;
                            break;
                    }
                    RectangleF gradientRectangle = tabRectangle;
                    gradientRectangle.Inflate(1, 1);
                    using (
                        Brush unselectedBrush = new LinearGradientBrush(gradientRectangle, backColorStart, backColorEnd,
                                                                        LinearGradientMode.Horizontal)) {
                        e.Graphics.FillPath(unselectedBrush, tabPath);
                    }
                }
                // Draw collapsed/expanded icon
                Point stateIconPosition = Point.Round(tabRectangle.Location);
                stateIconPosition.Y += (int) (tabRectangle.Height - _collapsed.Height) / 2;
                if (this.Opened) {
                    e.Graphics.DrawImageUnscaled(_expanded, stateIconPosition);
                } else {
                    e.Graphics.DrawImageUnscaled(_collapsed, stateIconPosition);
                }
                // Draw tab Text
                Brush textBrush = new SolidBrush(toolbox.ForeColor);
                SizeF textSize = e.Graphics.MeasureString(this.Text, toolbox.TabFont);
                RectangleF textRectangle = new RectangleF(tabRectangle.Left + _expanded.Width + Gap_IconFromText,
                                                          tabRectangle.Top + (tabRectangle.Height - textSize.Height) / 2,
                                                          tabRectangle.Width - _expanded.Width - Gap_IconFromText,
                                                          textSize.Height);
                e.Graphics.DrawString(this.Text, toolbox.TabFont, textBrush, textRectangle, _tabCaptionFormat);
                this._height = (int) tabRectangle.Height;
                // Draw items
                if (this.Opened) {
                    Point location = new Point((int) tabRectangle.Left + ((toolbox.DrawTabLevel) ? Gap_TabLevel : 0),
                                               (int) tabRectangle.Bottom);
                    if (this.Items.Count == 0 && (!toolbox.AllowNestedTabs || this.Categories.Count == 0) &&
                        (!toolbox.ShowAll || !toolbox.ShowPointer)) {
                        // Draw text ToolboxTabNoItemsText
                        // Before and after this text goes empty line with height = Toolbox.ItemHeight
                        string text = Resources.ToolboxTabNoItemsText;
                        textSize = e.Graphics.MeasureString(text, toolbox.Font,
                                                            this.Width - ((toolbox.DrawTabLevel) ? Gap_TabLevel : 0));
                        textRectangle = new RectangleF(location.X, location.Y + Gap_ItemBetween + toolbox.ItemHeight,
                                                       textSize.Width, textSize.Height);
                        e.Graphics.DrawString(text, toolbox.Font, textBrush, textRectangle, _tabEmptyTextFormat);
                        location.Offset(0, (int) (2 * toolbox.ItemHeight + 2 * Gap_ItemBetween + textSize.Height));
                    } else {
                        if (toolbox.AllowNestedTabs && this.Categories.Count > 0) {
                            foreach (Tab tab in this.Categories) {
                                if (tab.Visible || toolbox.ShowAll) {
                                    location.Offset(0, Gap_TabBetween);
                                    tab.InternalPaint(e, ref location);
                                }
                            }
                            location.Offset(0, Gap_TabBetween);
                        }
                        if (toolbox.ShowPointer && (this.Items.Count > 0 || toolbox.ShowAll)) {
                            location.Offset(0, Gap_ItemBetween);
                            this.PointerItem.InternalPaint(e, location);
                            location.Offset(0, toolbox.ItemHeight);
                        }
                        foreach (Item item in this.Items) {
                            if (item.Visible || toolbox.ShowAll) {
                                location.Offset(0, Gap_ItemBetween);
                                item.InternalPaint(e, location);
                                location.Offset(0, toolbox.ItemHeight);
                            }
                        }
                    }
                    this._height += (int) (location.Y - tabRectangle.Bottom);
                    if (toolbox.DrawTabLevel) {
                        using (Pen levelPen = new Pen(Color.Black, 1)) {
                            e.Graphics.DrawLine(levelPen, tabRectangle.Left + Gap_TabLevel / 3f,
                                                tabRectangle.Bottom + Gap_TabBetween,
                                                tabRectangle.Left + Gap_TabLevel / 3f, location.Y - Gap_TabBetween);
                            e.Graphics.DrawLine(levelPen, tabRectangle.Left + Gap_TabLevel / 3f,
                                                location.Y - Gap_TabBetween, tabRectangle.Left + Gap_TabLevel * 2 / 3f,
                                                location.Y - Gap_TabBetween);
                        }
                    }
                }
            }

            /// <summary>
            /// Invoked when the mouse button is pressed on the <see cref="Tab"/>.
            /// </summary>
            protected virtual void OnMouseDown(MouseEventArgs e) {}

            /// <summary>
            /// Invoked when the mouse button is depressed on the <see cref="Tab"/>. 
            /// </summary>
            /// <remarks>
            /// If the mouse button is <see cref="MouseButtons.Left"/> and the mouse location is withing <see cref="Tab"/> caption, the <see cref="Tab"/> state is toggled.
            /// <para/>If the mouse button is <see cref="MouseButtons.Right"/> the <see cref="IP.Components.Toolbox.TabMenu"/> is shown if available.
            /// </remarks>
            protected virtual void OnMouseUp(MouseEventArgs e) {
                Toolbox toolbox = this.Toolbox;
                if (e.Button == MouseButtons.Left) {
                    if (this.GetCaptionRectangle(true).Contains(e.Location)) {
                        this.Opened = !this.Opened;
                        toolbox.Invalidate();
                    }
                } else if (e.Button == MouseButtons.Right) {
                    if (toolbox.TabMenu != null) {
                        toolbox.TabMenu.Tag = this;
                        toolbox.TabMenu.Show(toolbox, e.Location);
                    }
                }
            }

            /// <summary>
            /// Invoked when the user clicks on the <see cref="Tab"/>.
            /// </summary>
            protected virtual void OnClick(EventArgs e) {}

            /// <summary>
            /// Invoked when the user double clicks on the <see cref="Tab"/>.
            /// </summary>
            protected virtual void OnDoubleClick(EventArgs e) {}

            #region Event Fires

            /// <summary>
            /// Raises the <see cref="Toggle"/> event.
            /// </summary>
            protected virtual void OnToggleTab() {
                if (this.Toggle != null) {
                    this.Toggle(this, new TabEventArgs(this));
                }
            }

            /// <summary>
            /// Raises the <see cref="SelectedChanged"/> event.
            /// </summary>
            protected virtual void OnSelectedChanged() {
                if (this.SelectedChanged != null) {
                    this.SelectedChanged(this, new TabEventArgs(this));
                }
            }

            #endregion

            #endregion

            #region Internal Methods

            internal void InternalPaint(PaintEventArgs e, ref Point position) {
                this._location = position;
                this.OnPaint(e);
                position.Offset(0, this.Height);
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
            /// Calculates the height of the <see cref="Tab"/>.
            /// </summary>
            /// <param name="g">A drawing surface.</param>
            /// <param name="visibleRect">A <see cref="Rectangle"/> representing the visible region of the <see cref="Toolbox"/>.</param>
            protected internal virtual int CalculateTabHeight(Graphics g, Rectangle visibleRect) {
                Toolbox toolbox = this.Toolbox;
                if (toolbox == null) {
                    return 0;
                }
                int height = toolbox.TabHeight;
                if (this.Opened) {
                    if (toolbox.DrawTabLevel) {
                        visibleRect.X += Gap_TabLevel;
                        visibleRect.Width -= Gap_TabLevel;
                    }
                    if (this.Items.Count == 0 && (!toolbox.AllowNestedTabs || this.Categories.Count == 0) &&
                        (!toolbox.ShowAll || !toolbox.ShowPointer)) {
                        height += 2 * toolbox.ItemHeight;
                        height += 2 * Gap_ItemBetween;
                        height +=
                            (int)
                            g.MeasureString(Resources.ToolboxTabNoItemsText, toolbox.Font, visibleRect.Width).Height;
                    } else {
                        if (toolbox.AllowNestedTabs && this.Categories.Count > 0) {
                            foreach (Tab tab in this.Categories) {
                                height += tab.CalculateTabHeight(g, visibleRect);
                                height += Gap_TabBetween;
                            }
                            height += Gap_TabBetween;
                        }
                        if (toolbox.ShowPointer && (this.Items.Count > 0 || toolbox.ShowAll)) {
                            height += (toolbox.ItemHeight + Gap_ItemBetween);
                        }
                        foreach (Item item in this.Items) {
                            if (item.Visible || toolbox.ShowAll) {
                                height += (toolbox.ItemHeight + Gap_ItemBetween);
                            }
                        }
                    }
                }
                return height;
            }

            /// <summary>
            /// Returns a <see cref="Rectangle"/> of the <see cref="Tab"/> caption.
            /// </summary>
            /// <param name="includeIcon">Indicates whether to include icon into returned rectangle.</param>
            protected internal Rectangle GetCaptionRectangle(bool includeIcon) {
                Toolbox toolbox = this.Toolbox;
                if (toolbox == null) {
                    return Rectangle.Empty;
                }
                Rectangle caption = new Rectangle(this.Location, new Size(this.Width, toolbox.TabHeight));
                if (!includeIcon) {
                    int offset = _collapsed.Width + Gap_IconFromText;
                    caption.Width -= offset;
                    caption.X += offset;
                }
                return caption;
            }

            #endregion

            #region Private Methods

            #endregion

            #region ITab Members

            /// <summary>
            /// Invalidates the <see cref="Tab.VisibleRectangle">bounds</see> of the <see cref="Tab"/>.
            /// </summary>
            public void Invalidate() {
                ITab parent = this.Owner;
                if (parent is Tab) {
                    ((Tab) parent).NotifyInvalidate();
                } else if (parent != null) {
                    parent.Invalidate();
                }
            }

            /// <summary>
            /// Gets the width of the <see cref="Tab"/> on the <see cref="Toolbox"/>.
            /// </summary>
            [Browsable(false)]
            public int Width {
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
                    } else if (toolbox.DrawTabLevel) {
                        width -= Gap_TabLevel;
                    }
                    return width;
                }
            }

            /// <summary>
            /// Gets the height of the <see cref="Tab"/> on the <see cref="Toolbox"/> taking into account whether the <see cref="Tab"/> is opened and including heights of all the visible items.
            /// </summary>
            [Browsable(false)]
            public int Height {
                [DebuggerStepThrough] get { return this._height; }
            }

            /// <summary>
            /// Gets the <see cref="ItemCollection">collection</see> of the items in the <see cref="Tab"/>.
            /// </summary>
            [MergableProperty(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public virtual ItemCollection Items {
                [DebuggerStepThrough] get { return this._items; }
            }

            /// <summary>
            /// Gets the <see cref="TabCollection">collection</see> of the tabs in the <see cref="Tab"/>.
            /// </summary>
            [MergableProperty(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
            public virtual TabCollection Categories {
                [DebuggerStepThrough] get { return this._tabs; }
            }

            #endregion

            #region IToolboxObject Members

            /// <summary>
            /// Gets the current (<see cref="Toolbox"/> or another <see cref="Tab"/>) object that contains the <see cref="Tab"/>.
            /// </summary>
            [Browsable(false)]
            public ITab Owner {
                [DebuggerStepThrough] get { return this._owner; }
                [DebuggerStepThrough] internal set { this._owner = value; }
            }

            #endregion
        }

        #endregion
    }
}