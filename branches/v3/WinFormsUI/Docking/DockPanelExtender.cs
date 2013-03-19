using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    public sealed class DockPanelExtender
    {
        private IAutoHideStripFactory m_autoHideStripFactory = null;
        private IDockPaneCaptionFactory m_dockPaneCaptionFactory = null;
        private IDockPaneFactory m_dockPaneFactory = null;
        private DockPanel m_dockPanel;
        private IDockPaneStripFactory m_dockPaneStripFactory = null;
        private IFloatWindowFactory m_floatWindowFactory = null;
        internal DockPanelExtender(DockPanel dockPanel)
        {
            this.m_dockPanel = dockPanel;
        }
        private DockPanel DockPanel
        {
            get { return this.m_dockPanel; }
        }
        public IDockPaneFactory DockPaneFactory
        {
            get
            {
                if(this.m_dockPaneFactory == null){
                    this.m_dockPaneFactory = new DefaultDockPaneFactory();
                }
                return this.m_dockPaneFactory;
            }
            set
            {
                if(this.DockPanel.Panes.Count > 0){
                    throw new InvalidOperationException();
                }
                this.m_dockPaneFactory = value;
            }
        }
        public IFloatWindowFactory FloatWindowFactory
        {
            get
            {
                if(this.m_floatWindowFactory == null){
                    this.m_floatWindowFactory = new DefaultFloatWindowFactory();
                }
                return this.m_floatWindowFactory;
            }
            set
            {
                if(this.DockPanel.FloatWindows.Count > 0){
                    throw new InvalidOperationException();
                }
                this.m_floatWindowFactory = value;
            }
        }
        public IDockPaneCaptionFactory DockPaneCaptionFactory
        {
            get
            {
                if(this.m_dockPaneCaptionFactory == null){
                    this.m_dockPaneCaptionFactory = new DefaultDockPaneCaptionFactory();
                }
                return this.m_dockPaneCaptionFactory;
            }
            set
            {
                if(this.DockPanel.Panes.Count > 0){
                    throw new InvalidOperationException();
                }
                this.m_dockPaneCaptionFactory = value;
            }
        }
        public IDockPaneStripFactory DockPaneStripFactory
        {
            get
            {
                if(this.m_dockPaneStripFactory == null){
                    this.m_dockPaneStripFactory = new DefaultDockPaneStripFactory();
                }
                return this.m_dockPaneStripFactory;
            }
            set
            {
                if(this.DockPanel.Contents.Count > 0){
                    throw new InvalidOperationException();
                }
                this.m_dockPaneStripFactory = value;
            }
        }
        public IAutoHideStripFactory AutoHideStripFactory
        {
            get
            {
                if(this.m_autoHideStripFactory == null){
                    this.m_autoHideStripFactory = new DefaultAutoHideStripFactory();
                }
                return this.m_autoHideStripFactory;
            }
            set
            {
                if(this.DockPanel.Contents.Count > 0){
                    throw new InvalidOperationException();
                }
                if(this.m_autoHideStripFactory == value){
                    return;
                }
                this.m_autoHideStripFactory = value;
                this.DockPanel.ResetAutoHideStripControl();
            }
        }

        #region Nested type: IAutoHideStripFactory
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IAutoHideStripFactory
        {
            AutoHideStripBase CreateAutoHideStrip(DockPanel panel);
        }
        #endregion

        #region DefaultDockPaneFactory
        private class DefaultDockPaneFactory : IDockPaneFactory
        {
            #region IDockPaneFactory Members
            public DockPane CreateDockPane(IDockContent content, DockState visibleState, bool show)
            {
                return new DockPane(content, visibleState, show);
            }
            public DockPane CreateDockPane(IDockContent content, FloatWindow floatWindow, bool show)
            {
                return new DockPane(content, floatWindow, show);
            }
            public DockPane CreateDockPane(IDockContent content, DockPane prevPane, DockAlignment alignment,
                                           double proportion, bool show)
            {
                return new DockPane(content, prevPane, alignment, proportion, show);
            }
            public DockPane CreateDockPane(IDockContent content, Rectangle floatWindowBounds, bool show)
            {
                return new DockPane(content, floatWindowBounds, show);
            }
            #endregion
        }
        #endregion

        #region DefaultFloatWindowFactory
        private class DefaultFloatWindowFactory : IFloatWindowFactory
        {
            #region IFloatWindowFactory Members
            public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane)
            {
                return new FloatWindow(dockPanel, pane);
            }
            public FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
            {
                return new FloatWindow(dockPanel, pane, bounds);
            }
            #endregion
        }
        #endregion

        #region DefaultDockPaneCaptionFactory
        private class DefaultDockPaneCaptionFactory : IDockPaneCaptionFactory
        {
            #region IDockPaneCaptionFactory Members
            public DockPaneCaptionBase CreateDockPaneCaption(DockPane pane)
            {
                return new VS2005DockPaneCaption(pane);
            }
            #endregion
        }
        #endregion

        #region DefaultDockPaneTabStripFactory
        private class DefaultDockPaneStripFactory : IDockPaneStripFactory
        {
            #region IDockPaneStripFactory Members
            public DockPaneStripBase CreateDockPaneStrip(DockPane pane)
            {
                return new VS2005DockPaneStrip(pane);
            }
            #endregion
        }
        #endregion

        #region DefaultAutoHideStripFactory
        private class DefaultAutoHideStripFactory : IAutoHideStripFactory
        {
            #region IAutoHideStripFactory Members
            public AutoHideStripBase CreateAutoHideStrip(DockPanel panel)
            {
                return new VS2005AutoHideStrip(panel);
            }
            #endregion
        }
        #endregion

        #region Nested type: IDockPaneCaptionFactory
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IDockPaneCaptionFactory
        {
            DockPaneCaptionBase CreateDockPaneCaption(DockPane pane);
        }
        #endregion

        #region Nested type: IDockPaneFactory
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IDockPaneFactory
        {
            DockPane CreateDockPane(IDockContent content, DockState visibleState, bool show);
            [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
            DockPane CreateDockPane(IDockContent content, FloatWindow floatWindow, bool show);
            DockPane CreateDockPane(IDockContent content, DockPane previousPane, DockAlignment alignment,
                                    double proportion, bool show);
            [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
            DockPane CreateDockPane(IDockContent content, Rectangle floatWindowBounds, bool show);
        }
        #endregion

        #region Nested type: IDockPaneStripFactory
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IDockPaneStripFactory
        {
            DockPaneStripBase CreateDockPaneStrip(DockPane pane);
        }
        #endregion

        #region Nested type: IFloatWindowFactory
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        public interface IFloatWindowFactory
        {
            FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane);
            FloatWindow CreateFloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds);
        }
        #endregion
    }
}