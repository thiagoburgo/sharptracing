namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design {
    partial class SphereMapEditorForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.ddlNamePrefix = new System.Windows.Forms.ComboBox();
            this.txtPathBase = new System.Windows.Forms.TextBox();
            this.btnBasePath = new System.Windows.Forms.Button();
            this.numericUpDownRadius = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 102);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(429, 133);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(229, 73);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(148, 73);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 17;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ddlNamePrefix
            // 
            this.ddlNamePrefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlNamePrefix.FormattingEnabled = true;
            this.ddlNamePrefix.Location = new System.Drawing.Point(5, 34);
            this.ddlNamePrefix.Name = "ddlNamePrefix";
            this.ddlNamePrefix.Size = new System.Drawing.Size(248, 21);
            this.ddlNamePrefix.TabIndex = 16;
            this.ddlNamePrefix.SelectedIndexChanged += new System.EventHandler(this.ddlNamePrefix_SelectedIndexChanged);
            // 
            // txtPathBase
            // 
            this.txtPathBase.Location = new System.Drawing.Point(5, 7);
            this.txtPathBase.Name = "txtPathBase";
            this.txtPathBase.ReadOnly = true;
            this.txtPathBase.Size = new System.Drawing.Size(248, 20);
            this.txtPathBase.TabIndex = 15;
            this.txtPathBase.Text = "D:\\Models & Textures\\Texturas\\SphereMap\\";
            this.txtPathBase.TextChanged += new System.EventHandler(this.txtPathBase_TextChanged);
            // 
            // btnBasePath
            // 
            this.btnBasePath.Location = new System.Drawing.Point(259, 5);
            this.btnBasePath.Name = "btnBasePath";
            this.btnBasePath.Size = new System.Drawing.Size(89, 23);
            this.btnBasePath.TabIndex = 14;
            this.btnBasePath.Text = "Base Folder ...";
            this.btnBasePath.UseVisualStyleBackColor = true;
            this.btnBasePath.Click += new System.EventHandler(this.btnBasePath_Click);
            // 
            // numericUpDownRadius
            // 
            this.numericUpDownRadius.Location = new System.Drawing.Point(295, 34);
            this.numericUpDownRadius.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownRadius.Name = "numericUpDownRadius";
            this.numericUpDownRadius.Size = new System.Drawing.Size(65, 20);
            this.numericUpDownRadius.TabIndex = 20;
            this.numericUpDownRadius.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(256, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Radius: ";
            // 
            // folderDialog
            // 
            this.folderDialog.Description = "Choose de base folder for sphere map textures (panorama images)";
            this.folderDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderDialog.ShowNewFolderButton = false;
            // 
            // SphereMapEditorForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(453, 244);
            this.Controls.Add(this.numericUpDownRadius);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.ddlNamePrefix);
            this.Controls.Add(this.txtPathBase);
            this.Controls.Add(this.btnBasePath);
            this.Controls.Add(this.label1);
            this.Name = "SphereMapEditorForm";
            this.Text = "SphereMapEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ComboBox ddlNamePrefix;
        private System.Windows.Forms.TextBox txtPathBase;
        private System.Windows.Forms.Button btnBasePath;
        private System.Windows.Forms.NumericUpDown numericUpDownRadius;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
    }
}