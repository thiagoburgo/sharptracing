using System;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    partial class DockPanel
    {
        private AutoHideWindowControl AutoHideWindow
        {
            get { return this.m_autoHideWindow; }
        }
        internal Control AutoHideControl
        {
            get { return this.m_autoHideWindow; }
        }
        internal Rectangle AutoHideWindowRectangle
        {
            get
            {
                DockState state = this.AutoHideWindow.DockState;
                Rectangle rectDockArea = this.DockArea;
                if(this.ActiveAutoHideContent == null){
                    return Rectangle.Empty;
                }
                if(this.Parent == null){
                    return Rectangle.Empty;
                }
                Rectangle rect = Rectangle.Empty;
                double autoHideSize = this.ActiveAutoHideContent.DockHandler.AutoHidePortion;
                if(state == DockState.DockLeftAutoHide){
                    if(autoHideSize < 1){
                        autoHideSize = rectDockArea.Width * autoHideSize;
                    }
                    if(autoHideSize > rectDockArea.Width - MeasurePane.MinSize){
                        autoHideSize = rectDockArea.Width - MeasurePane.MinSize;
                    }
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y;
                    rect.Width = (int)autoHideSize;
                    rect.Height = rectDockArea.Height;
                } else if(state == DockState.DockRightAutoHide){
                    if(autoHideSize < 1){
                        autoHideSize = rectDockArea.Width * autoHideSize;
                    }
                    if(autoHideSize > rectDockArea.Width - MeasurePane.MinSize){
                        autoHideSize = rectDockArea.Width - MeasurePane.MinSize;
                    }
                    rect.X = rectDockArea.X + rectDockArea.Width - (int)autoHideSize;
                    rect.Y = rectDockArea.Y;
                    rect.Width = (int)autoHideSize;
                    rect.Height = rectDockArea.Height;
                } else if(state == DockState.DockTopAutoHide){
                    if(autoHideSize < 1){
                        autoHideSize = rectDockArea.Height * autoHideSize;
                    }
                    if(autoHideSize > rectDockArea.Height - MeasurePane.MinSize){
                        autoHideSize = rectDockArea.Height - MeasurePane.MinSize;
                    }
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y;
                    rect.Width = rectDockArea.Width;
                    rect.Height = (int)autoHideSize;
                } else if(state == DockState.DockBottomAutoHide){
                    if(autoHideSize < 1){
                        autoHideSize = rectDockArea.Height * autoHideSize;
                    }
                    if(autoHideSize > rectDockArea.Height - MeasurePane.MinSize){
                        autoHideSize = rectDockArea.Height - MeasurePane.MinSize;
                    }
                    rect.X = rectDockArea.X;
                    rect.Y = rectDockArea.Y + rectDockArea.Height - (int)autoHideSize;
                    rect.Width = rectDockArea.Width;
                    rect.Height = (int)autoHideSize;
                }
                return rect;
            }
        }
        internal void RefreshActiveAutoHideContent()
        {
            this.AutoHideWindow.RefreshActiveContent();
        }
        internal Rectangle GetAutoHideWindowBounds(Rectangle rectAutoHideWindow)
        {
            if(this.DocumentStyle == DocumentStyle.SystemMdi || this.DocumentStyle == DocumentStyle.DockingMdi){
                return (this.Parent == null)
                               ? Rectangle.Empty
                               : this.Parent.RectangleToClient(this.RectangleToScreen(rectAutoHideWindow));
            } else{
                return rectAutoHideWindow;
            }
        }
        internal void RefreshAutoHideStrip()
        {
            this.AutoHideStripControl.RefreshChanges();
        }

        #region Nested type: AutoHideWindowControl
        private class AutoHideWindowControl : Panel, ISplitterDragSource
        {
            private IDockContent m_activeContent = null;
            private DockPane m_activePane = null;
            private DockPanel m_dockPanel = null;
            private bool m_flagAnimate = true;
            private bool m_flagDragging = false;
            private SplitterControl m_splitter;
            private Timer m_timerMouseTrack;
            public AutoHideWindowControl(DockPanel dockPanel)
            {
                this.m_dockPanel = dockPanel;
                this.m_timerMouseTrack = new Timer();
                this.m_timerMouseTrack.Tick += new EventHandler(this.TimerMouseTrack_Tick);
                this.Visible = false;
                this.m_splitter = new SplitterControl(this);
                this.Controls.Add(this.m_splitter);
            }
            public DockPanel DockPanel
            {
                get { return this.m_dockPanel; }
            }
            public DockPane ActivePane
            {
                get { return this.m_activePane; }
            }
            public IDockContent ActiveContent
            {
                get { return this.m_activeContent; }
                set
                {
                    if(value == this.m_activeContent){
                        return;
                    }
                    if(value != null){
                        if(!DockHelper.IsDockStateAutoHide(value.DockHandler.DockState)
                           || value.DockHandler.DockPanel != this.DockPanel){
                            throw (new InvalidOperationException(Strings.DockPanel_ActiveAutoHideContent_InvalidValue));
                        }
                    }
                    this.DockPanel.SuspendLayout();
                    if(this.m_activeContent != null){
                        if(this.m_activeContent.DockHandler.Form.ContainsFocus){
                            this.DockPanel.ContentFocusManager.GiveUpFocus(this.m_activeContent);
                        }
                        this.AnimateWindow(false);
                    }
                    this.m_activeContent = value;
                    this.SetActivePane();
                    if(this.ActivePane != null){
                        this.ActivePane.ActiveContent = this.m_activeContent;
                    }
                    if(this.m_activeContent != null){
                        this.AnimateWindow(true);
                    }
                    this.DockPanel.ResumeLayout();
                    this.DockPanel.RefreshAutoHideStrip();
                    this.SetTimerMouseTrack();
                }
            }
            public DockState DockState
            {
                get { return this.ActiveContent == null ? DockState.Unknown : this.ActiveContent.DockHandler.DockState; }
            }
            private bool FlagAnimate
            {
                get { return this.m_flagAnimate; }
                set { this.m_flagAnimate = value; }
            }
            internal bool FlagDragging
            {
                get { return this.m_flagDragging; }
                set
                {
                    if(this.m_flagDragging == value){
                        return;
                    }
                    this.m_flagDragging = value;
                    this.SetTimerMouseTrack();
                }
            }
            protected virtual Rectangle DisplayingRectangle
            {
                get
                {
                    Rectangle rect = this.ClientRectangle;
                    // exclude the border and the splitter
                    if(this.DockState == DockState.DockBottomAutoHide){
                        rect.Y += 2 + Measures.SplitterSize;
                        rect.Height -= 2 + Measures.SplitterSize;
                    } else if(this.DockState == DockState.DockRightAutoHide){
                        rect.X += 2 + Measures.SplitterSize;
                        rect.Width -= 2 + Measures.SplitterSize;
                    } else if(this.DockState == DockState.DockTopAutoHide){
                        rect.Height -= 2 + Measures.SplitterSize;
                    } else if(this.DockState == DockState.DockLeftAutoHide){
                        rect.Width -= 2 + Measures.SplitterSize;
                    }
                    return rect;
                }
            }

            #region ISplitterDragSource Members
            void ISplitterDragSource.BeginDrag(Rectangle rectSplitter)
            {
                this.FlagDragging = true;
            }
            void ISplitterDragSource.EndDrag()
            {
                this.FlagDragging = false;
            }
            bool ISplitterDragSource.IsVertical
            {
                get
                {
                    return (this.DockState == DockState.DockLeftAutoHide
                            || this.DockState == DockState.DockRightAutoHide);
                }
            }
            Rectangle ISplitterDragSource.DragLimitBounds
            {
                get
                {
                    Rectangle rectLimit = this.DockPanel.DockArea;
                    if((this as ISplitterDragSource).IsVertical){
                        rectLimit.X += MeasurePane.MinSize;
                        rectLimit.Width -= 2 * MeasurePane.MinSize;
                    } else{
                        rectLimit.Y += MeasurePane.MinSize;
                        rectLimit.Height -= 2 * MeasurePane.MinSize;
                    }
                    return this.DockPanel.RectangleToScreen(rectLimit);
                }
            }
            void ISplitterDragSource.MoveSplitter(int offset)
            {
                Rectangle rectDockArea = this.DockPanel.DockArea;
                IDockContent content = this.ActiveContent;
                if(this.DockState == DockState.DockLeftAutoHide && rectDockArea.Width > 0){
                    if(content.DockHandler.AutoHidePortion < 1){
                        content.DockHandler.AutoHidePortion += ((double)offset) / (double)rectDockArea.Width;
                    } else{
                        content.DockHandler.AutoHidePortion = this.Width + offset;
                    }
                } else if(this.DockState == DockState.DockRightAutoHide && rectDockArea.Width > 0){
                    if(content.DockHandler.AutoHidePortion < 1){
                        content.DockHandler.AutoHidePortion -= ((double)offset) / (double)rectDockArea.Width;
                    } else{
                        content.DockHandler.AutoHidePortion = this.Width - offset;
                    }
                } else if(this.DockState == DockState.DockBottomAutoHide && rectDockArea.Height > 0){
                    if(content.DockHandler.AutoHidePortion < 1){
                        content.DockHandler.AutoHidePortion -= ((double)offset) / (double)rectDockArea.Height;
                    } else{
                        content.DockHandler.AutoHidePortion = this.Height - offset;
                    }
                } else if(this.DockState == DockState.DockTopAutoHide && rectDockArea.Height > 0){
                    if(content.DockHandler.AutoHidePortion < 1){
                        content.DockHandler.AutoHidePortion += ((double)offset) / (double)rectDockArea.Height;
                    } else{
                        content.DockHandler.AutoHidePortion = this.Height + offset;
                    }
                }
            }
            Control IDragSource.DragControl
            {
                get { return this; }
            }
            #endregion

            protected override void Dispose(bool disposing)
            {
                if(disposing){
                    this.m_timerMouseTrack.Dispose();
                }
                base.Dispose(disposing);
            }
            private void SetActivePane()
            {
                DockPane value = (this.ActiveContent == null ? null : this.ActiveContent.DockHandler.Pane);
                if(value == this.m_activePane){
                    return;
                }
                this.m_activePane = value;
            }
            private void AnimateWindow(bool show)
            {
                if(!this.FlagAnimate && this.Visible != show){
                    this.Visible = show;
                    return;
                }
                this.Parent.SuspendLayout();
                Rectangle rectSource = this.GetRectangle(!show);
                Rectangle rectTarget = this.GetRectangle(show);
                int dxLoc, dyLoc;
                int dWidth, dHeight;
                dxLoc = dyLoc = dWidth = dHeight = 0;
                if(this.DockState == DockState.DockTopAutoHide){
                    dHeight = show ? 1 : -1;
                } else if(this.DockState == DockState.DockLeftAutoHide){
                    dWidth = show ? 1 : -1;
                } else if(this.DockState == DockState.DockRightAutoHide){
                    dxLoc = show ? -1 : 1;
                    dWidth = show ? 1 : -1;
                } else if(this.DockState == DockState.DockBottomAutoHide){
                    dyLoc = (show ? -1 : 1);
                    dHeight = (show ? 1 : -1);
                }
                if(show){
                    this.Bounds =
                            this.DockPanel.GetAutoHideWindowBounds(new Rectangle(-rectTarget.Width, -rectTarget.Height,
                                                                                 rectTarget.Width, rectTarget.Height));
                    if(this.Visible == false){
                        this.Visible = true;
                    }
                    this.PerformLayout();
                }
                this.SuspendLayout();
                this.LayoutAnimateWindow(rectSource);
                if(this.Visible == false){
                    this.Visible = true;
                }
                int speedFactor = 1;
                int totalPixels = (rectSource.Width != rectTarget.Width)
                                          ? Math.Abs(rectSource.Width - rectTarget.Width)
                                          : Math.Abs(rectSource.Height - rectTarget.Height);
                int remainPixels = totalPixels;
                DateTime startingTime = DateTime.Now;
                while(rectSource != rectTarget){
                    DateTime startPerMove = DateTime.Now;
                    rectSource.X += dxLoc * speedFactor;
                    rectSource.Y += dyLoc * speedFactor;
                    rectSource.Width += dWidth * speedFactor;
                    rectSource.Height += dHeight * speedFactor;
                    if(Math.Sign(rectTarget.X - rectSource.X) != Math.Sign(dxLoc)){
                        rectSource.X = rectTarget.X;
                    }
                    if(Math.Sign(rectTarget.Y - rectSource.Y) != Math.Sign(dyLoc)){
                        rectSource.Y = rectTarget.Y;
                    }
                    if(Math.Sign(rectTarget.Width - rectSource.Width) != Math.Sign(dWidth)){
                        rectSource.Width = rectTarget.Width;
                    }
                    if(Math.Sign(rectTarget.Height - rectSource.Height) != Math.Sign(dHeight)){
                        rectSource.Height = rectTarget.Height;
                    }
                    this.LayoutAnimateWindow(rectSource);
                    if(this.Parent != null){
                        this.Parent.Update();
                    }
                    remainPixels -= speedFactor;
                    while(true){
                        TimeSpan time = new TimeSpan(0, 0, 0, 0, ANIMATE_TIME);
                        TimeSpan elapsedPerMove = DateTime.Now - startPerMove;
                        TimeSpan elapsedTime = DateTime.Now - startingTime;
                        if(((int)((time - elapsedTime).TotalMilliseconds)) <= 0){
                            speedFactor = remainPixels;
                            break;
                        } else{
                            speedFactor = remainPixels * (int)elapsedPerMove.TotalMilliseconds
                                          / (int)((time - elapsedTime).TotalMilliseconds);
                        }
                        if(speedFactor >= 1){
                            break;
                        }
                    }
                }
                this.ResumeLayout();
                this.Parent.ResumeLayout();
            }
            private void LayoutAnimateWindow(Rectangle rect)
            {
                this.Bounds = this.DockPanel.GetAutoHideWindowBounds(rect);
                Rectangle rectClient = this.ClientRectangle;
                if(this.DockState == DockState.DockLeftAutoHide){
                    this.ActivePane.Location =
                            new Point(rectClient.Right - 2 - Measures.SplitterSize - this.ActivePane.Width,
                                      this.ActivePane.Location.Y);
                } else if(this.DockState == DockState.DockTopAutoHide){
                    this.ActivePane.Location = new Point(this.ActivePane.Location.X,
                                                         rectClient.Bottom - 2 - Measures.SplitterSize
                                                         - this.ActivePane.Height);
                }
            }
            private Rectangle GetRectangle(bool show)
            {
                if(this.DockState == DockState.Unknown){
                    return Rectangle.Empty;
                }
                Rectangle rect = this.DockPanel.AutoHideWindowRectangle;
                if(show){
                    return rect;
                }
                if(this.DockState == DockState.DockLeftAutoHide){
                    rect.Width = 0;
                } else if(this.DockState == DockState.DockRightAutoHide){
                    rect.X += rect.Width;
                    rect.Width = 0;
                } else if(this.DockState == DockState.DockTopAutoHide){
                    rect.Height = 0;
                } else{
                    rect.Y += rect.Height;
                    rect.Height = 0;
                }
                return rect;
            }
            private void SetTimerMouseTrack()
            {
                if(this.ActivePane == null || this.ActivePane.IsActivated || this.FlagDragging){
                    this.m_timerMouseTrack.Enabled = false;
                    return;
                }
                // start the timer
                int hovertime = SystemInformation.MouseHoverTime;
                // assign a default value 400 in case of setting Timer.Interval invalid value exception
                if(hovertime <= 0){
                    hovertime = 400;
                }
                this.m_timerMouseTrack.Interval = 2 * (int)hovertime;
                this.m_timerMouseTrack.Enabled = true;
            }
            protected override void OnLayout(LayoutEventArgs levent)
            {
                this.DockPadding.All = 0;
                if(this.DockState == DockState.DockLeftAutoHide){
                    this.DockPadding.Right = 2;
                    this.m_splitter.Dock = DockStyle.Right;
                } else if(this.DockState == DockState.DockRightAutoHide){
                    this.DockPadding.Left = 2;
                    this.m_splitter.Dock = DockStyle.Left;
                } else if(this.DockState == DockState.DockTopAutoHide){
                    this.DockPadding.Bottom = 2;
                    this.m_splitter.Dock = DockStyle.Bottom;
                } else if(this.DockState == DockState.DockBottomAutoHide){
                    this.DockPadding.Top = 2;
                    this.m_splitter.Dock = DockStyle.Top;
                }
                Rectangle rectDisplaying = this.DisplayingRectangle;
                Rectangle rectHidden = new Rectangle(-rectDisplaying.Width, rectDisplaying.Y, rectDisplaying.Width,
                                                     rectDisplaying.Height);
                foreach(Control c in this.Controls){
                    DockPane pane = c as DockPane;
                    if(pane == null){
                        continue;
                    }
                    if(pane == this.ActivePane){
                        pane.Bounds = rectDisplaying;
                    } else{
                        pane.Bounds = rectHidden;
                    }
                }
                base.OnLayout(levent);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                // Draw the border
                Graphics g = e.Graphics;
                if(this.DockState == DockState.DockBottomAutoHide){
                    g.DrawLine(SystemPens.ControlLightLight, 0, 1, this.ClientRectangle.Right, 1);
                } else if(this.DockState == DockState.DockRightAutoHide){
                    g.DrawLine(SystemPens.ControlLightLight, 1, 0, 1, this.ClientRectangle.Bottom);
                } else if(this.DockState == DockState.DockTopAutoHide){
                    g.DrawLine(SystemPens.ControlDark, 0, this.ClientRectangle.Height - 2, this.ClientRectangle.Right,
                               this.ClientRectangle.Height - 2);
                    g.DrawLine(SystemPens.ControlDarkDark, 0, this.ClientRectangle.Height - 1,
                               this.ClientRectangle.Right, this.ClientRectangle.Height - 1);
                } else if(this.DockState == DockState.DockLeftAutoHide){
                    g.DrawLine(SystemPens.ControlDark, this.ClientRectangle.Width - 2, 0, this.ClientRectangle.Width - 2,
                               this.ClientRectangle.Bottom);
                    g.DrawLine(SystemPens.ControlDarkDark, this.ClientRectangle.Width - 1, 0,
                               this.ClientRectangle.Width - 1, this.ClientRectangle.Bottom);
                }
                base.OnPaint(e);
            }
            public void RefreshActiveContent()
            {
                if(this.ActiveContent == null){
                    return;
                }
                if(!DockHelper.IsDockStateAutoHide(this.ActiveContent.DockHandler.DockState)){
                    this.FlagAnimate = false;
                    this.ActiveContent = null;
                    this.FlagAnimate = true;
                }
            }
            public void RefreshActivePane()
            {
                this.SetTimerMouseTrack();
            }
            private void TimerMouseTrack_Tick(object sender, EventArgs e)
            {
                if(this.IsDisposed){
                    return;
                }
                if(this.ActivePane == null || this.ActivePane.IsActivated){
                    this.m_timerMouseTrack.Enabled = false;
                    return;
                }
                DockPane pane = this.ActivePane;
                Point ptMouseInAutoHideWindow = this.PointToClient(MousePosition);
                Point ptMouseInDockPanel = this.DockPanel.PointToClient(MousePosition);
                Rectangle rectTabStrip = this.DockPanel.GetTabStripRectangle(pane.DockState);
                if(!this.ClientRectangle.Contains(ptMouseInAutoHideWindow) && !rectTabStrip.Contains(ptMouseInDockPanel)){
                    this.ActiveContent = null;
                    this.m_timerMouseTrack.Enabled = false;
                }
            }

            #region Nested type: SplitterControl
            private class SplitterControl : SplitterBase
            {
                private AutoHideWindowControl m_autoHideWindow;
                public SplitterControl(AutoHideWindowControl autoHideWindow)
                {
                    this.m_autoHideWindow = autoHideWindow;
                }
                private AutoHideWindowControl AutoHideWindow
                {
                    get { return this.m_autoHideWindow; }
                }
                protected override int SplitterSize
                {
                    get { return Measures.SplitterSize; }
                }
                protected override void StartDrag()
                {
                    this.AutoHideWindow.DockPanel.BeginDrag(this.AutoHideWindow,
                                                            this.AutoHideWindow.RectangleToScreen(this.Bounds));
                }
            }
            #endregion

            #region consts
            private const int ANIMATE_TIME = 100; // in mini-seconds
            #endregion
        }
        #endregion
    }
}