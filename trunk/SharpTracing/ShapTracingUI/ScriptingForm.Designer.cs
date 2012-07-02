namespace DrawEngine.SharpTracingUI
{
    partial class ScriptingForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptingForm));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.syntaxBoxControl1 = new Alsing.Windows.Forms.SyntaxBoxControl();
            this.syntaxDocument1 = new Alsing.SourceCode.SyntaxDocument(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonRun = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripCbScriptList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.syntaxBoxControl1);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(665, 445);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(665, 470);
            this.toolStripContainer1.TabIndex = 2;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // syntaxBoxControl1
            // 
            this.syntaxBoxControl1.ActiveView = Alsing.Windows.Forms.ActiveView.BottomRight;
            this.syntaxBoxControl1.AutoListPosition = null;
            this.syntaxBoxControl1.AutoListSelectedText = "a123";
            this.syntaxBoxControl1.AutoListVisible = false;
            this.syntaxBoxControl1.BackColor = System.Drawing.Color.White;
            this.syntaxBoxControl1.BorderStyle = Alsing.Windows.Forms.BorderStyle.None;
            this.syntaxBoxControl1.CopyAsRTF = false;
            this.syntaxBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syntaxBoxControl1.Document = this.syntaxDocument1;
            this.syntaxBoxControl1.FontName = "Courier new";
            this.syntaxBoxControl1.HighLightActiveLine = true;
            this.syntaxBoxControl1.HighLightedLineColor = System.Drawing.Color.PaleTurquoise;
            this.syntaxBoxControl1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.syntaxBoxControl1.Indent = Alsing.Windows.Forms.SyntaxBox.IndentStyle.Scope;
            this.syntaxBoxControl1.InfoTipCount = 1;
            this.syntaxBoxControl1.InfoTipPosition = null;
            this.syntaxBoxControl1.InfoTipSelectedIndex = 1;
            this.syntaxBoxControl1.InfoTipVisible = false;
            this.syntaxBoxControl1.Location = new System.Drawing.Point(0, 0);
            this.syntaxBoxControl1.LockCursorUpdate = false;
            this.syntaxBoxControl1.Name = "syntaxBoxControl1";
            this.syntaxBoxControl1.ShowTabGuides = true;
            this.syntaxBoxControl1.Size = new System.Drawing.Size(665, 445);
            this.syntaxBoxControl1.SmoothScroll = false;
            this.syntaxBoxControl1.SplitviewH = -4;
            this.syntaxBoxControl1.SplitviewV = -4;
            this.syntaxBoxControl1.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(219)))), ((int)(((byte)(214)))));
            this.syntaxBoxControl1.TabIndex = 0;
            this.syntaxBoxControl1.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            // 
            // syntaxDocument1
            // 
            this.syntaxDocument1.Lines = new string[] {
        ""};
            this.syntaxDocument1.MaxUndoBufferSize = 1000;
            this.syntaxDocument1.Modified = false;
            this.syntaxDocument1.SyntaxLanguage = Alsing.SourceCode.SyntaxLanguages.CSharp;
            this.syntaxDocument1.UndoStep = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRun,
            this.toolStripSeparator1,
            this.toolStripCbScriptList});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(665, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripButtonRun
            // 
            this.toolStripButtonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRun.Image = global::DrawEngine.SharpTracingUI.Properties.Resources.render;
            this.toolStripButtonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRun.Name = "toolStripButtonRun";
            this.toolStripButtonRun.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRun.Text = "Run";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripCbScriptList
            // 
            this.toolStripCbScriptList.Name = "toolStripCbScriptList";
            this.toolStripCbScriptList.Size = new System.Drawing.Size(121, 25);
            // 
            // ScriptingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 470);
            this.Controls.Add(this.toolStripContainer1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptingForm";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.TabText = "Scripting";
            this.Text = "Scripting";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Alsing.SourceCode.SyntaxDocument syntaxDocument1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRun;
        private Alsing.Windows.Forms.SyntaxBoxControl syntaxBoxControl1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripComboBox toolStripCbScriptList;
    }
}