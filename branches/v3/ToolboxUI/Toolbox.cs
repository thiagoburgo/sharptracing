using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using TooboxUI.Components.Design;
using ToolBoxUI.Components.Properties;

namespace TooboxUI.Components
{
    /// <summary>
    /// Represents a control, which displays a collection of items separated by categories.
    /// </summary>
    [TypeConverter(typeof(ToolboxConverter)), SRDescription("SRToolboxDescription")]
    public partial class Toolbox : Control, ITab
    {
        #region Fields
        internal const int Gap_IconFromText = 2;
        internal const int Gap_ItemBetween = 0;
        internal const int Gap_TabBetween = 2;
        internal const int Gap_TabBorder = 1;
        internal const int Gap_TabFromItem = 2;
        internal const int Gap_TabLevel = 7;
        internal const int Gap_TabTop = 1;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Type _itemType = typeof(Item);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private static readonly Type _tabType = typeof(Tab);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _allowNestedTabs = true;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _allowToolboxItems = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _autoScroll = true;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _backColorGradientEnd = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _backColorGradientStart = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private BorderStyle _borderStyle = BorderStyle.None;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _createGeneralCategory = true;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultBackColorGradientEnd = Color.FromArgb(228, 225, 208);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultBackColorGradientStart = Color.FromArgb(242, 240, 229);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultItemHoverColor = Color.LightBlue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultItemSelectBorderColor = Color.Blue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultItemSelectColor = Color.Honeydew;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultItemSelHoverColor = Color.PaleTurquoise;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Font _defaultTabFont = new Font("Arial", 9, FontStyle.Bold);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultTabSelectBorderColor = Color.Blue;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _defaultTabSelectColor = Color.Lavender;
        private Rectangle _dragBoxFromMouseDown = Rectangle.Empty;
        private IToolboxObject _draggedTool = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _drawTabLevel = true;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Tab _generalCategoryTab;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _itemHeight = 20;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _itemHoverColor = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ContextMenuStrip _itemMenu;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ItemCollection _items;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _itemSelectBorderColor = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _itemSelectColor = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _itemSelHoverColor = Color.Empty;
        private Item _lastHoveredItem = null;
        private Item _lastSelectedItem = null;
        private IToolboxObject _lastSelectedTool = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _showAll = false;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _showIcons = true;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _showPointer = true;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ColorStyle _tabColorStyle = ColorStyle.Standard;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Font _tabFont = null;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _tabHeight = 16;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ContextMenuStrip _tabMenu;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TabCollection _tabs;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _tabSelectBorderColor = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Color _tabSelectColor = Color.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ToolboxItemsTab _toolboxItemsTab;
        private BalloonToolTip _toolTip;
        private Point _toolTipPoint = Point.Empty;
        private VScrollBar _verticalScroll;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Toolbox"/> class.
        /// </summary>
        public Toolbox() : this(false) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="Toolbox"/> class and creating a 'General' category tab if needed.
        /// </summary>
        /// <param name="createGeneral">Indicates whether to create a 'General' category tab.</param>
        public Toolbox(bool createGeneral)
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.StandardDoubleClick, true);
            this.AllowDrop = true;
            this._items = new ItemCollection(this);
            this._tabs = new TabCollection(this);
            this._verticalScroll = new VScrollBar();
            this._verticalScroll.Visible = false;
            this._verticalScroll.Dock = DockStyle.Right;
            this._verticalScroll.Scroll += new ScrollEventHandler(this.OnVerticalScroll);
            this.Controls.Add(this._verticalScroll);
            this._toolTip = new BalloonToolTip();
            this.TabMenu = this.CreateTabMenu();
            this.ItemMenu = this.CreateItemMenu();
            if(createGeneral){
                this._generalCategoryTab = this.CreateNewTab(Resources.ToolboxTabGeneral);
                this._generalCategoryTab.AllowDelete = false;
                this.Categories.Add(this._generalCategoryTab);
            }
        }

        #region Area para facilitar a adicao de items e a mudanca de estado
        public void AddToolboxItem(Type itemType, string tabName)
        {
            this.AddToolboxItem(new ToolboxItem(itemType), tabName);
        }
        public void AddToolboxItem(ToolboxItem item, string tabName)
        {
            Tab tab = this.Categories[tabName];
            if(tab == null){
                tab = new Tab(tabName);
                this.Categories.Add(tab);
            }
            tab.Opened = true;
            tab.Items.Add(new HostToolbox.HostItem(item));
        }
        public void DisableItems()
        {
            this.ChangeItemsState(false);
        }
        public void EnableItems()
        {
            this.ChangeItemsState(true);
        }
        public void HideTabs()
        {
            this.ChangeTabsState(false);
        }
        public void ShowTabs()
        {
            this.ChangeTabsState(true);
        }
        private void ChangeItemsState(bool enable)
        {
            foreach(Tab tab in this.Categories){
                foreach(Item item in tab.Items){
                    item.Enabled = enable;
                }
                tab.PointerItem.Enabled = enable;
            }
            this.Invalidate();
        }
        private void ChangeTabsState(bool show)
        {
            foreach(Tab tab in this.Categories){
                if(tab != this.GeneralCategory){
                    tab.Visible = show;
                }
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Overriden. Gets or sets the site of the control.
        /// </summary>
        public override ISite Site
        {
            set
            {
                base.Site = value;
                if(value != null && this.CreateGeneralCategory && this.GeneralCategory != null){
                    this.Categories.Remove(this.GeneralCategory);
                }
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="ContextMenuStrip">menu</see> displayed for the <see cref="Tab">tabs</see>.
        /// </summary>
        [SRCategory("SRToolboxCategory"), SRDescription("SRToolboxTabMenuDescr")]
        public ContextMenuStrip TabMenu
        {
            [DebuggerStepThrough]
            get { return this._tabMenu; }
            [DebuggerStepThrough]
            set { this._tabMenu = value; }
        }
        /// <summary>
        /// Gets or sets the <see cref="ContextMenuStrip">menu</see> displayed for the <see cref="Item">items</see>.
        /// </summary>
        [SRCategory("SRToolboxCategory"), SRDescription("SRToolboxItemMenuDescr")]
        public ContextMenuStrip ItemMenu
        {
            [DebuggerStepThrough]
            get { return this._itemMenu; }
            [DebuggerStepThrough]
            set { this._itemMenu = value; }
        }
        /// <summary>
        /// Gets the currently selected <see cref="Item"/> object on the <see cref="Toolbox"/>.
        /// </summary>
        [Browsable(false)]
        public Item SelectedItem
        {
            get { return this._lastSelectedTool as Item; }
        }
        /// <summary>
        /// Gets or sets the border style for the <see cref="Toolbox"/>.
        /// </summary>
        /// <value>One of the <see cref="System.Windows.Forms.BorderStyle">BorderStyle</see> values. The default is <see cref="System.Windows.Forms.BorderStyle.None"/>.</value>
        /// <remarks>You can use this property to add a border to the <see cref="Toolbox"/>.</remarks>
        [DefaultValue(BorderStyle.None), SRCategory("SRAppearanceCategory"), SRDescription("SRToolboxBorderStyleDescr")]
        public BorderStyle BorderStyle
        {
            get { return this._borderStyle; }
            set
            {
                if(this._borderStyle != value){
                    int styleValue = (int)value;
                    if(styleValue < 0 || styleValue > 2){
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }
                    this._borderStyle = value;
                    base.UpdateStyles();
                }
            }
        }
        /// <summary>
        /// Gets or sets the height of the <see cref="Tab">tab</see> caption.
        /// </summary>
        [DefaultValue(16), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabHeightDescr")]
        public int TabHeight
        {
            [DebuggerStepThrough]
            get { return this._tabHeight; }
            [DebuggerStepThrough]
            set
            {
                if(value >= 16 && this._tabHeight != value){
                    this._tabHeight = value;
                    this.OnTabHeightChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the height of the <see cref="Item">items</see>.
        /// </summary>
        [DefaultValue(20), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemHeightDescr")]
        public int ItemHeight
        {
            [DebuggerStepThrough]
            get { return this._itemHeight; }
            [DebuggerStepThrough]
            set
            {
                if(value >= 20 && this._itemHeight != value){
                    this._itemHeight = value;
                    this.OnItemHeightChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the <see cref="Font">font</see> of the <see cref="Tab">tabs</see>.
        /// </summary>
        /// <remarks>
        /// The font of the <see cref="Item">items</see> is defined by the <see cref="Font"/> property of the <see cref="Toolbox"/>.
        /// </remarks>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabFontDescr")]
        public Font TabFont
        {
            [DebuggerStepThrough]
            get
            {
                if(this._tabFont == null){
                    return this.DefaultTabFont;
                }
                return this._tabFont;
            }
            [DebuggerStepThrough]
            set
            {
                if(value != null && this._tabFont != value){
                    this._tabFont = value;
                    this.OnTabFontChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the design-only value indicating whether to create 'General' category tab.
        /// </summary>
        [DefaultValue(true), DesignOnly(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         SRCategory("SRToolboxCategory"), SRDescription("SRToolboxCreateGeneralDescr")]
        public bool CreateGeneralCategory
        {
            [DebuggerStepThrough]
            get { return this._createGeneralCategory; }
            set
            {
                if(this._createGeneralCategory != value){
                    this._createGeneralCategory = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether to show icons for <see cref="Item">items</see>.
        /// </summary>
        [DefaultValue(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxShowIconsDescr")]
        public bool ShowIcons
        {
            [DebuggerStepThrough]
            get { return this._showIcons; }
            set
            {
                if(this._showIcons != value){
                    this._showIcons = value;
                    this.OnShowIconsChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether to show all <see cref="Item">items</see> and <see cref="Tab">categories</see> ignoring their <see cref="IToolboxObject.Visible">Visible</see> properties.
        /// </summary>
        [DefaultValue(false), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxShowAllDescr")]
        public bool ShowAll
        {
            [DebuggerStepThrough]
            get { return this._showAll; }
            set
            {
                if(this._showAll != value){
                    this._showAll = value;
                    this.OnShowAllChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether to show the pointer item for each <see cref="Tab"/>.
        /// </summary>
        [DefaultValue(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxShowPointerDescr")]
        public bool ShowPointer
        {
            [DebuggerStepThrough]
            get { return this._showPointer; }
            set
            {
                if(this._showPointer != value){
                    this._showPointer = value;
                    this.OnShowPointerChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether nested <see cref="Tab">tabs</see> are allowed.
        /// </summary>
        /// <remarks>If <b>false</b>, the <see cref="Tab.Categories">nested tabs</see> are not displayed.</remarks>
        [DefaultValue(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxAllowNestedTabsDescr")]
        public bool AllowNestedTabs
        {
            get { return this._allowNestedTabs; }
            set
            {
                if(this._allowNestedTabs != value){
                    this._allowNestedTabs = value;
                    this.OnAllowNestedTabsChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether to allow adding <see cref="Item">items</see> directly to <see cref="Toolbox"/>.
        /// </summary>
        /// <remarks>
        /// If <b>false</b>, items added to <see cref="Toolbox.Items"/> collection would be placed into additional <see cref="Tab">category</see>.
        /// </remarks>
        [DefaultValue(false), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxAllowToolboxItemsDescr")]
        public bool AllowToolboxItems
        {
            get { return this._allowToolboxItems; }
            set
            {
                if(this._allowToolboxItems != value){
                    this._allowToolboxItems = value;
                    this.OnAllowToolboxItemsChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether to draw the <see cref="Tab"/> level mark.
        /// </summary>
        [DefaultValue(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxDrawTabLevelDescr")]
        public bool DrawTabLevel
        {
            get { return this._drawTabLevel; }
            set
            {
                if(this._drawTabLevel != value){
                    this._drawTabLevel = value;
                    this.OnDrawTabLevelChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the value indicating whether the <see cref="Toolbox"/> enables autoscrolling.
        /// </summary>
        /// <value><b>true</b> to enable autoscrolling on the form; otherwise, <b>false</b>. The default is true.</value>
        /// <remarks>
        /// If this property is set to <b>true</b>, vertical scroll bar is displayed on the <see cref="Toolbox"/>.
        /// <para/>You can use this property if you want to synchronize the scrolling of the <see cref="Toolbox"/> with your own controls. Use the <see cref="Scroll"/> method in this case.
        /// </remarks>
        [DefaultValue(true), SRCategory("SRLayoutCategory"), SRDescription("SRToolboxAutoScrollDescr")]
        public bool AutoScroll
        {
            [DebuggerStepThrough]
            get { return this._autoScroll; }
            set
            {
                if(this._autoScroll != value){
                    this._autoScroll = value;
                    this.OnAutoScrollChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the background color of the <see cref="Item"/> when the mouse pointer is over it.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemHoverColorDescr")]
        public Color ItemHoverColor
        {
            get
            {
                if(this._itemHoverColor == Color.Empty){
                    return this.DefaultItemHoverColor;
                }
                return this._itemHoverColor;
            }
            set
            {
                if(this._itemHoverColor != value){
                    this._itemHoverColor = value;
                    this.OnItemHoverColorChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the background color of the selected <see cref="Item"/>.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemSelectColorDescr")]
        public Color ItemSelectColor
        {
            get
            {
                if(this._itemSelectColor == Color.Empty){
                    return this.DefaultItemSelectColor;
                }
                return this._itemSelectColor;
            }
            set
            {
                if(this._itemSelectColor != value){
                    this._itemSelectColor = value;
                    this.OnItemSelectColorChanged(EventArgs.Empty);
                    if(this.LastSelectedItem != null){
                        this.Invalidate(this.LastSelectedItem.Bounds);
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the background color of the selected <see cref="Item"/> when the mouse pointer is over it.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemSelHoverColorDescr")]
        public Color ItemSelectHoverColor
        {
            get
            {
                if(this._itemSelHoverColor == Color.Empty){
                    return this.DefaultItemSelHoverColor;
                }
                return this._itemSelHoverColor;
            }
            set
            {
                if(this._itemSelHoverColor != value){
                    this._itemSelHoverColor = value;
                    this.OnItemSelHoverColorChanged(EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Gets or sets the background color of the selected <see cref="Tab"/>.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabSelectColorDescr")]
        public Color TabSelectColor
        {
            get
            {
                if(this._tabSelectColor == Color.Empty){
                    return this.DefaultTabSelectColor;
                }
                return this._tabSelectColor;
            }
            set
            {
                if(this._tabSelectColor != value){
                    this._tabSelectColor = value;
                    this.OnTabSelectColorChanged(EventArgs.Empty);
                    if(this.LastSelectedTool is Tab){
                        this.Invalidate(((Tab)this.LastSelectedTool).GetCaptionRectangle(true));
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the border color of the selected <see cref="Item"/>.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemSelectBorderColorDescr")]
        public Color ItemSelectBorderColor
        {
            get
            {
                if(this._itemSelectBorderColor == Color.Empty){
                    return this.DefaultItemSelectBorderColor;
                }
                return this._itemSelectBorderColor;
            }
            set
            {
                if(this._itemSelectBorderColor != value){
                    this._itemSelectBorderColor = value;
                    this.OnItemSelectBorderColorChanged(EventArgs.Empty);
                    if(this.LastSelectedItem != null){
                        this.Invalidate(this.LastSelectedItem.Bounds);
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the border color of the selected <see cref="Tab"/>.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabSelectBorderColorDescr")]
        public Color TabSelectBorderColor
        {
            get
            {
                if(this._tabSelectBorderColor == Color.Empty){
                    return this.DefaultTabSelectBorderColor;
                }
                return this._tabSelectBorderColor;
            }
            set
            {
                if(this._tabSelectBorderColor != value){
                    this._tabSelectBorderColor = value;
                    this.OnTabSelectBorderColorChanged(EventArgs.Empty);
                    if(this.LastSelectedTool is Tab){
                        this.Invalidate(((Tab)this.LastSelectedTool).GetCaptionRectangle(true));
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the gradient start color of the <see cref="Toolbox"/>.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxBackColorGradientStartDescr")]
        public Color BackColorGradientStart
        {
            get
            {
                if(this._backColorGradientStart == Color.Empty){
                    return this.DefaultBackColorGradientStart;
                }
                return this._backColorGradientStart;
            }
            set
            {
                if(this._backColorGradientStart != value){
                    this._backColorGradientStart = value;
                    this.OnBackColorGradientStartChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the gradient end color of the <see cref="Toolbox"/>.
        /// </summary>
        [SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxBackColorGradientEndDescr")]
        public Color BackColorGradientEnd
        {
            get
            {
                if(this._backColorGradientEnd == Color.Empty){
                    return this.DefaultBackColorGradientEnd;
                }
                return this._backColorGradientEnd;
            }
            set
            {
                if(this._backColorGradientEnd != value){
                    this._backColorGradientEnd = value;
                    this.OnBackColorGradientEndChanged(EventArgs.Empty);
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Gets or sets the style of drawing background of the <see cref="Tab">tabs</see>.
        /// </summary>
        [DefaultValue(ColorStyle.Standard), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxTabColorStyleDescr")]
        public ColorStyle TabColorStyle
        {
            get { return this._tabColorStyle; }
            set
            {
                if(this._tabColorStyle != value){
                    int styleValue = (int)value;
                    if(styleValue < 0 || styleValue > 3){
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(ColorStyle));
                    }
                    this._tabColorStyle = value;
                    this.Invalidate();
                }
            }
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        /// <remarks>Not used for drawing <see cref="Toolbox"/> background. Use <see cref="BackColorGradientStart"/> and <see cref="BackColorGradientEnd"/> properties instead.</remarks>
        /// <seealso cref="BackColorGradientStart"/>
        /// <seealso cref="BackColorGradientEnd"/>
        [Browsable(false)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }
        /// <summary>
        /// Gets the visible rectangle of the <see cref="Toolbox"/> including the offset of the scroll bar.
        /// </summary>
        [Browsable(false)]
        public Rectangle VisibleRectangle
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                if(this._verticalScroll.Visible){
                    rect = new Rectangle(new Point(rect.Left, rect.Top + this._verticalScroll.Value),
                                         new Size(rect.Width - this._verticalScroll.Width, rect.Height));
                }
                return rect;
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets or sets the 'General' category tab.
        /// </summary>
        protected Tab GeneralCategory
        {
            [DebuggerStepThrough]
            get { return this._generalCategoryTab; }
            [DebuggerStepThrough]
            set { this._generalCategoryTab = value; }
        }
        /// <summary>
        /// Gets the last selected <see cref="IToolboxObject">tool</see>.
        /// </summary>
        protected IToolboxObject LastSelectedTool
        {
            [DebuggerStepThrough]
            get { return this._lastSelectedTool; }
        }
        /// <summary>
        /// Gets the last selected <see cref="Item"/>.
        /// </summary>
        protected Item LastSelectedItem
        {
            [DebuggerStepThrough]
            get { return this._lastSelectedItem; }
        }
        /// <summary>
        /// Gets or sets the default <see cref="Font"/> of the <see cref="Tab"/> text.
        /// </summary>
        protected virtual Font DefaultTabFont
        {
            [DebuggerStepThrough]
            get { return this._defaultTabFont; }
            [DebuggerStepThrough]
            set { this._defaultTabFont = value; }
        }
        /// <summary>
        /// Gets or sets the default <see cref="Color"/> of the <see cref="Item"/> when the mouse pointer is over it.
        /// </summary>
        protected virtual Color DefaultItemHoverColor
        {
            [DebuggerStepThrough]
            get { return this._defaultItemHoverColor; }
            [DebuggerStepThrough]
            set { this._defaultItemHoverColor = value; }
        }
        /// <summary>
        /// Gets or sets the default <see cref="Color"/> of the selected <see cref="Item"/>.
        /// </summary>
        protected virtual Color DefaultItemSelectColor
        {
            [DebuggerStepThrough]
            get { return this._defaultItemSelectColor; }
            [DebuggerStepThrough]
            set { this._defaultItemSelectColor = value; }
        }
        /// <summary>
        /// Gets or sets the default <see cref="Color"/> of the selected <see cref="Item"/> when the mouse pointer is over it.
        /// </summary>
        protected virtual Color DefaultItemSelHoverColor
        {
            [DebuggerStepThrough]
            get { return this._defaultItemSelHoverColor; }
            [DebuggerStepThrough]
            set { this._defaultItemSelHoverColor = value; }
        }
        /// <summary>
        /// Gets or sets the default <see cref="Color"/> of the selected <see cref="Tab"/>.
        /// </summary>
        protected virtual Color DefaultTabSelectColor
        {
            [DebuggerStepThrough]
            get { return this._defaultTabSelectColor; }
            [DebuggerStepThrough]
            set { this._defaultTabSelectColor = value; }
        }
        /// <summary>
        /// Gets or sets the default border <see cref="Color"/> of the selected <see cref="Item"/>.
        /// </summary>
        protected virtual Color DefaultItemSelectBorderColor
        {
            [DebuggerStepThrough]
            get { return this._defaultItemSelectBorderColor; }
            [DebuggerStepThrough]
            set { this._defaultItemSelectBorderColor = value; }
        }
        /// <summary>
        /// Gets or sets the default border <see cref="Color"/> of the selected <see cref="Tab"/>.
        /// </summary>
        protected virtual Color DefaultTabSelectBorderColor
        {
            [DebuggerStepThrough]
            get { return this._defaultTabSelectBorderColor; }
            [DebuggerStepThrough]
            set { this._defaultTabSelectBorderColor = value; }
        }
        /// <summary>
        /// Gets or sets the default gradient start <see cref="Color"/> of the <see cref="Toolbox"/>.
        /// </summary>
        protected virtual Color DefaultBackColorGradientStart
        {
            [DebuggerStepThrough]
            get { return this._defaultBackColorGradientStart; }
            [DebuggerStepThrough]
            set { this._defaultBackColorGradientStart = value; }
        }
        /// <summary>
        /// Gets or sets the default gradient end <see cref="Color"/> of the <see cref="Toolbox"/>.
        /// </summary>
        protected virtual Color DefaultBackColorGradientEnd
        {
            [DebuggerStepThrough]
            get { return this._defaultBackColorGradientEnd; }
            [DebuggerStepThrough]
            set { this._defaultBackColorGradientEnd = value; }
        }
        /// <summary>
        /// Overridden.
        /// </summary>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams parameters = base.CreateParams;
                switch(this._borderStyle){
                    case BorderStyle.FixedSingle:
                        parameters.Style |= 0x800000;
                        return parameters;
                    case BorderStyle.Fixed3D:
                        parameters.ExStyle |= 0x200;
                        return parameters;
                }
                return parameters;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Scrolls current view by the specified offset value.
        /// </summary>
        /// <param name="offset">The integer value of scrolling. Negative values mean scrolling up, positive values - scrolling down.</param>
        public void ScrollView(int offset)
        {
            int oldValue = this._verticalScroll.Value;
            int newValue = this._verticalScroll.Value + offset;
            if(newValue > this._verticalScroll.Maximum){
                newValue = this._verticalScroll.Maximum;
            }
            if(newValue < this._verticalScroll.Minimum){
                newValue = this._verticalScroll.Minimum;
            }
            this._verticalScroll.Value = newValue;
            this.Invoke(new ScrollEventHandler(this.OnVerticalScroll), this._verticalScroll,
                        new ScrollEventArgs(ScrollEventType.ThumbPosition, oldValue, newValue,
                                            ScrollOrientation.VerticalScroll));
        }
        /// <summary>
        /// Returns an object with information on which portion of a toolbox control is at a location specified by a <see cref="Point"/>.
        /// </summary>
        /// <param name="location">A <see cref="Point"/> containing the <see cref="Point.X"/> and <see cref="Point.Y"/> coordinates of the point to be hit tested.</param>
        /// <returns>A <see cref="HitTestInfo"/> that contains information about the specified point on the <see cref="Toolbox"/>.</returns>
        public virtual HitTestInfo HitTest(Point location)
        {
            HitTestInfo info = new HitTestInfo(location);
            Rectangle tabsRect = this.ClientRectangle;
            tabsRect.Inflate(-Gap_TabBorder, -Gap_TabTop);
            if(tabsRect.Contains(location)){
                foreach(Tab tab in this.Categories){
                    if((this.ShowAll || tab.Visible) && tab.VisibleRectangle.Contains(location)){
                        return tab.HitTest(location);
                    }
                }
                if(this.Items.Count > 0){
                    if(this.AllowToolboxItems){
                        foreach(Item item in this.Items){
                            if((item.Visible || this.ShowAll) && item.Bounds.Contains(location)){
                                info.HitArea = HitArea.Item;
                                info.Tool = item;
                                break;
                            }
                        }
                    } else{
                        Tab toolboxItems = this.GetToolboxItemsTab(true);
                        if((toolboxItems.Visible || this.ShowAll) && toolboxItems.VisibleRectangle.Contains(location)){
                            info = toolboxItems.HitTest(location);
                        }
                    }
                }
                if(info.HitArea == HitArea.None){
                    info.HitArea = HitArea.Background;
                }
            }
            return info;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Creates a new <see cref="Tab"/> object with the default <paramref name="text"/> value.
        /// </summary>
        /// <param name="text">The default text of the <see cref="Tab"/>.</param>
        protected virtual Tab CreateNewTab(string text)
        {
            return new Tab(text);
        }
        /// <summary>
        /// Ensures that the <paramref name="rect"/> is visible within the <see cref="Toolbox"/>, scrolling the contents of the <see cref="Toolbox"/> if necessary.
        /// </summary>
        /// <param name="rect">The <see cref="Rectangle"/> to set visible.</param>
        protected virtual void EnsureVisible(Rectangle rect)
        {
            Rectangle visible = this.VisibleRectangle;
            if(!visible.Contains(rect)){
                int offset = 0;
                if(rect.Top < visible.Top){
                    offset = rect.Top;
                } else if(rect.Height <= visible.Height){
                    offset = rect.Bottom - visible.Bottom;
                } else{
                    offset = visible.Top - rect.Top;
                }
                this.ScrollView(offset);
            }
        }
        /// <summary>
        /// Selects a specified <paramref name="tool"/> on the <see cref="Toolbox"/>.
        /// </summary>
        /// <param name="tool">A <see cref="IToolboxObject"/> object to select.</param>
        protected virtual void SelectTool(IToolboxObject tool)
        {
            if(tool == null){
                throw new ArgumentNullException("tool");
            }
            if(!tool.Visible){
                throw new ArgumentException(Resources.ToolboxSelectNonVisibleTool);
            }
            if(tool is Item && !((Item)tool).Enabled){
                throw new ArgumentException(Resources.ToolboxSelectDisabledItem);
            }
            this.InternalSelectTool(tool);
        }
        /// <summary>
        /// Invoked when user double clicks on an <see cref="Item"/>.
        /// </summary>
        /// <param name="picked">An <see cref="IDataObject"/> object that contains an <see cref="Item"/> in independent format.</param>
        protected virtual bool OnToolboxItemPicked(IDataObject picked)
        {
            return false;
        }
        /// <summary>
        /// Returns <see cref="Tab"/> object which contains <see cref="Toolbox"/>-level items.
        /// </summary>
        /// <param name="createIfNull">Indicates whether to create new <see cref="Tab"/> if it is Null.</param>
        /// <remarks>Use this method only if property <see cref="AllowToolboxItems"/> is set to <b>false</b>. The collection of <see cref="Item">items</see> inside this <see cref="Tab"/> is identical to <see cref="Toolbox.Items">Toolbox Items</see>.</remarks>
        protected Tab GetToolboxItemsTab(bool createIfNull)
        {
            if(this._toolboxItemsTab == null && createIfNull){
                this._toolboxItemsTab = new ToolboxItemsTab(this.Items);
            }
            return this._toolboxItemsTab;
        }
        /// <summary>
        /// Creates a default unique category name.
        /// </summary>
        protected string GetUnusedCategoryName()
        {
            return GetUnusedCategoryName(this, Resources.ToolboxTabDefaultName);
        }
        /// <summary>
        /// Creates a default unique category name using the <paramref name="baseName"/>.
        /// </summary>
        /// <param name="baseName">A base category name which is changed by adding a number until it becomes unique.</param>
        protected string GetUnusedCategoryName(string baseName)
        {
            return GetUnusedCategoryName(this, baseName);
        }
        /// <summary>
        /// Creates a default unique category name.
        /// </summary>
        protected static string GetUnusedCategoryName(ITab tab)
        {
            return GetUnusedCategoryName(tab, Resources.ToolboxTabDefaultName);
        }
        /// <summary>
        /// Creates a default unique category name using the <paramref name="baseName"/>..
        /// </summary>
        protected static string GetUnusedCategoryName(ITab tab, string baseName)
        {
            string category = baseName;
            for(int i = 1;; i++){
                category = string.Format("{0}{1}", baseName, i);
                if(tab.Categories[category] == null){
                    break;
                }
            }
            return category;
        }

        #region Overridables

        #region Paint
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using(
                    Brush backgroundBrush = new LinearGradientBrush(this.ClientRectangle, this.BackColorGradientStart,
                                                                    this.BackColorGradientEnd,
                                                                    LinearGradientMode.Horizontal)){
                e.Graphics.FillRectangle(backgroundBrush, this.ClientRectangle);
            }

            #region Drawing Background Image
            if(this.BackgroundImage != null){
                using(Bitmap backgroundImage = new Bitmap(this.BackgroundImage)){
                    switch(this.BackgroundImageLayout){
                        case ImageLayout.None:
                            e.Graphics.DrawImageUnscaled(backgroundImage, Point.Empty);
                            break;
                        case ImageLayout.Center:
                            PointF drawImagePoint =
                                    new PointF((this.ClientSize.Width - backgroundImage.Width) / (float)2,
                                               (this.ClientSize.Height - backgroundImage.Height) / (float)2);
                            e.Graphics.DrawImageUnscaled(backgroundImage, Point.Round(drawImagePoint));
                            break;
                        case ImageLayout.Stretch:
                            e.Graphics.DrawImage(
                                    backgroundImage.GetThumbnailImage(this.ClientSize.Width, this.ClientSize.Height,
                                                                      delegate { return false; }, IntPtr.Zero),
                                    this.ClientRectangle);
                            break;
                        case ImageLayout.Zoom:
                            float xRelation = this.ClientSize.Width / (float)backgroundImage.Width;
                            float yRelation = this.ClientSize.Height / (float)backgroundImage.Height;
                            SizeF imageSize = SizeF.Empty;
                            if(xRelation > 1 || yRelation > 1){
                                float multiplier = (xRelation < yRelation) ? xRelation : yRelation;
                                imageSize = new SizeF(backgroundImage.Width * multiplier,
                                                      backgroundImage.Height * multiplier);
                            } else{
                                float divisor = (xRelation > yRelation) ? xRelation : yRelation;
                                imageSize = new SizeF(backgroundImage.Width / divisor, backgroundImage.Height / divisor);
                            }
                            RectangleF imageRect =
                                    new RectangleF(
                                            new PointF((this.ClientSize.Width - imageSize.Width) / 2,
                                                       (this.ClientSize.Height - imageSize.Height) / 2), imageSize);
                            e.Graphics.DrawImage(
                                    backgroundImage.GetThumbnailImage((int)imageSize.Width, (int)imageSize.Height,
                                                                      delegate { return false; }, IntPtr.Zero),
                                    imageRect);
                            break;
                        case ImageLayout.Tile:
                            using(Brush imageBrush = new TextureBrush(backgroundImage, WrapMode.Tile)){
                                e.Graphics.FillRectangle(imageBrush, this.ClientRectangle);
                            }
                            break;
                    }
                }
            }
            #endregion

            this.SuspendLayout();
            Point location = new Point(Gap_TabBorder, Gap_TabTop);
            if(!this.AutoScroll){
                this._verticalScroll.Visible = false;
            } else{
                int totalHeight = this.CalculateToolboxHeight(e.Graphics, false);
                if(totalHeight > this.ClientRectangle.Height){
                    totalHeight = this.CalculateToolboxHeight(e.Graphics, true);
                    this._verticalScroll.Maximum = totalHeight - this.ClientSize.Height;
                    this._verticalScroll.LargeChange = Math.Min(this._verticalScroll.Maximum / 3, this.ClientSize.Height);
                    this._verticalScroll.Maximum += this._verticalScroll.LargeChange;
                    this._verticalScroll.SmallChange = this.ItemHeight;
                    this._verticalScroll.Visible = true;
                } else{
                    this._verticalScroll.Visible = false;
                }
                if(this._verticalScroll.Visible){
                    location.Y -= this._verticalScroll.Value;
                }
            }
            foreach(Tab tab in this.Categories){
                if(tab.Visible || this.ShowAll){
                    tab.InternalPaint(e, ref location);
                    location.Offset(0, Gap_TabBetween);
                }
            }
            if(this.Items.Count > 0){
                if(this.AllowToolboxItems){
                    foreach(Item item in this.Items){
                        if(item.Visible || this.ShowAll){
                            location.Offset(0, Gap_ItemBetween);
                            item.InternalPaint(e, location);
                            location.Offset(0, this.ItemHeight);
                        }
                    }
                } else{
                    Tab toolboxItems = this.GetToolboxItemsTab(true);
                    if(toolboxItems.Visible || this.ShowAll){
                        toolboxItems.InternalPaint(e, ref location);
                        location.Offset(0, Gap_TabBetween);
                    }
                }
            }
            this.ResumeLayout(true);
        }
        #endregion

        #region Mouse Events
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.InternalSelectTool(this.HitTest(e.Location).Tool);
            if(this.LastSelectedTool != null){
                this._draggedTool = this.LastSelectedTool;
                Size dragSize = SystemInformation.DragSize;
                this._dragBoxFromMouseDown =
                        new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                Tab tab = this.LastSelectedTool as Tab;
                if(tab != null){
                    tab.InternalMouseDown(e);
                }
                Item item = this.LastSelectedTool as Item;
                if(item != null){
                    item.InternalMouseDown(e);
                }
            } else{
                this._dragBoxFromMouseDown = Rectangle.Empty;
            }
            base.OnMouseDown(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._dragBoxFromMouseDown = Rectangle.Empty;
            if(this.LastSelectedTool != null){
                Tab tab = this.LastSelectedTool as Tab;
                if(tab != null){
                    tab.InternalMouseUp(e);
                    goto Label_MouseUpBaseCall;
                }
                Item item = this.LastSelectedTool as Item;
                if(item != null){
                    item.InternalMouseUp(e);
                    goto Label_MouseUpBaseCall;
                }
            } else if(e.Button == MouseButtons.Right){
                if(this.TabMenu != null){
                    this.TabMenu.Tag = null;
                    this.TabMenu.Show(this, e.Location);
                }
            }
            Label_MouseUpBaseCall:
            base.OnMouseUp(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if((MouseButtons == MouseButtons.Left) && this._dragBoxFromMouseDown != Rectangle.Empty
               && !this._dragBoxFromMouseDown.Contains(e.Location)){
                Item item = this._draggedTool as Item;
                if(item != null && this.IsPointerItem(item)){
                    goto Label_MouseMoveBaseCall;
                }
                this.DoDragDrop(this._draggedTool, DragDropEffects.Move | DragDropEffects.Copy);
            } else{
                IToolboxObject tool = this.HitTest(e.Location).Tool;
                Item item = tool as Item;
                if(item != null){
                    item.Highlighted = true;
                    if(item.Tooltip != this._toolTip.GetBalloonText(this) || Math.Abs(e.X - this._toolTipPoint.X) > 5
                       || Math.Abs(e.Y - this._toolTipPoint.Y) > 5){
                        this._toolTip.SetBalloonText(this, item.Tooltip);
                        this._toolTipPoint = e.Location;
                    }
                } else{
                    this._toolTip.SetBalloonText(this, string.Empty);
                }
                if(this._lastHoveredItem != item){
                    if(this._lastHoveredItem != null){
                        this._lastHoveredItem.Highlighted = false;
                    }
                    this._lastHoveredItem = item;
                }
            }
            Label_MouseMoveBaseCall:
            base.OnMouseMove(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            //this.Focus();
            base.OnMouseEnter(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            if(this._lastHoveredItem != null){
                this._lastHoveredItem.Highlighted = false;
            }
            base.OnMouseLeave(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if(this._verticalScroll.Visible){
                int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines
                                              / SystemInformation.MouseWheelScrollDelta;
                int numberOfPixelsToMove = (int)(numberOfTextLinesToMove * this.Font.Size);
                if(numberOfPixelsToMove != 0){
                    int result = this._verticalScroll.Value - numberOfPixelsToMove;
                    if(result < 0){
                        this.OnVerticalScroll(this,
                                              new ScrollEventArgs(ScrollEventType.First, this._verticalScroll.Value, 0,
                                                                  ScrollOrientation.VerticalScroll));
                        this._verticalScroll.Value = 0;
                    } else if(result > this._verticalScroll.Maximum){
                        this.OnVerticalScroll(this,
                                              new ScrollEventArgs(ScrollEventType.Last, this._verticalScroll.Value,
                                                                  this._verticalScroll.Maximum,
                                                                  ScrollOrientation.VerticalScroll));
                        this._verticalScroll.Value = this._verticalScroll.Maximum;
                    } else{
                        this.OnVerticalScroll(this,
                                              new ScrollEventArgs(ScrollEventType.ThumbPosition,
                                                                  this._verticalScroll.Value, result,
                                                                  ScrollOrientation.VerticalScroll));
                        this._verticalScroll.Value = result;
                    }
                }
            }
            base.OnMouseWheel(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            if(this._lastSelectedTool is Item){
                ((Item)this._lastSelectedTool).InternalClick(e);
            } else if(this._lastSelectedTool is Tab){
                ((Tab)this._lastSelectedTool).InternalClick(e);
            }
            base.OnClick(e);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnDoubleClick(EventArgs e)
        {
            if(this._lastSelectedTool is Item){
                this.OnToolboxItemPicked(new DataObject(this._lastSelectedTool));
                ((Item)this._lastSelectedTool).InternalDoubleClick(e);
            } else if(this._lastSelectedTool is Tab){
                ((Tab)this._lastSelectedTool).InternalDoubleClick(e);
            }
            base.OnDoubleClick(e);
        }
        #endregion

        #region Drag'n'Drop
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            IToolboxObject dropped = this.GetDragDropTool(drgevent.Data);
            if(dropped != null){
                Point ptDrop = this.PointToClient(new Point(drgevent.X, drgevent.Y));
                IToolboxObject selected = this.HitTest(ptDrop).Tool;
                if(dropped is Item){
                    Item item = (Item)dropped;
                    if(item.Toolbox == this){
                        item.Owner.Items.Remove(item);
                    } else{
                        item = item.Clone();
                    }
                    if(selected is Item){
                        Item selItem = (Item)selected;
                        selItem.Owner.Items.InsertAfter(selItem, item);
                    } else if(selected is Tab){
                        ((Tab)selected).Items.Add(item);
                    } else if(this.AllowToolboxItems){
                        this.Items.Add(item);
                    } else{
                        Tab tab = this.Categories[this.Categories.Count - 1];
                        tab.Items.Add(item);
                        if(!tab.Opened){
                            tab.Opened = true;
                        }
                    }
                } else if(dropped is Tab){
                    Tab tab = (Tab)dropped;
                    if(tab.Toolbox == this){
                        TabCollection sourceCollection = tab.Owner.Categories;
                        TabCollection destCollection = this.Categories;
                        int index = sourceCollection.IndexOf(tab);
                        int destination = -1;
                        Tab over = selected as Tab;
                        if(over == null) // Mouse is not over Tab (empty tab or header)
                        {
                            Item selItem = selected as Item;
                            if(selItem != null){
                                over = selItem.Category;
                            }
                        }
                        if(over != null){
                            ITab parent = over.Owner;
                            while(parent != null){
                                if(parent == dropped){
                                    goto Label_Out;
                                }
                                parent = parent.Owner;
                            }
                            destCollection = over.Owner.Categories;
                            HitArea hit = over.HitTest(ptDrop).HitArea;
                            destination = destCollection.IndexOf(over);
                            if(hit != HitArea.TabHeader){
                                destination++;
                            }
                        } else{
                            destination = this.Categories.Count - 1;
                        }
                        if(sourceCollection != destCollection || index != destination){
                            if(sourceCollection == destCollection && index < destination){
                                destination--;
                            }
                            bool allowDelete = tab.AllowDelete;
                            try{
                                tab.AllowDelete = true;
                                sourceCollection.Remove(tab);
                                destCollection.Insert(destination, tab);
                            } finally{
                                tab.AllowDelete = allowDelete;
                            }
                        }
                    }
                }
            }
            Label_Out:
            base.OnDragDrop(drgevent);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            IToolboxObject tool = this.GetDragDropTool(drgevent.Data);
            if(tool is Tab && ((Tab)tool).Toolbox != this){
                drgevent.Effect = DragDropEffects.None;
            } else{
                drgevent.Effect = (tool != null) ? drgevent.AllowedEffect : DragDropEffects.None;
            }
            base.OnDragEnter(drgevent);
        }
        /// <summary>
        /// Overriden.
        /// </summary>
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            IToolboxObject dragged = this.GetDragDropTool(drgevent.Data);
            if(dragged is Tab){
                IToolboxObject tool = this.HitTest(this.PointToClient(new Point(drgevent.X, drgevent.Y))).Tool;
                if(tool is Tab){
                    drgevent.Effect = drgevent.AllowedEffect;
                    ITab current = (ITab)tool;
                    while(current != null){
                        if(current == dragged){
                            drgevent.Effect = DragDropEffects.None;
                            break;
                        }
                        current = current.Owner;
                    }
                } else if(tool is Item && tool.Owner == dragged){
                    drgevent.Effect = DragDropEffects.None;
                } else if(tool == null && this.Categories.IndexOf((Tab)dragged) == this.Categories.Count - 1){
                    drgevent.Effect = DragDropEffects.None;
                } else{
                    drgevent.Effect = drgevent.AllowedEffect;
                }
            } else if(dragged is Item){
                IToolboxObject tool = this.HitTest(this.PointToClient(new Point(drgevent.X, drgevent.Y))).Tool;
                if(tool is Item && dragged == tool){
                    drgevent.Effect = DragDropEffects.None;
                } else{
                    drgevent.Effect = drgevent.AllowedEffect;
                }
            }
            base.OnDragOver(drgevent);
        }
        /// <summary>
        /// Retrieves an <see cref="IToolboxObject"/> object from the specified format-independent data storage.
        /// </summary>
        /// <param name="dragged">An <see cref="IDataObject"/> object that contains drag'n'drop data.</param>
        protected virtual IToolboxObject GetDragDropTool(IDataObject dragged)
        {
            if(dragged.GetDataPresent(_itemType)){
                return (Item)dragged.GetData(_itemType);
            }
            if(dragged.GetDataPresent(_tabType)){
                return (Tab)dragged.GetData(_tabType);
            }
            return null;
        }
        #endregion

        #endregion

        #endregion

        #region Private Methods
        private void OnVerticalScroll(object sender, ScrollEventArgs e)
        {
            if(e.Type == ScrollEventType.SmallIncrement){
                int val = Math.Min(Math.Max(e.NewValue, e.OldValue + this._verticalScroll.SmallChange),
                                   this._verticalScroll.Maximum);
                if(val != e.NewValue){
                    e.NewValue = val;
                }
            }
            this.OnScroll(e);
            if(e.Type == ScrollEventType.EndScroll){
                return;
            }
            this.Invalidate();
        }
        private int CalculateToolboxHeight(Graphics g, bool scrollVisible)
        {
            int height = Gap_TabTop - Gap_TabBetween;
            Rectangle toolboxRect = (!scrollVisible)
                                            ? this.ClientRectangle
                                            : new Rectangle(this.ClientRectangle.Location,
                                                            new Size(
                                                                    this.ClientRectangle.Width
                                                                    - this._verticalScroll.Width,
                                                                    this.ClientRectangle.Height));
            toolboxRect.Inflate(-2 * Gap_TabBorder, -2 * Gap_TabTop);
            foreach(Tab tab in this.Categories){
                if(tab.Visible || this.ShowAll){
                    height += tab.CalculateTabHeight(g, toolboxRect);
                    height += Gap_TabBetween;
                }
            }
            if(this.Items.Count > 0){
                if(this.AllowToolboxItems){
                    foreach(Item item in this.Items){
                        if(item.Visible || this.ShowAll){
                            height += Gap_ItemBetween;
                            height += this.ItemHeight;
                        }
                    }
                } else{
                    Tab toolboxItems = this.GetToolboxItemsTab(true);
                    if(toolboxItems.Visible || this.ShowAll){
                        height += toolboxItems.CalculateTabHeight(g, toolboxRect);
                        height += Gap_TabBetween;
                    }
                }
            }
            return height;
        }
        private void InternalSelectTool(IToolboxObject tool)
        {
            IToolboxObject previouslySelected = this.LastSelectedTool;
            if(tool is Item && !((Item)tool).Enabled){
                tool = null;
            }
            this._lastSelectedTool = tool;
            if(previouslySelected != null && previouslySelected != this.LastSelectedTool){
                if(previouslySelected is Tab){
                    ((Tab)previouslySelected).Selected = false;
                } else if(previouslySelected is Item){
                    ((Item)previouslySelected).Selected = false;
                }
            }
            if(this.LastSelectedTool != null){
                Tab tab = this.LastSelectedTool as Tab;
                if(tab != null){
                    tab.Selected = true;
                    return;
                }
                Item item = this.LastSelectedTool as Item;
                if(item != null){
                    if(this.LastSelectedItem != item){
                        if(this.LastSelectedItem != null){
                            this.LastSelectedItem.Selected = false;
                        }
                        this._lastSelectedItem = item;
                    }
                    item.Selected = true;
                    if(item.Category != null && !item.Category.Opened){
                        item.Category.Opened = true;
                    }
                    this.OnSelectItem(new ItemSelectionEventArgs(item));
                    return;
                }
            }
        }
        private bool IsPointerItem(Item item)
        {
            Tab tab = item.Owner as Tab;
            return tab != null && tab.PointerItem == item;
        }
        private bool ShouldSerializeItemHoverColor()
        {
            return this.ItemHoverColor != this.DefaultItemHoverColor;
        }
        private bool ShouldSerializeItemSelectColor()
        {
            return this.ItemSelectColor != this.DefaultItemSelectColor;
        }
        private bool ShouldSerializeItemSelectHoverColor()
        {
            return this.ItemSelectHoverColor != this.DefaultItemSelHoverColor;
        }
        private bool ShouldSerializeTabSelectColor()
        {
            return this.TabSelectColor != this.DefaultTabSelectColor;
        }
        private bool ShouldSerializeItemSelectBorderColor()
        {
            return this.ItemSelectBorderColor != this.DefaultItemSelectBorderColor;
        }
        private bool ShouldSerializeTabSelectBorderColor()
        {
            return this.TabSelectBorderColor != this.DefaultTabSelectBorderColor;
        }
        private bool ShouldSerializeBackColorGradientStart()
        {
            return this.BackColorGradientStart != this.DefaultBackColorGradientStart;
        }
        private bool ShouldSerializeBackColorGradientEnd()
        {
            return this.BackColorGradientEnd != this.DefaultBackColorGradientEnd;
        }
        private bool ShouldSerializeTabFont()
        {
            return this.TabFont != this.DefaultTabFont;
        }
        #endregion

        #region Events
        private static readonly object EventAllowNestedTabsChanged = new object();
        private static readonly object EventAllowToolboxItemsChanged = new object();
        private static readonly object EventAutoScrollChanged = new object();
        private static readonly object EventBackColorGradientEndChanged = new object();
        private static readonly object EventBackColorGradientStartChanged = new object();
        private static readonly object EventDrawTabLevelChanged = new object();
        private static readonly object EventItemHeightChanged = new object();
        private static readonly object EventItemHoverColorChanged = new object();
        private static readonly object EventItemSelectBorderColorChanged = new object();
        private static readonly object EventItemSelectColorChanged = new object();
        private static readonly object EventItemSelHoverColorChanged = new object();
        private static readonly object EventScroll = new object();
        private static readonly object EventSelectItem = new object();
        private static readonly object EventShowAllChanged = new object();
        private static readonly object EventShowIconsChanged = new object();
        private static readonly object EventShowPointerChanged = new object();
        private static readonly object EventTabColorStyleChanged = new object();
        private static readonly object EventTabFontChanged = new object();
        private static readonly object EventTabHeightChanged = new object();
        private static readonly object EventTabSelectBorderColorChanged = new object();
        private static readonly object EventTabSelectColorChanged = new object();
        /// <summary>
        /// Occurs when the <see cref="ItemHoverColor"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxItemHoverColorChangedDescr")]
        public event EventHandler ItemHoverColorChanged
        {
            add { this.Events.AddHandler(EventItemHoverColorChanged, value); }
            remove { this.Events.RemoveHandler(EventItemHoverColorChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ItemSelectColor"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxItemSelectColorChangedDescr")]
        public event EventHandler ItemSelectColorChanged
        {
            add { this.Events.AddHandler(EventItemSelectColorChanged, value); }
            remove { this.Events.RemoveHandler(EventItemSelectColorChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ItemSelectHoverColor"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemSelHoverColorChanged")]
        public event EventHandler ItemSelectHoverColorChanged
        {
            add { this.Events.AddHandler(EventItemSelHoverColorChanged, value); }
            remove { this.Events.RemoveHandler(EventItemSelHoverColorChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="TabSelectColor"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabSelectColorChanged")]
        public event EventHandler TabSelectColorChanged
        {
            add { this.Events.AddHandler(EventTabSelectColorChanged, value); }
            remove { this.Events.RemoveHandler(EventTabSelectColorChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ItemSelectBorderColor"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxItemSelectBorderColorChangedDescr")]
        public event EventHandler ItemSelectBorderColorChanged
        {
            add { this.Events.AddHandler(EventItemSelectBorderColorChanged, value); }
            remove { this.Events.RemoveHandler(EventItemSelectBorderColorChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="TabSelectBorderColor"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxTabSelectBorderColorChangedDescr")]
        public event EventHandler TabSelectBorderColorChanged
        {
            add { this.Events.AddHandler(EventTabSelectBorderColorChanged, value); }
            remove { this.Events.RemoveHandler(EventTabSelectBorderColorChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="BackColorGradientStart"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxBackColorGradientStartChangedDescr")]
        public event EventHandler BackColorGradientStartChanged
        {
            add { this.Events.AddHandler(EventBackColorGradientStartChanged, value); }
            remove { this.Events.RemoveHandler(EventBackColorGradientStartChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="BackColorGradientEnd"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxBackColorGradientEndChangedDescr")]
        public event EventHandler BackColorGradientEndChanged
        {
            add { this.Events.AddHandler(EventBackColorGradientEndChanged, value); }
            remove { this.Events.RemoveHandler(EventBackColorGradientEndChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="TabColorStyle"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabColorStyleChangedDescr")
        ]
        public event EventHandler TabColorStyleChanged
        {
            add { this.Events.AddHandler(EventTabColorStyleChanged, value); }
            remove { this.Events.RemoveHandler(EventTabColorStyleChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ShowAll"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxShowAllChangedDescr")]
        public event EventHandler ShowAllChanged
        {
            add { this.Events.AddHandler(EventShowAllChanged, value); }
            remove { this.Events.RemoveHandler(EventShowAllChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ShowIcons"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxShowIconsChangedDescr")]
        public event EventHandler ShowIconsChanged
        {
            add { this.Events.AddHandler(EventShowIconsChanged, value); }
            remove { this.Events.RemoveHandler(EventShowIconsChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ShowPointer"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxShowPointerChangedDescr")]
        public event EventHandler ShowPointerChanged
        {
            add { this.Events.AddHandler(EventShowPointerChanged, value); }
            remove { this.Events.RemoveHandler(EventShowPointerChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="AllowNestedTabs"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxAllowNestedTabsChangedDescr")]
        public event EventHandler AllowNestedTabsChanged
        {
            add { this.Events.AddHandler(EventAllowNestedTabsChanged, value); }
            remove { this.Events.RemoveHandler(EventAllowNestedTabsChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="AllowToolboxItems"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"),
         SRDescription("SRToolboxAllowToolboxItemsChangedDescr")]
        public event EventHandler AllowToolboxItemsChanged
        {
            add { this.Events.AddHandler(EventAllowToolboxItemsChanged, value); }
            remove { this.Events.RemoveHandler(EventAllowToolboxItemsChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="DrawTabLevel"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxDrawTabLevelChangedDescr")]
        public event EventHandler DrawTabLevelChanged
        {
            add { this.Events.AddHandler(EventDrawTabLevelChanged, value); }
            remove { this.Events.RemoveHandler(EventDrawTabLevelChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="AutoScroll"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRLayoutCategory"), SRDescription("SRToolboxAutoScrollChangedDescr")]
        public event EventHandler AutoScrollChanged
        {
            add { this.Events.AddHandler(EventAutoScrollChanged, value); }
            remove { this.Events.RemoveHandler(EventAutoScrollChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="ItemHeight"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxItemHeightChangedDescr")]
        public event EventHandler ItemHeightChanged
        {
            add { this.Events.AddHandler(EventItemHeightChanged, value); }
            remove { this.Events.RemoveHandler(EventItemHeightChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="TabHeight"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabHeightChangedDescr")]
        public event EventHandler TabHeightChanged
        {
            add { this.Events.AddHandler(EventTabHeightChanged, value); }
            remove { this.Events.RemoveHandler(EventTabHeightChanged, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="TabFont"/> property value changes.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxAppearanceCategory"), SRDescription("SRToolboxTabFontChangedDescr")]
        public event EventHandler TabFontChanged
        {
            add { this.Events.AddHandler(EventTabFontChanged, value); }
            remove { this.Events.RemoveHandler(EventTabFontChanged, value); }
        }
        /// <summary>
        /// Occurs when the user selects an <see cref="Item"/> on the <see cref="Toolbox"/>.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxToolboxCategory"), SRDescription("SRToolboxSelectItemDescr")]
        public event EventHandler<ItemSelectionEventArgs> SelectItem
        {
            add { this.Events.AddHandler(EventSelectItem, value); }
            remove { this.Events.RemoveHandler(EventSelectItem, value); }
        }
        /// <summary>
        /// Occurs when the <see cref="Toolbox"/> is scrolled.
        /// </summary>
        [Browsable(true), SRCategory("SRToolboxToolboxCategory"), SRDescription("SRToolboxScrollDescr")]
        public event ScrollEventHandler Scroll
        {
            add { this.Events.AddHandler(EventScroll, value); }
            remove { this.Events.RemoveHandler(EventScroll, value); }
        }
        #endregion

        #region Event Fires
        /// <summary>
        /// Raises the <see cref="ItemHoverColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnItemHoverColorChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventItemHoverColorChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ItemSelectColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnItemSelectColorChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventItemSelectColorChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ItemSelectHoverColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnItemSelHoverColorChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventItemSelHoverColorChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="TabSelectColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTabSelectColorChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventTabSelectColorChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ItemSelectBorderColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnItemSelectBorderColorChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventItemSelectBorderColorChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="TabSelectBorderColorChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTabSelectBorderColorChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventTabSelectBorderColorChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="BackColorGradientStartChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnBackColorGradientStartChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventBackColorGradientStartChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="BackColorGradientEndChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnBackColorGradientEndChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventBackColorGradientEndChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="TabColorStyleChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTabColorStyleChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventTabColorStyleChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ShowAllChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnShowAllChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventShowAllChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ShowIconsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnShowIconsChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventShowIconsChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ShowPointerChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnShowPointerChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventShowPointerChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="AllowNestedTabsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnAllowNestedTabsChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventAllowNestedTabsChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="AllowToolboxItemsChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnAllowToolboxItemsChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventAllowToolboxItemsChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="DrawTabLevelChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnDrawTabLevelChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventDrawTabLevelChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="AutoScrollChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnAutoScrollChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventAutoScrollChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="ItemHeightChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnItemHeightChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventItemHeightChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="TabHeightChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTabHeightChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventTabHeightChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="TabFontChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTabFontChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[EventTabFontChanged];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="SelectItem"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ItemSelectionEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectItem(ItemSelectionEventArgs e)
        {
            EventHandler<ItemSelectionEventArgs> handler =
                    (EventHandler<ItemSelectionEventArgs>)this.Events[EventSelectItem];
            if(handler != null){
                handler(this, e);
            }
        }
        /// <summary>
        /// Raises the <see cref="Scroll"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ScrollEventArgs"/> that contains the scrolling event data.</param>
        protected virtual void OnScroll(ScrollEventArgs e)
        {
            ScrollEventHandler handler = (ScrollEventHandler)this.Events[EventScroll];
            if(handler != null){
                handler(this, e);
            }
        }
        #endregion

        #region ITab Members
        /// <summary>
        /// Gets the <see cref="ItemCollection">collection</see> of the <see cref="Item"/> objects contained in the <see cref="Toolbox"/>.
        /// </summary>
        [MergableProperty(false), SRCategory("SRToolboxCategory"), SRDescription("SRToolboxItemsDescr"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ItemCollection Items
        {
            get { return this._items; }
        }
        /// <summary>
        /// Gets the <see cref="TabCollection">collection</see> of the <see cref="Tab"/> objects contained in the <see cref="Toolbox"/>.
        /// </summary>
        [MergableProperty(false), SRCategory("SRToolboxCategory"), SRDescription("SRToolboxCategoriesDescr"),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TabCollection Categories
        {
            get { return this._tabs; }
        }
        ITab ITab.Owner
        {
            get { return null; }
        }
        #endregion
    }
}