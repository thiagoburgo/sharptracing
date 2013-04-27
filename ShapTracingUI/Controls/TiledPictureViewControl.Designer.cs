namespace DrawEngine.SharpTracingUI.Controls
{
    partial class TiledPictureViewControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTiledContainer = new System.Windows.Forms.Panel();
            this.panelDummyEvents = new TransparentPanel();
            this.SuspendLayout();
            // 
            // panelTiledContainer
            // 
            this.panelTiledContainer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelTiledContainer.BackColor = System.Drawing.Color.Black;
            this.panelTiledContainer.Location = new System.Drawing.Point(49, 41);
            this.panelTiledContainer.Margin = new System.Windows.Forms.Padding(0);
            this.panelTiledContainer.Name = "panelTiledContainer";
            this.panelTiledContainer.Size = new System.Drawing.Size(294, 215);
            this.panelTiledContainer.TabIndex = 0;
            // 
            // panelDummyEvents
            // 
            this.panelDummyEvents.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelDummyEvents.BackColor = System.Drawing.Color.Transparent;
            this.panelDummyEvents.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelDummyEvents.CausesValidation = false;
            this.panelDummyEvents.ForeColor = System.Drawing.Color.Transparent;
            this.panelDummyEvents.Location = new System.Drawing.Point(0, 0);
            this.panelDummyEvents.Margin = new System.Windows.Forms.Padding(0);
            this.panelDummyEvents.Name = "panelDummyEvents";
            this.panelDummyEvents.Size = new System.Drawing.Size(294, 215);
            this.panelDummyEvents.TabIndex = 1;
            // 
            // TiledPictureViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelDummyEvents);
            this.Controls.Add(this.panelTiledContainer);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TiledPictureViewControl";
            this.Size = new System.Drawing.Size(394, 301);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelTiledContainer;
        private TransparentPanel panelDummyEvents;

    }
}
