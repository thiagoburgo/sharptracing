namespace DrawEngine.Renderer.Mesh.Design {
    partial class LoadingMeshModelDialog
    {
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
            this.components = new System.ComponentModel.Container();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblTrianglesInfo = new System.Windows.Forms.Label();
            this.lblTimeElapsed = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(25, 24);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(324, 12);
            this.progressBar.TabIndex = 0;
            this.progressBar.UseWaitCursor = true;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(22, 8);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(57, 13);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = "Loading ...";
            this.lblInfo.UseWaitCursor = true;
            // 
            // lblTrianglesInfo
            // 
            this.lblTrianglesInfo.AutoSize = true;
            this.lblTrianglesInfo.Location = new System.Drawing.Point(22, 41);
            this.lblTrianglesInfo.Name = "lblTrianglesInfo";
            this.lblTrianglesInfo.Size = new System.Drawing.Size(62, 13);
            this.lblTrianglesInfo.TabIndex = 2;
            this.lblTrianglesInfo.Text = "Triangles: 0";
            this.lblTrianglesInfo.UseWaitCursor = true;
            // 
            // lblTimeElapsed
            // 
            this.lblTimeElapsed.AutoSize = true;
            this.lblTimeElapsed.Location = new System.Drawing.Point(199, 39);
            this.lblTimeElapsed.Name = "lblTimeElapsed";
            this.lblTimeElapsed.Size = new System.Drawing.Size(114, 13);
            this.lblTimeElapsed.TabIndex = 4;
            this.lblTimeElapsed.Text = "Elapsed Time: 00m00s";
            this.lblTimeElapsed.UseWaitCursor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 950;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LoadingModelDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 63);
            this.Controls.Add(this.lblTimeElapsed);
            this.Controls.Add(this.lblTrianglesInfo);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LoadingModelDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loading Model ...";
            this.UseWaitCursor = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoadingModelDialog_FormClosing);
            this.Load += new System.EventHandler(this.LoadingModelDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblTrianglesInfo;
        private System.Windows.Forms.Label lblTimeElapsed;
        private System.Windows.Forms.Timer timer1;
    }
}