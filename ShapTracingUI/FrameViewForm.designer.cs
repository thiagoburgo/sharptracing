namespace DrawEngine.SharpTracingUI {
    partial class FrameViewForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            //if (disposing && (components != null)) {
            //    components.Dispose();
            //}
            //base.Dispose(disposing);
            this.Visible = false;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameViewForm));
            this.frameView1 = new DrawEngine.SharpTracingUI.Components.FrameView();
            this.SuspendLayout();
            // 
            // frameView1
            // 
            this.frameView1.AutoScroll = true;
            this.frameView1.BackColor = System.Drawing.SystemColors.Control;
            this.frameView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frameView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameView1.FrameHeight = 250;
            this.frameView1.FramePadding = 10;
            this.frameView1.Location = new System.Drawing.Point(0, 0);
            this.frameView1.Margin = new System.Windows.Forms.Padding(10);
            this.frameView1.Name = "frameView1";
            this.frameView1.Padding = new System.Windows.Forms.Padding(10);
            this.frameView1.SelectedIndex = -1;
            this.frameView1.Size = new System.Drawing.Size(177, 470);
            this.frameView1.TabIndex = 0;
            // 
            // FrameViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(177, 470);
            this.Controls.Add(this.frameView1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrameViewForm";
            this.TabText = "Frame View";
            this.Text = "Frame View";
            this.ResumeLayout(false);

        }

        #endregion

        private DrawEngine.SharpTracingUI.Components.FrameView frameView1;
    }
}