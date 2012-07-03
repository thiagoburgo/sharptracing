namespace DrawEngine.Renderer.Materials.Design {
    partial class MaterialTreeViewEditorControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Materials");
            this.treeViewMaterials = new System.Windows.Forms.TreeView();
            this.linkLblNewMaterial = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // treeViewMaterials
            // 
            this.treeViewMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewMaterials.Location = new System.Drawing.Point(0, 0);
            this.treeViewMaterials.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewMaterials.Name = "treeViewMaterials";
            treeNode1.Name = "materials";
            treeNode1.NodeFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            treeNode1.Text = "Materials";
            this.treeViewMaterials.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeViewMaterials.Size = new System.Drawing.Size(164, 160);
            this.treeViewMaterials.TabIndex = 0;
            this.treeViewMaterials.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewMaterials_NodeMouseDoubleClick);
            this.treeViewMaterials.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewMaterials_AfterSelect);
            // 
            // linkLblNewMaterial
            // 
            this.linkLblNewMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLblNewMaterial.AutoSize = true;
            this.linkLblNewMaterial.Location = new System.Drawing.Point(83, 160);
            this.linkLblNewMaterial.Name = "linkLblNewMaterial";
            this.linkLblNewMaterial.Size = new System.Drawing.Size(81, 13);
            this.linkLblNewMaterial.TabIndex = 1;
            this.linkLblNewMaterial.TabStop = true;
            this.linkLblNewMaterial.Text = "<New Material>";
            this.linkLblNewMaterial.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblNewMaterial_LinkClicked);
            // 
            // MaterialTreeViewEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLblNewMaterial);
            this.Controls.Add(this.treeViewMaterials);
            this.Name = "MaterialTreeViewEditorControl";
            this.Size = new System.Drawing.Size(164, 176);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewMaterials;
        private System.Windows.Forms.LinkLabel linkLblNewMaterial;
    }
}
