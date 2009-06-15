using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking.Win32;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal interface IContentFocusManager
    {
        void Activate(IDockContent content);
        void GiveUpFocus(IDockContent content);
        void AddToList(IDockContent content);
        void RemoveFromList(IDockContent content);
    }

    partial class DockPanel
    {
        private static readonly object ActiveContentChangedEvent = new object();
        private static readonly object ActiveDocumentChangedEvent = new object();
        private static readonly object ActivePaneChangedEvent = new object();
        private IFocusManager FocusManager
        {
            get { return this.m_focusManager; }
        }
        internal IContentFocusManager ContentFocusManager
        {
            get { return this.m_focusManager; }
        }
        [Browsable(false)]
        public IDockContent ActiveContent
        {
            get { return this.FocusManager.ActiveContent; }
        }
        [Browsable(false)]
        public DockPane ActivePane
        {
            get { return this.FocusManager.ActivePane; }
        }
        [Browsable(false)]
        public IDockContent ActiveDocument
        {
            get { return this.FocusManager.ActiveDocument; }
        }
        [Browsable(false)]
        public DockPane ActiveDocumentPane
        {
            get { return this.FocusManager.ActiveDocumentPane; }
        }
        internal void SaveFocus()
        {
            this.DummyControl.Focus();
        }
        [LocalizedCategory("Category_PropertyChanged"),
         LocalizedDescription("DockPanel_ActiveDocumentChanged_Description")]
        public event EventHandler ActiveDocumentChanged
        {
            add { this.Events.AddHandler(ActiveDocumentChangedEvent, value); }
            remove { this.Events.RemoveHandler(ActiveDocumentChangedEvent, value); }
        }
        protected virtual void OnActiveDocumentChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[ActiveDocumentChangedEvent];
            if(handler != null){
                handler(this, e);
            }
        }
        [LocalizedCategory("Category_PropertyChanged"),
         LocalizedDescription("DockPanel_ActiveContentChanged_Description")]
        public event EventHandler ActiveContentChanged
        {
            add { this.Events.AddHandler(ActiveContentChangedEvent, value); }
            remove { this.Events.RemoveHandler(ActiveContentChangedEvent, value); }
        }
        protected void OnActiveContentChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[ActiveContentChangedEvent];
            if(handler != null){
                handler(this, e);
            }
        }
        [LocalizedCategory("Category_PropertyChanged"), LocalizedDescription("DockPanel_ActivePaneChanged_Description")]
        public event EventHandler ActivePaneChanged
        {
            add { this.Events.AddHandler(ActivePaneChangedEvent, value); }
            remove { this.Events.RemoveHandler(ActivePaneChangedEvent, value); }
        }
        protected virtual void OnActivePaneChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)this.Events[ActivePaneChangedEvent];
            if(handler != null){
                handler(this, e);
            }
        }

        #region Nested type: FocusManagerImpl
        private class FocusManagerImpl : Component, IContentFocusManager, IFocusManager
        {
            private IDockContent m_activeContent = null;
            private IDockContent m_activeDocument = null;
            private DockPane m_activeDocumentPane = null;
            private DockPane m_activePane = null;
            private IDockContent m_contentActivating = null;
            private int m_countSuspendFocusTracking = 0;
            private bool m_disposed = false;
            private DockPanel m_dockPanel;
            private LocalWindowsHook.HookEventHandler m_hookEventHandler;
            private bool m_inRefreshActiveWindow = false;
            private IDockContent m_lastActiveContent = null;
            private List<IDockContent> m_listContent = new List<IDockContent>();
            private LocalWindowsHook m_localWindowsHook;
            public FocusManagerImpl(DockPanel dockPanel)
            {
                this.m_dockPanel = dockPanel;
                this.m_localWindowsHook = new LocalWindowsHook(HookType.WH_CALLWNDPROCRET);
                this.m_hookEventHandler = new LocalWindowsHook.HookEventHandler(this.HookEventHandler);
                this.m_localWindowsHook.HookInvoked += this.m_hookEventHandler;
                this.m_localWindowsHook.Install();
            }
            public DockPanel DockPanel
            {
                get { return this.m_dockPanel; }
            }
            private IDockContent ContentActivating
            {
                get { return this.m_contentActivating; }
                set { this.m_contentActivating = value; }
            }
            private List<IDockContent> ListContent
            {
                get { return this.m_listContent; }
            }
            private IDockContent LastActiveContent
            {
                get { return this.m_lastActiveContent; }
                set { this.m_lastActiveContent = value; }
            }
            private bool InRefreshActiveWindow
            {
                get { return this.m_inRefreshActiveWindow; }
            }

            #region IContentFocusManager Members
            public void Activate(IDockContent content)
            {
                if(this.IsFocusTrackingSuspended){
                    this.ContentActivating = content;
                    return;
                }
                if(content == null){
                    return;
                }
                DockContentHandler handler = content.DockHandler;
                if(handler.Form.IsDisposed){
                    return; // Should not reach here, but better than throwing an exception
                }
                if(ContentContains(content, handler.ActiveWindowHandle)){
                    NativeMethods.SetFocus(handler.ActiveWindowHandle);
                }
                if(!handler.Form.ContainsFocus){
                    if(!handler.Form.SelectNextControl(handler.Form.ActiveControl, true, true, true, true)){
                        // Since DockContent Form is not selectalbe, use Win32 SetFocus instead
                        NativeMethods.SetFocus(handler.Form.Handle);
                    }
                }
            }
            public void AddToList(IDockContent content)
            {
                if(this.ListContent.Contains(content) || this.IsInActiveList(content)){
                    return;
                }
                this.ListContent.Add(content);
            }
            public void RemoveFromList(IDockContent content)
            {
                if(this.IsInActiveList(content)){
                    this.RemoveFromActiveList(content);
                }
                if(this.ListContent.Contains(content)){
                    this.ListContent.Remove(content);
                }
            }
            public void GiveUpFocus(IDockContent content)
            {
                DockContentHandler handler = content.DockHandler;
                if(!handler.Form.ContainsFocus){
                    return;
                }
                if(this.IsFocusTrackingSuspended){
                    this.DockPanel.DummyControl.Focus();
                }
                if(this.LastActiveContent == content){
                    IDockContent prev = handler.PreviousActive;
                    if(prev != null){
                        this.Activate(prev);
                    } else if(this.ListContent.Count > 0){
                        this.Activate(this.ListContent[this.ListContent.Count - 1]);
                    }
                } else if(this.LastActiveContent != null){
                    this.Activate(this.LastActiveContent);
                } else if(this.ListContent.Count > 0){
                    this.Activate(this.ListContent[this.ListContent.Count - 1]);
                }
            }
            #endregion

            #region IFocusManager Members
            public void SuspendFocusTracking()
            {
                this.m_countSuspendFocusTracking++;
                this.m_localWindowsHook.HookInvoked -= this.m_hookEventHandler;
            }
            public void ResumeFocusTracking()
            {
                if(this.m_countSuspendFocusTracking > 0){
                    this.m_countSuspendFocusTracking--;
                }
                if(this.m_countSuspendFocusTracking == 0){
                    if(this.ContentActivating != null){
                        this.Activate(this.ContentActivating);
                        this.ContentActivating = null;
                    }
                    this.m_localWindowsHook.HookInvoked += this.m_hookEventHandler;
                    if(!this.InRefreshActiveWindow){
                        this.RefreshActiveWindow();
                    }
                }
            }
            public bool IsFocusTrackingSuspended
            {
                get { return this.m_countSuspendFocusTracking != 0; }
            }
            public DockPane ActivePane
            {
                get { return this.m_activePane; }
            }
            public IDockContent ActiveContent
            {
                get { return this.m_activeContent; }
            }
            public DockPane ActiveDocumentPane
            {
                get { return this.m_activeDocumentPane; }
            }
            public IDockContent ActiveDocument
            {
                get { return this.m_activeDocument; }
            }
            #endregion

            protected override void Dispose(bool disposing)
            {
                lock(this){
                    if(!this.m_disposed && disposing){
                        this.m_localWindowsHook.Dispose();
                        this.m_disposed = true;
                    }
                    base.Dispose(disposing);
                }
            }
            private bool IsInActiveList(IDockContent content)
            {
                return !(content.DockHandler.NextActive == null && this.LastActiveContent != content);
            }
            private void AddLastToActiveList(IDockContent content)
            {
                IDockContent last = this.LastActiveContent;
                if(last == content){
                    return;
                }
                DockContentHandler handler = content.DockHandler;
                if(this.IsInActiveList(content)){
                    this.RemoveFromActiveList(content);
                }
                handler.PreviousActive = last;
                handler.NextActive = null;
                this.LastActiveContent = content;
                if(last != null){
                    last.DockHandler.NextActive = this.LastActiveContent;
                }
            }
            private void RemoveFromActiveList(IDockContent content)
            {
                if(this.LastActiveContent == content){
                    this.LastActiveContent = content.DockHandler.PreviousActive;
                }
                IDockContent prev = content.DockHandler.PreviousActive;
                IDockContent next = content.DockHandler.NextActive;
                if(prev != null){
                    prev.DockHandler.NextActive = next;
                }
                if(next != null){
                    next.DockHandler.PreviousActive = prev;
                }
                content.DockHandler.PreviousActive = null;
                content.DockHandler.NextActive = null;
            }
            private static bool ContentContains(IDockContent content, IntPtr hWnd)
            {
                Control control = FromChildHandle(hWnd);
                for(Control parent = control; parent != null; parent = parent.Parent){
                    if(parent == content.DockHandler.Form){
                        return true;
                    }
                }
                return false;
            }
            // Windows hook event handler
            private void HookEventHandler(object sender, HookEventArgs e)
            {
                Msgs msg = (Msgs)Marshal.ReadInt32(e.lParam, IntPtr.Size * 3);
                if(msg == Msgs.WM_KILLFOCUS){
                    IntPtr wParam = Marshal.ReadIntPtr(e.lParam, IntPtr.Size * 2);
                    DockPane pane = this.GetPaneFromHandle(wParam);
                    if(pane == null){
                        this.RefreshActiveWindow();
                    }
                } else if(msg == Msgs.WM_SETFOCUS){
                    this.RefreshActiveWindow();
                }
            }
            private DockPane GetPaneFromHandle(IntPtr hWnd)
            {
                Control control = FromChildHandle(hWnd);
                IDockContent content = null;
                DockPane pane = null;
                for(; control != null; control = control.Parent){
                    content = control as IDockContent;
                    if(content != null){
                        content.DockHandler.ActiveWindowHandle = hWnd;
                    }
                    if(content != null && content.DockHandler.DockPanel == this.DockPanel){
                        return content.DockHandler.Pane;
                    }
                    pane = control as DockPane;
                    if(pane != null && pane.DockPanel == this.DockPanel){
                        break;
                    }
                }
                return pane;
            }
            private void RefreshActiveWindow()
            {
                this.SuspendFocusTracking();
                this.m_inRefreshActiveWindow = true;
                DockPane oldActivePane = this.ActivePane;
                IDockContent oldActiveContent = this.ActiveContent;
                IDockContent oldActiveDocument = this.ActiveDocument;
                this.SetActivePane();
                this.SetActiveContent();
                this.SetActiveDocumentPane();
                this.SetActiveDocument();
                this.DockPanel.AutoHideWindow.RefreshActivePane();
                this.ResumeFocusTracking();
                this.m_inRefreshActiveWindow = false;
                if(oldActiveContent != this.ActiveContent){
                    this.DockPanel.OnActiveContentChanged(EventArgs.Empty);
                }
                if(oldActiveDocument != this.ActiveDocument){
                    this.DockPanel.OnActiveDocumentChanged(EventArgs.Empty);
                }
                if(oldActivePane != this.ActivePane){
                    this.DockPanel.OnActivePaneChanged(EventArgs.Empty);
                }
            }
            private void SetActivePane()
            {
                DockPane value = this.GetPaneFromHandle(NativeMethods.GetFocus());
                if(this.m_activePane == value){
                    return;
                }
                if(this.m_activePane != null){
                    this.m_activePane.SetIsActivated(false);
                }
                this.m_activePane = value;
                if(this.m_activePane != null){
                    this.m_activePane.SetIsActivated(true);
                }
            }
            internal void SetActiveContent()
            {
                IDockContent value = this.ActivePane == null ? null : this.ActivePane.ActiveContent;
                if(this.m_activeContent == value){
                    return;
                }
                if(this.m_activeContent != null){
                    this.m_activeContent.DockHandler.IsActivated = false;
                }
                this.m_activeContent = value;
                if(this.m_activeContent != null){
                    this.m_activeContent.DockHandler.IsActivated = true;
                    if(!DockHelper.IsDockStateAutoHide((this.m_activeContent.DockHandler.DockState))){
                        this.AddLastToActiveList(this.m_activeContent);
                    }
                }
            }
            private void SetActiveDocumentPane()
            {
                DockPane value = null;
                if(this.ActivePane != null && this.ActivePane.DockState == DockState.Document){
                    value = this.ActivePane;
                }
                if(value == null && this.DockPanel.DockWindows != null){
                    if(this.ActiveDocumentPane == null){
                        value = this.DockPanel.DockWindows[DockState.Document].DefaultPane;
                    } else if(this.ActiveDocumentPane.DockPanel != this.DockPanel
                              || this.ActiveDocumentPane.DockState != DockState.Document){
                        value = this.DockPanel.DockWindows[DockState.Document].DefaultPane;
                    } else{
                        value = this.ActiveDocumentPane;
                    }
                }
                if(this.m_activeDocumentPane == value){
                    return;
                }
                if(this.m_activeDocumentPane != null){
                    this.m_activeDocumentPane.SetIsActiveDocumentPane(false);
                }
                this.m_activeDocumentPane = value;
                if(this.m_activeDocumentPane != null){
                    this.m_activeDocumentPane.SetIsActiveDocumentPane(true);
                }
            }
            private void SetActiveDocument()
            {
                IDockContent value = this.ActiveDocumentPane == null ? null : this.ActiveDocumentPane.ActiveContent;
                if(this.m_activeDocument == value){
                    return;
                }
                this.m_activeDocument = value;
            }

            #region Nested type: HookEventArgs
            private class HookEventArgs : EventArgs
            {
                [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
                public int HookCode;
                public IntPtr lParam;
                [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
                public IntPtr wParam;
            }
            #endregion

            #region Nested type: LocalWindowsHook
            private class LocalWindowsHook : IDisposable
            {
                // Internal properties

                #region Delegates
                public delegate void HookEventHandler(object sender, HookEventArgs e);
                #endregion

                private NativeMethods.HookProc m_filterFunc = null;
                private IntPtr m_hHook = IntPtr.Zero;
                private HookType m_hookType;
                public LocalWindowsHook(HookType hook)
                {
                    this.m_hookType = hook;
                    this.m_filterFunc = new NativeMethods.HookProc(this.CoreHookProc);
                }

                #region IDisposable Members
                public void Dispose()
                {
                    this.Dispose(true);
                    GC.SuppressFinalize(this);
                }
                #endregion

                // Event delegate
                // Event: HookInvoked 
                public event HookEventHandler HookInvoked;
                protected void OnHookInvoked(HookEventArgs e)
                {
                    if(this.HookInvoked != null){
                        this.HookInvoked(this, e);
                    }
                }
                // Default filter function
                public IntPtr CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
                {
                    if(code < 0){
                        return NativeMethods.CallNextHookEx(this.m_hHook, code, wParam, lParam);
                    }
                    // Let clients determine what to do
                    HookEventArgs e = new HookEventArgs();
                    e.HookCode = code;
                    e.wParam = wParam;
                    e.lParam = lParam;
                    this.OnHookInvoked(e);
                    // Yield to the next hook in the chain
                    return NativeMethods.CallNextHookEx(this.m_hHook, code, wParam, lParam);
                }
                // Install the hook
                public void Install()
                {
                    if(this.m_hHook != IntPtr.Zero){
                        this.Uninstall();
                    }
                    int threadId = NativeMethods.GetCurrentThreadId();
                    this.m_hHook = NativeMethods.SetWindowsHookEx(this.m_hookType, this.m_filterFunc, IntPtr.Zero,
                                                                  threadId);
                }
                // Uninstall the hook
                public void Uninstall()
                {
                    if(this.m_hHook != IntPtr.Zero){
                        NativeMethods.UnhookWindowsHookEx(this.m_hHook);
                        this.m_hHook = IntPtr.Zero;
                    }
                }
                ~LocalWindowsHook()
                {
                    this.Dispose(false);
                }
                protected virtual void Dispose(bool disposing)
                {
                    this.Uninstall();
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: IFocusManager
        private interface IFocusManager
        {
            bool IsFocusTrackingSuspended { get; }
            IDockContent ActiveContent { get; }
            DockPane ActivePane { get; }
            IDockContent ActiveDocument { get; }
            DockPane ActiveDocumentPane { get; }
            void SuspendFocusTracking();
            void ResumeFocusTracking();
        }
        #endregion
    }
}