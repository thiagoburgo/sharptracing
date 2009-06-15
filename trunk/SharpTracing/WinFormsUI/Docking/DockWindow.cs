using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    [ToolboxItem(false)]
    public partial class DockWindow : Panel, INestedPanesContainer, ISplitterDragSource
    {
        private DockPanel m_dockPanel;
        private DockState m_dockState;
        private NestedPaneCollection m_nestedPanes;
        private SplitterControl m_splitter;
        internal DockWindow(DockPanel dockPanel, DockState dockState)
        {
            this.m_nestedPanes = new NestedPaneCollection(this);
            this.m_dockPanel = dockPanel;
            this.m_dockState = dockState;
            this.Visible = false;
            this.SuspendLayout();
            if(this.DockState == DockState.DockLeft || this.DockState == DockState.DockRight
               || this.DockState == DockState.DockTop || this.DockState == DockState.DockBottom){
                this.m_splitter = new SplitterControl();
                this.Controls.Add(this.m_splitter);
            }
            if(this.DockState == DockState.DockLeft){
                this.Dock = DockStyle.Left;
                this.m_splitter.Dock = DockStyle.Right;
            } else if(this.DockState == DockState.DockRight){
                this.Dock = DockStyle.Right;
                this.m_splitter.Dock = DockStyle.Left;
            } else if(this.DockState == DockState.DockTop){
                this.Dock = DockStyle.Top;
                this.m_splitter.Dock = DockStyle.Bottom;
            } else if(this.DockState == DockState.DockBottom){
                this.Dock = DockStyle.Bottom;
                this.m_splitter.Dock = DockStyle.Top;
            } else if(this.DockState == DockState.Document){
                this.Dock = DockStyle.Fill;
            }
            this.ResumeLayout();
        }
        public DockPanel DockPanel
        {
            get { return this.m_dockPanel; }
        }
        internal DockPane DefaultPane
        {
            get { return this.VisibleNestedPanes.Count == 0 ? null : this.VisibleNestedPanes[0]; }
        }

        #region INestedPanesContainer Members
        public VisibleNestedPaneCollection VisibleNestedPanes
        {
            get { return this.NestedPanes.VisibleNestedPanes; }
        }
        public NestedPaneCollection NestedPanes
        {
            get { return this.m_nestedPanes; }
        }
        public DockState DockState
        {
            get { return this.m_dockState; }
        }
        public bool IsFloat
        {
            get { return this.DockState == DockState.Float; }
        }
        public virtual Rectangle DisplayingRectangle
        {
            get
            {
                Rectangle rect = this.ClientRectangle;
                // if DockWindow is document, exclude the border
                if(this.DockState == DockState.Document){
                    rect.X += 1;
                    rect.Y += 1;
                    rect.Width -= 2;
                    rect.Height -= 2;
                }
                        // exclude the splitter
                else if(this.DockState == DockState.DockLeft){
                    rect.Width -= Measures.SplitterSize;
                } else if(this.DockState == DockState.DockRight){
                    rect.X += Measures.SplitterSize;
                    rect.Width -= Measures.SplitterSize;
                } else if(this.DockState == DockState.DockTop){
                    rect.Height -= Measures.SplitterSize;
                } else if(this.DockState == DockState.DockBottom){
                    rect.Y += Measures.SplitterSize;
                    rect.Height -= Measures.SplitterSize;
                }
                return rect;
            }
        }
        #endregion

        #region ISplitterDragSource Members
        void ISplitterDragSource.BeginDrag(Rectangle rectSplitter) {}
        void ISplitterDragSource.EndDrag() {}
        bool ISplitterDragSource.IsVertical
        {
            get { return (this.DockState == DockState.DockLeft || this.DockState == DockState.DockRight); }
        }
        Rectangle ISplitterDragSource.DragLimitBounds
        {
            get
            {
                Rectangle rectLimit = this.DockPanel.DockArea;
                Point location;
                if((ModifierKeys & Keys.Shift) == 0){
                    location = this.Location;
                } else{
                    location = this.DockPanel.DockArea.Location;
                }
                if(((ISplitterDragSource)this).IsVertical){
                    rectLimit.X += MeasurePane.MinSize;
                    rectLimit.Width -= 2 * MeasurePane.MinSize;
                    rectLimit.Y = location.Y;
                    if((ModifierKeys & Keys.Shift) == 0){
                        rectLimit.Height = this.Height;
                    }
                } else{
                    rectLimit.Y += MeasurePane.MinSize;
                    rectLimit.Height -= 2 * MeasurePane.MinSize;
                    rectLimit.X = location.X;
                    if((ModifierKeys & Keys.Shift) == 0){
                        rectLimit.Width = this.Width;
                    }
                }
                return this.DockPanel.RectangleToScreen(rectLimit);
            }
        }
        void ISplitterDragSource.MoveSplitter(int offset)
        {
            if((ModifierKeys & Keys.Shift) != 0){
                this.SendToBack();
            }
            Rectangle rectDockArea = this.DockPanel.DockArea;
            if(this.DockState == DockState.DockLeft && rectDockArea.Width > 0){
                if(this.DockPanel.DockLeftPortion > 1){
                    this.DockPanel.DockLeftPortion = this.Width + offset;
                } else{
                    this.DockPanel.DockLeftPortion += ((double)offset) / (double)rectDockArea.Width;
                }
            } else if(this.DockState == DockState.DockRight && rectDockArea.Width > 0){
                if(this.DockPanel.DockRightPortion > 1){
                    this.DockPanel.DockRightPortion = this.Width - offset;
                } else{
                    this.DockPanel.DockRightPortion -= ((double)offset) / (double)rectDockArea.Width;
                }
            } else if(this.DockState == DockState.DockBottom && rectDockArea.Height > 0){
                if(this.DockPanel.DockBottomPortion > 1){
                    this.DockPanel.DockBottomPortion = this.Height - offset;
                } else{
                    this.DockPanel.DockBottomPortion -= ((double)offset) / (double)rectDockArea.Height;
                }
            } else if(this.DockState == DockState.DockTop && rectDockArea.Height > 0){
                if(this.DockPanel.DockTopPortion > 1){
                    this.DockPanel.DockTopPortion = this.Height + offset;
                } else{
                    this.DockPanel.DockTopPortion += ((double)offset) / (double)rectDockArea.Height;
                }
            }
        }
        Control IDragSource.DragControl
        {
            get { return this; }
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            // if DockWindow is document, draw the border
            if(this.DockState == DockState.Document){
                e.Graphics.DrawRectangle(SystemPens.ControlDark, this.ClientRectangle.X, this.ClientRectangle.Y,
                                         this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
            }
            base.OnPaint(e);
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            this.VisibleNestedPanes.Refresh();
            if(this.VisibleNestedPanes.Count == 0){
                if(this.Visible){
                    this.Visible = false;
                }
            } else if(!this.Visible){
                this.Visible = true;
                this.VisibleNestedPanes.Refresh();
            }
            base.OnLayout(levent);
        }
    }
}