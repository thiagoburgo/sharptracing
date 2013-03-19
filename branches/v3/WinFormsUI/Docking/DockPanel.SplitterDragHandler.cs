using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    partial class DockPanel
    {
        private SplitterDragHandler m_splitterDragHandler = null;
        private SplitterDragHandler GetSplitterDragHandler()
        {
            if(this.m_splitterDragHandler == null){
                this.m_splitterDragHandler = new SplitterDragHandler(this);
            }
            return this.m_splitterDragHandler;
        }
        internal void BeginDrag(ISplitterDragSource dragSource, Rectangle rectSplitter)
        {
            this.GetSplitterDragHandler().BeginDrag(dragSource, rectSplitter);
        }

        #region Nested type: SplitterDragHandler
        private sealed class SplitterDragHandler : DragHandler
        {
            private SplitterOutline m_outline;
            private Rectangle m_rectSplitter;
            public SplitterDragHandler(DockPanel dockPanel) : base(dockPanel) {}
            public new ISplitterDragSource DragSource
            {
                get { return base.DragSource as ISplitterDragSource; }
                private set { base.DragSource = value; }
            }
            private SplitterOutline Outline
            {
                get { return this.m_outline; }
                set { this.m_outline = value; }
            }
            private Rectangle RectSplitter
            {
                get { return this.m_rectSplitter; }
                set { this.m_rectSplitter = value; }
            }
            public void BeginDrag(ISplitterDragSource dragSource, Rectangle rectSplitter)
            {
                this.DragSource = dragSource;
                this.RectSplitter = rectSplitter;
                if(!this.BeginDrag()){
                    this.DragSource = null;
                    return;
                }
                this.Outline = new SplitterOutline();
                this.Outline.Show(rectSplitter);
                this.DragSource.BeginDrag(rectSplitter);
            }
            protected override void OnDragging()
            {
                this.Outline.Show(this.GetSplitterOutlineBounds(MousePosition));
            }
            protected override void OnEndDrag(bool abort)
            {
                this.DockPanel.SuspendLayout(true);
                this.Outline.Close();
                if(!abort){
                    this.DragSource.MoveSplitter(this.GetMovingOffset(MousePosition));
                }
                this.DragSource.EndDrag();
                this.DockPanel.ResumeLayout(true, true);
            }
            private int GetMovingOffset(Point ptMouse)
            {
                Rectangle rect = this.GetSplitterOutlineBounds(ptMouse);
                if(this.DragSource.IsVertical){
                    return rect.X - this.RectSplitter.X;
                } else{
                    return rect.Y - this.RectSplitter.Y;
                }
            }
            private Rectangle GetSplitterOutlineBounds(Point ptMouse)
            {
                Rectangle rectLimit = this.DragSource.DragLimitBounds;
                Rectangle rect = this.RectSplitter;
                if(rectLimit.Width <= 0 || rectLimit.Height <= 0){
                    return rect;
                }
                if(this.DragSource.IsVertical){
                    rect.X += ptMouse.X - this.StartMousePosition.X;
                    rect.Height = rectLimit.Height;
                } else{
                    rect.Y += ptMouse.Y - this.StartMousePosition.Y;
                    rect.Width = rectLimit.Width;
                }
                if(rect.Left < rectLimit.Left){
                    rect.X = rectLimit.X;
                }
                if(rect.Top < rectLimit.Top){
                    rect.Y = rectLimit.Y;
                }
                if(rect.Right > rectLimit.Right){
                    rect.X -= rect.Right - rectLimit.Right;
                }
                if(rect.Bottom > rectLimit.Bottom){
                    rect.Y -= rect.Bottom - rectLimit.Bottom;
                }
                return rect;
            }

            #region Nested type: SplitterOutline
            private class SplitterOutline
            {
                DragForm m_dragForm;
                public SplitterOutline()
                {
                    this.m_dragForm = new DragForm();
                    this.SetDragForm(Rectangle.Empty);
                    this.DragForm.BackColor = Color.Black;
                    this.DragForm.Opacity = 0.7;
                    this.DragForm.Show(false);
                }
                private DragForm DragForm
                {
                    get { return this.m_dragForm; }
                }
                public void Show(Rectangle rect)
                {
                    this.SetDragForm(rect);
                }
                public void Close()
                {
                    this.DragForm.Close();
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
            }
            #endregion
        }
        #endregion
    }
}