using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking.Win32;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class FloatWindow : Form, INestedPanesContainer, IDockDragSource
    {
        internal const int WM_CHECKDISPOSE = (int)(Msgs.WM_USER + 1);
        private bool m_allowEndUserDocking = true;
        private DockPanel m_dockPanel;
        private NestedPaneCollection m_nestedPanes;
        internal protected FloatWindow(DockPanel dockPanel, DockPane pane)
        {
            this.InternalConstruct(dockPanel, pane, false, Rectangle.Empty);
        }
        internal protected FloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
        {
            this.InternalConstruct(dockPanel, pane, true, bounds);
        }
        public bool AllowEndUserDocking
        {
            get { return this.m_allowEndUserDocking; }
            set { this.m_allowEndUserDocking = value; }
        }
        public DockPanel DockPanel
        {
            get { return this.m_dockPanel; }
        }

        #region IDockDragSource Members
        Control IDragSource.DragControl
        {
            get { return this; }
        }
        bool IDockDragSource.IsDockStateValid(DockState dockState)
        {
            return this.IsDockStateValid(dockState);
        }
        bool IDockDragSource.CanDockTo(DockPane pane)
        {
            if(!this.IsDockStateValid(pane.DockState)){
                return false;
            }
            if(pane.FloatWindow == this){
                return false;
            }
            return true;
        }
        Rectangle IDockDragSource.BeginDrag(Point ptMouse)
        {
            return this.Bounds;
        }
        public void FloatAt(Rectangle floatWindowBounds)
        {
            this.Bounds = floatWindowBounds;
        }
        public void DockTo(DockPane pane, DockStyle dockStyle, int contentIndex)
        {
            if(dockStyle == DockStyle.Fill){
                for(int i = this.NestedPanes.Count - 1; i >= 0; i--){
                    DockPane paneFrom = this.NestedPanes[i];
                    for(int j = paneFrom.Contents.Count - 1; j >= 0; j--){
                        IDockContent c = paneFrom.Contents[j];
                        c.DockHandler.Pane = pane;
                        if(contentIndex != -1){
                            pane.SetContentIndex(c, contentIndex);
                        }
                    }
                }
            } else{
                DockAlignment alignment = DockAlignment.Left;
                if(dockStyle == DockStyle.Left){
                    alignment = DockAlignment.Left;
                } else if(dockStyle == DockStyle.Right){
                    alignment = DockAlignment.Right;
                } else if(dockStyle == DockStyle.Top){
                    alignment = DockAlignment.Top;
                } else if(dockStyle == DockStyle.Bottom){
                    alignment = DockAlignment.Bottom;
                }
                MergeNestedPanes(this.VisibleNestedPanes, pane.NestedPanesContainer.NestedPanes, pane, alignment, 0.5);
            }
        }
        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            if(panel != this.DockPanel){
                throw new ArgumentException(Strings.IDockDragSource_DockTo_InvalidPanel, "panel");
            }
            NestedPaneCollection nestedPanesTo = null;
            if(dockStyle == DockStyle.Top){
                nestedPanesTo = this.DockPanel.DockWindows[DockState.DockTop].NestedPanes;
            } else if(dockStyle == DockStyle.Bottom){
                nestedPanesTo = this.DockPanel.DockWindows[DockState.DockBottom].NestedPanes;
            } else if(dockStyle == DockStyle.Left){
                nestedPanesTo = this.DockPanel.DockWindows[DockState.DockLeft].NestedPanes;
            } else if(dockStyle == DockStyle.Right){
                nestedPanesTo = this.DockPanel.DockWindows[DockState.DockRight].NestedPanes;
            } else if(dockStyle == DockStyle.Fill){
                nestedPanesTo = this.DockPanel.DockWindows[DockState.Document].NestedPanes;
            }
            DockPane prevPane = null;
            for(int i = nestedPanesTo.Count - 1; i >= 0; i--){
                if(nestedPanesTo[i] != this.VisibleNestedPanes[0]){
                    prevPane = nestedPanesTo[i];
                }
            }
            MergeNestedPanes(this.VisibleNestedPanes, nestedPanesTo, prevPane, DockAlignment.Left, 0.5);
        }
        #endregion

        #region INestedPanesContainer Members
        public NestedPaneCollection NestedPanes
        {
            get { return this.m_nestedPanes; }
        }
        public VisibleNestedPaneCollection VisibleNestedPanes
        {
            get { return this.NestedPanes.VisibleNestedPanes; }
        }
        public DockState DockState
        {
            get { return DockState.Float; }
        }
        public bool IsFloat
        {
            get { return this.DockState == DockState.Float; }
        }
        public virtual Rectangle DisplayingRectangle
        {
            get { return this.ClientRectangle; }
        }
        #endregion

        private void InternalConstruct(DockPanel dockPanel, DockPane pane, bool boundsSpecified, Rectangle bounds)
        {
            if(dockPanel == null){
                throw (new ArgumentNullException(Strings.FloatWindow_Constructor_NullDockPanel));
            }
            this.m_nestedPanes = new NestedPaneCollection(this);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = false;
            if(dockPanel.RightToLeft != this.RightToLeft){
                this.RightToLeft = dockPanel.RightToLeft;
            }
            if(this.RightToLeftLayout != dockPanel.RightToLeftLayout){
                this.RightToLeftLayout = dockPanel.RightToLeftLayout;
            }
            this.SuspendLayout();
            if(boundsSpecified){
                this.Bounds = bounds;
                this.StartPosition = FormStartPosition.Manual;
            } else{
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                this.Size = dockPanel.DefaultFloatWindowSize;
            }
            this.m_dockPanel = dockPanel;
            this.Owner = this.DockPanel.FindForm();
            this.DockPanel.AddFloatWindow(this);
            if(pane != null){
                pane.FloatWindow = this;
            }
            this.ResumeLayout();
        }
        protected override void Dispose(bool disposing)
        {
            if(disposing){
                if(this.DockPanel != null){
                    this.DockPanel.RemoveFloatWindow(this);
                }
                this.m_dockPanel = null;
            }
            base.Dispose(disposing);
        }
        internal bool IsDockStateValid(DockState dockState)
        {
            foreach(DockPane pane in this.NestedPanes){
                foreach(IDockContent content in pane.Contents){
                    if(!DockHelper.IsDockStateValid(dockState, content.DockHandler.DockAreas)){
                        return false;
                    }
                }
            }
            return true;
        }
        protected override void OnActivated(EventArgs e)
        {
            this.DockPanel.FloatWindows.BringWindowToFront(this);
            base.OnActivated(e);
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            this.VisibleNestedPanes.Refresh();
            this.RefreshChanges();
            this.Visible = (this.VisibleNestedPanes.Count > 0);
            this.SetText();
            base.OnLayout(levent);
        }
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters",
                MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        internal void SetText()
        {
            DockPane theOnlyPane = (this.VisibleNestedPanes.Count == 1) ? this.VisibleNestedPanes[0] : null;
            if(theOnlyPane == null){
                this.Text = " ";
                // use " " instead of string.Empty because the whole title bar will disappear when ControlBox is set to false.
            } else if(theOnlyPane.ActiveContent == null){
                this.Text = " ";
            } else{
                this.Text = theOnlyPane.ActiveContent.DockHandler.TabText;
            }
        }
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Rectangle rectWorkArea = SystemInformation.VirtualScreen;
            if(y + height > rectWorkArea.Bottom){
                y -= (y + height) - rectWorkArea.Bottom;
            }
            if(y < rectWorkArea.Top){
                y += rectWorkArea.Top - y;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == (int)Msgs.WM_NCLBUTTONDOWN){
                if(this.IsDisposed){
                    return;
                }
                uint result = NativeMethods.SendMessage(this.Handle, (int)Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if(result == 2 && this.DockPanel.AllowEndUserDocking && this.AllowEndUserDocking) // HITTEST_CAPTION
                {
                    this.Activate();
                    this.m_dockPanel.BeginDrag(this);
                } else{
                    base.WndProc(ref m);
                }
                return;
            } else if(m.Msg == (int)Msgs.WM_NCRBUTTONDOWN){
                uint result = NativeMethods.SendMessage(this.Handle, (int)Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if(result == 2) // HITTEST_CAPTION
                {
                    DockPane theOnlyPane = (this.VisibleNestedPanes.Count == 1) ? this.VisibleNestedPanes[0] : null;
                    if(theOnlyPane != null && theOnlyPane.ActiveContent != null){
                        theOnlyPane.ShowTabPageContextMenu(this, this.PointToClient(MousePosition));
                        return;
                    }
                }
                base.WndProc(ref m);
                return;
            } else if(m.Msg == (int)Msgs.WM_CLOSE){
                if(this.NestedPanes.Count == 0){
                    base.WndProc(ref m);
                    return;
                }
                for(int i = this.NestedPanes.Count - 1; i >= 0; i--){
                    DockContentCollection contents = this.NestedPanes[i].Contents;
                    for(int j = contents.Count - 1; j >= 0; j--){
                        IDockContent content = contents[j];
                        if(content.DockHandler.DockState != DockState.Float){
                            continue;
                        }
                        if(!content.DockHandler.CloseButton){
                            continue;
                        }
                        if(content.DockHandler.HideOnClose){
                            content.DockHandler.Hide();
                        } else{
                            content.DockHandler.Close();
                        }
                    }
                }
                return;
            } else if(m.Msg == (int)Msgs.WM_NCLBUTTONDBLCLK){
                uint result = NativeMethods.SendMessage(this.Handle, (int)Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if(result != 2) // HITTEST_CAPTION
                {
                    base.WndProc(ref m);
                    return;
                }
                this.DockPanel.SuspendLayout(true);
                // Restore to panel
                foreach(DockPane pane in this.NestedPanes){
                    if(pane.DockState != DockState.Float){
                        continue;
                    }
                    pane.RestoreToPanel();
                }
                this.DockPanel.ResumeLayout(true, true);
                return;
            } else if(m.Msg == WM_CHECKDISPOSE){
                if(this.NestedPanes.Count == 0){
                    this.Dispose();
                }
                return;
            }
            base.WndProc(ref m);
        }
        internal void RefreshChanges()
        {
            if(this.IsDisposed){
                return;
            }
            if(this.VisibleNestedPanes.Count == 0){
                this.ControlBox = true;
                return;
            }
            for(int i = this.VisibleNestedPanes.Count - 1; i >= 0; i--){
                DockContentCollection contents = this.VisibleNestedPanes[i].Contents;
                for(int j = contents.Count - 1; j >= 0; j--){
                    IDockContent content = contents[j];
                    if(content.DockHandler.DockState != DockState.Float){
                        continue;
                    }
                    if(content.DockHandler.CloseButton){
                        this.ControlBox = true;
                        return;
                    }
                }
            }
            this.ControlBox = false;
        }
        internal void TestDrop(IDockDragSource dragSource, DockOutlineBase dockOutline)
        {
            if(this.VisibleNestedPanes.Count == 1){
                DockPane pane = this.VisibleNestedPanes[0];
                if(!dragSource.CanDockTo(pane)){
                    return;
                }
                Point ptMouse = MousePosition;
                uint lParam = Win32Helper.MakeLong(ptMouse.X, ptMouse.Y);
                if(NativeMethods.SendMessage(this.Handle, (int)Msgs.WM_NCHITTEST, 0, lParam) == (uint)HitTest.HTCAPTION){
                    dockOutline.Show(this.VisibleNestedPanes[0], -1);
                }
            }
        }
        private static void MergeNestedPanes(VisibleNestedPaneCollection nestedPanesFrom,
                                             NestedPaneCollection nestedPanesTo, DockPane prevPane,
                                             DockAlignment alignment, double proportion)
        {
            if(nestedPanesFrom.Count == 0){
                return;
            }
            int count = nestedPanesFrom.Count;
            DockPane[] panes = new DockPane[count];
            DockPane[] prevPanes = new DockPane[count];
            DockAlignment[] alignments = new DockAlignment[count];
            double[] proportions = new double[count];
            for(int i = 0; i < count; i++){
                panes[i] = nestedPanesFrom[i];
                prevPanes[i] = nestedPanesFrom[i].NestedDockingStatus.PreviousPane;
                alignments[i] = nestedPanesFrom[i].NestedDockingStatus.Alignment;
                proportions[i] = nestedPanesFrom[i].NestedDockingStatus.Proportion;
            }
            DockPane pane = panes[0].DockTo(nestedPanesTo.Container, prevPane, alignment, proportion);
            panes[0].DockState = nestedPanesTo.DockState;
            for(int i = 1; i < count; i++){
                for(int j = i; j < count; j++){
                    if(prevPanes[j] == panes[i - 1]){
                        prevPanes[j] = pane;
                    }
                }
                pane = panes[i].DockTo(nestedPanesTo.Container, prevPanes[i], alignments[i], proportions[i]);
                panes[i].DockState = nestedPanesTo.DockState;
            }
        }
    }
}