using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal class VS2005DockPaneCaption : DockPaneCaptionBase
    {
        private static Blend _activeBackColorGradientBlend;
        private static Bitmap _imageButtonAutoHide;
        private static Bitmap _imageButtonClose;
        private static Bitmap _imageButtonDock;
        private static Bitmap _imageButtonOptions;
        private static TextFormatFlags _textFormat = TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis
                                                     | TextFormatFlags.VerticalCenter;
        private static string _toolTipAutoHide;
        private static string _toolTipClose;
        private static string _toolTipOptions;
        private InertButton m_buttonAutoHide;
        private InertButton m_buttonClose;
        private InertButton m_buttonOptions;
        private IContainer m_components;
        private ToolTip m_toolTip;
        public VS2005DockPaneCaption(DockPane pane) : base(pane)
        {
            this.SuspendLayout();
            this.m_components = new Container();
            this.m_toolTip = new ToolTip(this.Components);
            this.ResumeLayout();
        }
        private static Bitmap ImageButtonClose
        {
            get
            {
                if(_imageButtonClose == null){
                    _imageButtonClose = Resources.DockPane_Close;
                }
                return _imageButtonClose;
            }
        }
        private InertButton ButtonClose
        {
            get
            {
                if(this.m_buttonClose == null){
                    this.m_buttonClose = new InertButton(this, ImageButtonClose, ImageButtonClose);
                    this.m_toolTip.SetToolTip(this.m_buttonClose, ToolTipClose);
                    this.m_buttonClose.Click += new EventHandler(this.Close_Click);
                    this.Controls.Add(this.m_buttonClose);
                }
                return this.m_buttonClose;
            }
        }
        private static Bitmap ImageButtonAutoHide
        {
            get
            {
                if(_imageButtonAutoHide == null){
                    _imageButtonAutoHide = Resources.DockPane_AutoHide;
                }
                return _imageButtonAutoHide;
            }
        }
        private static Bitmap ImageButtonDock
        {
            get
            {
                if(_imageButtonDock == null){
                    _imageButtonDock = Resources.DockPane_Dock;
                }
                return _imageButtonDock;
            }
        }
        private InertButton ButtonAutoHide
        {
            get
            {
                if(this.m_buttonAutoHide == null){
                    this.m_buttonAutoHide = new InertButton(this, ImageButtonDock, ImageButtonAutoHide);
                    this.m_toolTip.SetToolTip(this.m_buttonAutoHide, ToolTipAutoHide);
                    this.m_buttonAutoHide.Click += new EventHandler(this.AutoHide_Click);
                    this.Controls.Add(this.m_buttonAutoHide);
                }
                return this.m_buttonAutoHide;
            }
        }
        private static Bitmap ImageButtonOptions
        {
            get
            {
                if(_imageButtonOptions == null){
                    _imageButtonOptions = Resources.DockPane_Option;
                }
                return _imageButtonOptions;
            }
        }
        private InertButton ButtonOptions
        {
            get
            {
                if(this.m_buttonOptions == null){
                    this.m_buttonOptions = new InertButton(this, ImageButtonOptions, ImageButtonOptions);
                    this.m_toolTip.SetToolTip(this.m_buttonOptions, ToolTipOptions);
                    this.m_buttonOptions.Click += new EventHandler(this.Options_Click);
                    this.Controls.Add(this.m_buttonOptions);
                }
                return this.m_buttonOptions;
            }
        }
        private IContainer Components
        {
            get { return this.m_components; }
        }
        private static int TextGapTop
        {
            get { return _TextGapTop; }
        }
        private static Font TextFont
        {
            get { return SystemInformation.MenuFont; }
        }
        private static int TextGapBottom
        {
            get { return _TextGapBottom; }
        }
        private static int TextGapLeft
        {
            get { return _TextGapLeft; }
        }
        private static int TextGapRight
        {
            get { return _TextGapRight; }
        }
        private static int ButtonGapTop
        {
            get { return _ButtonGapTop; }
        }
        private static int ButtonGapBottom
        {
            get { return _ButtonGapBottom; }
        }
        private static int ButtonGapLeft
        {
            get { return _ButtonGapLeft; }
        }
        private static int ButtonGapRight
        {
            get { return _ButtonGapRight; }
        }
        private static int ButtonGapBetween
        {
            get { return _ButtonGapBetween; }
        }
        private static string ToolTipClose
        {
            get
            {
                if(_toolTipClose == null){
                    _toolTipClose = Strings.DockPaneCaption_ToolTipClose;
                }
                return _toolTipClose;
            }
        }
        private static string ToolTipOptions
        {
            get
            {
                if(_toolTipOptions == null){
                    _toolTipOptions = Strings.DockPaneCaption_ToolTipOptions;
                }
                return _toolTipOptions;
            }
        }
        private static string ToolTipAutoHide
        {
            get
            {
                if(_toolTipAutoHide == null){
                    _toolTipAutoHide = Strings.DockPaneCaption_ToolTipAutoHide;
                }
                return _toolTipAutoHide;
            }
        }
        private static Blend ActiveBackColorGradientBlend
        {
            get
            {
                if(_activeBackColorGradientBlend == null){
                    Blend blend = new Blend(2);
                    blend.Factors = new float[]{0.5F, 1.0F};
                    blend.Positions = new float[]{0.0F, 1.0F};
                    _activeBackColorGradientBlend = blend;
                }
                return _activeBackColorGradientBlend;
            }
        }
        private static Color ActiveBackColorGradientBegin
        {
            get { return SystemColors.GradientActiveCaption; }
        }
        private static Color ActiveBackColorGradientEnd
        {
            get { return SystemColors.ActiveCaption; }
        }
        private static Color InactiveBackColor
        {
            get
            {
                string colorScheme = VisualStyleInformation.ColorScheme;
                if(colorScheme == "HomeStead" || colorScheme == "Metallic"){
                    return SystemColors.GradientInactiveCaption;
                } else{
                    return SystemColors.GrayText;
                }
            }
        }
        private static Color ActiveTextColor
        {
            get { return SystemColors.ActiveCaptionText; }
        }
        private static Color InactiveTextColor
        {
            get { return SystemColors.ControlText; }
        }
        private Color TextColor
        {
            get { return this.DockPane.IsActivated ? ActiveTextColor : InactiveTextColor; }
        }
        private TextFormatFlags TextFormat
        {
            get
            {
                if(this.RightToLeft == RightToLeft.No){
                    return _textFormat;
                } else{
                    return _textFormat | TextFormatFlags.RightToLeft | TextFormatFlags.Right;
                }
            }
        }
        private bool CloseButtonEnabled
        {
            get
            {
                return (this.DockPane.ActiveContent != null)
                               ? this.DockPane.ActiveContent.DockHandler.CloseButton
                               : false;
            }
        }
        private bool ShouldShowAutoHideButton
        {
            get { return !this.DockPane.IsFloat; }
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing){
                this.Components.Dispose();
            }
            base.Dispose(disposing);
        }
        protected internal override int MeasureHeight()
        {
            int height = TextFont.Height + TextGapTop + TextGapBottom;
            if(height < this.ButtonClose.Image.Height + ButtonGapTop + ButtonGapBottom){
                height = this.ButtonClose.Image.Height + ButtonGapTop + ButtonGapBottom;
            }
            return height;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.DrawCaption(e.Graphics);
        }
        private void DrawCaption(Graphics g)
        {
            if(this.ClientRectangle.Width == 0 || this.ClientRectangle.Height == 0){
                return;
            }
            if(this.DockPane.IsActivated){
                using(
                        LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                                                                            ActiveBackColorGradientBegin,
                                                                            ActiveBackColorGradientEnd,
                                                                            LinearGradientMode.Vertical)){
                    brush.Blend = ActiveBackColorGradientBlend;
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            } else{
                using(SolidBrush brush = new SolidBrush(InactiveBackColor)){
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }
            Rectangle rectCaption = this.ClientRectangle;
            Rectangle rectCaptionText = rectCaption;
            rectCaptionText.X += TextGapLeft;
            rectCaptionText.Width -= TextGapLeft + TextGapRight;
            rectCaptionText.Width -= ButtonGapLeft + this.ButtonClose.Width + ButtonGapRight;
            if(this.ShouldShowAutoHideButton){
                rectCaptionText.Width -= this.ButtonAutoHide.Width + ButtonGapBetween;
            }
            if(this.HasTabPageContextMenu){
                rectCaptionText.Width -= this.ButtonOptions.Width + ButtonGapBetween;
            }
            rectCaptionText.Y += TextGapTop;
            rectCaptionText.Height -= TextGapTop + TextGapBottom;
            TextRenderer.DrawText(g, this.DockPane.CaptionText, TextFont, DrawHelper.RtlTransform(this, rectCaptionText),
                                  this.TextColor, this.TextFormat);
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            this.SetButtonsPosition();
            base.OnLayout(levent);
        }
        protected override void OnRefreshChanges()
        {
            this.SetButtons();
            this.Invalidate();
        }
        private void SetButtons()
        {
            this.ButtonClose.Enabled = this.CloseButtonEnabled;
            this.ButtonAutoHide.Visible = this.ShouldShowAutoHideButton;
            this.ButtonOptions.Visible = this.HasTabPageContextMenu;
            this.ButtonClose.RefreshChanges();
            this.ButtonAutoHide.RefreshChanges();
            this.ButtonOptions.RefreshChanges();
            this.SetButtonsPosition();
        }
        private void SetButtonsPosition()
        {
            // set the size and location for close and auto-hide buttons
            Rectangle rectCaption = this.ClientRectangle;
            int buttonWidth = this.ButtonClose.Image.Width;
            int buttonHeight = this.ButtonClose.Image.Height;
            int height = rectCaption.Height - ButtonGapTop - ButtonGapBottom;
            if(buttonHeight < height){
                buttonWidth = buttonWidth * (height / buttonHeight);
                buttonHeight = height;
            }
            Size buttonSize = new Size(buttonWidth, buttonHeight);
            int x = rectCaption.X + rectCaption.Width - 1 - ButtonGapRight - this.m_buttonClose.Width;
            int y = rectCaption.Y + ButtonGapTop;
            Point point = new Point(x, y);
            this.ButtonClose.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
            point.Offset(-(buttonWidth + ButtonGapBetween), 0);
            this.ButtonAutoHide.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
            if(this.ShouldShowAutoHideButton){
                point.Offset(-(buttonWidth + ButtonGapBetween), 0);
            }
            this.ButtonOptions.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
        }
        private void Close_Click(object sender, EventArgs e)
        {
            this.DockPane.CloseActiveContent();
        }
        private void AutoHide_Click(object sender, EventArgs e)
        {
            this.DockPane.DockState = DockHelper.ToggleAutoHideState(this.DockPane.DockState);
        }
        private void Options_Click(object sender, EventArgs e)
        {
            this.ShowTabPageContextMenu(this.PointToClient(MousePosition));
        }
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.PerformLayout();
        }

        #region Nested type: InertButton
        private sealed class InertButton : InertButtonBase
        {
            private VS2005DockPaneCaption m_dockPaneCaption;
            private Bitmap m_image, m_imageAutoHide;
            public InertButton(VS2005DockPaneCaption dockPaneCaption, Bitmap image, Bitmap imageAutoHide) : base()
            {
                this.m_dockPaneCaption = dockPaneCaption;
                this.m_image = image;
                this.m_imageAutoHide = imageAutoHide;
                this.RefreshChanges();
            }
            private VS2005DockPaneCaption DockPaneCaption
            {
                get { return this.m_dockPaneCaption; }
            }
            public bool IsAutoHide
            {
                get { return this.DockPaneCaption.DockPane.IsAutoHide; }
            }
            public override Bitmap Image
            {
                get { return this.IsAutoHide ? this.m_imageAutoHide : this.m_image; }
            }
            protected override void OnRefreshChanges()
            {
                if(this.DockPaneCaption.TextColor != this.ForeColor){
                    this.ForeColor = this.DockPaneCaption.TextColor;
                    this.Invalidate();
                }
            }
        }
        #endregion

        #region consts
        private const int _ButtonGapBetween = 1;
        private const int _ButtonGapBottom = 1;
        private const int _ButtonGapLeft = 1;
        private const int _ButtonGapRight = 2;
        private const int _ButtonGapTop = 2;
        private const int _TextGapBottom = 0;
        private const int _TextGapLeft = 3;
        private const int _TextGapRight = 3;
        private const int _TextGapTop = 2;
        #endregion
    }
}