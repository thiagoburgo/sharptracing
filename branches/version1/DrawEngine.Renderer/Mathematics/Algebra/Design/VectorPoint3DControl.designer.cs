namespace DrawEngine.Renderer.Mathematics.Algebra.Design
{
    partial class VectorPoint3DControl {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (this.components != null)) {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.Z = new System.Windows.Forms.NumericUpDown();
            this.Y = new System.Windows.Forms.NumericUpDown();
            this.X = new System.Windows.Forms.NumericUpDown();
            this.lblVectorName = new System.Windows.Forms.Label();
            this.UnitX = new System.Windows.Forms.Button();
            this.UnitY = new System.Windows.Forms.Button();
            this.UnitZ = new System.Windows.Forms.Button();
            this.Zero = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Z)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.X)).BeginInit();
            this.SuspendLayout();
            // 
            // Z
            // 
            this.Z.DecimalPlaces = 3;
            this.Z.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Z.Location = new System.Drawing.Point(136, 2);
            this.Z.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Z.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.Z.Name = "Z";
            this.Z.Size = new System.Drawing.Size(62, 20);
            this.Z.TabIndex = 28;
            this.Z.ValueChanged += new System.EventHandler(this.Z_ValueChanged);
            // 
            // Y
            // 
            this.Y.DecimalPlaces = 3;
            this.Y.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.Y.Location = new System.Drawing.Point(69, 2);
            this.Y.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.Y.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(62, 20);
            this.Y.TabIndex = 27;
            this.Y.ValueChanged += new System.EventHandler(this.Y_ValueChanged);
            // 
            // X
            // 
            this.X.DecimalPlaces = 3;
            this.X.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.X.Location = new System.Drawing.Point(2, 2);
            this.X.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.X.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(62, 20);
            this.X.TabIndex = 25;
            this.X.ValueChanged += new System.EventHandler(this.X_ValueChanged);
            // 
            // lblVectorName
            // 
            this.lblVectorName.AutoSize = true;
            this.lblVectorName.Location = new System.Drawing.Point(-2, -1);
            this.lblVectorName.Name = "lblVectorName";
            this.lblVectorName.Size = new System.Drawing.Size(0, 13);
            this.lblVectorName.TabIndex = 26;
            // 
            // UnitX
            // 
            this.UnitX.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.UnitX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnitX.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnitX.Location = new System.Drawing.Point(0, 24);
            this.UnitX.Name = "UnitX";
            this.UnitX.Size = new System.Drawing.Size(63, 20);
            this.UnitX.TabIndex = 29;
            this.UnitX.Text = "Unit X";
            this.UnitX.UseVisualStyleBackColor = true;
            this.UnitX.Click += new System.EventHandler(this.UnitX_Click);
            // 
            // UnitY
            // 
            this.UnitY.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.UnitY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnitY.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnitY.Location = new System.Drawing.Point(69, 24);
            this.UnitY.Name = "UnitY";
            this.UnitY.Size = new System.Drawing.Size(62, 20);
            this.UnitY.TabIndex = 30;
            this.UnitY.Text = "Unit Y";
            this.UnitY.UseVisualStyleBackColor = true;
            this.UnitY.Click += new System.EventHandler(this.UnitY_Click);
            // 
            // UnitZ
            // 
            this.UnitZ.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.UnitZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UnitZ.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnitZ.Location = new System.Drawing.Point(136, 24);
            this.UnitZ.Name = "UnitZ";
            this.UnitZ.Size = new System.Drawing.Size(62, 20);
            this.UnitZ.TabIndex = 31;
            this.UnitZ.Text = "Unit Z";
            this.UnitZ.UseVisualStyleBackColor = true;
            this.UnitZ.Click += new System.EventHandler(this.UnitZ_Click);
            // 
            // Zero
            // 
            this.Zero.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.Zero.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Zero.Font = new System.Drawing.Font("Calibri", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zero.Location = new System.Drawing.Point(69, 47);
            this.Zero.Margin = new System.Windows.Forms.Padding(0);
            this.Zero.Name = "Zero";
            this.Zero.Size = new System.Drawing.Size(62, 20);
            this.Zero.TabIndex = 32;
            this.Zero.Text = "Zero";
            this.Zero.UseVisualStyleBackColor = true;
            this.Zero.Click += new System.EventHandler(this.Zero_Click);
            // 
            // VectorPoint3DControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.Zero);
            this.Controls.Add(this.UnitZ);
            this.Controls.Add(this.UnitY);
            this.Controls.Add(this.UnitX);
            this.Controls.Add(this.Z);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.X);
            this.Controls.Add(this.lblVectorName);
            this.Name = "VectorPoint3DControl";
            this.Size = new System.Drawing.Size(199, 68);
            ((System.ComponentModel.ISupportInitialize)(this.Z)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.X)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown Z;
        private System.Windows.Forms.NumericUpDown Y;
        private System.Windows.Forms.NumericUpDown X;
        private System.Windows.Forms.Label lblVectorName;
        private System.Windows.Forms.Button UnitX;
        private System.Windows.Forms.Button UnitY;
        private System.Windows.Forms.Button UnitZ;
        private System.Windows.Forms.Button Zero;
    }
}