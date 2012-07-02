using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DrawEngine.Renderer;
using DrawEngine.Renderer.Properties;

namespace DrawEngine.SharpTracingUI
{
    partial class TreeViewScenes {
        #region
        private ContextMenuStrip contextMenuStripScene;
        private IContainer components;
        private ToolStripMenuItem changeToolStripMenu;
        private ToolStripMenuItem removeToolStripMenu;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem toolStripMenuItem9;
        private ToolStripMenuItem toolStripMenuItem10;
        private ToolStripMenuItem toolStripMenuItem11;
        private ToolStripMenuItem toolStripMenuItem12;
        private ToolStripMenuItem toolStripMenuItem13;
        private ToolStripMenuItem toolStripMenuItem14;
        private ToolStripMenuItem toolStripMenuItem15;
        private ToolStripMenuItem toolStripMenuItem16;
        private ToolStripMenuItem toolStripMenuItem17;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem toolStripMenuItem18;
        private ToolStripMenuItem toolStripMenuItem19;
        private ContextMenuStrip contextMenuPrimitive;
        private ToolStripMenuItem changeToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ImageList sceneImages;
        #endregion

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeViewScenes));
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
            this.contextMenuPrimitive = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.changeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sceneImages = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStripScene.SuspendLayout();
            this.contextMenuPrimitive.SuspendLayout();
            this.SuspendLayout();
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
            this.contextMenuStripScene.Size = new System.Drawing.Size(137, 126);
            // 
            // changeToolStripMenu
            // 
            this.changeToolStripMenu.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.member;
            this.changeToolStripMenu.Name = "changeToolStripMenu";
            this.changeToolStripMenu.Size = new System.Drawing.Size(136, 22);
            this.changeToolStripMenu.Text = "&Change";
            // 
            // removeToolStripMenu
            // 
            this.removeToolStripMenu.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.remover;
            this.removeToolStripMenu.Name = "removeToolStripMenu";
            this.removeToolStripMenu.Size = new System.Drawing.Size(136, 22);
            this.removeToolStripMenu.Text = "&Remove";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(133, 6);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem12});
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(136, 22);
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
            this.toolStripMenuItem5.Size = new System.Drawing.Size(132, 22);
            this.toolStripMenuItem5.Text = "&Plane Based";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem6.Text = "&Triangle";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem7.Text = "&Disc";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem8.Text = "&Plane";
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem9.Text = "&Box";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuItem10.Text = "&Quadrilatero";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(143, 22);
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
            this.toolStripMenuItem12.Size = new System.Drawing.Size(132, 22);
            this.toolStripMenuItem12.Text = "&Quadrics";
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItem13.Text = "&Sphere";
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItem14.Text = "C&ylinder";
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItem15.Text = "&Ellipsoid";
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItem16.Text = "&Torus";
            // 
            // toolStripMenuItem17
            // 
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            this.toolStripMenuItem17.Size = new System.Drawing.Size(113, 22);
            this.toolStripMenuItem17.Text = "&Cone";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(133, 6);
            // 
            // toolStripMenuItem18
            // 
            this.toolStripMenuItem18.Name = "toolStripMenuItem18";
            this.toolStripMenuItem18.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem18.Text = "&Save Scene";
            // 
            // toolStripMenuItem19
            // 
            this.toolStripMenuItem19.Name = "toolStripMenuItem19";
            this.toolStripMenuItem19.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem19.Text = "&Load Scene";
            // 
            // contextMenuPrimitive
            // 
            this.contextMenuPrimitive.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.contextMenuPrimitive.Name = "contextMenuPrimitive";
            this.contextMenuPrimitive.Size = new System.Drawing.Size(114, 48);
            this.contextMenuPrimitive.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuPrimitive_ItemClicked);
            // 
            // changeToolStripMenuItem
            // 
            this.changeToolStripMenuItem.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.member;
            this.changeToolStripMenuItem.Name = "changeToolStripMenuItem";
            this.changeToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.changeToolStripMenuItem.Text = "&Change";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.remover;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.removeToolStripMenuItem.Text = "&Remove";
            // 
            // sceneImages
            // 
            this.sceneImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("sceneImages.ImageStream")));
            this.sceneImages.TransparentColor = System.Drawing.Color.Transparent;
            this.sceneImages.Images.SetKeyName(0, "scene.png");
            this.sceneImages.Images.SetKeyName(1, "shell32_4.ico");
            // 
            // TreeViewScenes
            // 
            this.ImageIndex = 0;
            this.ImageList = this.sceneImages;
            this.SelectedImageIndex = 0;
            this.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewScenes_AfterSelect);
            this.contextMenuStripScene.ResumeLayout(false);
            this.contextMenuPrimitive.ResumeLayout(false);
            this.ResumeLayout(false);

        }
       
    }
}
