using System.ComponentModel;
using System.Windows.Forms;

namespace DrawEngine.SharpTracingUI {
    partial class DocumentWindow
    {
        private ToolStripMenuItem menuItem1;
        private ToolStripMenuItem menuItem2;
        private ContextMenuStrip contextMenuTabPage;
        private ToolStripMenuItem menuItemRender;
        private ToolStripMenuItem menuItemCloseButThis;
        private ToolStripMenuItem menuItemCloseAll;
        private ToolStripMenuItem menuItemCheckTest;
        private ToolTip toolTip;
        private IContainer components;

           /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentWindow));
            this.menuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCheckTest = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuTabPage = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemRender = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripAddFrame = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemCloseButThis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCloseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblRenderStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBarRendering = new System.Windows.Forms.ToolStripProgressBar();
            this.lblPercentRend = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblTimeElapsed = new System.Windows.Forms.ToolStripStatusLabel();
            this.OuterPanel = new System.Windows.Forms.Panel();
            this.pictureView = new System.Windows.Forms.PictureBox();
            this.contextMenuTabPage.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.OuterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureView)).BeginInit();
            this.SuspendLayout();
            // 
            // menuItem1
            // 
            this.menuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem2,
            this.menuItemCheckTest});
            this.menuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.menuItem1.MergeIndex = 1;
            this.menuItem1.Name = "menuItem1";
            this.menuItem1.Size = new System.Drawing.Size(89, 20);
            this.menuItem1.Text = "&MDI Document";
            // 
            // menuItem2
            // 
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new System.Drawing.Size(127, 22);
            this.menuItem2.Text = "Test";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItemCheckTest
            // 
            this.menuItemCheckTest.Name = "menuItemCheckTest";
            this.menuItemCheckTest.Size = new System.Drawing.Size(127, 22);
            this.menuItemCheckTest.Text = "Check Test";
            this.menuItemCheckTest.Click += new System.EventHandler(this.menuItemCheckTest_Click);
            // 
            // contextMenuTabPage
            // 
            this.contextMenuTabPage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemRender,
            this.toolStripMenuItemStop,
            this.toolStripMenuItem4,
            this.toolStripAddFrame,
            this.toolStripMenuItem1,
            this.menuItemCloseButThis,
            this.menuItemCloseAll,
            this.menuItemClose,
            this.toolStripMenuItem2,
            this.fullScreenToolStripMenuItem});
            this.contextMenuTabPage.Name = "contextMenuTabPage";
            this.contextMenuTabPage.Size = new System.Drawing.Size(217, 198);
            // 
            // menuItemRender
            // 
            this.menuItemRender.Name = "menuItemRender";
            this.menuItemRender.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.menuItemRender.Size = new System.Drawing.Size(216, 22);
            this.menuItemRender.Text = "&Render";
            this.menuItemRender.Click += new System.EventHandler(this.menuItemRender_Click);
            // 
            // toolStripMenuItemStop
            // 
            this.toolStripMenuItemStop.Name = "toolStripMenuItemStop";
            this.toolStripMenuItemStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Pause)));
            this.toolStripMenuItemStop.Size = new System.Drawing.Size(216, 22);
            this.toolStripMenuItemStop.Text = "Sto&p";
            this.toolStripMenuItemStop.Click += new System.EventHandler(this.toolStripMenuItemStop_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(213, 6);
            // 
            // toolStripAddFrame
            // 
            this.toolStripAddFrame.Name = "toolStripAddFrame";
            this.toolStripAddFrame.Size = new System.Drawing.Size(216, 22);
            this.toolStripAddFrame.Text = "Add &Frame";
            this.toolStripAddFrame.Click += new System.EventHandler(this.toolStripAddFrame_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(213, 6);
            // 
            // menuItemCloseButThis
            // 
            this.menuItemCloseButThis.Name = "menuItemCloseButThis";
            this.menuItemCloseButThis.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
                        | System.Windows.Forms.Keys.W)));
            this.menuItemCloseButThis.Size = new System.Drawing.Size(216, 22);
            this.menuItemCloseButThis.Text = "Close All &But this";
            this.menuItemCloseButThis.Click += new System.EventHandler(this.menuItemCloseButThis_Click);
            // 
            // menuItemCloseAll
            // 
            this.menuItemCloseAll.Name = "menuItemCloseAll";
            this.menuItemCloseAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.W)));
            this.menuItemCloseAll.Size = new System.Drawing.Size(216, 22);
            this.menuItemCloseAll.Text = "Close &All";
            this.menuItemCloseAll.Click += new System.EventHandler(this.menuItemCloseAll_Click);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.menuItemClose.Size = new System.Drawing.Size(216, 22);
            this.menuItemClose.Text = "&Close";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(213, 6);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F11)));
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(216, 22);
            this.fullScreenToolStripMenuItem.Text = "&Full Screen";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.GripMargin = new System.Windows.Forms.Padding(0);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblRenderStatus,
            this.progressBarRendering,
            this.lblPercentRend,
            this.lblTimeElapsed});
            this.statusBar.Location = new System.Drawing.Point(0, 450);
            this.statusBar.Name = "statusBar";
            this.statusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusBar.Size = new System.Drawing.Size(528, 22);
            this.statusBar.TabIndex = 2;
            // 
            // lblRenderStatus
            // 
            this.lblRenderStatus.Name = "lblRenderStatus";
            this.lblRenderStatus.Size = new System.Drawing.Size(90, 17);
            this.lblRenderStatus.Text = "Rendering Status";
            // 
            // progressBarRendering
            // 
            this.progressBarRendering.Name = "progressBarRendering";
            this.progressBarRendering.Size = new System.Drawing.Size(100, 16);
            // 
            // lblPercentRend
            // 
            this.lblPercentRend.Name = "lblPercentRend";
            this.lblPercentRend.Size = new System.Drawing.Size(24, 17);
            this.lblPercentRend.Text = "0%";
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(51, 17);
            this.lblTimeElapsed.Text = "00:00:00";
            // 
            // OuterPanel
            // 
            this.OuterPanel.AutoScroll = true;
            this.OuterPanel.Controls.Add(this.pictureView);
            this.OuterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OuterPanel.Location = new System.Drawing.Point(0, 4);
            this.OuterPanel.Name = "OuterPanel";
            this.OuterPanel.Size = new System.Drawing.Size(528, 446);
            this.OuterPanel.TabIndex = 5;
            this.OuterPanel.Click += new System.EventHandler(this.DocumentWindow_Click);
            // 
            // pictureView
            // 
            this.pictureView.BackColor = System.Drawing.Color.Black;
            this.pictureView.Location = new System.Drawing.Point(0, 0);
            this.pictureView.Name = "pictureView";
            this.pictureView.Size = new System.Drawing.Size(528, 446);
            this.pictureView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureView.TabIndex = 3;
            this.pictureView.TabStop = false;
            this.pictureView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseMove);
            this.pictureView.Click += new System.EventHandler(this.DocumentWindow_Click);
            this.pictureView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseDoubleClick);
            this.pictureView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseDown);
            this.pictureView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseUp);
            this.pictureView.SizeChanged += new System.EventHandler(this.panelRender_SizeChanged);
            // 
            // DocumentWindow
            // 
            this.AllowDrop = true;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(528, 472);
            this.ContextMenuStrip = this.contextMenuTabPage;
            this.Controls.Add(this.OuterPanel);
            this.Controls.Add(this.statusBar);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DocumentWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.TabPageContextMenuStrip = this.contextMenuTabPage;
            this.SizeChanged += new System.EventHandler(this.panelRender_SizeChanged);
            this.Activated += new System.EventHandler(this.DocumentWindow_Click);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DocumentWindow_DragDrop);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DocumentWindow_FormClosed);
            this.Click += new System.EventHandler(this.DocumentWindow_Click);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DocumentWindow_DragEnter);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentWindow_FormClosing);
            this.contextMenuTabPage.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.OuterPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private StatusStrip statusBar;
        private ToolStripStatusLabel lblRenderStatus;
        private ToolStripProgressBar progressBarRendering;
        private ToolStripStatusLabel lblPercentRend;
        private ToolStripStatusLabel lblTimeElapsed;
        private ToolStripMenuItem menuItemClose;
        private Panel OuterPanel;
        private PictureBox pictureView;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem fullScreenToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem toolStripAddFrame;
        private ToolStripMenuItem toolStripMenuItemStop;




    }
}
