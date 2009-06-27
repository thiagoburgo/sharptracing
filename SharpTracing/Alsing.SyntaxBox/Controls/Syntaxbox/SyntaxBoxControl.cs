// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *

#region using...
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using Alsing.Drawing.GDI;
using Alsing.SourceCode;
using Alsing.Windows.Forms.CoreLib;
using Alsing.Windows.Forms.SyntaxBox;

#endregion

namespace Alsing.Windows.Forms
{
    /// <summary>
    /// Syntaxbox control that can be used as a pure text editor or as a code editor when a syntaxfile is used.
    /// </summary>
    [Designer(typeof(SyntaxBoxDesigner), typeof(IDesigner))]
    public class SyntaxBoxControl : SplitViewParentControl
    {
        protected internal bool DisableAutoList;
        protected internal bool DisableFindForm;
        protected internal bool DisableInfoTip;
        protected internal bool DisableIntelliMouse;

        #region General Declarations
        private bool _AllowBreakPoints = true;
        private Color _BackColor = Color.White;
        private Color _BracketBackColor = Color.LightSteelBlue;
        private Color _BracketBorderColor = Color.DarkBlue;
        private Color _BracketForeColor = Color.Black;
        private bool _BracketMatching = true;
        private Color _BreakPointBackColor = Color.DarkRed;
        private Color _BreakPointForeColor = Color.White;
        private SyntaxDocument _Document;
        private string _FontName = "Courier new";
        private float _FontSize = 10f;
        private Color _GutterMarginBorderColor = SystemColors.ControlDark;
        private Color _GutterMarginColor = SystemColors.Control;
        private int _GutterMarginWidth = 19;
        private bool _HighLightActiveLine;
        private Color _HighLightedLineColor = Color.LightYellow;
        private Color _InactiveSelectionBackColor = SystemColors.ControlDark;
        private Color _InactiveSelectionForeColor = SystemColors.ControlLight;
        private IndentStyle _Indent = IndentStyle.LastRow;
        private KeyboardActionList _KeyboardActions = new KeyboardActionList();
        private Color _LineNumberBackColor = SystemColors.Window;
        private Color _LineNumberBorderColor = Color.Teal;
        private Color _LineNumberForeColor = Color.Teal;
        private Color _OutlineColor = SystemColors.ControlDark;
        private bool _ParseOnPaste;
        private Color _RevisionMarkAfterSave = Color.LimeGreen;
        private Color _RevisionMarkBeforeSave = Color.Gold;
        private Color _ScopeBackColor = Color.Transparent;
        private Color _ScopeIndicatorColor = Color.Transparent;
        private Color _SelectionBackColor = SystemColors.Highlight;
        private Color _SelectionForeColor = SystemColors.HighlightText;
        private Color _SeparatorColor = SystemColors.Control;
        private bool _ShowGutterMargin = true;
        private bool _ShowLineNumbers = true;
        private bool _ShowRevisionMarks = true;
        private bool _ShowTabGuides;
        private bool _ShowWhitespace;
        private int _SmoothScrollSpeed = 2;
        private Color _TabGuideColor = ControlPaint.Light(SystemColors.ControlLight);
        private int _TabSize = 4;
        private int _TooltipDelay = 240;
        private bool _VirtualWhitespace;
        private Color _WhitespaceColor = SystemColors.ControlDark;
        private IContainer components;
        #endregion

        #region Internal Components/Controls
        private ImageList _AutoListIcons;
        private ImageList _GutterIcons;
        private Timer ParseTimer;
        #endregion

        #region Public Events
        /// <summary>
        /// An event that is fired when the cursor hovers a pattern;
        /// </summary>
        public event WordMouseHandler WordMouseHover = null;
        /// <summary>
        /// An event that is fired when the cursor hovers a pattern;
        /// </summary>
        public event WordMouseHandler WordMouseDown = null;
        /// <summary>
        /// An event that is fired when the control has updated the clipboard
        /// </summary>
        public event CopyHandler ClipboardUpdated = null;
        /// <summary>
        /// Event fired when the caret of the active view have moved.
        /// </summary>
        public event EventHandler CaretChange = null;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler SelectionChange = null;
        /// <summary>
        /// Event fired when the user presses the up or the down button on the infotip.
        /// </summary>
        public event EventHandler InfoTipSelectedIndexChanged = null;
        /// <summary>
        /// Event fired when a row is rendered.
        /// </summary>
        public event RowPaintHandler RenderRow = null;
        /// <summary>
        /// An event that is fired when mouse down occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseDown = null;
        /// <summary>
        /// An event that is fired when mouse move occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseMove = null;
        /// <summary>
        /// An event that is fired when mouse up occurs on a row
        /// </summary>
        public event RowMouseHandler RowMouseUp = null;
        /// <summary>
        /// An event that is fired when a click occurs on a row
        /// </summary>
        public event RowMouseHandler RowClick = null;
        /// <summary>
        /// An event that is fired when a double click occurs on a row
        /// </summary>
        public event RowMouseHandler RowDoubleClick = null;
        #endregion //END PUBLIC EGENTS

        #region Public Properties

        #region PUBLIC PROPERTY SHOWEOLMARKER
        private bool _ShowEOLMarker;
        [Category("Appearance"), Description("Determines if a ¶ should be displayed at the end of a line"),
         DefaultValue(false)]
        public bool ShowEOLMarker
        {
            get { return this._ShowEOLMarker; }
            set
            {
                this._ShowEOLMarker = value;
                this.Redraw();
            }
        }
        #endregion

        #region PUBLIC PROPERTY EOLMARKERCOLOR
        private Color _EOLMarkerColor = Color.Red;
        [Category("Appearance"), Description("The color of the EOL marker"), DefaultValue(typeof(Color), "Red")]
        public Color EOLMarkerColor
        {
            get { return this._EOLMarkerColor; }
            set
            {
                this._EOLMarkerColor = value;
                this.Redraw();
            }
        }
        #endregion

        #region PUBLIC PROPERTY AUTOLISTAUTOSELECT
        private bool _AutoListAutoSelect = true;
        [DefaultValue(true)]
        public bool AutoListAutoSelect
        {
            get { return this._AutoListAutoSelect; }
            set { this._AutoListAutoSelect = value; }
        }
        #endregion

        #region PUBLIC PROPERTY COPYASRTF
        [Category("Behavior - Clipboard"), Description("determines if the copy actions should be stored as RTF"),
         DefaultValue(typeof(Color), "false")]
        public bool CopyAsRTF { get; set; }
        #endregion

        private bool _CollapsedBlockTooltipsEnabled = true;
        [Category("Appearance - Scopes"), Description("The color of the active scope"),
         DefaultValue(typeof(Color), "Transparent")]
        public Color ScopeBackColor
        {
            get { return this._ScopeBackColor; }
            set
            {
                this._ScopeBackColor = value;
                this.Redraw();
            }
        }
        [Category("Appearance - Scopes"), Description("The color of the scope indicator"),
         DefaultValue(typeof(Color), "Transparent")]
        public Color ScopeIndicatorColor
        {
            get { return this._ScopeIndicatorColor; }
            set
            {
                this._ScopeIndicatorColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Positions the AutoList
        /// </summary>
        [Category("Behavior"), Browsable(false)]
        public TextPoint AutoListPosition
        {
            get { return ((EditViewControl)this._ActiveView).AutoListPosition; }
            set
            {
                if((this._ActiveView) == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).AutoListPosition = value;
            }
        }
        /// <summary>
        /// Positions the InfoTip
        /// </summary>
        [Category("Behavior"), Browsable(false)]
        public TextPoint InfoTipPosition
        {
            get { return ((EditViewControl)this._ActiveView).InfoTipPosition; }
            set
            {
                if((this._ActiveView) == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).InfoTipPosition = value;
            }
        }
        /// <summary>
        /// Determines if the revision marks should be visible.
        /// </summary>
        [Category("Appearance - Revision Marks"), Description("Determines if the revision marks should be visible"),
         DefaultValue(true)]
        public bool ShowRevisionMarks
        {
            get { return this._ShowRevisionMarks; }
            set
            {
                this._ShowRevisionMarks = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Prevents the control from changing the cursor.
        /// </summary>
        [Description("Prevents the control from changing the cursor."), Category("Appearance"), Browsable(false)]
        public bool LockCursorUpdate { get; set; }
        /// <summary>
        /// The row padding in pixels.
        /// </summary>
        [Category("Appearance"), Description("The number of pixels to add between rows"), DefaultValue(0)]
        public int RowPadding { get; set; }
        /// <summary>
        /// The selected index in the infotip.
        /// </summary>
        [Category("Appearance - Infotip"), Description("The currently active selection in the infotip"),
         Browsable(false)]
        public int InfoTipSelectedIndex
        {
            get { return ((EditViewControl)this._ActiveView).InfoTip.SelectedIndex; }
            set
            {
                if((this._ActiveView) == null || ((EditViewControl)this._ActiveView).InfoTip == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).InfoTip.SelectedIndex = value;
            }
        }
        /// <summary>
        /// Gets or Sets the image used in the infotip.
        /// </summary>
        [Category("Appearance - InfoTip"), Description("An image to show in the infotip"), DefaultValue(null)]
        public Image InfoTipImage
        {
            get { return ((EditViewControl)this._ActiveView).InfoTip.Image; }
            set
            {
                if((this._ActiveView) == null || ((EditViewControl)this._ActiveView).InfoTip == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).InfoTip.Image = value;
            }
        }
        /// <summary>
        /// Get or Sets the number of choices that could be made in the infotip.
        /// </summary>
        [Category("Appearance"), Description("Get or Sets the number of choices that could be made in the infotip"),
         Browsable(false)]
        public int InfoTipCount
        {
            get { return ((EditViewControl)this._ActiveView).InfoTip.Count; }
            set
            {
                if((this._ActiveView) == null || ((EditViewControl)this._ActiveView).InfoTip == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).InfoTip.Count = value;
                ((EditViewControl)this._ActiveView).InfoTip.Init();
            }
        }
        /// <summary>
        /// The text in the Infotip.
        /// </summary>
        /// <remarks><br/>
        /// The text uses a HTML like syntax.<br/>
        /// <br/>
        /// Supported tags are:<br/>
        /// <br/>
        /// &lt;Font Size="Size in Pixels" Face="Font Name" Color="Named color" &gt;&lt;/Font&gt; Set Font size,color and fontname.<br/>
        /// &lt;HR&gt; : Inserts a horizontal separator line.<br/>
        /// &lt;BR&gt; : Line break.<br/>
        /// &lt;B&gt;&lt;/B&gt; : Activate/Deactivate Bold style.<br/>
        /// &lt;I&gt;&lt;/I&gt; : Activate/Deactivate Italic style.<br/>
        /// &lt;U&gt;&lt;/U&gt; : Activate/Deactivate Underline style.	<br/>			
        /// </remarks>	
        /// <example >
        /// <code>
        /// MySyntaxBox.InfoTipText="public void MyMethod ( &lt;b&gt; string text &lt;/b&gt; );"; 		
        /// </code>
        /// </example>	
        [Category("Appearance - InfoTip"), Description("The infotip text"), DefaultValue("")]
        public string InfoTipText
        {
            get { return ((EditViewControl)this._ActiveView).InfoTip.Data; }
            set
            {
                if((this._ActiveView) == null || ((EditViewControl)this._ActiveView).InfoTip == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).InfoTip.Data = value;
            }
        }
        /// <summary>
        /// Gets the Selection object from the active view.
        /// </summary>
        [Browsable(false)]
        public Selection Selection
        {
            get
            {
                if((this._ActiveView) != null){
                    return ((EditViewControl)this._ActiveView).Selection;
                }
                return null;
            }
        }
        /// <summary>
        /// Collection of KeyboardActions that is used by the control.
        /// Keyboard actions to add shortcut key combinations to certain tasks.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public KeyboardActionList KeyboardActions
        {
            get { return this._KeyboardActions; }
            set { this._KeyboardActions = value; }
        }
        /// <summary>
        /// Gets or Sets if the AutoList is visible in the active view.
        /// </summary>
        [Category("Appearance"), Description("Gets or Sets if the AutoList is visible in the active view."),
         Browsable(false)]
        public bool AutoListVisible
        {
            get { return (this._ActiveView) != null && ((EditViewControl)this._ActiveView).AutoListVisible; }
            set
            {
                if((this._ActiveView) != null){
                    ((EditViewControl)this._ActiveView).AutoListVisible = value;
                }
            }
        }
        /// <summary>
        /// Gets or Sets if the InfoTip is visible in the active view.
        /// </summary>
        [Category("Appearance"), Description("Gets or Sets if the InfoTip is visible in the active view."),
         Browsable(false)]
        public bool InfoTipVisible
        {
            get { return (this._ActiveView) != null && ((EditViewControl)this._ActiveView).InfoTipVisible; }
            set
            {
                if((this._ActiveView) != null){
                    ((EditViewControl)this._ActiveView).InfoTipVisible = value;
                }
            }
        }
        /// <summary>
        /// Gets if the control can perform a Copy action.
        /// </summary>
        [Browsable(false)]
        public bool CanCopy
        {
            get { return ((EditViewControl)this._ActiveView).CanCopy; }
        }
        /// <summary>
        /// Gets if the control can perform a Paste action.
        /// (if the clipboard contains a valid text).
        /// </summary>
        [Browsable(false)]
        public bool CanPaste
        {
            get { return ((EditViewControl)this._ActiveView).CanPaste; }
        }
        /// <summary>
        /// Gets if the control can perform a ReDo action.
        /// </summary>
        [Browsable(false)]
        public bool CanRedo
        {
            get { return ((EditViewControl)this._ActiveView).CanRedo; }
        }
        /// <summary>
        /// Gets if the control can perform an Undo action.
        /// </summary>
        [Browsable(false)]
        public bool CanUndo
        {
            get { return ((EditViewControl)this._ActiveView).CanUndo; }
        }
        /// <summary>
        /// Gets or Sets the imagelist to use in the gutter margin.
        /// </summary>
        /// <remarks>
        /// Image Index 0 is used to display the Breakpoint icon.
        /// Image Index 1 is used to display the Bookmark icon.
        /// </remarks>		
        [Category("Appearance - Gutter Margin"), Description("Gets or Sets the imagelist to use in the gutter margin.")]
        public ImageList GutterIcons
        {
            get { return this._GutterIcons; }
            set
            {
                this._GutterIcons = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the imagelist to use in the autolist.
        /// </summary>
        [Category("Appearance"), Description("Gets or Sets the imagelist to use in the autolist."), DefaultValue(null)]
        public ImageList AutoListIcons
        {
            get { return this._AutoListIcons; }
            set
            {
                this._AutoListIcons = value;
                foreach(EditViewControl ev in this.Views){
                    if(ev != null && ev.AutoList != null){
                        ev.AutoList.Images = value;
                    }
                }
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the color to use when rendering Tab guides.
        /// </summary>
        [Category("Appearance - Tabs"), Description("Gets or Sets the color to use when rendering Tab guides."),
         DefaultValue(typeof(Color), "Control")]
        public Color TabGuideColor
        {
            get { return this._TabGuideColor; }
            set
            {
                this._TabGuideColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the color of the bracket match borders.
        /// </summary>
        /// <remarks>
        /// NOTE: use Color.Transparent to turn off the bracket match borders.
        /// </remarks>
        [Category("Appearance - Bracket Match"), Description("Gets or Sets the color of the bracket match borders."),
         DefaultValue(typeof(Color), "DarkBlue")]
        public Color BracketBorderColor
        {
            get { return this._BracketBorderColor; }
            set
            {
                this._BracketBorderColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets if the control should render Tab guides.
        /// </summary>
        [Category("Appearance - Tabs"), Description("Gets or Sets if the control should render Tab guides."),
         DefaultValue(false)]
        public bool ShowTabGuides
        {
            get { return this._ShowTabGuides; }
            set
            {
                this._ShowTabGuides = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the color to use when rendering whitespace characters
        /// </summary>
        [Category("Appearance"), Description("Gets or Sets the color to use when rendering whitespace characters."),
         DefaultValue(typeof(Color), "Control")]
        public Color WhitespaceColor
        {
            get { return this._WhitespaceColor; }
            set
            {
                this._WhitespaceColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the color of the code Outlining (both folding lines and collapsed blocks).
        /// </summary>
        [Category("Appearance"),
         Description("Gets or Sets the color of the code Outlining (both folding lines and collapsed blocks)."),
         DefaultValue(typeof(Color), "ControlDark")]
        public Color OutlineColor
        {
            get { return this._OutlineColor; }
            set
            {
                this._OutlineColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Determines if the control should use a smooth scroll when scrolling one row up or down.
        /// </summary>
        [Category("Behavior"),
         Description("Determines if the control should use a smooth scroll when scrolling one row up or down."),
         DefaultValue(typeof(Color), "False")]
        public bool SmoothScroll { get; set; }
        /// <summary>
        /// Gets or Sets the speed of the vertical scroll when SmoothScroll is activated
        /// </summary>
        [Category("Behavior"),
         Description("Gets or Sets the speed of the vertical scroll when SmoothScroll is activated"), DefaultValue(2)]
        public int SmoothScrollSpeed
        {
            get { return this._SmoothScrollSpeed; }
            set
            {
                if(value <= 0){
                    throw (new Exception("Scroll speed may not be less than 1"));
                }
                this._SmoothScrollSpeed = value;
            }
        }
        /// <summary>
        /// Gets or Sets if the control can display breakpoints or not.
        /// </summary>
        [Category("Behavior"), Description("Gets or Sets if the control can display breakpoints or not."),
         DefaultValue(true)]
        public bool AllowBreakPoints
        {
            get { return this._AllowBreakPoints; }
            set { this._AllowBreakPoints = value; }
        }
        /// <summary>
        /// Gets or Sets the RevisionMarkBeforeSave Color to use for modified rows.
        /// </summary>
        [Category("Appearance - Revision Marks"),
         Description("The color to use for revision mark when line is modified"), DefaultValue(typeof(Color), "Gold")]
        public Color RevisionMarkBeforeSave
        {
            get { return this._RevisionMarkBeforeSave; }
            set
            {
                this._RevisionMarkBeforeSave = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the RevisionMarkAfterSave Color to use for saved rows.
        /// </summary>
        [Category("Appearance - Revision Marks"), Description("The color to use for revision mark when line is saved"),
         DefaultValue(typeof(Color), "LimeGreen")]
        public Color RevisionMarkAfterSave
        {
            get { return this._RevisionMarkAfterSave; }
            set
            {
                this._RevisionMarkAfterSave = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets if the control should perform a full parse of the document when content is drag dropped or pasted into the control
        /// </summary>
        [Category("Behavior - Clipboard"),
         Description(
                 "Gets or Sets if the control should perform a full parse of the document when content is drag dropped or pasted into the control"
                 ), DefaultValue(false)]
        public bool ParseOnPaste
        {
            get { return this._ParseOnPaste; }
            set
            {
                this._ParseOnPaste = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the Size of the font.
        /// <seealso cref="FontName"/>
        /// </summary>
        [Category("Appearance - Font"), Description("The size of the font"), DefaultValue(10f)]
        public float FontSize
        {
            get { return this._FontSize; }
            set
            {
                this._FontSize = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Determines what indentstyle to use on a new line.
        /// </summary>
        [Category("Behavior"), Description("Determines how the the control indents a new line"),
         DefaultValue(IndentStyle.LastRow)]
        public IndentStyle Indent
        {
            get { return this._Indent; }
            set { this._Indent = value; }
        }
        /// <summary>
        /// Gets or Sets the SyntaxDocument the control is currently attatched to.
        /// </summary>
        [Category("Content"), Description("The SyntaxDocument that is attatched to the contro")]
        public SyntaxDocument Document
        {
            get { return this._Document; }
            set { this.AttachDocument(value); }
        }
        /// <summary>
        /// Get or Set the delay before the tooltip is displayed over a collapsed block
        /// </summary>
        [Category("Behavior"), Description("The delay before the tooltip is displayed over a collapsed block"),
         DefaultValue(240)]
        public int TooltipDelay
        {
            get { return this._TooltipDelay; }
            set { this._TooltipDelay = value; }
        }
        // ROB: Added property to turn collapsed block tooltips on and off.
        /// <summary>
        /// Get or Set whether or not tooltips will be deplayed for collapsed blocks.
        /// </summary>
        [Category("Behavior"), Description("The delay before the tooltip is displayed over a collapsed block"),
         DefaultValue(true)]
        public bool CollapsedBlockTooltipsEnabled
        {
            get { return this._CollapsedBlockTooltipsEnabled; }
            set { this._CollapsedBlockTooltipsEnabled = value; }
        }
        // END-ROB ----------------------------------------------------------
        /// <summary>
        /// Get or Set the delay before the tooltip is displayed over a collapsed block
        /// </summary>
        [Category("Behavior"), Description("Determines if the control is readonly or not"), DefaultValue(false)]
        public bool ReadOnly { get; set; }
        /// <summary>
        /// Gets or Sets the name of the font.
        /// <seealso cref="FontSize"/>
        /// </summary>
        [Category("Appearance - Font"), Description("The name of the font that is used to render the control"),
         Editor(typeof(FontList), typeof(UITypeEditor)), DefaultValue("Courier New")]
        public string FontName
        {
            get { return this._FontName; }
            set
            {
                if(this.Views == null){
                    return;
                }
                this._FontName = value;
                this.InitGraphics();
                foreach(EditViewControl evc in this.Views){
                    evc.CalcMaxCharWidth();
                }
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets if bracketmatching is active
        /// <seealso cref="BracketForeColor"/>
        /// <seealso cref="BracketBackColor"/>
        /// </summary>
        [Category("Appearance - Bracket Match"),
         Description("Determines if the control should highlight scope patterns"), DefaultValue(true)]
        public bool BracketMatching
        {
            get { return this._BracketMatching; }
            set
            {
                this._BracketMatching = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets if Virtual Whitespace is active.
        /// <seealso cref="ShowWhitespace"/>
        /// </summary>
        [Category("Behavior"), Description("Determines if virtual Whitespace is active"), DefaultValue(false)]
        public bool VirtualWhitespace
        {
            get { return this._VirtualWhitespace; }
            set
            {
                this._VirtualWhitespace = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the separator Color.
        /// <seealso cref="BracketMatching"/>
        /// <seealso cref="BracketBackColor"/>
        /// </summary>
        [Category("Appearance"), Description("The separator color"), DefaultValue(typeof(Color), "Control")]
        public Color SeparatorColor
        {
            get { return this._SeparatorColor; }
            set
            {
                this._SeparatorColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the foreground Color to use when BracketMatching is activated.
        /// <seealso cref="BracketMatching"/>
        /// <seealso cref="BracketBackColor"/>
        /// </summary>
        [Category("Appearance - Bracket Match"),
         Description("The foreground color to use when BracketMatching is activated"),
         DefaultValue(typeof(Color), "Black")]
        public Color BracketForeColor
        {
            get { return this._BracketForeColor; }
            set
            {
                this._BracketForeColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the background Color to use when BracketMatching is activated.
        /// <seealso cref="BracketMatching"/>
        /// <seealso cref="BracketForeColor"/>
        /// </summary>
        [Category("Appearance - Bracket Match"),
         Description("The background color to use when BracketMatching is activated"),
         DefaultValue(typeof(Color), "LightSteelBlue")]
        public Color BracketBackColor
        {
            get { return this._BracketBackColor; }
            set
            {
                this._BracketBackColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// The inactive selection background color.
        /// </summary>
        [Category("Appearance - Selection"), Description("The inactive selection background color."),
         DefaultValue(typeof(Color), "ControlDark")]
        public Color InactiveSelectionBackColor
        {
            get { return this._InactiveSelectionBackColor; }
            set
            {
                this._InactiveSelectionBackColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// The inactive selection foreground color.
        /// </summary>
        [Category("Appearance - Selection"), Description("The inactive selection foreground color."),
         DefaultValue(typeof(Color), "ControlLight")]
        public Color InactiveSelectionForeColor
        {
            get { return this._InactiveSelectionForeColor; }
            set
            {
                this._InactiveSelectionForeColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// The selection background color.
        /// </summary>
        [Category("Appearance - Selection"), Description("The selection background color."),
         DefaultValue(typeof(Color), "Highlight")]
        public Color SelectionBackColor
        {
            get { return this._SelectionBackColor; }
            set
            {
                this._SelectionBackColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// The selection foreground color.
        /// </summary>
        [Category("Appearance - Selection"), Description("The selection foreground color."),
         DefaultValue(typeof(Color), "HighlightText")]
        public Color SelectionForeColor
        {
            get { return this._SelectionForeColor; }
            set
            {
                this._SelectionForeColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the border Color of the gutter margin.
        /// <seealso cref="GutterMarginColor"/>
        /// </summary>
        [Category("Appearance - Gutter Margin"), Description("The border color of the gutter margin"),
         DefaultValue(typeof(Color), "ControlDark")]
        public Color GutterMarginBorderColor
        {
            get { return this._GutterMarginBorderColor; }
            set
            {
                this._GutterMarginBorderColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the border Color of the line number margin
        /// <seealso cref="LineNumberForeColor"/>
        /// <seealso cref="LineNumberBackColor"/>
        /// </summary>
        [Category("Appearance - Line Numbers"), Description("The border color of the line number margin"),
         DefaultValue(typeof(Color), "Teal")]
        public Color LineNumberBorderColor
        {
            get { return this._LineNumberBorderColor; }
            set
            {
                this._LineNumberBorderColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the foreground Color of a Breakpoint.
        /// <seealso cref="BreakPointBackColor"/>
        /// </summary>
        [Category("Appearance - BreakPoints"), Description("The foreground color of a Breakpoint"),
         DefaultValue(typeof(Color), "White")]
        public Color BreakPointForeColor
        {
            get { return this._BreakPointForeColor; }
            set
            {
                this._BreakPointForeColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the background Color to use for breakpoint rows.
        /// <seealso cref="BreakPointForeColor"/>
        /// </summary>
        [Category("Appearance - BreakPoints"),
         Description("The background color to use when BracketMatching is activated"),
         DefaultValue(typeof(Color), "DarkRed")]
        public Color BreakPointBackColor
        {
            get { return this._BreakPointBackColor; }
            set
            {
                this._BreakPointBackColor = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the foreground Color of line numbers.
        /// <seealso cref="LineNumberBorderColor"/>
        /// <seealso cref="LineNumberBackColor"/>
        /// </summary>
        [Category("Appearance - Line Numbers"), Description("The foreground color of line numbers"),
         DefaultValue(typeof(Color), "Teal")]
        public Color LineNumberForeColor
        {
            get { return this._LineNumberForeColor; }
            set
            {
                this._LineNumberForeColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the background Color of line numbers.
        /// <seealso cref="LineNumberForeColor"/>
        /// <seealso cref="LineNumberBorderColor"/>
        /// </summary>
        [Category("Appearance - Line Numbers"), Description("The background color of line numbers"),
         DefaultValue(typeof(Color), "Window")]
        public Color LineNumberBackColor
        {
            get { return this._LineNumberBackColor; }
            set
            {
                this._LineNumberBackColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the Color of the gutter margin
        /// <seealso cref="GutterMarginBorderColor"/>
        /// </summary>
        [Category("Appearance - Gutter Margin"), Description("The color of the gutter margin"),
         DefaultValue(typeof(Color), "Control")]
        public Color GutterMarginColor
        {
            get { return this._GutterMarginColor; }
            set
            {
                this._GutterMarginColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the background Color of the client area.
        /// </summary>
        [Category("Appearance"), Description("The background color of the client area"),
         DefaultValue(typeof(Color), "Window")]
        public new Color BackColor
        {
            get { return this._BackColor; }
            set
            {
                this._BackColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the background Color of the active line.
        /// <seealso cref="HighLightActiveLine"/>
        /// </summary>
        [Category("Appearance - Active Line"), Description("The background color of the active line"),
         DefaultValue(typeof(Color), "LightYellow")]
        public Color HighLightedLineColor
        {
            get { return this._HighLightedLineColor; }
            set
            {
                this._HighLightedLineColor = value;
                this.InitGraphics();
                this.Redraw();
            }
        }
        /// <summary>
        /// Determines if the active line should be highlighted.
        /// </summary>
        [Category("Appearance - Active Line"), Description("Determines if the active line should be highlighted"),
         DefaultValue(false)]
        public bool HighLightActiveLine
        {
            get { return this._HighLightActiveLine; }
            set
            {
                this._HighLightActiveLine = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Determines if Whitespace should be rendered as symbols.
        /// </summary>
        [Category("Appearance"), Description("Determines if Whitespace should be rendered as symbols"),
         DefaultValue(false)]
        public bool ShowWhitespace
        {
            get { return this._ShowWhitespace; }
            set
            {
                this._ShowWhitespace = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Determines if the line number margin should be visible.
        /// </summary>
        [Category("Appearance - Line Numbers"), Description("Determines if the line number margin should be visible"),
         DefaultValue(true)]
        public bool ShowLineNumbers
        {
            get { return this._ShowLineNumbers; }
            set
            {
                this._ShowLineNumbers = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Determines if the gutter margin should be visible.
        /// </summary>
        [Category("Appearance - Gutter Margin"), Description("Determines if the gutter margin should be visible"),
         DefaultValue(true)]
        public bool ShowGutterMargin
        {
            get { return this._ShowGutterMargin; }
            set
            {
                this._ShowGutterMargin = value;
                this.Redraw();
            }
        }
        /// <summary>
        /// Gets or Sets the witdth of the gutter margin in pixels.
        /// </summary>
        [Category("Appearance - Gutter Margin"), Description("Determines the width of the gutter margin in pixels"),
         DefaultValue(19)]
        public int GutterMarginWidth
        {
            get { return this._GutterMarginWidth; }
            set
            {
                this._GutterMarginWidth = value;
                this.Redraw();
            }
        }
        // ROB: Added .TabsToSpaces property.
        /// <summary>
        /// Gets or Sets the 'Tabs To Spaces' feature of the editor.
        /// </summary>
        [Category("Appearance - Tabs"),
         Description("Determines whether or not the SyntaxBox converts tabs to spaces as you type."),
         DefaultValue(false)]
        public bool TabsToSpaces { get; set; }
        /// <summary>
        /// Get or Sets the size of a TAB char in number of SPACES.
        /// </summary>
        [Category("Appearance - Tabs"), Description("Determines the size of a TAB in number of SPACE chars"),
         DefaultValue(4)]
        public int TabSize
        {
            get { return this._TabSize; }
            set
            {
                this._TabSize = value;
                this.Redraw();
            }
        }

        #region PUBLIC PROPERTY SHOWSCOPEINDICATOR
        private bool _ShowScopeIndicator;
        [Category("Appearance - Scopes"), Description("Determines if the scope indicator should be shown"),
         DefaultValue(true)]
        public bool ShowScopeIndicator
        {
            get { return this._ShowScopeIndicator; }
            set
            {
                this._ShowScopeIndicator = value;
                this.Redraw();
            }
        }
        #endregion

        // END-ROB
        // ROB: Added method: ConvertTabsToSpaces()
        /// <summary>
        /// Converts all tabs to spaces the size of .TabSize in the Document.
        /// </summary>
        public void ConvertTabsToSpaces()
        {
            if(this._Document != null){
                this._Document.StartUndoCapture();
                var spaces = new string(' ', this._TabSize);
                // Iterate all rows and convert tabs to spaces.
                for(int count = 0; count < this._Document.Count; count++){
                    Row row = this._Document[count];
                    string rowText = row.Text;
                    string newText = rowText.Replace("\t", spaces);
                    // If this has made a change to the row, update it.
                    if(newText != rowText){
                        this._Document.DeleteRange(new TextRange(0, count, rowText.Length, count));
                        this._Document.InsertText(newText, 0, count, true);
                    }
                }
                this._Document.EndUndoCapture();
            }
        }
        // END-ROB
        // ROB: Added method: ConvertSpacesToTabs()
        /// <summary>
        /// Converts all spaces the size of .TabSize in the Document to tabs.
        /// </summary>
        public void ConvertSpacesToTabs()
        {
            if(this._Document != null){
                this._Document.StartUndoCapture();
                var spaces = new string(' ', this._TabSize);
                // Iterate all rows and convert tabs to spaces.
                for(int count = 0; count < this._Document.Count; count++){
                    Row row = this._Document[count];
                    string rowText = row.Text;
                    string newText = rowText.Replace(spaces, "\t");
                    // If this has made a change to the row, update it.
                    if(newText != rowText){
                        this._Document.DeleteRange(new TextRange(0, count, rowText.Length - 1, count));
                        this._Document.InsertText(newText, 0, count, true);
                    }
                }
                this._Document.EndUndoCapture();
            }
        }
        // END-ROB
        #endregion // PUBLIC PROPERTIES

        #region Public Methods
        /// <summary>
        /// Gets the Caret object from the active view.
        /// </summary>
        [Browsable(false)]
        public Caret Caret
        {
            get
            {
                if((this._ActiveView) != null){
                    return ((EditViewControl)this._ActiveView).Caret;
                }
                return null;
            }
        }
        public void ScrollIntoView(int RowIndex)
        {
            ((EditViewControl)this._ActiveView).ScrollIntoView(RowIndex);
        }
        /// <summary>
        /// Disables painting while loading data into the Autolist
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <example>
        /// <code>
        /// MySyntaxBox.AutoListClear();
        /// MySyntaxBox.AutoListBeginLoad();
        /// MySyntaxBox.AutoListAdd ("test",1);
        /// MySyntaxBox.AutoListAdd ("test",2);
        /// MySyntaxBox.AutoListAdd ("test",3);
        /// MySyntaxBox.AutoListAdd ("test",4);
        /// MySyntaxBox.AutoListEndLoad();
        /// </code>
        /// </example>
        public void AutoListBeginLoad()
        {
            ((EditViewControl)this._ActiveView).AutoListBeginLoad();
        }
        /// <summary>
        /// Resumes painting and autosizes the Autolist.			
        /// </summary>		
        public void AutoListEndLoad()
        {
            ((EditViewControl)this._ActiveView).AutoListEndLoad();
        }
        /// <summary>
        /// Clears the content in the autolist.
        /// </summary>
        public void AutoListClear()
        {
            ((EditViewControl)this._ActiveView).AutoList.Clear();
        }
        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <example>
        /// <code>
        /// MySyntaxBox.AutoListClear();
        /// MySyntaxBox.AutoListBeginLoad();
        /// MySyntaxBox.AutoListAdd ("test",1);
        /// MySyntaxBox.AutoListAdd ("test",2);
        /// MySyntaxBox.AutoListAdd ("test",3);
        /// MySyntaxBox.AutoListAdd ("test",4);
        /// MySyntaxBox.AutoListEndLoad();
        /// </code>
        /// </example>
        /// <param name="text">The text to display in the autolist</param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string text, int ImageIndex)
        {
            ((EditViewControl)this._ActiveView).AutoList.Add(text, ImageIndex);
        }
        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <param name="text">The text to display in the autolist</param>
        /// <param name="InsertText">The text to insert in the code</param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string text, string InsertText, int ImageIndex)
        {
            ((EditViewControl)this._ActiveView).AutoList.Add(text, InsertText, ImageIndex);
        }
        /// <summary>
        /// Adds an item to the autolist control.
        /// </summary>
        /// <param name="text">The text to display in the autolist</param>
        /// <param name="InsertText">The text to insert in the code</param>
        /// <param name="ToolTip"></param>
        /// <param name="ImageIndex">The image index in the AutoListIcons</param>
        public void AutoListAdd(string text, string InsertText, string ToolTip, int ImageIndex)
        {
            ((EditViewControl)this._ActiveView).AutoList.Add(text, InsertText, ToolTip, ImageIndex);
        }
        /// <summary>
        /// Converts a Client pixel coordinate into a TextPoint (Column/Row)
        /// </summary>
        /// <param name="x">Pixel x position</param>
        /// <param name="y">Pixel y position</param>
        /// <returns>The row and column at the given pixel coordinate.</returns>
        public TextPoint CharFromPixel(int x, int y)
        {
            return ((EditViewControl)this._ActiveView).CharFromPixel(x, y);
        }
        /// <summary>
        /// Clears the selection in the active view.
        /// </summary>
        public void ClearSelection()
        {
            ((EditViewControl)this._ActiveView).ClearSelection();
        }
        /// <summary>
        /// Executes a Copy action on the selection in the active view.
        /// </summary>
        public void Copy()
        {
            ((EditViewControl)this._ActiveView).Copy();
        }
        /// <summary>
        /// Executes a Cut action on the selection in the active view.
        /// </summary>
        public void Cut()
        {
            ((EditViewControl)this._ActiveView).Cut();
        }
        /// <summary>
        /// Executes a Delete action on the selection in the active view.
        /// </summary>
        public void Delete()
        {
            ((EditViewControl)this._ActiveView).Delete();
        }
        /// <summary>
        /// Moves the caret of the active view to a specific row.
        /// </summary>
        /// <param name="RowIndex">the row to jump to</param>
        public void GotoLine(int RowIndex)
        {
            ((EditViewControl)this._ActiveView).GotoLine(RowIndex);
        }
        /// <summary>
        /// Moves the caret of the active view to the next bookmark.
        /// </summary>
        public void GotoNextBookmark()
        {
            ((EditViewControl)this._ActiveView).GotoNextBookmark();
        }
        /// <summary>
        /// Moves the caret of the active view to the previous bookmark.
        /// </summary>
        public void GotoPreviousBookmark()
        {
            ((EditViewControl)this._ActiveView).GotoPreviousBookmark();
        }
        /// <summary>
        /// Takes a pixel position and returns true if that position is inside the selected text.
        /// 
        /// </summary>
        /// <param name="x">Pixel x position.</param>
        /// <param name="y">Pixel y position</param>
        /// <returns>true if the position is inside the selection.</returns>
        public bool IsOverSelection(int x, int y)
        {
            return ((EditViewControl)this._ActiveView).IsOverSelection(x, y);
        }
        /// <summary>
        /// Execute a Paste action if possible.
        /// </summary>
        public void Paste()
        {
            ((EditViewControl)this._ActiveView).Paste();
        }
        /// <summary>
        /// Execute a ReDo action if possible.
        /// </summary>
        public void Redo()
        {
            ((EditViewControl)this._ActiveView).Redo();
        }
        /// <summary>
        /// Makes the caret in the active view visible on screen.
        /// </summary>
        public void ScrollIntoView()
        {
            ((EditViewControl)this._ActiveView).ScrollIntoView();
        }
        /// <summary>
        /// Scrolls the active view to a specific position.
        /// </summary>
        /// <param name="Pos"></param>
        public void ScrollIntoView(TextPoint Pos)
        {
            ((EditViewControl)this._ActiveView).ScrollIntoView(Pos);
        }
        /// <summary>
        /// Select all the text in the active view.
        /// </summary>
        public void SelectAll()
        {
            ((EditViewControl)this._ActiveView).SelectAll();
        }
        /// <summary>
        /// Selects the next word (from the current caret position) that matches the parameter criterias.
        /// </summary>
        /// <param name="Pattern">The pattern to find</param>
        /// <param name="MatchCase">Match case , true/false</param>
        /// <param name="WholeWords">Match whole words only , true/false</param>
        /// <param name="UseRegEx">To be implemented</param>
        public void FindNext(string Pattern, bool MatchCase, bool WholeWords, bool UseRegEx)
        {
            ((EditViewControl)this._ActiveView).SelectNext(Pattern, MatchCase, WholeWords, UseRegEx);
        }
        /// <summary>
        /// Finds the next occurance of the pattern in the find/replace dialog
        /// </summary>
        public void FindNext()
        {
            ((EditViewControl)this._ActiveView).FindNext();
        }
        /// <summary>
        /// Shows the default GotoLine dialog.
        /// </summary>
        /// <example>
        /// <code>
        /// //Display the Goto Line dialog
        /// MySyntaxBox.ShowGotoLine();
        /// </code>
        /// </example>
        public void ShowGotoLine()
        {
            ((EditViewControl)this._ActiveView).ShowGotoLine();
        }
        /// <summary>
        /// Not yet implemented
        /// </summary>
        public void ShowSettings()
        {
            ((EditViewControl)this._ActiveView).ShowSettings();
        }
        /// <summary>
        /// Toggles a bookmark on the active row of the active view.
        /// </summary>
        public void ToggleBookmark()
        {
            ((EditViewControl)this._ActiveView).ToggleBookmark();
        }
        /// <summary>
        /// Executes an undo action if possible.
        /// </summary>
        public void Undo()
        {
            ((EditViewControl)this._ActiveView).Undo();
        }
        /// <summary>
        /// Shows the Find dialog
        /// </summary>
        /// <example>
        /// <code>
        /// //Show FindReplace dialog
        /// MySyntaxBox.ShowFind();
        /// </code>
        /// </example>
        public void ShowFind()
        {
            ((EditViewControl)this._ActiveView).ShowFind();
        }
        /// <summary>
        /// Shows the Replace dialog
        /// </summary>
        /// <example>
        /// <code>
        /// //Show FindReplace dialog
        /// MySyntaxBox.ShowReplace();
        /// </code>
        /// </example>
        public void ShowReplace()
        {
            ((EditViewControl)this._ActiveView).ShowReplace();
        }
        #endregion //END Public Methods

        [Browsable(false), Obsolete("Use .FontName and .FontSize", true)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }
        //		[Browsable(true)]
        //		[DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
        //		[RefreshProperties (RefreshProperties.All)]
        //		public override string Text
        //		{
        //			get
        //			{
        //				return this.Document.Text;
        //			}
        //			set
        //			{
        //				this.Document.Text=value;
        //			}
        //		}
        [Browsable(false), Obsolete("Apply a syntax instead", true)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }
        /// <summary>
        /// The currently highlighted text in the autolist.
        /// </summary>
        [Browsable(false)]
        public string AutoListSelectedText
        {
            get { return ((EditViewControl)this._ActiveView).AutoList.SelectedText; }
            set
            {
                if((this._ActiveView) == null || ((EditViewControl)this._ActiveView).AutoList == null){
                    return;
                }
                ((EditViewControl)this._ActiveView).AutoList.SelectItem(value);
            }
        }
        public void Save(string filename)
        {
            string text = this.Document.Text;
            var swr = new StreamWriter(filename);
            swr.Write(text);
            swr.Flush();
            swr.Close();
        }
        public void Open(string filename)
        {
            if(this.Document == null){
                throw new NullReferenceException("CodeEditorControl.Document");
            }
            var swr = new StreamReader(filename);
            this.Document.Text = swr.ReadToEnd();
            swr.Close();
        }
        public void AttachDocument(SyntaxDocument document)
        {
            //_Document=document;
            if(this._Document != null){
                this._Document.ParsingCompleted -= this.OnParsingCompleted;
                this._Document.Parsing -= this.OnParse;
                this._Document.Change -= this.OnChange;
            }
            if(document == null){
                document = new SyntaxDocument();
            }
            this._Document = document;
            if(this._Document != null){
                this._Document.ParsingCompleted += this.OnParsingCompleted;
                this._Document.Parsing += this.OnParse;
                this._Document.Change += this.OnChange;
            }
            this.Redraw();
        }
        protected virtual void OnParse(object Sender, EventArgs e)
        {
            foreach(EditViewControl ev in this.Views){
                ev.OnParse();
            }
        }
        protected virtual void OnParsingCompleted(object Sender, EventArgs e)
        {
            foreach(EditViewControl ev in this.Views){
                ev.Invalidate();
            }
        }
        protected virtual void OnChange(object Sender, EventArgs e)
        {
            if(this.Views == null){
                return;
            }
            foreach(EditViewControl ev in this.Views){
                ev.OnChange();
            }
            this.OnTextChanged(EventArgs.Empty);
        }
        public void RemoveCurrentRow()
        {
            ((EditViewControl)this._ActiveView).RemoveCurrentRow();
        }
        public void CutClear()
        {
            ((EditViewControl)this._ActiveView).CutClear();
        }
        public void AutoListInsertSelectedText()
        {
            ((EditViewControl)this._ActiveView).InsertAutolistText();
        }
        protected override SplitViewChildControl GetNewView()
        {
            return new EditViewControl(this);
        }
        protected override void OnImeModeChanged(EventArgs e)
        {
            base.OnImeModeChanged(e);
            foreach(EditViewControl ev in this.Views){
                ev.ImeMode = this.ImeMode;
            }
        }

        #region Constructor
        /// <summary>
        /// Default constructor for the SyntaxBoxControl
        /// </summary>
        public SyntaxBoxControl()
        {
            try{
                this.Document = new SyntaxDocument();
                this.CreateViews();
                this.InitializeComponent();
                this.SetStyle(ControlStyles.Selectable, true);
                //assign keys
                this.KeyboardActions.Add(new KeyboardAction(Keys.Z, false, true, false, false, this.Undo));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Y, false, true, false, false, this.Redo));
                this.KeyboardActions.Add(new KeyboardAction(Keys.F3, false, false, false, true, this.FindNext));
                this.KeyboardActions.Add(new KeyboardAction(Keys.C, false, true, false, true, this.Copy));
                this.KeyboardActions.Add(new KeyboardAction(Keys.X, false, true, false, false, this.CutClear));
                this.KeyboardActions.Add(new KeyboardAction(Keys.V, false, true, false, false, this.Paste));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Insert, false, true, false, true, this.Copy));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Delete, true, false, false, false, this.Cut));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Insert, true, false, false, false, this.Paste));
                this.KeyboardActions.Add(new KeyboardAction(Keys.A, false, true, false, true, this.SelectAll));
                this.KeyboardActions.Add(new KeyboardAction(Keys.F, false, true, false, false, this.ShowFind));
                this.KeyboardActions.Add(new KeyboardAction(Keys.H, false, true, false, false, this.ShowReplace));
                this.KeyboardActions.Add(new KeyboardAction(Keys.G, false, true, false, true, this.ShowGotoLine));
                this.KeyboardActions.Add(new KeyboardAction(Keys.T, false, true, false, false, this.ShowSettings));
                this.KeyboardActions.Add(new KeyboardAction(Keys.F2, false, true, false, true, this.ToggleBookmark));
                this.KeyboardActions.Add(new KeyboardAction(Keys.F2, false, false, false, true, this.GotoNextBookmark));
                this.KeyboardActions.Add(new KeyboardAction(Keys.F2, true, false, false, true, this.GotoPreviousBookmark));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Escape, false, false, false, true, this.ClearSelection));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Tab, false, false, false, false, this.Selection.Indent));
                this.KeyboardActions.Add(new KeyboardAction(Keys.Tab, true, false, false, false, this.Selection.Outdent));
                this.AutoListIcons = this._AutoListIcons;
            } catch{
                //	Console.WriteLine (x.StackTrace);
            }
        }
        #endregion //END Constructor		

        #region EventHandlers
        protected virtual void OnClipboardUpdated(CopyEventArgs e)
        {
            if(this.ClipboardUpdated != null){
                this.ClipboardUpdated(this, e);
            }
        }
        protected virtual void OnRowMouseDown(RowMouseEventArgs e)
        {
            if(this.RowMouseDown != null){
                this.RowMouseDown(this, e);
            }
        }
        protected virtual void OnRowMouseMove(RowMouseEventArgs e)
        {
            if(this.RowMouseMove != null){
                this.RowMouseMove(this, e);
            }
        }
        protected virtual void OnRowMouseUp(RowMouseEventArgs e)
        {
            if(this.RowMouseUp != null){
                this.RowMouseUp(this, e);
            }
        }
        protected virtual void OnRowClick(RowMouseEventArgs e)
        {
            if(this.RowClick != null){
                this.RowClick(this, e);
            }
        }
        protected virtual void OnRowDoubleClick(RowMouseEventArgs e)
        {
            if(this.RowDoubleClick != null){
                this.RowDoubleClick(this, e);
            }
        }
        private void ParseTimer_Tick(object sender, EventArgs e)
        {
            this.Document.ParseSome();
        }
        protected virtual void OnInfoTipSelectedIndexChanged()
        {
            if(this.InfoTipSelectedIndexChanged != null){
                this.InfoTipSelectedIndexChanged(null, null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if((this._ActiveView) != null){
                (this._ActiveView).Focus();
            }
        }
        private void View_RowClick(object sender, RowMouseEventArgs e)
        {
            this.OnRowClick(e);
        }
        private void View_RowDoubleClick(object sender, RowMouseEventArgs e)
        {
            this.OnRowDoubleClick(e);
        }
        private void View_RowMouseDown(object sender, RowMouseEventArgs e)
        {
            this.OnRowMouseDown(e);
        }
        private void View_RowMouseMove(object sender, RowMouseEventArgs e)
        {
            this.OnRowMouseMove(e);
        }
        private void View_RowMouseUp(object sender, RowMouseEventArgs e)
        {
            this.OnRowMouseUp(e);
        }
        private void View_ClipboardUpdated(object sender, CopyEventArgs e)
        {
            this.OnClipboardUpdated(e);
        }
        public void OnRenderRow(RowPaintEventArgs e)
        {
            if(this.RenderRow != null){
                this.RenderRow(this, e);
            }
        }
        public void OnWordMouseHover(ref WordMouseEventArgs e)
        {
            if(this.WordMouseHover != null){
                this.WordMouseHover(this, ref e);
            }
        }
        public void OnWordMouseDown(ref WordMouseEventArgs e)
        {
            if(this.WordMouseDown != null){
                this.WordMouseDown(this, ref e);
            }
        }
        protected virtual void OnCaretChange(object sender)
        {
            if(this.CaretChange != null){
                this.CaretChange(this, null);
            }
        }
        protected virtual void OnSelectionChange(object sender)
        {
            if(this.SelectionChange != null){
                this.SelectionChange(this, null);
            }
        }
        private void View_CaretChanged(object s, EventArgs e)
        {
            this.OnCaretChange(s);
        }
        private void View_SelectionChanged(object s, EventArgs e)
        {
            this.OnSelectionChange(s);
        }
        private void View_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }
        private void View_MouseUp(object sender, MouseEventArgs e)
        {
            var ev = (EditViewControl)sender;
            var ea = new MouseEventArgs(e.Button, e.Clicks, e.X + ev.Location.X + ev.BorderWidth,
                                        e.Y + ev.Location.Y + ev.BorderWidth, e.Delta);
            this.OnMouseUp(ea);
        }
        private void View_MouseMove(object sender, MouseEventArgs e)
        {
            var ev = (EditViewControl)sender;
            var ea = new MouseEventArgs(e.Button, e.Clicks, e.X + ev.Location.X + ev.BorderWidth,
                                        e.Y + ev.Location.Y + ev.BorderWidth, e.Delta);
            this.OnMouseMove(ea);
        }
        private void View_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }
        private void View_MouseHover(object sender, EventArgs e)
        {
            this.OnMouseHover(e);
        }
        private void View_MouseEnter(object sender, EventArgs e)
        {
            this.OnMouseEnter(e);
        }
        private void View_MouseDown(object sender, MouseEventArgs e)
        {
            var ev = (EditViewControl)sender;
            var ea = new MouseEventArgs(e.Button, e.Clicks, e.X + ev.Location.X + ev.BorderWidth,
                                        e.Y + ev.Location.Y + ev.BorderWidth, e.Delta);
            this.OnMouseDown(ea);
        }
        private void View_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }
        private void View_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }
        private void View_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }
        private void View_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
        private void View_DragOver(object sender, DragEventArgs e)
        {
            this.OnDragOver(e);
        }
        private void View_DragLeave(object sender, EventArgs e)
        {
            this.OnDragLeave(e);
        }
        private void View_DragEnter(object sender, DragEventArgs e)
        {
            this.OnDragEnter(e);
        }
        private void View_DragDrop(object sender, DragEventArgs e)
        {
            this.OnDragDrop(e);
        }
        private void View_InfoTipSelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnInfoTipSelectedIndexChanged();
        }
        #endregion

        #region DISPOSE()
        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if(disposing){
                if(this.components != null){
                    this.components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion //END DISPOSE

        #region Private/Protected/Internal methods
        private void InitializeComponent()
        {
            this.components = new Container();
            var resources = new ResourceManager(typeof(SyntaxBoxControl));
            this._GutterIcons = new ImageList(this.components);
            this._AutoListIcons = new ImageList(this.components);
            this.ParseTimer = new Timer(this.components);
            // 
            // _GutterIcons
            // 
            this._GutterIcons.ColorDepth = ColorDepth.Depth32Bit;
            this._GutterIcons.ImageSize = new Size(17, 17);
            this._GutterIcons.ImageStream = ((ImageListStreamer)(resources.GetObject("_GutterIcons.ImageStream")));
            this._GutterIcons.TransparentColor = Color.Transparent;
            // 
            // _AutoListIcons
            // 
            this._AutoListIcons.ColorDepth = ColorDepth.Depth8Bit;
            this._AutoListIcons.ImageSize = new Size(16, 16);
            this._AutoListIcons.ImageStream = ((ImageListStreamer)(resources.GetObject("_AutoListIcons.ImageStream")));
            this._AutoListIcons.TransparentColor = Color.Transparent;
            // 
            // ParseTimer
            // 
            this.ParseTimer.Enabled = true;
            this.ParseTimer.Interval = 1;
            this.ParseTimer.Tick += this.ParseTimer_Tick;
        }
        protected override void OnLoad(EventArgs e)
        {
            this.Refresh();
        }
        private void Redraw()
        {
            if(this.Views == null){
                return;
            }
            foreach(EditViewControl ev in this.Views){
                if(ev != null){
                    ev.Refresh();
                }
            }
        }
        private void InitGraphics()
        {
            if(this.Views == null || this.Parent == null){
                return;
            }
            foreach(EditViewControl ev in this.Views){
                ev.InitGraphics();
            }
        }
        protected override void CreateViews()
        {
            base.CreateViews();
            foreach(EditViewControl ev in this.Views){
                if(this.DoOnce && ev == this.LowerRight){
                    continue;
                }
                //attatch events to views
                ev.Enter += this.View_Enter;
                ev.Leave += this.View_Leave;
                ev.GotFocus += this.View_Enter;
                ev.LostFocus += this.View_Leave;
                ev.CaretChange += this.View_CaretChanged;
                ev.SelectionChange += this.View_SelectionChanged;
                ev.Click += this.View_Click;
                ev.DoubleClick += this.View_DoubleClick;
                ev.MouseDown += this.View_MouseDown;
                ev.MouseEnter += this.View_MouseEnter;
                ev.MouseHover += this.View_MouseHover;
                ev.MouseLeave += this.View_MouseLeave;
                ev.MouseMove += this.View_MouseMove;
                ev.MouseUp += this.View_MouseUp;
                ev.KeyDown += this.View_KeyDown;
                ev.KeyPress += this.View_KeyPress;
                ev.KeyUp += this.View_KeyUp;
                ev.DragDrop += this.View_DragDrop;
                ev.DragOver += this.View_DragOver;
                ev.DragLeave += this.View_DragLeave;
                ev.DragEnter += this.View_DragEnter;
                if(ev.InfoTip != null){
                    ev.InfoTip.Data = "";
                    ev.InfoTip.SelectedIndexChanged += this.View_InfoTipSelectedIndexChanged;
                }
                ev.RowClick += this.View_RowClick;
                ev.RowDoubleClick += this.View_RowDoubleClick;
                ev.RowMouseDown += this.View_RowMouseDown;
                ev.RowMouseMove += this.View_RowMouseMove;
                ev.RowMouseUp += this.View_RowMouseUp;
                ev.ClipboardUpdated += this.View_ClipboardUpdated;
            }
            this.DoOnce = true;
            this.AutoListIcons = this.AutoListIcons;
            this.InfoTipImage = this.InfoTipImage;
            this.ChildBorderStyle = this.ChildBorderStyle;
            this.ChildBorderColor = this.ChildBorderColor;
            this.BackColor = this.BackColor;
            this.Document = this.Document;
            this.ImeMode = this.ImeMode;
            this.Redraw();
        }
        #endregion //END Private/Protected/Internal methods
    }
}