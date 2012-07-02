using System.Windows.Forms;
namespace TooboxUI.Components {
    partial class ChooseToolboxItemsDialog {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listViewComponents = new System.Windows.Forms.ListView();
            this.name = new System.Windows.Forms.ColumnHeader();
            this._namespace = new System.Windows.Forms.ColumnHeader();
            this.assembly = new System.Windows.Forms.ColumnHeader();
            this.version = new System.Windows.Forms.ColumnHeader();
            this.chooseTab = new System.Windows.Forms.TabControl();
            this.gacComponents = new System.Windows.Forms.TabPage();
            this.lblFilteringStatus = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupType = new System.Windows.Forms.GroupBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.componentIcon = new System.Windows.Forms.PictureBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.btnChoose = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chooseTab.SuspendLayout();
            this.gacComponents.SuspendLayout();
            this.groupType.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.componentIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewComponents
            // 
            this.listViewComponents.AllowColumnReorder = true;
            this.listViewComponents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this._namespace,
            this.assembly,
            this.version});
            this.listViewComponents.Dock = System.Windows.Forms.DockStyle.Top;
            this.listViewComponents.FullRowSelect = true;
            this.listViewComponents.Location = new System.Drawing.Point(3, 3);
            this.listViewComponents.Name = "listViewComponents";
            this.listViewComponents.Size = new System.Drawing.Size(565, 197);
            this.listViewComponents.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewComponents.TabIndex = 6;
            this.listViewComponents.UseCompatibleStateImageBehavior = false;
            this.listViewComponents.View = System.Windows.Forms.View.Details;
            this.listViewComponents.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewComponents_MouseDoubleClick);
            this.listViewComponents.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listViewComponents.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listViewComponents.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewComponents_KeyUp);
            // 
            // name
            // 
            this.name.Text = "Name";
            this.name.Width = 194;
            // 
            // _namespace
            // 
            this._namespace.Text = "Namespace";
            this._namespace.Width = 201;
            // 
            // assembly
            // 
            this.assembly.Text = "Assembly";
            this.assembly.Width = 204;
            // 
            // version
            // 
            this.version.Text = "Version";
            this.version.Width = 98;
            // 
            // chooseTab
            // 
            this.chooseTab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chooseTab.Controls.Add(this.gacComponents);
            this.chooseTab.Location = new System.Drawing.Point(0, 5);
            this.chooseTab.Name = "chooseTab";
            this.chooseTab.SelectedIndex = 0;
            this.chooseTab.Size = new System.Drawing.Size(579, 341);
            this.chooseTab.TabIndex = 7;
            // 
            // gacComponents
            // 
            this.gacComponents.Controls.Add(this.listViewComponents);
            this.gacComponents.Controls.Add(this.lblFilteringStatus);
            this.gacComponents.Controls.Add(this.label4);
            this.gacComponents.Controls.Add(this.groupType);
            this.gacComponents.Controls.Add(this.txtFilter);
            this.gacComponents.Controls.Add(this.btnChoose);
            this.gacComponents.Location = new System.Drawing.Point(4, 22);
            this.gacComponents.Name = "gacComponents";
            this.gacComponents.Padding = new System.Windows.Forms.Padding(3);
            this.gacComponents.Size = new System.Drawing.Size(571, 315);
            this.gacComponents.TabIndex = 0;
            this.gacComponents.Text = ".NET Framework Components";
            this.gacComponents.UseVisualStyleBackColor = true;
            // 
            // lblFilteringStatus
            // 
            this.lblFilteringStatus.AutoSize = true;
            this.lblFilteringStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblFilteringStatus.Location = new System.Drawing.Point(492, 209);
            this.lblFilteringStatus.Name = "lblFilteringStatus";
            this.lblFilteringStatus.Size = new System.Drawing.Size(52, 13);
            this.lblFilteringStatus.TabIndex = 12;
            this.lblFilteringStatus.Text = "Filtering...";
            this.lblFilteringStatus.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 209);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Filter: ";
            // 
            // groupType
            // 
            this.groupType.Controls.Add(this.lblVersion);
            this.groupType.Controls.Add(this.lblLanguage);
            this.groupType.Controls.Add(this.label2);
            this.groupType.Controls.Add(this.label1);
            this.groupType.Controls.Add(this.componentIcon);
            this.groupType.Location = new System.Drawing.Point(17, 232);
            this.groupType.Name = "groupType";
            this.groupType.Size = new System.Drawing.Size(469, 74);
            this.groupType.TabIndex = 8;
            this.groupType.TabStop = false;
            this.groupType.Text = "Type";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.ForeColor = System.Drawing.Color.Blue;
            this.lblVersion.Location = new System.Drawing.Point(140, 44);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(42, 13);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = "Version";
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.ForeColor = System.Drawing.Color.Blue;
            this.lblLanguage.Location = new System.Drawing.Point(140, 26);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(48, 13);
            this.lblLanguage.TabIndex = 3;
            this.lblLanguage.Text = "Invariant";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Version: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(73, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Language: ";
            // 
            // componentIcon
            // 
            this.componentIcon.Location = new System.Drawing.Point(13, 26);
            this.componentIcon.Name = "componentIcon";
            this.componentIcon.Size = new System.Drawing.Size(37, 31);
            this.componentIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.componentIcon.TabIndex = 0;
            this.componentIcon.TabStop = false;
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(66, 206);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(420, 20);
            this.txtFilter.TabIndex = 10;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // btnChoose
            // 
            this.btnChoose.Location = new System.Drawing.Point(492, 237);
            this.btnChoose.Name = "btnChoose";
            this.btnChoose.Size = new System.Drawing.Size(75, 23);
            this.btnChoose.TabIndex = 9;
            this.btnChoose.Text = "Choose...";
            this.btnChoose.UseVisualStyleBackColor = true;
            this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(211, 351);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 13;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(292, 351);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ChooseToolboxItemsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 386);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.chooseTab);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ChooseToolboxItemsDialog";
            this.ShowInTaskbar = false;
            this.Text = "Choose Toolbox Items...";
            this.chooseTab.ResumeLayout(false);
            this.gacComponents.ResumeLayout(false);
            this.gacComponents.PerformLayout();
            this.groupType.ResumeLayout(false);
            this.groupType.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.componentIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListView listViewComponents;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader _namespace;
        private System.Windows.Forms.ColumnHeader assembly;
        private System.Windows.Forms.ColumnHeader version;
        private ListViewItemComparer _lvwItemComparer;
        private TabControl chooseTab;
        private TabPage gacComponents;
        private GroupBox groupType;
        private TextBox txtFilter;
        private Button btnChoose;
        private Label label2;
        private Label label1;
        private PictureBox componentIcon;
        private Label lblVersion;
        private Label lblLanguage;
        private Label label4;
        private Label lblFilteringStatus;
        private Button btnOk;
        private Button btnCancel;

    }
}

