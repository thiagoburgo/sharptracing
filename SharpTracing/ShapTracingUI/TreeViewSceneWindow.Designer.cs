namespace DrawEngine.SharpTracingUI
{
    partial class TreeViewSceneWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeViewSceneWindow));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Scenes(0)", 1, 1);
            this.contextMenuPrimitive = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sceneImages = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripScene = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem18 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem19 = new System.Windows.Forms.ToolStripMenuItem();
            this.treeViewScene = new DrawEngine.SharpTracingUI.TreeViewScenes();
            this.contextMenuPrimitive.SuspendLayout();
            this.contextMenuStripScene.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuPrimitive
            // 
            this.contextMenuPrimitive.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuPrimitive.Name = "contextMenuPrimitive";
            this.contextMenuPrimitive.Size = new System.Drawing.Size(118, 48);
            // 
            // changeToolStripMenuItem
            // 
            this.changeToolStripMenuItem.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.member;
            this.changeToolStripMenuItem.Name = "changeToolStripMenuItem";
            this.changeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.changeToolStripMenuItem.Text = "&Change";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.remover;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // sceneImages
            // 
            this.sceneImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("sceneImages.ImageStream")));
            this.sceneImages.TransparentColor = System.Drawing.Color.Transparent;
            this.sceneImages.Images.SetKeyName(0, "scene.png");
            this.sceneImages.Images.SetKeyName(1, "shell32_4.ico");
            // 
            // contextMenuStripScene
            // 
            this.contextMenuStripScene.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeToolStripMenu,
            this.removeToolStripMenu,
            this.toolStripSeparator2,
            this.toolStripMenuItem4,
            this.toolStripSeparator3,
            this.toolStripMenuItem18,
            this.toolStripMenuItem19});
            this.contextMenuStripScene.Name = "contextMenuPrimitive";
            this.contextMenuStripScene.Size = new System.Drawing.Size(147, 126);
            // 
            // changeToolStripMenu
            // 
            this.changeToolStripMenu.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.member;
            this.changeToolStripMenu.Name = "changeToolStripMenu";
            this.changeToolStripMenu.Size = new System.Drawing.Size(146, 22);
            this.changeToolStripMenu.Text = "&Change";
            // 
            // removeToolStripMenu
            // 
            this.removeToolStripMenu.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.remover;
            this.removeToolStripMenu.Name = "removeToolStripMenu";
            this.removeToolStripMenu.Size = new System.Drawing.Size(146, 22);
            this.removeToolStripMenu.Text = "&Remove";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem12});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem4.Text = "Add &Primitive";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11});
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(137, 22);
            this.toolStripMenuItem5.Text = "&Plane Based";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem6.Text = "&Triangle";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem7.Text = "&Disc";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem8.Text = "&Plane";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem9.Text = "&Box";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem10.Text = "&Quadrilatero";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(154, 22);
            this.toolStripMenuItem11.Text = "Triangle &Model";
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem13,
            this.toolStripMenuItem14,
            this.toolStripMenuItem15,
            this.toolStripMenuItem16,
            this.toolStripMenuItem17});
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(137, 22);
            this.toolStripMenuItem12.Text = "&Quadrics";
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItem13.Text = "&Sphere";
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItem14.Text = "C&ylinder";
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItem15.Text = "&Ellipsoid";
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItem16.Text = "&Torus";
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            this.toolStripMenuItem17.Size = new System.Drawing.Size(118, 22);
            this.toolStripMenuItem17.Text = "&Cone";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(143, 6);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            this.toolStripMenuItem18.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem18.Text = "&Save Scene";
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(146, 22);
            this.toolStripMenuItem19.Text = "&Load Scene";
            // 
            // treeViewScene
            // 
            this.treeViewScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewScene.ImageIndex = 0;
            this.treeViewScene.ImageList = this.sceneImages;
            this.treeViewScene.Location = new System.Drawing.Point(0, 0);
            this.treeViewScene.Name = "treeViewScene";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "Scenes";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Text = "Scenes(0)";
            this.treeViewScene.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeViewScene.SelectedImageIndex = 0;
            this.treeViewScene.Size = new System.Drawing.Size(256, 415);
            this.treeViewScene.TabIndex = 2;
            // 
            // TreeViewSceneWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 415);
            this.Controls.Add(this.treeViewScene);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TreeViewSceneWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.TabText = "Scene Tree";
            this.Text = "Scene Tree";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TreeViewSceneWindow_FormClosing);
            this.contextMenuPrimitive.ResumeLayout(false);
            this.contextMenuStripScene.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList sceneImages;
        private System.Windows.Forms.ContextMenuStrip contextMenuPrimitive;
        private System.Windows.Forms.ToolStripMenuItem changeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripScene;
        private System.Windows.Forms.ToolStripMenuItem changeToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem16;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem17;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem18;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem19;
        private TreeViewScenes treeViewScene;
    }
}