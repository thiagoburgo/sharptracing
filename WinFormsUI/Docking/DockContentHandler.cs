using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking.Win32;

namespace WeifenLuo.WinFormsUI.Docking
{
    public delegate string GetPersistStringCallback();

    public class DockContentHandler : IDisposable, IDockDragSource
    {
        private IntPtr m_activeWindowHandle = IntPtr.Zero;
        private DockAreas m_allowedAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop
                                           | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float;
        private bool m_allowEndUserDocking = true;
        private double m_autoHidePortion = 0.25;
        private IDisposable m_autoHideTab = null;
        private bool m_closeButton = true;
        private int m_countSetDockState = 0;
        private DockPanel m_dockPanel = null;
        private DockState m_dockState = DockState.Unknown;
        private EventHandlerList m_events;
        private bool m_flagClipWindow = false;
        private DockPane m_floatPane = null;
        private Form m_form;
        private GetPersistStringCallback m_getPersistStringCallback = null;
        private bool m_hideOnClose = false;
        private bool m_isActivated = false;
        private bool m_isFloat = false;
        private bool m_isHidden = true;
        private IDockContent m_nextActive = null;
        private DockPane m_panelPane = null;
        private IDockContent m_previousActive = null;
        private DockState m_showHint = DockState.Unknown;
        private DockPaneStripBase.Tab m_tab = null;
        private ContextMenu m_tabPageContextMenu = null;
        private ContextMenuStrip m_tabPageContextMenuStrip = null;
        private string m_tabText = null;
        private string m_toolTipText = null;
        private DockState m_visibleState = DockState.Unknown;
        public DockContentHandler(Form form) : this(form, null) {}
        public DockContentHandler(Form form, GetPersistStringCallback getPersistStringCallback)
        {
            if(!(form is IDockContent)){
                throw new ArgumentException(Strings.DockContent_Constructor_InvalidForm, "form");
            }
            this.m_form = form;
            this.m_getPersistStringCallback = getPersistStringCallback;
            this.m_events = new EventHandlerList();
            this.Form.Disposed += new EventHandler(this.Form_Disposed);
            this.Form.TextChanged += new EventHandler(this.Form_TextChanged);
        }
        public Form Form
        {
            get { return this.m_form; }
        }
        public IDockContent Content
        {
            get { return this.Form as IDockContent; }
        }
        public IDockContent PreviousActive
        {
            get { return this.m_previousActive; }
            internal set { this.m_previousActive = value; }
        }
        public IDockContent NextActive
        {
            get { return this.m_nextActive; }
            internal set { this.m_nextActive = value; }
        }
        private EventHandlerList Events
        {
            get { return this.m_events; }
        }
        public bool AllowEndUserDocking
        {
            get { return this.m_allowEndUserDocking; }
            set { this.m_allowEndUserDocking = value; }
        }
        public double AutoHidePortion
        {
            get { return this.m_autoHidePortion; }
            set
            {
                if(value <= 0){
                    throw (new ArgumentOutOfRangeException(Strings.DockContentHandler_AutoHidePortion_OutOfRange));
                }
                if(this.m_autoHidePortion == value){
                    return;
                }
                this.m_autoHidePortion = value;
                if(this.DockPanel == null){
                    return;
                }
                if(this.DockPanel.ActiveAutoHideContent == this.Content){
                    this.DockPanel.PerformLayout();
                }
            }
        }
        public bool CloseButton
        {
            get { return this.m_closeButton; }
            set
            {
                if(this.m_closeButton == value){
                    return;
                }
                this.m_closeButton = value;
                if(this.Pane != null){
                    if(this.Pane.ActiveContent.DockHandler == this){
                        this.Pane.RefreshChanges();
                    }
                }
            }
        }
        private DockState DefaultDockState
        {
            get
            {
                if(this.ShowHint != DockState.Unknown && this.ShowHint != DockState.Hidden){
                    return this.ShowHint;
                }
                if((this.DockAreas & DockAreas.Document) != 0){
                    return DockState.Document;
                }
                if((this.DockAreas & DockAreas.DockRight) != 0){
                    return DockState.DockRight;
                }
                if((this.DockAreas & DockAreas.DockLeft) != 0){
                    return DockState.DockLeft;
                }
                if((this.DockAreas & DockAreas.DockBottom) != 0){
                    return DockState.DockBottom;
                }
                if((this.DockAreas & DockAreas.DockTop) != 0){
                    return DockState.DockTop;
                }
                return DockState.Unknown;
            }
        }
        private DockState DefaultShowState
        {
            get
            {
                if(this.ShowHint != DockState.Unknown){
                    return this.ShowHint;
                }
                if((this.DockAreas & DockAreas.Document) != 0){
                    return DockState.Document;
                }
                if((this.DockAreas & DockAreas.DockRight) != 0){
                    return DockState.DockRight;
                }
                if((this.DockAreas & DockAreas.DockLeft) != 0){
                    return DockState.DockLeft;
                }
                if((this.DockAreas & DockAreas.DockBottom) != 0){
                    return DockState.DockBottom;
                }
                if((this.DockAreas & DockAreas.DockTop) != 0){
                    return DockState.DockTop;
                }
                if((this.DockAreas & DockAreas.Float) != 0){
                    return DockState.Float;
                }
                return DockState.Unknown;
            }
        }
        public DockAreas DockAreas
        {
            get { return this.m_allowedAreas; }
            set
            {
                if(this.m_allowedAreas == value){
                    return;
                }
                if(!DockHelper.IsDockStateValid(this.DockState, value)){
                    throw (new InvalidOperationException(Strings.DockContentHandler_DockAreas_InvalidValue));
                }
                this.m_allowedAreas = value;
                if(!DockHelper.IsDockStateValid(this.ShowHint, this.m_allowedAreas)){
                    this.ShowHint = DockState.Unknown;
                }
            }
        }
        public DockState DockState
        {
            get { return this.m_dockState; }
            set
            {
                if(this.m_dockState == value){
                    return;
                }
                this.DockPanel.SuspendLayout(true);
                if(value == DockState.Hidden){
                    this.IsHidden = true;
                } else{
                    this.SetDockState(false, value, this.Pane);
                }
                this.DockPanel.ResumeLayout(true, true);
            }
        }
        public DockPanel DockPanel
        {
            get { return this.m_dockPanel; }
            set
            {
                if(this.m_dockPanel == value){
                    return;
                }
                this.Pane = null;
                if(this.m_dockPanel != null){
                    this.m_dockPanel.RemoveContent(this.Content);
                }
                if(this.m_tab != null){
                    this.m_tab.Dispose();
                    this.m_tab = null;
                }
                if(this.m_autoHideTab != null){
                    this.m_autoHideTab.Dispose();
                    this.m_autoHideTab = null;
                }
                this.m_dockPanel = value;
                if(this.m_dockPanel != null){
                    this.m_dockPanel.AddContent(this.Content);
                    this.Form.TopLevel = false;
                    this.Form.FormBorderStyle = FormBorderStyle.None;
                    this.Form.ShowInTaskbar = false;
                    this.Form.WindowState = FormWindowState.Normal;
                    NativeMethods.SetWindowPos(this.Form.Handle, IntPtr.Zero, 0, 0, 0, 0,
                                               FlagsSetWindowPos.SWP_NOACTIVATE | FlagsSetWindowPos.SWP_NOMOVE
                                               | FlagsSetWindowPos.SWP_NOSIZE | FlagsSetWindowPos.SWP_NOZORDER
                                               | FlagsSetWindowPos.SWP_NOOWNERZORDER
                                               | FlagsSetWindowPos.SWP_FRAMECHANGED);
                }
            }
        }
        public Icon Icon
        {
            get { return this.Form.Icon; }
        }
        public DockPane Pane
        {
            get { return this.IsFloat ? this.FloatPane : this.PanelPane; }
            set
            {
                if(this.Pane == value){
                    return;
                }
                this.DockPanel.SuspendLayout(true);
                DockPane oldPane = this.Pane;
                this.SuspendSetDockState();
                this.FloatPane = (value == null ? null : (value.IsFloat ? value : this.FloatPane));
                this.PanelPane = (value == null ? null : (value.IsFloat ? this.PanelPane : value));
                this.ResumeSetDockState(this.IsHidden, value != null ? value.DockState : DockState.Unknown, oldPane);
                this.DockPanel.ResumeLayout(true, true);
            }
        }
        public bool IsHidden
        {
            get { return this.m_isHidden; }
            set
            {
                if(this.m_isHidden == value){
                    return;
                }
                this.SetDockState(value, this.VisibleState, this.Pane);
            }
        }
        public string TabText
        {
            get { return this.m_tabText == null ? this.Form.Text : this.m_tabText; }
            set
            {
                if(this.m_tabText == value){
                    return;
                }
                this.m_tabText = value;
                if(this.Pane != null){
                    this.Pane.RefreshChanges();
                }
            }
        }
        public DockState VisibleState
        {
            get { return this.m_visibleState; }
            set
            {
                if(this.m_visibleState == value){
                    return;
                }
                this.SetDockState(this.IsHidden, value, this.Pane);
            }
        }
        public bool IsFloat
        {
            get { return this.m_isFloat; }
            set
            {
                if(this.m_isFloat == value){
                    return;
                }
                DockState visibleState = this.CheckDockState(value);
                if(visibleState == DockState.Unknown){
                    throw new InvalidOperationException(Strings.DockContentHandler_IsFloat_InvalidValue);
                }
                this.SetDockState(this.IsHidden, visibleState, this.Pane);
            }
        }
        public DockPane PanelPane
        {
            get { return this.m_panelPane; }
            set
            {
                if(this.m_panelPane == value){
                    return;
                }
                if(value != null){
                    if(value.IsFloat || value.DockPanel != this.DockPanel){
                        throw new InvalidOperationException(Strings.DockContentHandler_DockPane_InvalidValue);
                    }
                }
                DockPane oldPane = this.Pane;
                if(this.m_panelPane != null){
                    this.RemoveFromPane(this.m_panelPane);
                }
                this.m_panelPane = value;
                if(this.m_panelPane != null){
                    this.m_panelPane.AddContent(this.Content);
                    this.SetDockState(this.IsHidden, this.IsFloat ? DockState.Float : this.m_panelPane.DockState,
                                      oldPane);
                } else{
                    this.SetDockState(this.IsHidden, DockState.Unknown, oldPane);
                }
            }
        }
        public DockPane FloatPane
        {
            get { return this.m_floatPane; }
            set
            {
                if(this.m_floatPane == value){
                    return;
                }
                if(value != null){
                    if(!value.IsFloat || value.DockPanel != this.DockPanel){
                        throw new InvalidOperationException(Strings.DockContentHandler_FloatPane_InvalidValue);
                    }
                }
                DockPane oldPane = this.Pane;
                if(this.m_floatPane != null){
                    this.RemoveFromPane(this.m_floatPane);
                }
                this.m_floatPane = value;
                if(this.m_floatPane != null){
                    this.m_floatPane.AddContent(this.Content);
                    this.SetDockState(this.IsHidden, this.IsFloat ? DockState.Float : this.VisibleState, oldPane);
                } else{
                    this.SetDockState(this.IsHidden, DockState.Unknown, oldPane);
                }
            }
        }
        internal bool IsSuspendSetDockState
        {
            get { return this.m_countSetDockState != 0; }
        }
        internal string PersistString
        {
            get
            {
                return this.GetPersistStringCallback == null
                               ? this.Form.GetType().ToString()
                               : this.GetPersistStringCallback();
            }
        }
        public GetPersistStringCallback GetPersistStringCallback
        {
            get { return this.m_getPersistStringCallback; }
            set { this.m_getPersistStringCallback = value; }
        }
        public bool HideOnClose
        {
            get { return this.m_hideOnClose; }
            set { this.m_hideOnClose = value; }
        }
        public DockState ShowHint
        {
            get { return this.m_showHint; }
            set
            {
                if(!DockHelper.IsDockStateValid(value, this.DockAreas)){
                    throw (new InvalidOperationException(Strings.DockContentHandler_ShowHint_InvalidValue));
                }
                if(this.m_showHint == value){
                    return;
                }
                this.m_showHint = value;
            }
        }
        public bool IsActivated
        {
            get { return this.m_isActivated; }
            internal set
            {
                if(this.m_isActivated == value){
                    return;
                }
                this.m_isActivated = value;
            }
        }
        public ContextMenu TabPageContextMenu
        {
            get { return this.m_tabPageContextMenu; }
            set { this.m_tabPageContextMenu = value; }
        }
        public string ToolTipText
        {
            get { return this.m_toolTipText; }
            set { this.m_toolTipText = value; }
        }
        internal IntPtr ActiveWindowHandle
        {
            get { return this.m_activeWindowHandle; }
            set { this.m_activeWindowHandle = value; }
        }
        internal IDisposable AutoHideTab
        {
            get { return this.m_autoHideTab; }
            set { this.m_autoHideTab = value; }
        }
        internal bool FlagClipWindow
        {
            get { return this.m_flagClipWindow; }
            set
            {
                if(this.m_flagClipWindow == value){
                    return;
                }
                this.m_flagClipWindow = value;
                if(this.m_flagClipWindow){
                    this.Form.Region = new Region(Rectangle.Empty);
                } else{
                    this.Form.Region = null;
                }
            }
        }
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return this.m_tabPageContextMenuStrip; }
            set { this.m_tabPageContextMenuStrip = value; }
        }

        #region Events
        private static readonly object DockStateChangedEvent = new object();
        public event EventHandler DockStateChanged
        {
            add { this.Events.AddHandler(DockStateChangedEvent, value); }
            remove { this.Events.RemoveHandler(DockStateChangedEvent, value); }
        }
        protected virtual void OnDockStateChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[DockStateChangedEvent];
            if(handler != null){
                handler(this, e);
            }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region IDockDragSource Members
        public bool IsDockStateValid(DockState dockState)
        {
            if(this.DockPanel != null && dockState == DockState.Document
               && this.DockPanel.DocumentStyle == DocumentStyle.SystemMdi){
                return false;
            } else{
                return DockHelper.IsDockStateValid(dockState, this.DockAreas);
            }
        }
        Control IDragSource.DragControl
        {
            get { return this.Form; }
        }
        bool IDockDragSource.CanDockTo(DockPane pane)
        {
            if(!this.IsDockStateValid(pane.DockState)){
                return false;
            }
            if(this.Pane == pane && pane.DisplayingContents.Count == 1){
                return false;
            }
            return true;
        }
        Rectangle IDockDragSource.BeginDrag(Point ptMouse)
        {
            Size size;
            DockPane floatPane = this.FloatPane;
            if(this.DockState == DockState.Float || floatPane == null || floatPane.FloatWindow.NestedPanes.Count != 1){
                size = this.DockPanel.DefaultFloatWindowSize;
            } else{
                size = floatPane.FloatWindow.Size;
            }
            Point location;
            Rectangle rectPane = this.Pane.ClientRectangle;
            if(this.DockState == DockState.Document){
                location = new Point(rectPane.Left, rectPane.Top);
            } else{
                location = new Point(rectPane.Left, rectPane.Bottom);
                location.Y -= size.Height;
            }
            location = this.Pane.PointToScreen(location);
            if(ptMouse.X > location.X + size.Width){
                location.X += ptMouse.X - (location.X + size.Width) + Measures.SplitterSize;
            }
            return new Rectangle(location, size);
        }
        public void FloatAt(Rectangle floatWindowBounds)
        {
            DockPane pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, floatWindowBounds, true);
        }
        public void DockTo(DockPane pane, DockStyle dockStyle, int contentIndex)
        {
            if(dockStyle == DockStyle.Fill){
                bool samePane = (this.Pane == pane);
                if(!samePane){
                    this.Pane = pane;
                }
                if(contentIndex == -1 || !samePane){
                    pane.SetContentIndex(this.Content, contentIndex);
                } else{
                    DockContentCollection contents = pane.Contents;
                    int oldIndex = contents.IndexOf(this.Content);
                    int newIndex = contentIndex;
                    if(oldIndex < newIndex){
                        newIndex += 1;
                        if(newIndex > contents.Count - 1){
                            newIndex = -1;
                        }
                    }
                    pane.SetContentIndex(this.Content, newIndex);
                }
            } else{
                DockPane paneFrom = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, pane.DockState, true);
                INestedPanesContainer container = pane.NestedPanesContainer;
                if(dockStyle == DockStyle.Left){
                    paneFrom.DockTo(container, pane, DockAlignment.Left, 0.5);
                } else if(dockStyle == DockStyle.Right){
                    paneFrom.DockTo(container, pane, DockAlignment.Right, 0.5);
                } else if(dockStyle == DockStyle.Top){
                    paneFrom.DockTo(container, pane, DockAlignment.Top, 0.5);
                } else if(dockStyle == DockStyle.Bottom){
                    paneFrom.DockTo(container, pane, DockAlignment.Bottom, 0.5);
                }
                paneFrom.DockState = pane.DockState;
            }
        }
        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            if(panel != this.DockPanel){
                throw new ArgumentException(Strings.IDockDragSource_DockTo_InvalidPanel, "panel");
            }
            DockPane pane;
            if(dockStyle == DockStyle.Top){
                pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.DockTop, true);
            } else if(dockStyle == DockStyle.Bottom){
                pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.DockBottom, true);
            } else if(dockStyle == DockStyle.Left){
                pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.DockLeft, true);
            } else if(dockStyle == DockStyle.Right){
                pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.DockRight, true);
            } else if(dockStyle == DockStyle.Fill){
                pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.Document, true);
            } else{
                return;
            }
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if(disposing){
                lock(this){
                    this.DockPanel = null;
                    if(this.m_autoHideTab != null){
                        this.m_autoHideTab.Dispose();
                    }
                    if(this.m_tab != null){
                        this.m_tab.Dispose();
                    }
                    this.Form.Disposed -= new EventHandler(this.Form_Disposed);
                    this.Form.TextChanged -= new EventHandler(this.Form_TextChanged);
                    this.m_events.Dispose();
                }
            }
        }
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public DockState CheckDockState(bool isFloat)
        {
            DockState dockState;
            if(isFloat){
                if(!this.IsDockStateValid(DockState.Float)){
                    dockState = DockState.Unknown;
                } else{
                    dockState = DockState.Float;
                }
            } else{
                dockState = (this.PanelPane != null) ? this.PanelPane.DockState : this.DefaultDockState;
                if(dockState != DockState.Unknown && !this.IsDockStateValid(dockState)){
                    dockState = DockState.Unknown;
                }
            }
            return dockState;
        }
        private void RemoveFromPane(DockPane pane)
        {
            pane.RemoveContent(this.Content);
            this.SetPane(null);
            if(pane.Contents.Count == 0){
                pane.Dispose();
            }
        }
        private void SuspendSetDockState()
        {
            this.m_countSetDockState ++;
        }
        private void ResumeSetDockState()
        {
            this.m_countSetDockState --;
            if(this.m_countSetDockState < 0){
                this.m_countSetDockState = 0;
            }
        }
        private void ResumeSetDockState(bool isHidden, DockState visibleState, DockPane oldPane)
        {
            this.ResumeSetDockState();
            this.SetDockState(isHidden, visibleState, oldPane);
        }
        internal void SetDockState(bool isHidden, DockState visibleState, DockPane oldPane)
        {
            if(this.IsSuspendSetDockState){
                return;
            }
            if(this.DockPanel == null && visibleState != DockState.Unknown){
                throw new InvalidOperationException(Strings.DockContentHandler_SetDockState_NullPanel);
            }
            if(visibleState == DockState.Hidden
               || (visibleState != DockState.Unknown && !this.IsDockStateValid(visibleState))){
                throw new InvalidOperationException(Strings.DockContentHandler_SetDockState_InvalidState);
            }
            DockPanel dockPanel = this.DockPanel;
            if(dockPanel != null){
                dockPanel.SuspendLayout(true);
            }
            this.SuspendSetDockState();
            DockState oldDockState = this.DockState;
            if(this.m_isHidden != isHidden || oldDockState == DockState.Unknown){
                this.m_isHidden = isHidden;
            }
            this.m_visibleState = visibleState;
            this.m_dockState = isHidden ? DockState.Hidden : visibleState;
            if(visibleState == DockState.Unknown){
                this.Pane = null;
            } else{
                this.m_isFloat = (this.m_visibleState == DockState.Float);
                if(this.Pane == null){
                    this.Pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, visibleState, true);
                } else if(this.Pane.DockState != visibleState){
                    if(this.Pane.Contents.Count == 1){
                        this.Pane.SetDockState(visibleState);
                    } else{
                        this.Pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, visibleState, true);
                    }
                }
            }
            if(this.Form.ContainsFocus){
                if(this.DockState == DockState.Hidden || this.DockState == DockState.Unknown){
                    this.DockPanel.ContentFocusManager.GiveUpFocus(this.Content);
                }
            }
            this.SetPaneAndVisible(this.Pane);
            if(oldPane != null && !oldPane.IsDisposed && oldDockState == oldPane.DockState){
                RefreshDockPane(oldPane);
            }
            if(this.Pane != null && this.DockState == this.Pane.DockState){
                if((this.Pane != oldPane) || (this.Pane == oldPane && oldDockState != oldPane.DockState)){
                    RefreshDockPane(this.Pane);
                }
            }
            if(oldDockState != this.DockState){
                if(this.DockState == DockState.Hidden || this.DockState == DockState.Unknown
                   || DockHelper.IsDockStateAutoHide(this.DockState)){
                    this.DockPanel.ContentFocusManager.RemoveFromList(this.Content);
                } else{
                    this.DockPanel.ContentFocusManager.AddToList(this.Content);
                }
                this.OnDockStateChanged(EventArgs.Empty);
            }
            this.ResumeSetDockState();
            if(dockPanel != null){
                dockPanel.ResumeLayout(true, true);
            }
        }
        private static void RefreshDockPane(DockPane pane)
        {
            pane.RefreshChanges();
            pane.ValidateActiveContent();
        }
        public void Activate()
        {
            if(this.DockPanel == null){
                this.Form.Activate();
            } else if(this.Pane == null){
                this.Show(this.DockPanel);
            } else{
                this.IsHidden = false;
                this.Pane.ActiveContent = this.Content;
                if(this.DockState == DockState.Document && this.DockPanel.DocumentStyle == DocumentStyle.SystemMdi){
                    this.Form.Activate();
                    return;
                } else if(DockHelper.IsDockStateAutoHide(this.DockState)){
                    this.DockPanel.ActiveAutoHideContent = this.Content;
                }
                if(!this.Form.ContainsFocus){
                    this.DockPanel.ContentFocusManager.Activate(this.Content);
                }
            }
        }
        public void GiveUpFocus()
        {
            this.DockPanel.ContentFocusManager.GiveUpFocus(this.Content);
        }
        public void Hide()
        {
            this.IsHidden = true;
        }
        internal void SetPaneAndVisible(DockPane pane)
        {
            this.SetPane(pane);
            this.SetVisible();
        }
        private void SetPane(DockPane pane)
        {
            if(pane != null && pane.DockState == DockState.Document
               && this.DockPanel.DocumentStyle == DocumentStyle.DockingMdi){
                if(this.Form.Parent is DockPane){
                    this.SetParent(null);
                }
                if(this.Form.MdiParent != this.DockPanel.ParentForm){
                    this.FlagClipWindow = true;
                    this.Form.MdiParent = this.DockPanel.ParentForm;
                }
            } else{
                this.FlagClipWindow = true;
                if(this.Form.MdiParent != null){
                    this.Form.MdiParent = null;
                }
                if(this.Form.TopLevel){
                    this.Form.TopLevel = false;
                }
                this.SetParent(pane);
            }
        }
        internal void SetVisible()
        {
            bool visible;
            if(this.IsHidden){
                visible = false;
            } else if(this.Pane != null && this.Pane.DockState == DockState.Document
                      && this.DockPanel.DocumentStyle == DocumentStyle.DockingMdi){
                visible = true;
            } else if(this.Pane != null && this.Pane.ActiveContent == this.Content){
                visible = true;
            } else if(this.Pane != null && this.Pane.ActiveContent != this.Content){
                visible = false;
            } else{
                visible = this.Form.Visible;
            }
            if(this.Form.Visible != visible){
                this.Form.Visible = visible;
            }
        }
        private void SetParent(Control value)
        {
            if(this.Form.Parent == value){
                return;
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            bool bRestoreFocus = false;
            if(this.Form.ContainsFocus){
                if(value == null){
                    this.DockPanel.ContentFocusManager.GiveUpFocus(this.Content);
                } else{
                    this.DockPanel.SaveFocus();
                    bRestoreFocus = true;
                }
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            this.Form.Parent = value;
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if(bRestoreFocus){
                this.Activate();
            }
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }
        public void Show()
        {
            if(this.DockPanel == null){
                this.Form.Show();
            } else{
                this.Show(this.DockPanel);
            }
        }
        public void Show(DockPanel dockPanel)
        {
            if(dockPanel == null){
                throw (new ArgumentNullException(Strings.DockContentHandler_Show_NullDockPanel));
            }
            if(this.DockState == DockState.Unknown){
                this.Show(dockPanel, this.DefaultShowState);
            } else{
                this.Activate();
            }
        }
        public void Show(DockPanel dockPanel, DockState dockState)
        {
            if(dockPanel == null){
                throw (new ArgumentNullException(Strings.DockContentHandler_Show_NullDockPanel));
            }
            if(dockState == DockState.Unknown || dockState == DockState.Hidden){
                throw (new ArgumentException(Strings.DockContentHandler_Show_InvalidDockState));
            }
            dockPanel.SuspendLayout(true);
            this.DockPanel = dockPanel;
            if(dockState == DockState.Float && this.FloatPane == null){
                this.Pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.Float, true);
            } else if(this.PanelPane == null){
                DockPane paneExisting = null;
                foreach(DockPane pane in this.DockPanel.Panes){
                    if(pane.DockState == dockState){
                        paneExisting = pane;
                        break;
                    }
                }
                if(paneExisting == null){
                    this.Pane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, dockState, true);
                } else{
                    this.Pane = paneExisting;
                }
            }
            this.DockState = dockState;
            this.Activate();
            dockPanel.ResumeLayout(true, true);
        }
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void Show(DockPanel dockPanel, Rectangle floatWindowBounds)
        {
            if(dockPanel == null){
                throw (new ArgumentNullException(Strings.DockContentHandler_Show_NullDockPanel));
            }
            dockPanel.SuspendLayout(true);
            this.DockPanel = dockPanel;
            if(this.FloatPane == null){
                this.IsHidden = true; // to reduce the screen flicker
                this.FloatPane = this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, DockState.Float, false);
                this.FloatPane.FloatWindow.StartPosition = FormStartPosition.Manual;
            }
            this.FloatPane.FloatWindow.Bounds = floatWindowBounds;
            this.Show(dockPanel, DockState.Float);
            this.Activate();
            dockPanel.ResumeLayout(true, true);
        }
        public void Show(DockPane pane, IDockContent beforeContent)
        {
            if(pane == null){
                throw (new ArgumentNullException(Strings.DockContentHandler_Show_NullPane));
            }
            if(beforeContent != null && pane.Contents.IndexOf(beforeContent) == -1){
                throw (new ArgumentException(Strings.DockContentHandler_Show_InvalidBeforeContent));
            }
            pane.DockPanel.SuspendLayout(true);
            this.DockPanel = pane.DockPanel;
            this.Pane = pane;
            pane.SetContentIndex(this.Content, pane.Contents.IndexOf(beforeContent));
            this.Show();
            pane.DockPanel.ResumeLayout(true, true);
        }
        public void Show(DockPane previousPane, DockAlignment alignment, double proportion)
        {
            if(previousPane == null){
                throw (new ArgumentException(Strings.DockContentHandler_Show_InvalidPrevPane));
            }
            if(DockHelper.IsDockStateAutoHide(previousPane.DockState)){
                throw (new ArgumentException(Strings.DockContentHandler_Show_InvalidPrevPane));
            }
            previousPane.DockPanel.SuspendLayout(true);
            this.DockPanel = previousPane.DockPanel;
            this.DockPanel.DockPaneFactory.CreateDockPane(this.Content, previousPane, alignment, proportion, true);
            this.Show();
            previousPane.DockPanel.ResumeLayout(true, true);
        }
        public void Close()
        {
            DockPanel dockPanel = this.DockPanel;
            if(dockPanel != null){
                dockPanel.SuspendLayout(true);
            }
            this.Form.Close();
            if(dockPanel != null){
                dockPanel.ResumeLayout(true, true);
            }
        }
        internal DockPaneStripBase.Tab GetTab(DockPaneStripBase dockPaneStrip)
        {
            if(this.m_tab == null){
                this.m_tab = dockPaneStrip.CreateTab(this.Content);
            }
            return this.m_tab;
        }
        private void Form_Disposed(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void Form_TextChanged(object sender, EventArgs e)
        {
            if(DockHelper.IsDockStateAutoHide(this.DockState)){
                this.DockPanel.RefreshAutoHideStrip();
            } else if(this.Pane != null){
                if(this.Pane.FloatWindow != null){
                    this.Pane.FloatWindow.SetText();
                }
                this.Pane.RefreshChanges();
            }
        }
    }
}