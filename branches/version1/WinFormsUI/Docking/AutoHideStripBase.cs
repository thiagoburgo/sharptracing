using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public abstract partial class AutoHideStripBase : Control
    {
        private GraphicsPath m_displayingArea = null;
        private DockPanel m_dockPanel;
        private PaneCollection m_panesBottom;
        private PaneCollection m_panesLeft;
        private PaneCollection m_panesRight;
        private PaneCollection m_panesTop;
        protected AutoHideStripBase(DockPanel panel)
        {
            this.m_dockPanel = panel;
            this.m_panesTop = new PaneCollection(panel, DockState.DockTopAutoHide);
            this.m_panesBottom = new PaneCollection(panel, DockState.DockBottomAutoHide);
            this.m_panesLeft = new PaneCollection(panel, DockState.DockLeftAutoHide);
            this.m_panesRight = new PaneCollection(panel, DockState.DockRightAutoHide);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.Selectable, false);
        }
        protected DockPanel DockPanel
        {
            get { return this.m_dockPanel; }
        }
        protected PaneCollection PanesTop
        {
            get { return this.m_panesTop; }
        }
        protected PaneCollection PanesBottom
        {
            get { return this.m_panesBottom; }
        }
        protected PaneCollection PanesLeft
        {
            get { return this.m_panesLeft; }
        }
        protected PaneCollection PanesRight
        {
            get { return this.m_panesRight; }
        }
        protected Rectangle RectangleTopLeft
        {
            get
            {
                int height = this.MeasureHeight();
                return this.PanesTop.Count > 0 && this.PanesLeft.Count > 0
                               ? new Rectangle(0, 0, height, height)
                               : Rectangle.Empty;
            }
        }
        protected Rectangle RectangleTopRight
        {
            get
            {
                int height = this.MeasureHeight();
                return this.PanesTop.Count > 0 && this.PanesRight.Count > 0
                               ? new Rectangle(this.Width - height, 0, height, height)
                               : Rectangle.Empty;
            }
        }
        protected Rectangle RectangleBottomLeft
        {
            get
            {
                int height = this.MeasureHeight();
                return this.PanesBottom.Count > 0 && this.PanesLeft.Count > 0
                               ? new Rectangle(0, this.Height - height, height, height)
                               : Rectangle.Empty;
            }
        }
        protected Rectangle RectangleBottomRight
        {
            get
            {
                int height = this.MeasureHeight();
                return this.PanesBottom.Count > 0 && this.PanesRight.Count > 0
                               ? new Rectangle(this.Width - height, this.Height - height, height, height)
                               : Rectangle.Empty;
            }
        }
        private GraphicsPath DisplayingArea
        {
            get
            {
                if(this.m_displayingArea == null){
                    this.m_displayingArea = new GraphicsPath();
                }
                return this.m_displayingArea;
            }
        }
        protected PaneCollection GetPanes(DockState dockState)
        {
            if(dockState == DockState.DockTopAutoHide){
                return this.PanesTop;
            } else if(dockState == DockState.DockBottomAutoHide){
                return this.PanesBottom;
            } else if(dockState == DockState.DockLeftAutoHide){
                return this.PanesLeft;
            } else if(dockState == DockState.DockRightAutoHide){
                return this.PanesRight;
            } else{
                throw new ArgumentOutOfRangeException("dockState");
            }
        }
        internal int GetNumberOfPanes(DockState dockState)
        {
            return this.GetPanes(dockState).Count;
        }
        protected internal Rectangle GetTabStripRectangle(DockState dockState)
        {
            int height = this.MeasureHeight();
            if(dockState == DockState.DockTopAutoHide && this.PanesTop.Count > 0){
                return new Rectangle(this.RectangleTopLeft.Width, 0,
                                     this.Width - this.RectangleTopLeft.Width - this.RectangleTopRight.Width, height);
            } else if(dockState == DockState.DockBottomAutoHide && this.PanesBottom.Count > 0){
                return new Rectangle(this.RectangleBottomLeft.Width, this.Height - height,
                                     this.Width - this.RectangleBottomLeft.Width - this.RectangleBottomRight.Width,
                                     height);
            } else if(dockState == DockState.DockLeftAutoHide && this.PanesLeft.Count > 0){
                return new Rectangle(0, this.RectangleTopLeft.Width, height,
                                     this.Height - this.RectangleTopLeft.Height - this.RectangleBottomLeft.Height);
            } else if(dockState == DockState.DockRightAutoHide && this.PanesRight.Count > 0){
                return new Rectangle(this.Width - height, this.RectangleTopRight.Width, height,
                                     this.Height - this.RectangleTopRight.Height - this.RectangleBottomRight.Height);
            } else{
                return Rectangle.Empty;
            }
        }
        private void SetRegion()
        {
            this.DisplayingArea.Reset();
            this.DisplayingArea.AddRectangle(this.RectangleTopLeft);
            this.DisplayingArea.AddRectangle(this.RectangleTopRight);
            this.DisplayingArea.AddRectangle(this.RectangleBottomLeft);
            this.DisplayingArea.AddRectangle(this.RectangleBottomRight);
            this.DisplayingArea.AddRectangle(this.GetTabStripRectangle(DockState.DockTopAutoHide));
            this.DisplayingArea.AddRectangle(this.GetTabStripRectangle(DockState.DockBottomAutoHide));
            this.DisplayingArea.AddRectangle(this.GetTabStripRectangle(DockState.DockLeftAutoHide));
            this.DisplayingArea.AddRectangle(this.GetTabStripRectangle(DockState.DockRightAutoHide));
            this.Region = new Region(this.DisplayingArea);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(e.Button != MouseButtons.Left){
                return;
            }
            IDockContent content = this.HitTest();
            if(content == null){
                return;
            }
            content.DockHandler.Activate();
        }
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            IDockContent content = this.HitTest();
            if(content != null && this.DockPanel.ActiveAutoHideContent != content){
                this.DockPanel.ActiveAutoHideContent = content;
            }
            // requires further tracking of mouse hover behavior,
            this.ResetMouseEventArgs();
        }
        protected override void OnLayout(LayoutEventArgs levent)
        {
            this.RefreshChanges();
            base.OnLayout(levent);
        }
        internal void RefreshChanges()
        {
            if(this.IsDisposed){
                return;
            }
            this.SetRegion();
            this.OnRefreshChanges();
        }
        protected virtual void OnRefreshChanges() {}
        protected internal abstract int MeasureHeight();
        private IDockContent HitTest()
        {
            Point ptMouse = this.PointToClient(MousePosition);
            return this.HitTest(ptMouse);
        }
        protected virtual Tab CreateTab(IDockContent content)
        {
            return new Tab(content);
        }
        protected virtual Pane CreatePane(DockPane dockPane)
        {
            return new Pane(dockPane);
        }
        protected abstract IDockContent HitTest(Point point);

        #region Nested type: Pane
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected class Pane : IDisposable
        {
            private DockPane m_dockPane;
            protected internal Pane(DockPane dockPane)
            {
                this.m_dockPane = dockPane;
            }
            public DockPane DockPane
            {
                get { return this.m_dockPane; }
            }
            public TabCollection AutoHideTabs
            {
                get
                {
                    if(this.DockPane.AutoHideTabs == null){
                        this.DockPane.AutoHideTabs = new TabCollection(this.DockPane);
                    }
                    return this.DockPane.AutoHideTabs as TabCollection;
                }
            }

            #region IDisposable Members
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion

            ~Pane()
            {
                this.Dispose(false);
            }
            protected virtual void Dispose(bool disposing) {}
        }
        #endregion

        #region Nested type: PaneCollection
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected sealed class PaneCollection : IEnumerable<Pane>
        {
            private DockPanel m_dockPanel;
            private AutoHideStateCollection m_states;
            internal PaneCollection(DockPanel panel, DockState dockState)
            {
                this.m_dockPanel = panel;
                this.m_states = new AutoHideStateCollection();
                this.States[DockState.DockTopAutoHide].Selected = (dockState == DockState.DockTopAutoHide);
                this.States[DockState.DockBottomAutoHide].Selected = (dockState == DockState.DockBottomAutoHide);
                this.States[DockState.DockLeftAutoHide].Selected = (dockState == DockState.DockLeftAutoHide);
                this.States[DockState.DockRightAutoHide].Selected = (dockState == DockState.DockRightAutoHide);
            }
            public DockPanel DockPanel
            {
                get { return this.m_dockPanel; }
            }
            private AutoHideStateCollection States
            {
                get { return this.m_states; }
            }
            public int Count
            {
                get
                {
                    int count = 0;
                    foreach(DockPane pane in this.DockPanel.Panes){
                        if(this.States.ContainsPane(pane)){
                            count++;
                        }
                    }
                    return count;
                }
            }
            public Pane this[int index]
            {
                get
                {
                    int count = 0;
                    foreach(DockPane pane in this.DockPanel.Panes){
                        if(!this.States.ContainsPane(pane)){
                            continue;
                        }
                        if(count == index){
                            if(pane.AutoHidePane == null){
                                pane.AutoHidePane = this.DockPanel.AutoHideStripControl.CreatePane(pane);
                            }
                            return pane.AutoHidePane as Pane;
                        }
                        count++;
                    }
                    throw new ArgumentOutOfRangeException("index");
                }
            }

            #region IEnumerable<Pane> Members
            IEnumerator<Pane> IEnumerable<Pane>.GetEnumerator()
            {
                for(int i = 0; i < this.Count; i++){
                    yield return this[i];
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                for(int i = 0; i < this.Count; i++){
                    yield return this[i];
                }
            }
            #endregion

            public bool Contains(Pane pane)
            {
                return (this.IndexOf(pane) != -1);
            }
            public int IndexOf(Pane pane)
            {
                if(pane == null){
                    return -1;
                }
                int index = 0;
                foreach(DockPane dockPane in this.DockPanel.Panes){
                    if(!this.States.ContainsPane(pane.DockPane)){
                        continue;
                    }
                    if(pane == dockPane.AutoHidePane){
                        return index;
                    }
                    index++;
                }
                return -1;
            }

            #region Nested type: AutoHideState
            private class AutoHideState
            {
                public DockState m_dockState;
                public bool m_selected = false;
                public AutoHideState(DockState dockState)
                {
                    this.m_dockState = dockState;
                }
                public DockState DockState
                {
                    get { return this.m_dockState; }
                }
                public bool Selected
                {
                    get { return this.m_selected; }
                    set { this.m_selected = value; }
                }
            }
            #endregion

            #region Nested type: AutoHideStateCollection
            private class AutoHideStateCollection
            {
                private AutoHideState[] m_states;
                public AutoHideStateCollection()
                {
                    this.m_states = new AutoHideState[]{
                                                               new AutoHideState(DockState.DockTopAutoHide),
                                                               new AutoHideState(DockState.DockBottomAutoHide),
                                                               new AutoHideState(DockState.DockLeftAutoHide),
                                                               new AutoHideState(DockState.DockRightAutoHide)
                                                       };
                }
                public AutoHideState this[DockState dockState]
                {
                    get
                    {
                        for(int i = 0; i < this.m_states.Length; i++){
                            if(this.m_states[i].DockState == dockState){
                                return this.m_states[i];
                            }
                        }
                        throw new ArgumentOutOfRangeException("dockState");
                    }
                }
                public bool ContainsPane(DockPane pane)
                {
                    if(pane.IsHidden){
                        return false;
                    }
                    for(int i = 0; i < this.m_states.Length; i++){
                        if(this.m_states[i].DockState == pane.DockState && this.m_states[i].Selected){
                            return true;
                        }
                    }
                    return false;
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: Tab
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected class Tab : IDisposable
        {
            private IDockContent m_content;
            protected internal Tab(IDockContent content)
            {
                this.m_content = content;
            }
            public IDockContent Content
            {
                get { return this.m_content; }
            }

            #region IDisposable Members
            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion

            ~Tab()
            {
                this.Dispose(false);
            }
            protected virtual void Dispose(bool disposing) {}
        }
        #endregion

        #region Nested type: TabCollection
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected sealed class TabCollection : IEnumerable<Tab>
        {
            private DockPane m_dockPane = null;
            internal TabCollection(DockPane pane)
            {
                this.m_dockPane = pane;
            }
            public DockPane DockPane
            {
                get { return this.m_dockPane; }
            }
            public DockPanel DockPanel
            {
                get { return this.DockPane.DockPanel; }
            }
            public int Count
            {
                get { return this.DockPane.DisplayingContents.Count; }
            }
            public Tab this[int index]
            {
                get
                {
                    IDockContent content = this.DockPane.DisplayingContents[index];
                    if(content == null){
                        throw (new ArgumentOutOfRangeException("index"));
                    }
                    if(content.DockHandler.AutoHideTab == null){
                        content.DockHandler.AutoHideTab = (this.DockPanel.AutoHideStripControl.CreateTab(content));
                    }
                    return content.DockHandler.AutoHideTab as Tab;
                }
            }

            #region IEnumerable<Tab> Members
            IEnumerator<Tab> IEnumerable<Tab>.GetEnumerator()
            {
                for(int i = 0; i < this.Count; i++){
                    yield return this[i];
                }
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                for(int i = 0; i < this.Count; i++){
                    yield return this[i];
                }
            }
            #endregion

            public bool Contains(Tab tab)
            {
                return (IndexOf(tab) != -1);
            }
            public bool Contains(IDockContent content)
            {
                return (IndexOf(content) != -1);
            }
            public int IndexOf(Tab tab)
            {
                if(tab == null){
                    return -1;
                }
                return IndexOf(tab.Content);
            }
            public int IndexOf(IDockContent content)
            {
                return this.DockPane.DisplayingContents.IndexOf(content);
            }
        }
        #endregion
    }
}