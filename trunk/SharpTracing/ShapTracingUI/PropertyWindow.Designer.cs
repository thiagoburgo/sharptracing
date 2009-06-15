using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace DrawEngine.SharpTracingUI {
    partial class PropertyWindow {
        private IContainer components;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyWindow));
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(0, 23);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(255, 403);
            this.propertyGrid.TabIndex = 0;
            // 
            // menuItem1
            // 
            this.menuItem1.Index = -1;
            this.menuItem1.Text = "";
            // 
            // comboBox
            // 
            this.comboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.Location = new System.Drawing.Point(0, 2);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(255, 21);
            this.comboBox.TabIndex = 2;
            this.comboBox.Visible = false;
            this.comboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);
            // 
            // PropertyWindow
            // 
            this.ClientSize = new System.Drawing.Size(255, 428);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.comboBox);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PropertyWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight;
            this.TabText = "Properties";
            this.Text = "Properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropertyWindow_FormClosing);
            this.ResumeLayout(false);

        }
        #endregion
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
