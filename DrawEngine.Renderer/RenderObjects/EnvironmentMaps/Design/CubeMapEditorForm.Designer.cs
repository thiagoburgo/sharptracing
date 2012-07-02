namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design
{
    partial class CubeMapEditorForm
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
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
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.btnBasePath = new System.Windows.Forms.Button();
            this.txtPathBase = new System.Windows.Forms.TextBox();
            this.ddlNamePrefix = new System.Windows.Forms.ComboBox();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(159, 80);
            this.numericUpDown3.Maximum = new decimal(new int[] {
                                                                        10000,
                                                                        0,
                                                                        0,
                                                                        0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(48, 20);
            this.numericUpDown3.TabIndex = 2;
            this.numericUpDown3.Value = new decimal(new int[] {
                                                                      700,
                                                                      0,
                                                                      0,
                                                                      0});
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(105, 80);
            this.numericUpDown1.Maximum = new decimal(new int[] {
                                                                        10000,
                                                                        0,
                                                                        0,
                                                                        0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(48, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
                                                                      700,
                                                                      0,
                                                                      0,
                                                                      0});
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(213, 80);
            this.numericUpDown2.Maximum = new decimal(new int[] {
                                                                        10000,
                                                                        0,
                                                                        0,
                                                                        0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(48, 20);
            this.numericUpDown2.TabIndex = 4;
            this.numericUpDown2.Value = new decimal(new int[] {
                                                                      700,
                                                                      0,
                                                                      0,
                                                                      0});
            // 
            // btnBasePath
            // 
            this.btnBasePath.Location = new System.Drawing.Point(265, 10);
            this.btnBasePath.Name = "btnBasePath";
            this.btnBasePath.Size = new System.Drawing.Size(89, 23);
            this.btnBasePath.TabIndex = 5;
            this.btnBasePath.Text = "Base Folder ...";
            this.btnBasePath.UseVisualStyleBackColor = true;
            this.btnBasePath.Click += new System.EventHandler(this.btnBasePath_Click);
            // 
            // txtPathBase
            // 
            this.txtPathBase.Location = new System.Drawing.Point(11, 12);
            this.txtPathBase.Name = "txtPathBase";
            this.txtPathBase.ReadOnly = true;
            this.txtPathBase.Size = new System.Drawing.Size(248, 20);
            this.txtPathBase.TabIndex = 6;
            this.txtPathBase.Text = "D:\\Models & Textures\\Texturas\\CubeMaps\\";
            this.txtPathBase.TextChanged += new System.EventHandler(this.txtPathBase_TextChanged);
            // 
            // ddlNamePrefix
            // 
            this.ddlNamePrefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlNamePrefix.FormattingEnabled = true;
            this.ddlNamePrefix.Location = new System.Drawing.Point(11, 39);
            this.ddlNamePrefix.Name = "ddlNamePrefix";
            this.ddlNamePrefix.Size = new System.Drawing.Size(248, 21);
            this.ddlNamePrefix.TabIndex = 7;
            this.ddlNamePrefix.SelectedIndexChanged += new System.EventHandler(this.ddlNamePrefix_SelectedIndexChanged);
            // 
            // folderDialog
            // 
            this.folderDialog.Description = "Choose de base folder for cube map textures (_NX, _NY...)";
            this.folderDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderDialog.ShowNewFolderButton = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(105, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Width:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(156, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Depth:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(105, 113);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 11;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(186, 113);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(266, 39);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(187, 133);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // CubeMapEditorForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(465, 184);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ddlNamePrefix);
            this.Controls.Add(this.txtPathBase);
            this.Controls.Add(this.btnBasePath);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.numericUpDown3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CubeMapEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CubeMap Editor";
            this.Load += new System.EventHandler(this.txtPathBase_TextChanged);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Button btnBasePath;
        private System.Windows.Forms.TextBox txtPathBase;
        private System.Windows.Forms.ComboBox ddlNamePrefix;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}