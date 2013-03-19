using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockContent : Form, IDockContent
    {
        private DockContentHandler m_dockHandler = null;
        public DockContent()
        {
            this.m_dockHandler = new DockContentHandler(this, new GetPersistStringCallback(this.GetPersistString));
            this.m_dockHandler.DockStateChanged += new EventHandler(this.DockHandler_DockStateChanged);
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_AllowEndUserDocking_Description"),
         DefaultValue(true)]
        public bool AllowEndUserDocking
        {
            get { return this.DockHandler.AllowEndUserDocking; }
            set { this.DockHandler.AllowEndUserDocking = value; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_DockAreas_Description"),
         DefaultValue(
                 DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom
                 | DockAreas.Document | DockAreas.Float)]
        public DockAreas DockAreas
        {
            get { return this.DockHandler.DockAreas; }
            set { this.DockHandler.DockAreas = value; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_AutoHidePortion_Description"),
         DefaultValue(0.25)]
        public double AutoHidePortion
        {
            get { return this.DockHandler.AutoHidePortion; }
            set { this.DockHandler.AutoHidePortion = value; }
        }
        [Localizable(true), LocalizedCategory("Category_Docking"),
         LocalizedDescription("DockContent_TabText_Description"), DefaultValue(null)]
        public string TabText
        {
            get { return this.DockHandler.TabText; }
            set { this.DockHandler.TabText = value; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_CloseButton_Description"),
         DefaultValue(true)]
        public bool CloseButton
        {
            get { return this.DockHandler.CloseButton; }
            set { this.DockHandler.CloseButton = value; }
        }
        [Browsable(false)]
        public DockPanel DockPanel
        {
            get { return this.DockHandler.DockPanel; }
            set { this.DockHandler.DockPanel = value; }
        }
        [Browsable(false)]
        public DockState DockState
        {
            get { return this.DockHandler.DockState; }
            set { this.DockHandler.DockState = value; }
        }
        [Browsable(false)]
        public DockPane Pane
        {
            get { return this.DockHandler.Pane; }
            set { this.DockHandler.Pane = value; }
        }
        [Browsable(false)]
        public bool IsHidden
        {
            get { return this.DockHandler.IsHidden; }
            set { this.DockHandler.IsHidden = value; }
        }
        [Browsable(false)]
        public DockState VisibleState
        {
            get { return this.DockHandler.VisibleState; }
            set { this.DockHandler.VisibleState = value; }
        }
        [Browsable(false)]
        public bool IsFloat
        {
            get { return this.DockHandler.IsFloat; }
            set { this.DockHandler.IsFloat = value; }
        }
        [Browsable(false)]
        public DockPane PanelPane
        {
            get { return this.DockHandler.PanelPane; }
            set { this.DockHandler.PanelPane = value; }
        }
        [Browsable(false)]
        public DockPane FloatPane
        {
            get { return this.DockHandler.FloatPane; }
            set { this.DockHandler.FloatPane = value; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_HideOnClose_Description"),
         DefaultValue(false)]
        public bool HideOnClose
        {
            get { return this.DockHandler.HideOnClose; }
            set { this.DockHandler.HideOnClose = value; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_ShowHint_Description"),
         DefaultValue(DockState.Unknown)]
        public DockState ShowHint
        {
            get { return this.DockHandler.ShowHint; }
            set { this.DockHandler.ShowHint = value; }
        }
        [Browsable(false)]
        public bool IsActivated
        {
            get { return this.DockHandler.IsActivated; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_TabPageContextMenu_Description"),
         DefaultValue(null)]
        public ContextMenu TabPageContextMenu
        {
            get { return this.DockHandler.TabPageContextMenu; }
            set { this.DockHandler.TabPageContextMenu = value; }
        }
        [LocalizedCategory("Category_Docking"), LocalizedDescription("DockContent_TabPageContextMenuStrip_Description"),
         DefaultValue(null)]
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return this.DockHandler.TabPageContextMenuStrip; }
            set { this.DockHandler.TabPageContextMenuStrip = value; }
        }
        [Localizable(true), Category("Appearance"), LocalizedDescription("DockContent_ToolTipText_Description"),
         DefaultValue(null)]
        public string ToolTipText
        {
            get { return this.DockHandler.ToolTipText; }
            set { this.DockHandler.ToolTipText = value; }
        }

        #region IDockContent Members
        [Browsable(false)]
        public DockContentHandler DockHandler
        {
            get { return this.m_dockHandler; }
        }
        #endregion

        private bool ShouldSerializeTabText()
        {
            return (this.DockHandler.TabText != null);
        }
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected virtual string GetPersistString()
        {
            return this.GetType().ToString();
        }
        public bool IsDockStateValid(DockState dockState)
        {
            return this.DockHandler.IsDockStateValid(dockState);
        }
        public new void Activate()
        {
            this.DockHandler.Activate();
        }
        public new void Hide()
        {
            this.DockHandler.Hide();
        }
        public new void Show()
        {
            this.DockHandler.Show();
        }
        public void Show(DockPanel dockPanel)
        {
            this.DockHandler.Show(dockPanel);
        }
        public void Show(DockPanel dockPanel, DockState dockState)
        {
            this.DockHandler.Show(dockPanel, dockState);
        }
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void Show(DockPanel dockPanel, Rectangle floatWindowBounds)
        {
            this.DockHandler.Show(dockPanel, floatWindowBounds);
        }
        public void Show(DockPane pane, IDockContent beforeContent)
        {
            this.DockHandler.Show(pane, beforeContent);
        }
        public void Show(DockPane previousPane, DockAlignment alignment, double proportion)
        {
            this.DockHandler.Show(previousPane, alignment, proportion);
        }
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")]
        public void FloatAt(Rectangle floatWindowBounds)
        {
            this.DockHandler.FloatAt(floatWindowBounds);
        }
        public void DockTo(DockPane paneTo, DockStyle dockStyle, int contentIndex)
        {
            this.DockHandler.DockTo(paneTo, dockStyle, contentIndex);
        }
        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            this.DockHandler.DockTo(panel, dockStyle);
        }

        #region Events
        private static readonly object DockStateChangedEvent = new object();
        private void DockHandler_DockStateChanged(object sender, EventArgs e)
        {
            this.OnDockStateChanged(e);
        }
        [LocalizedCategory("Category_PropertyChanged"), LocalizedDescription("Pane_DockStateChanged_Description")]
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
    }
}