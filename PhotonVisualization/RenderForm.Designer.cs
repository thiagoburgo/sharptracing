namespace PhotonVisualization
{
    partial class RenderForm
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
            this.pic3d = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pic3d
            // 
            this.pic3d.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pic3d.Location = new System.Drawing.Point(0, 0);
            this.pic3d.Name = "pic3d";
            this.pic3d.Size = new System.Drawing.Size(584, 564);
            this.pic3d.TabIndex = 0;
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 564);
            this.Controls.Add(this.pic3d);
            this.Name = "RenderForm";
            this.Text = "Form1";
            this.Resize += new System.EventHandler(this.RenderForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pic3d;
    }
}

