using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    public sealed class VisibleNestedPaneCollection : ReadOnlyCollection<DockPane>
    {
        private NestedPaneCollection m_nestedPanes;
        internal VisibleNestedPaneCollection(NestedPaneCollection nestedPanes) : base(new List<DockPane>())
        {
            this.m_nestedPanes = nestedPanes;
        }
        public NestedPaneCollection NestedPanes
        {
            get { return this.m_nestedPanes; }
        }
        public INestedPanesContainer Container
        {
            get { return this.NestedPanes.Container; }
        }
        public DockState DockState
        {
            get { return this.NestedPanes.DockState; }
        }
        public bool IsFloat
        {
            get { return this.NestedPanes.IsFloat; }
        }
        internal void Refresh()
        {
            this.Items.Clear();
            for(int i = 0; i < this.NestedPanes.Count; i++){
                DockPane pane = this.NestedPanes[i];
                NestedDockingStatus status = pane.NestedDockingStatus;
                status.SetDisplayingStatus(true, status.PreviousPane, status.Alignment, status.Proportion);
                this.Items.Add(pane);
            }
            foreach(DockPane pane in this.NestedPanes){
                if(pane.DockState != this.DockState || pane.IsHidden){
                    pane.Bounds = Rectangle.Empty;
                    pane.SplitterBounds = Rectangle.Empty;
                    this.Remove(pane);
                }
            }
            this.CalculateBounds();
            foreach(DockPane pane in this){
                NestedDockingStatus status = pane.NestedDockingStatus;
                pane.Bounds = status.PaneBounds;
                pane.SplitterBounds = status.SplitterBounds;
                pane.SplitterAlignment = status.Alignment;
            }
        }
        private void Remove(DockPane pane)
        {
            if(!this.Contains(pane)){
                return;
            }
            NestedDockingStatus statusPane = pane.NestedDockingStatus;
            DockPane lastNestedPane = null;
            for(int i = this.Count - 1; i > this.IndexOf(pane); i--){
                if(this[i].NestedDockingStatus.PreviousPane == pane){
                    lastNestedPane = this[i];
                    break;
                }
            }
            if(lastNestedPane != null){
                int indexLastNestedPane = this.IndexOf(lastNestedPane);
                this.Items.Remove(lastNestedPane);
                this.Items[this.IndexOf(pane)] = lastNestedPane;
                NestedDockingStatus lastNestedDock = lastNestedPane.NestedDockingStatus;
                lastNestedDock.SetDisplayingStatus(true, statusPane.DisplayingPreviousPane,
                                                   statusPane.DisplayingAlignment, statusPane.DisplayingProportion);
                for(int i = indexLastNestedPane - 1; i > this.IndexOf(lastNestedPane); i--){
                    NestedDockingStatus status = this[i].NestedDockingStatus;
                    if(status.PreviousPane == pane){
                        status.SetDisplayingStatus(true, lastNestedPane, status.DisplayingAlignment,
                                                   status.DisplayingProportion);
                    }
                }
            } else{
                this.Items.Remove(pane);
            }
            statusPane.SetDisplayingStatus(false, null, DockAlignment.Left, 0.5);
        }
        private void CalculateBounds()
        {
            if(this.Count == 0){
                return;
            }
            this[0].NestedDockingStatus.SetDisplayingBounds(this.Container.DisplayingRectangle,
                                                            this.Container.DisplayingRectangle, Rectangle.Empty);
            for(int i = 1; i < this.Count; i++){
                DockPane pane = this[i];
                NestedDockingStatus status = pane.NestedDockingStatus;
                DockPane prevPane = status.DisplayingPreviousPane;
                NestedDockingStatus statusPrev = prevPane.NestedDockingStatus;
                Rectangle rect = statusPrev.PaneBounds;
                bool bVerticalSplitter = (status.DisplayingAlignment == DockAlignment.Left
                                          || status.DisplayingAlignment == DockAlignment.Right);
                Rectangle rectThis = rect;
                Rectangle rectPrev = rect;
                Rectangle rectSplitter = rect;
                if(status.DisplayingAlignment == DockAlignment.Left){
                    rectThis.Width = (int)((double)rect.Width * status.DisplayingProportion)
                                     - (Measures.SplitterSize / 2);
                    rectSplitter.X = rectThis.X + rectThis.Width;
                    rectSplitter.Width = Measures.SplitterSize;
                    rectPrev.X = rectSplitter.X + rectSplitter.Width;
                    rectPrev.Width = rect.Width - rectThis.Width - rectSplitter.Width;
                } else if(status.DisplayingAlignment == DockAlignment.Right){
                    rectPrev.Width = (rect.Width - (int)((double)rect.Width * status.DisplayingProportion))
                                     - (Measures.SplitterSize / 2);
                    rectSplitter.X = rectPrev.X + rectPrev.Width;
                    rectSplitter.Width = Measures.SplitterSize;
                    rectThis.X = rectSplitter.X + rectSplitter.Width;
                    rectThis.Width = rect.Width - rectPrev.Width - rectSplitter.Width;
                } else if(status.DisplayingAlignment == DockAlignment.Top){
                    rectThis.Height = (int)((double)rect.Height * status.DisplayingProportion)
                                      - (Measures.SplitterSize / 2);
                    rectSplitter.Y = rectThis.Y + rectThis.Height;
                    rectSplitter.Height = Measures.SplitterSize;
                    rectPrev.Y = rectSplitter.Y + rectSplitter.Height;
                    rectPrev.Height = rect.Height - rectThis.Height - rectSplitter.Height;
                } else if(status.DisplayingAlignment == DockAlignment.Bottom){
                    rectPrev.Height = (rect.Height - (int)((double)rect.Height * status.DisplayingProportion))
                                      - (Measures.SplitterSize / 2);
                    rectSplitter.Y = rectPrev.Y + rectPrev.Height;
                    rectSplitter.Height = Measures.SplitterSize;
                    rectThis.Y = rectSplitter.Y + rectSplitter.Height;
                    rectThis.Height = rect.Height - rectPrev.Height - rectSplitter.Height;
                } else{
                    rectThis = Rectangle.Empty;
                }
                rectSplitter.Intersect(rect);
                rectThis.Intersect(rect);
                rectPrev.Intersect(rect);
                status.SetDisplayingBounds(rect, rectThis, rectSplitter);
                statusPrev.SetDisplayingBounds(statusPrev.LogicalBounds, rectPrev, statusPrev.SplitterBounds);
            }
        }
    }
}