using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    partial class DockPanel
    {
        private DockDragHandler m_dockDragHandler = null;
        private DockDragHandler GetDockDragHandler()
        {
            if(this.m_dockDragHandler == null){
                this.m_dockDragHandler = new DockDragHandler(this);
            }
            return this.m_dockDragHandler;
        }
        internal void BeginDrag(IDockDragSource dragSource)
        {
            this.GetDockDragHandler().BeginDrag(dragSource);
        }

        #region Nested type: DockDragHandler
        private sealed class DockDragHandler : DragHandler
        {
            private Rectangle m_floatOutlineBounds;
            private DockIndicator m_indicator;
            private DockOutlineBase m_outline;
            public DockDragHandler(DockPanel panel) : base(panel) {}
            public new IDockDragSource DragSource
            {
                get { return base.DragSource as IDockDragSource; }
                set { base.DragSource = value; }
            }
            public DockOutlineBase Outline
            {
                get { return this.m_outline; }
                private set { this.m_outline = value; }
            }
            private DockIndicator Indicator
            {
                get { return this.m_indicator; }
                set { this.m_indicator = value; }
            }
            private Rectangle FloatOutlineBounds
            {
                get { return this.m_floatOutlineBounds; }
                set { this.m_floatOutlineBounds = value; }
            }
            public void BeginDrag(IDockDragSource dragSource)
            {
                this.DragSource = dragSource;
                if(!this.BeginDrag()){
                    this.DragSource = null;
                    return;
                }
                this.Outline = new DockOutline();
                this.Indicator = new DockIndicator(this);
                this.Indicator.Show(false);
                this.FloatOutlineBounds = this.DragSource.BeginDrag(this.StartMousePosition);
            }
            protected override void OnDragging()
            {
                this.TestDrop();
            }
            protected override void OnEndDrag(bool abort)
            {
                this.DockPanel.SuspendLayout(true);
                this.Outline.Close();
                this.Indicator.Close();
                this.EndDrag(abort);
                this.DockPanel.ResumeLayout(true, true);
                this.DragSource = null;
            }
            private void TestDrop()
            {
                this.Outline.FlagTestDrop = false;
                this.Indicator.FullPanelEdge = ((ModifierKeys & Keys.Shift) != 0);
                if((ModifierKeys & Keys.Control) == 0){
                    this.Indicator.TestDrop();
                    if(!this.Outline.FlagTestDrop){
                        DockPane pane = DockHelper.PaneAtPoint(MousePosition, this.DockPanel);
                        if(pane != null && this.DragSource.IsDockStateValid(pane.DockState)){
                            pane.TestDrop(this.DragSource, this.Outline);
                        }
                    }
                    if(!this.Outline.FlagTestDrop && this.DragSource.IsDockStateValid(DockState.Float)){
                        FloatWindow floatWindow = DockHelper.FloatWindowAtPoint(MousePosition, this.DockPanel);
                        if(floatWindow != null){
                            floatWindow.TestDrop(this.DragSource, this.Outline);
                        }
                    }
                } else{
                    this.Indicator.DockPane = DockHelper.PaneAtPoint(MousePosition, this.DockPanel);
                }
                if(!this.Outline.FlagTestDrop){
                    if(this.DragSource.IsDockStateValid(DockState.Float)){
                        Rectangle rect = this.FloatOutlineBounds;
                        rect.Offset(MousePosition.X - this.StartMousePosition.X,
                                    MousePosition.Y - this.StartMousePosition.Y);
                        this.Outline.Show(rect);
                    }
                }
                if(!this.Outline.FlagTestDrop){
                    Cursor.Current = Cursors.No;
                    this.Outline.Show();
                } else{
                    Cursor.Current = this.DragControl.Cursor;
                }
            }
            private void EndDrag(bool abort)
            {
                if(abort){
                    return;
                }
                if(!this.Outline.FloatWindowBounds.IsEmpty){
                    this.DragSource.FloatAt(this.Outline.FloatWindowBounds);
                } else if(this.Outline.DockTo is DockPane){
                    DockPane pane = this.Outline.DockTo as DockPane;
                    this.DragSource.DockTo(pane, this.Outline.Dock, this.Outline.ContentIndex);
                } else if(this.Outline.DockTo is DockPanel){
                    DockPanel panel = this.Outline.DockTo as DockPanel;
                    panel.UpdateDockWindowZOrder(this.Outline.Dock, this.Outline.FlagFullEdge);
                    this.DragSource.DockTo(panel, this.Outline.Dock);
                }
            }

            #region Nested type: DockIndicator
            private class DockIndicator : DragForm
            {
                #region IHitTest
                private interface IHitTest
                {
                    DockStyle Status { get; set; }
                    DockStyle HitTest(Point pt);
                }
                #endregion

                #region PanelIndicator
                private class PanelIndicator : PictureBox, IHitTest
                {
                    private static Image _imagePanelBottom = Resources.DockIndicator_PanelBottom;
                    private static Image _imagePanelBottomActive = Resources.DockIndicator_PanelBottom_Active;
                    private static Image _imagePanelFill = Resources.DockIndicator_PanelFill;
                    private static Image _imagePanelFillActive = Resources.DockIndicator_PanelFill_Active;
                    private static Image _imagePanelLeft = Resources.DockIndicator_PanelLeft;
                    private static Image _imagePanelLeftActive = Resources.DockIndicator_PanelLeft_Active;
                    private static Image _imagePanelRight = Resources.DockIndicator_PanelRight;
                    private static Image _imagePanelRightActive = Resources.DockIndicator_PanelRight_Active;
                    private static Image _imagePanelTop = Resources.DockIndicator_PanelTop;
                    private static Image _imagePanelTopActive = Resources.DockIndicator_PanelTop_Active;
                    private DockStyle m_dockStyle;
                    private bool m_isActivated = false;
                    private DockStyle m_status;
                    public PanelIndicator(DockStyle dockStyle)
                    {
                        this.m_dockStyle = dockStyle;
                        this.SizeMode = PictureBoxSizeMode.AutoSize;
                        this.Image = this.ImageInactive;
                    }
                    private DockStyle DockStyle
                    {
                        get { return this.m_dockStyle; }
                    }
                    private Image ImageInactive
                    {
                        get
                        {
                            if(this.DockStyle == DockStyle.Left){
                                return _imagePanelLeft;
                            } else if(this.DockStyle == DockStyle.Right){
                                return _imagePanelRight;
                            } else if(this.DockStyle == DockStyle.Top){
                                return _imagePanelTop;
                            } else if(this.DockStyle == DockStyle.Bottom){
                                return _imagePanelBottom;
                            } else if(this.DockStyle == DockStyle.Fill){
                                return _imagePanelFill;
                            } else{
                                return null;
                            }
                        }
                    }
                    private Image ImageActive
                    {
                        get
                        {
                            if(this.DockStyle == DockStyle.Left){
                                return _imagePanelLeftActive;
                            } else if(this.DockStyle == DockStyle.Right){
                                return _imagePanelRightActive;
                            } else if(this.DockStyle == DockStyle.Top){
                                return _imagePanelTopActive;
                            } else if(this.DockStyle == DockStyle.Bottom){
                                return _imagePanelBottomActive;
                            } else if(this.DockStyle == DockStyle.Fill){
                                return _imagePanelFillActive;
                            } else{
                                return null;
                            }
                        }
                    }
                    private bool IsActivated
                    {
                        get { return this.m_isActivated; }
                        set
                        {
                            this.m_isActivated = value;
                            this.Image = this.IsActivated ? this.ImageActive : this.ImageInactive;
                        }
                    }

                    #region IHitTest Members
                    public DockStyle Status
                    {
                        get { return this.m_status; }
                        set
                        {
                            if(value != this.DockStyle && value != DockStyle.None){
                                throw new InvalidEnumArgumentException();
                            }
                            if(this.m_status == value){
                                return;
                            }
                            this.m_status = value;
                            this.IsActivated = (this.m_status != DockStyle.None);
                        }
                    }
                    public DockStyle HitTest(Point pt)
                    {
                        return this.Visible && this.ClientRectangle.Contains(this.PointToClient(pt))
                                       ? this.DockStyle
                                       : DockStyle.None;
                    }
                    #endregion
                }
                #endregion PanelIndicator

                #region PaneIndicator
                private class PaneIndicator : PictureBox, IHitTest
                {
                    private static Bitmap _bitmapPaneDiamond = Resources.DockIndicator_PaneDiamond;
                    private static Bitmap _bitmapPaneDiamondBottom = Resources.DockIndicator_PaneDiamond_Bottom;
                    private static Bitmap _bitmapPaneDiamondFill = Resources.DockIndicator_PaneDiamond_Fill;
                    private static Bitmap _bitmapPaneDiamondHotSpot = Resources.DockIndicator_PaneDiamond_HotSpot;
                    private static Bitmap _bitmapPaneDiamondHotSpotIndex =
                            Resources.DockIndicator_PaneDiamond_HotSpotIndex;
                    private static Bitmap _bitmapPaneDiamondLeft = Resources.DockIndicator_PaneDiamond_Left;
                    private static Bitmap _bitmapPaneDiamondRight = Resources.DockIndicator_PaneDiamond_Right;
                    private static Bitmap _bitmapPaneDiamondTop = Resources.DockIndicator_PaneDiamond_Top;
                    private static GraphicsPath _displayingGraphicsPath =
                            DrawHelper.CalculateGraphicsPathFromBitmap(_bitmapPaneDiamond);
                    private static HotSpotIndex[] _hotSpots = new HotSpotIndex[]{
                                                                                        new HotSpotIndex(1, 0, DockStyle.Top),
                                                                                        new HotSpotIndex(0, 1, DockStyle.Left),
                                                                                        new HotSpotIndex(1, 1, DockStyle.Fill),
                                                                                        new HotSpotIndex(2, 1, DockStyle.Right),
                                                                                        new HotSpotIndex(1, 2, DockStyle.Bottom)
                                                                                };
                    private DockStyle m_status = DockStyle.None;
                    public PaneIndicator()
                    {
                        this.SizeMode = PictureBoxSizeMode.AutoSize;
                        this.Image = _bitmapPaneDiamond;
                        this.Region = new Region(DisplayingGraphicsPath);
                    }
                    public static GraphicsPath DisplayingGraphicsPath
                    {
                        get { return _displayingGraphicsPath; }
                    }

                    #region IHitTest Members
                    public DockStyle HitTest(Point pt)
                    {
                        if(!this.Visible){
                            return DockStyle.None;
                        }
                        pt = this.PointToClient(pt);
                        if(!this.ClientRectangle.Contains(pt)){
                            return DockStyle.None;
                        }
                        for(int i = _hotSpots.GetLowerBound(0); i <= _hotSpots.GetUpperBound(0); i++){
                            if(_bitmapPaneDiamondHotSpot.GetPixel(pt.X, pt.Y)
                               == _bitmapPaneDiamondHotSpotIndex.GetPixel(_hotSpots[i].X, _hotSpots[i].Y)){
                                return _hotSpots[i].DockStyle;
                            }
                        }
                        return DockStyle.None;
                    }
                    public DockStyle Status
                    {
                        get { return this.m_status; }
                        set
                        {
                            this.m_status = value;
                            if(this.m_status == DockStyle.None){
                                this.Image = _bitmapPaneDiamond;
                            } else if(this.m_status == DockStyle.Left){
                                this.Image = _bitmapPaneDiamondLeft;
                            } else if(this.m_status == DockStyle.Right){
                                this.Image = _bitmapPaneDiamondRight;
                            } else if(this.m_status == DockStyle.Top){
                                this.Image = _bitmapPaneDiamondTop;
                            } else if(this.m_status == DockStyle.Bottom){
                                this.Image = _bitmapPaneDiamondBottom;
                            } else if(this.m_status == DockStyle.Fill){
                                this.Image = _bitmapPaneDiamondFill;
                            }
                        }
                    }
                    #endregion

                    #region Nested type: HotSpotIndex
                    private struct HotSpotIndex
                    {
                        private DockStyle m_dockStyle;
                        private int m_x;
                        private int m_y;
                        public HotSpotIndex(int x, int y, DockStyle dockStyle)
                        {
                            this.m_x = x;
                            this.m_y = y;
                            this.m_dockStyle = dockStyle;
                        }
                        public int X
                        {
                            get { return this.m_x; }
                        }
                        public int Y
                        {
                            get { return this.m_y; }
                        }
                        public DockStyle DockStyle
                        {
                            get { return this.m_dockStyle; }
                        }
                    }
                    #endregion
                }
                #endregion PaneIndicator

                #region consts
                private int _PanelIndicatorMargin = 10;
                #endregion

                private DockPane m_dockPane = null;
                private DockDragHandler m_dragHandler;
                private bool m_fullPanelEdge = false;
                private IHitTest m_hitTest = null;
                private PaneIndicator m_paneDiamond = null;
                private PanelIndicator m_panelBottom = null;
                private PanelIndicator m_panelFill = null;
                private PanelIndicator m_panelLeft = null;
                private PanelIndicator m_panelRight = null;
                private PanelIndicator m_panelTop = null;
                public DockIndicator(DockDragHandler dragHandler)
                {
                    this.m_dragHandler = dragHandler;
                    this.Controls.AddRange(new Control[]{
                                                                this.PaneDiamond, this.PanelLeft, this.PanelRight, this.PanelTop,
                                                                this.PanelBottom, this.PanelFill
                                                        });
                    this.Region = new Region(Rectangle.Empty);
                }
                private PaneIndicator PaneDiamond
                {
                    get
                    {
                        if(this.m_paneDiamond == null){
                            this.m_paneDiamond = new PaneIndicator();
                        }
                        return this.m_paneDiamond;
                    }
                }
                private PanelIndicator PanelLeft
                {
                    get
                    {
                        if(this.m_panelLeft == null){
                            this.m_panelLeft = new PanelIndicator(DockStyle.Left);
                        }
                        return this.m_panelLeft;
                    }
                }
                private PanelIndicator PanelRight
                {
                    get
                    {
                        if(this.m_panelRight == null){
                            this.m_panelRight = new PanelIndicator(DockStyle.Right);
                        }
                        return this.m_panelRight;
                    }
                }
                private PanelIndicator PanelTop
                {
                    get
                    {
                        if(this.m_panelTop == null){
                            this.m_panelTop = new PanelIndicator(DockStyle.Top);
                        }
                        return this.m_panelTop;
                    }
                }
                private PanelIndicator PanelBottom
                {
                    get
                    {
                        if(this.m_panelBottom == null){
                            this.m_panelBottom = new PanelIndicator(DockStyle.Bottom);
                        }
                        return this.m_panelBottom;
                    }
                }
                private PanelIndicator PanelFill
                {
                    get
                    {
                        if(this.m_panelFill == null){
                            this.m_panelFill = new PanelIndicator(DockStyle.Fill);
                        }
                        return this.m_panelFill;
                    }
                }
                public bool FullPanelEdge
                {
                    get { return this.m_fullPanelEdge; }
                    set
                    {
                        if(this.m_fullPanelEdge == value){
                            return;
                        }
                        this.m_fullPanelEdge = value;
                        this.RefreshChanges();
                    }
                }
                public DockDragHandler DragHandler
                {
                    get { return this.m_dragHandler; }
                }
                public DockPanel DockPanel
                {
                    get { return this.DragHandler.DockPanel; }
                }
                public DockPane DockPane
                {
                    get { return this.m_dockPane; }
                    internal set
                    {
                        if(this.m_dockPane == value){
                            return;
                        }
                        DockPane oldDisplayingPane = this.DisplayingPane;
                        this.m_dockPane = value;
                        if(oldDisplayingPane != this.DisplayingPane){
                            this.RefreshChanges();
                        }
                    }
                }
                private IHitTest HitTestResult
                {
                    get { return this.m_hitTest; }
                    set
                    {
                        if(this.m_hitTest == value){
                            return;
                        }
                        if(this.m_hitTest != null){
                            this.m_hitTest.Status = DockStyle.None;
                        }
                        this.m_hitTest = value;
                    }
                }
                private DockPane DisplayingPane
                {
                    get { return this.ShouldPaneDiamondVisible() ? this.DockPane : null; }
                }
                private void RefreshChanges()
                {
                    Region region = new Region(Rectangle.Empty);
                    Rectangle rectDockArea = this.FullPanelEdge
                                                     ? this.DockPanel.DockArea
                                                     : this.DockPanel.DocumentWindowBounds;
                    rectDockArea = this.RectangleToClient(this.DockPanel.RectangleToScreen(rectDockArea));
                    if(this.ShouldPanelIndicatorVisible(DockState.DockLeft)){
                        this.PanelLeft.Location = new Point(rectDockArea.X + this._PanelIndicatorMargin,
                                                            rectDockArea.Y
                                                            + (rectDockArea.Height - this.PanelRight.Height) / 2);
                        this.PanelLeft.Visible = true;
                        region.Union(this.PanelLeft.Bounds);
                    } else{
                        this.PanelLeft.Visible = false;
                    }
                    if(this.ShouldPanelIndicatorVisible(DockState.DockRight)){
                        this.PanelRight.Location =
                                new Point(
                                        rectDockArea.X + rectDockArea.Width - this.PanelRight.Width
                                        - this._PanelIndicatorMargin,
                                        rectDockArea.Y + (rectDockArea.Height - this.PanelRight.Height) / 2);
                        this.PanelRight.Visible = true;
                        region.Union(this.PanelRight.Bounds);
                    } else{
                        this.PanelRight.Visible = false;
                    }
                    if(this.ShouldPanelIndicatorVisible(DockState.DockTop)){
                        this.PanelTop.Location =
                                new Point(rectDockArea.X + (rectDockArea.Width - this.PanelTop.Width) / 2,
                                          rectDockArea.Y + this._PanelIndicatorMargin);
                        this.PanelTop.Visible = true;
                        region.Union(this.PanelTop.Bounds);
                    } else{
                        this.PanelTop.Visible = false;
                    }
                    if(this.ShouldPanelIndicatorVisible(DockState.DockBottom)){
                        this.PanelBottom.Location =
                                new Point(rectDockArea.X + (rectDockArea.Width - this.PanelBottom.Width) / 2,
                                          rectDockArea.Y + rectDockArea.Height - this.PanelBottom.Height
                                          - this._PanelIndicatorMargin);
                        this.PanelBottom.Visible = true;
                        region.Union(this.PanelBottom.Bounds);
                    } else{
                        this.PanelBottom.Visible = false;
                    }
                    if(this.ShouldPanelIndicatorVisible(DockState.Document)){
                        Rectangle rectDocumentWindow =
                                this.RectangleToClient(
                                        this.DockPanel.RectangleToScreen(this.DockPanel.DocumentWindowBounds));
                        this.PanelFill.Location =
                                new Point(rectDocumentWindow.X + (rectDocumentWindow.Width - this.PanelFill.Width) / 2,
                                          rectDocumentWindow.Y + (rectDocumentWindow.Height - this.PanelFill.Height) / 2);
                        this.PanelFill.Visible = true;
                        region.Union(this.PanelFill.Bounds);
                    } else{
                        this.PanelFill.Visible = false;
                    }
                    if(this.ShouldPaneDiamondVisible()){
                        Rectangle rect =
                                this.RectangleToClient(this.DockPane.RectangleToScreen(this.DockPane.ClientRectangle));
                        this.PaneDiamond.Location = new Point(rect.Left + (rect.Width - this.PaneDiamond.Width) / 2,
                                                              rect.Top + (rect.Height - this.PaneDiamond.Height) / 2);
                        this.PaneDiamond.Visible = true;
                        using(GraphicsPath graphicsPath = PaneIndicator.DisplayingGraphicsPath.Clone() as GraphicsPath){
                            Point[] pts = new Point[]{
                                                             new Point(this.PaneDiamond.Left, this.PaneDiamond.Top),
                                                             new Point(this.PaneDiamond.Right, this.PaneDiamond.Top),
                                                             new Point(this.PaneDiamond.Left, this.PaneDiamond.Bottom)
                                                     };
                            using(Matrix matrix = new Matrix(this.PaneDiamond.ClientRectangle, pts)){
                                graphicsPath.Transform(matrix);
                            }
                            region.Union(graphicsPath);
                        }
                    } else{
                        this.PaneDiamond.Visible = false;
                    }
                    this.Region = region;
                }
                private bool ShouldPanelIndicatorVisible(DockState dockState)
                {
                    if(!this.Visible){
                        return false;
                    }
                    if(this.DockPanel.DockWindows[dockState].Visible){
                        return false;
                    }
                    return this.DragHandler.DragSource.IsDockStateValid(dockState);
                }
                private bool ShouldPaneDiamondVisible()
                {
                    if(this.DockPane == null){
                        return false;
                    }
                    if(!this.DockPanel.AllowEndUserNestedDocking){
                        return false;
                    }
                    return this.DragHandler.DragSource.CanDockTo(this.DockPane);
                }
                public override void Show(bool bActivate)
                {
                    base.Show(bActivate);
                    this.Bounds = SystemInformation.VirtualScreen;
                    this.RefreshChanges();
                }
                public void TestDrop()
                {
                    Point pt = MousePosition;
                    this.DockPane = DockHelper.PaneAtPoint(pt, this.DockPanel);
                    if(TestDrop(this.PanelLeft, pt) != DockStyle.None){
                        this.HitTestResult = this.PanelLeft;
                    } else if(TestDrop(this.PanelRight, pt) != DockStyle.None){
                        this.HitTestResult = this.PanelRight;
                    } else if(TestDrop(this.PanelTop, pt) != DockStyle.None){
                        this.HitTestResult = this.PanelTop;
                    } else if(TestDrop(this.PanelBottom, pt) != DockStyle.None){
                        this.HitTestResult = this.PanelBottom;
                    } else if(TestDrop(this.PanelFill, pt) != DockStyle.None){
                        this.HitTestResult = this.PanelFill;
                    } else if(TestDrop(this.PaneDiamond, pt) != DockStyle.None){
                        this.HitTestResult = this.PaneDiamond;
                    } else{
                        this.HitTestResult = null;
                    }
                    if(this.HitTestResult != null){
                        if(this.HitTestResult is PaneIndicator){
                            this.DragHandler.Outline.Show(this.DockPane, this.HitTestResult.Status);
                        } else{
                            this.DragHandler.Outline.Show(this.DockPanel, this.HitTestResult.Status, this.FullPanelEdge);
                        }
                    }
                }
                private static DockStyle TestDrop(IHitTest hitTest, Point pt)
                {
                    return hitTest.Status = hitTest.HitTest(pt);
                }
            }
            #endregion

            #region Nested type: DockOutline
            private class DockOutline : DockOutlineBase
            {
                DragForm m_dragForm;
                public DockOutline()
                {
                    this.m_dragForm = new DragForm();
                    this.SetDragForm(Rectangle.Empty);
                    this.DragForm.BackColor = SystemColors.ActiveCaption;
                    this.DragForm.Opacity = 0.5;
                    this.DragForm.Show(false);
                }
                private DragForm DragForm
                {
                    get { return this.m_dragForm; }
                }
                protected override void OnShow()
                {
                    this.CalculateRegion();
                }
                protected override void OnClose()
                {
                    this.DragForm.Close();
                }
                private void CalculateRegion()
                {
                    if(this.SameAsOldValue){
                        return;
                    }
                    if(!this.FloatWindowBounds.IsEmpty){
                        this.SetOutline(this.FloatWindowBounds);
                    } else if(this.DockTo is DockPanel){
                        this.SetOutline(this.DockTo as DockPanel, this.Dock, (this.ContentIndex != 0));
                    } else if(this.DockTo is DockPane){
                        this.SetOutline(this.DockTo as DockPane, this.Dock, this.ContentIndex);
                    } else{
                        this.SetOutline();
                    }
                }
                private void SetOutline()
                {
                    this.SetDragForm(Rectangle.Empty);
                }
                private void SetOutline(Rectangle floatWindowBounds)
                {
                    this.SetDragForm(floatWindowBounds);
                }
                private void SetOutline(DockPanel dockPanel, DockStyle dock, bool fullPanelEdge)
                {
                    Rectangle rect = fullPanelEdge ? dockPanel.DockArea : dockPanel.DocumentWindowBounds;
                    rect.Location = dockPanel.PointToScreen(rect.Location);
                    if(dock == DockStyle.Top){
                        int height = dockPanel.GetDockWindowSize(DockState.DockTop);
                        rect = new Rectangle(rect.X, rect.Y, rect.Width, height);
                    } else if(dock == DockStyle.Bottom){
                        int height = dockPanel.GetDockWindowSize(DockState.DockBottom);
                        rect = new Rectangle(rect.X, rect.Bottom - height, rect.Width, height);
                    } else if(dock == DockStyle.Left){
                        int width = dockPanel.GetDockWindowSize(DockState.DockLeft);
                        rect = new Rectangle(rect.X, rect.Y, width, rect.Height);
                    } else if(dock == DockStyle.Right){
                        int width = dockPanel.GetDockWindowSize(DockState.DockRight);
                        rect = new Rectangle(rect.Right - width, rect.Y, width, rect.Height);
                    } else if(dock == DockStyle.Fill){
                        rect = dockPanel.DocumentWindowBounds;
                        rect.Location = dockPanel.PointToScreen(rect.Location);
                    }
                    this.SetDragForm(rect);
                }
                private void SetOutline(DockPane pane, DockStyle dock, int contentIndex)
                {
                    if(dock != DockStyle.Fill){
                        Rectangle rect = pane.DisplayingRectangle;
                        if(dock == DockStyle.Right){
                            rect.X += rect.Width / 2;
                        }
                        if(dock == DockStyle.Bottom){
                            rect.Y += rect.Height / 2;
                        }
                        if(dock == DockStyle.Left || dock == DockStyle.Right){
                            rect.Width -= rect.Width / 2;
                        }
                        if(dock == DockStyle.Top || dock == DockStyle.Bottom){
                            rect.Height -= rect.Height / 2;
                        }
                        rect.Location = pane.PointToScreen(rect.Location);
                        this.SetDragForm(rect);
                    } else if(contentIndex == -1){
                        Rectangle rect = pane.DisplayingRectangle;
                        rect.Location = pane.PointToScreen(rect.Location);
                        this.SetDragForm(rect);
                    } else{
                        using(GraphicsPath path = pane.TabStripControl.GetOutline(contentIndex)){
                            RectangleF rectF = path.GetBounds();
                            Rectangle rect = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width,
                                                           (int)rectF.Height);
                            using(
                                    Matrix matrix = new Matrix(rect,
                                                               new Point[]{
                                                                                  new Point(0, 0), new Point(rect.Width, 0),
                                                                                  new Point(0, rect.Height)
                                                                          })){
                                path.Transform(matrix);
                            }
                            Region region = new Region(path);
                            this.SetDragForm(rect, region);
                        }
                    }
                }
                private void SetDragForm(Rectangle rect)
                {
                    this.DragForm.Bounds = rect;
                    if(rect == Rectangle.Empty){
                        this.DragForm.Region = new Region(Rectangle.Empty);
                    } else if(this.DragForm.Region != null){
                        this.DragForm.Region = null;
                    }
                }
                private void SetDragForm(Rectangle rect, Region region)
                {
                    this.DragForm.Bounds = rect;
                    this.DragForm.Region = region;
                }
            }
            #endregion
        }
        #endregion
    }
}