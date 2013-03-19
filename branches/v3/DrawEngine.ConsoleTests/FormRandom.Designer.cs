namespace DrawEngine.ConsoleTests {
    partial class FormRandom {
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
            this.resultPicture = new System.Windows.Forms.PictureBox();
            this.btnHalton = new System.Windows.Forms.Button();
            this.btnSobol = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownSamples = new System.Windows.Forms.NumericUpDown();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.resultPicture)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSamples)).BeginInit();
            this.SuspendLayout();
            // 
            // resultPicture
            // 
            this.resultPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultPicture.Location = new System.Drawing.Point(0, 0);
            this.resultPicture.Name = "resultPicture";
            this.resultPicture.Size = new System.Drawing.Size(498, 430);
            this.resultPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.resultPicture.TabIndex = 0;
            this.resultPicture.TabStop = false;
            // 
            // btnHalton
            // 
            this.btnHalton.Location = new System.Drawing.Point(306, 9);
            this.btnHalton.Name = "btnHalton";
            this.btnHalton.Size = new System.Drawing.Size(75, 23);
            this.btnHalton.TabIndex = 1;
            this.btnHalton.Text = "Halton";
            this.btnHalton.UseVisualStyleBackColor = true;
            this.btnHalton.Click += new System.EventHandler(this.btnHalton_Click);
            // 
            // btnSobol
            // 
            this.btnSobol.Location = new System.Drawing.Point(387, 9);
            this.btnSobol.Name = "btnSobol";
            this.btnSobol.Size = new System.Drawing.Size(75, 23);
            this.btnSobol.TabIndex = 2;
            this.btnSobol.Text = "Sobol";
            this.btnSobol.UseVisualStyleBackColor = true;
            this.btnSobol.Click += new System.EventHandler(this.btnSobol_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.numericUpDownSamples);
            this.panel1.Controls.Add(this.btnHalton);
            this.panel1.Controls.Add(this.btnSobol);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 436);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 67);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(273, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Samples";
            // 
            // numericUpDownSamples
            // 
            this.numericUpDownSamples.Location = new System.Drawing.Point(324, 37);
            this.numericUpDownSamples.Maximum = new decimal(new int[] {
            1569325056,
            23283064,
            0,
            0});
            this.numericUpDownSamples.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numericUpDownSamples.Name = "numericUpDownSamples";
            this.numericUpDownSamples.Size = new System.Drawing.Size(138, 20);
            this.numericUpDownSamples.TabIndex = 3;
            this.numericUpDownSamples.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyGrid1.Location = new System.Drawing.Point(504, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(264, 436);
            this.propertyGrid1.TabIndex = 4;
            // 
            // FormRandom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 503);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.resultPicture);
            this.Name = "FormRandom";
            this.Text = "FormRandom";
            this.Load += new System.EventHandler(this.FormRandom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.resultPicture)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSamples)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox resultPicture;
        private System.Windows.Forms.Button btnHalton;
        private System.Windows.Forms.Button btnSobol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDownSamples;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
    }
}