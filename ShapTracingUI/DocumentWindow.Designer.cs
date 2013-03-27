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
            this.pictureView = new System.Windows.Forms.PictureBox();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripRenderer = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripStrategy = new System.Windows.Forms.ToolStripComboBox();
            this.contextMenuTabPage.SuspendLayout();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureView)).BeginInit();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
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
            this.menuItem2.Size = new System.Drawing.Size(132, 22);
            this.menuItem2.Text = "Test";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItemCheckTest
            // 
            this.menuItemCheckTest.Name = "menuItemCheckTest";
            this.menuItemCheckTest.Size = new System.Drawing.Size(132, 22);
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
            this.contextMenuTabPage.Size = new System.Drawing.Size(232, 176);
            // 
            // menuItemRender
            // 
            this.menuItemRender.Name = "menuItemRender";
            this.menuItemRender.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.menuItemRender.Size = new System.Drawing.Size(231, 22);
            this.menuItemRender.Text = "&Render";
            this.menuItemRender.Click += new System.EventHandler(this.menuItemRender_Click);
            // 
            // toolStripMenuItemStop
            // 
            this.toolStripMenuItemStop.Name = "toolStripMenuItemStop";
            this.toolStripMenuItemStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Pause)));
            this.toolStripMenuItemStop.Size = new System.Drawing.Size(231, 22);
            this.toolStripMenuItemStop.Text = "Sto&p";
            this.toolStripMenuItemStop.Click += new System.EventHandler(this.toolStripMenuItemStop_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(228, 6);
            // 
            // toolStripAddFrame
            // 
            this.toolStripAddFrame.Name = "toolStripAddFrame";
            this.toolStripAddFrame.Size = new System.Drawing.Size(231, 22);
            this.toolStripAddFrame.Text = "Add &Frame";
            this.toolStripAddFrame.Click += new System.EventHandler(this.toolStripAddFrame_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(228, 6);
            // 
            // menuItemCloseButThis
            // 
            this.menuItemCloseButThis.Name = "menuItemCloseButThis";
            this.menuItemCloseButThis.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.W)));
            this.menuItemCloseButThis.Size = new System.Drawing.Size(231, 22);
            this.menuItemCloseButThis.Text = "Close All &But this";
            this.menuItemCloseButThis.Click += new System.EventHandler(this.menuItemCloseButThis_Click);
            // 
            // menuItemCloseAll
            // 
            this.menuItemCloseAll.Name = "menuItemCloseAll";
            this.menuItemCloseAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.menuItemCloseAll.Size = new System.Drawing.Size(231, 22);
            this.menuItemCloseAll.Text = "Close &All";
            this.menuItemCloseAll.Click += new System.EventHandler(this.menuItemCloseAll_Click);
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.menuItemClose.Size = new System.Drawing.Size(231, 22);
            this.menuItemClose.Text = "&Close";
            this.menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(228, 6);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F11)));
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
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
            this.lblRenderStatus.Size = new System.Drawing.Size(96, 17);
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
            this.lblPercentRend.Size = new System.Drawing.Size(23, 17);
            this.lblPercentRend.Text = "0%";
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(49, 17);
            this.lblTimeElapsed.Text = "00:00:00";
            // 
            // pictureView
            // 
            this.pictureView.BackColor = System.Drawing.Color.Black;
            this.pictureView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureView.Location = new System.Drawing.Point(0, 0);
            this.pictureView.Name = "pictureView";
            this.pictureView.Size = new System.Drawing.Size(528, 421);
            this.pictureView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureView.TabIndex = 3;
            this.pictureView.TabStop = false;
            this.pictureView.SizeChanged += new System.EventHandler(this.panelRender_SizeChanged);
            this.pictureView.Click += new System.EventHandler(this.DocumentWindow_Click);
            this.pictureView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseDoubleClick);
            this.pictureView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseDown);
            this.pictureView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseMove);
            this.pictureView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseUp);
            // 
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pictureView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(528, 421);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 4);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(528, 446);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripRenderer,
            this.toolStripSeparator1,
            this.toolStripLabel2,
            this.toolStripStrategy});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(407, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(57, 22);
            this.toolStripLabel1.Text = "Renderer:";
            // 
            // toolStripRenderer
            // 
            this.toolStripRenderer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripRenderer.Name = "toolStripRenderer";
            this.toolStripRenderer.Size = new System.Drawing.Size(121, 25);
            this.toolStripRenderer.SelectedIndexChanged += new System.EventHandler(this.toolStripRenderer_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(53, 22);
            this.toolStripLabel2.Text = "Strategy:";
            // 
            // toolStripStrategy
            // 
            this.toolStripStrategy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripStrategy.Name = "toolStripStrategy";
            this.toolStripStrategy.Size = new System.Drawing.Size(121, 25);
            this.toolStripStrategy.SelectedIndexChanged += new System.EventHandler(this.toolStripRenderer_SelectedIndexChanged);
            // 
            // DocumentWindow
            // 
            this.AllowDrop = true;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(528, 472);
            this.ContextMenuStrip = this.contextMenuTabPage;
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusBar);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DocumentWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.TabPageContextMenuStrip = this.contextMenuTabPage;
            this.Activated += new System.EventHandler(this.DocumentWindow_Click);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DocumentWindow_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DocumentWindow_FormClosed);
            this.SizeChanged += new System.EventHandler(this.panelRender_SizeChanged);
            this.Click += new System.EventHandler(this.DocumentWindow_Click);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DocumentWindow_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DocumentWindow_DragEnter);
            this.contextMenuTabPage.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureView)).EndInit();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private PictureBox pictureView;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem fullScreenToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem toolStripAddFrame;
        private ToolStripMenuItem toolStripMenuItemStop;
        private ToolStripContainer toolStripContainer1;
        private ToolStrip toolStrip1;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox toolStripRenderer;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripLabel toolStripLabel2;
        private ToolStripComboBox toolStripStrategy;




    }
}
