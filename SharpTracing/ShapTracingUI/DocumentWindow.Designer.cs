using System.ComponentModel;
using System.Windows.Forms;
using DrawEngine.Renderer.Util;

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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripRenderer = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripStrategy = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.txtXParallel = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.txtYParallel = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxRenderByTiles = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.renderOnWheelTimer = new System.Windows.Forms.Timer(this.components);
            this.tiledPictureViewControlView = new DrawEngine.SharpTracingUI.Controls.TiledPictureViewControl();
            this.contextMenuTabPage.SuspendLayout();
            this.statusBar.SuspendLayout();
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
            this.statusBar.Size = new System.Drawing.Size(816, 22);
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
            // toolStripContainer1
            // 
            this.toolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tiledPictureViewControlView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(816, 421);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 4);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(816, 446);
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
            this.toolStripStrategy,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.txtXParallel,
            this.toolStripLabel4,
            this.txtYParallel,
            this.toolStripSeparator3,
            this.toolStripLabel5,
            this.toolStripTextBoxRenderByTiles,
            this.toolStripLabel7});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(625, 25);
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(34, 22);
            this.toolStripLabel3.Text = "Tiles:";
            // 
            // txtXParallel
            // 
            this.txtXParallel.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtXParallel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtXParallel.MaxLength = 3;
            this.txtXParallel.Name = "txtXParallel";
            this.txtXParallel.Size = new System.Drawing.Size(25, 25);
            this.txtXParallel.Text = "3";
            this.txtXParallel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtParallel_KeyPress);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(12, 22);
            this.toolStripLabel4.Text = "x";
            // 
            // txtYParallel
            // 
            this.txtYParallel.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtYParallel.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtYParallel.MaxLength = 3;
            this.txtYParallel.Name = "txtYParallel";
            this.txtYParallel.Size = new System.Drawing.Size(25, 25);
            this.txtYParallel.Text = "3";
            this.txtYParallel.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtParallel_KeyPress);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(73, 22);
            this.toolStripLabel5.Text = "  1 Render to";
            // 
            // toolStripTextBoxRenderByTiles
            // 
            this.toolStripTextBoxRenderByTiles.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toolStripTextBoxRenderByTiles.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.toolStripTextBoxRenderByTiles.MaxLength = 3;
            this.toolStripTextBoxRenderByTiles.Name = "toolStripTextBoxRenderByTiles";
            this.toolStripTextBoxRenderByTiles.Size = new System.Drawing.Size(25, 25);
            this.toolStripTextBoxRenderByTiles.Text = "1";
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel7.Text = "Tile(s)";
            // 
            // renderOnWheelTimer
            // 
            this.renderOnWheelTimer.Enabled = true;
            this.renderOnWheelTimer.Interval = 200;
            this.renderOnWheelTimer.Tick += new System.EventHandler(this.renderOnWheelTimer_Tick);
            // 
            // tiledPictureViewControlView
            // 
            this.tiledPictureViewControlView.AutoScroll = true;
            this.tiledPictureViewControlView.BackColor = System.Drawing.SystemColors.Control;
            this.tiledPictureViewControlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tiledPictureViewControlView.Location = new System.Drawing.Point(0, 0);
            this.tiledPictureViewControlView.Margin = new System.Windows.Forms.Padding(0);
            this.tiledPictureViewControlView.Name = "tiledPictureViewControlView";
            this.tiledPictureViewControlView.Size = new System.Drawing.Size(816, 421);
            this.tiledPictureViewControlView.TabIndex = 0;
            this.tiledPictureViewControlView.TiledBitmap = null;
            this.tiledPictureViewControlView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tiledPictureViewControlView_MouseClick);
            this.tiledPictureViewControlView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseDown);
            this.tiledPictureViewControlView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseMove);
            this.tiledPictureViewControlView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseUp);
            this.tiledPictureViewControlView.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureView_MouseWheel);
            // 
            // DocumentWindow
            // 
            this.AllowDrop = true;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(816, 472);
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
            this.Click += new System.EventHandler(this.DocumentWindow_Click);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.DocumentWindow_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.DocumentWindow_DragEnter);
            this.contextMenuTabPage.ResumeLayout(false);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
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
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripLabel toolStripLabel3;
        private ToolStripLabel toolStripLabel4;
        private ToolStripTextBox txtYParallel;
        private ToolStripTextBox txtXParallel;
        private Controls.TiledPictureViewControl tiledPictureViewControlView;
        private ToolStripLabel toolStripLabel5;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripTextBox toolStripTextBoxRenderByTiles;
        private ToolStripLabel toolStripLabel7;
        private Timer renderOnWheelTimer;




    }
}
