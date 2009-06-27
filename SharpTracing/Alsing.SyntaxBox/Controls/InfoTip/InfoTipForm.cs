// *
// * Copyright (C) 2008 Roger Alsing : http://www.RogerAlsing.com
// *
// * This library is free software; you can redistribute it and/or modify it
// * under the terms of the GNU Lesser General Public License 2.1 or later, as
// * published by the Free Software Foundation. See the included license.txt
// * or http://www.gnu.org/copyleft/lesser.html for details.
// *
// *
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Alsing.Windows.Forms.CoreLib;

namespace Alsing.Windows.Forms
{
    /// <summary>
    /// Summary description for InfoTip.
    /// </summary>
    public class InfoTipForm : Form
    {
        private WeakReference _Control;
        private int _Count = 1;
        private int _SelectedIndex;
        private PictureBox btnNext;
        private PictureBox btnPrev;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components;
        private FormatLabelControl InfoText;
        private Label lblIndex;
        private Panel panel1;
        private Panel panel2;
        private PictureBox picIcon;
        private Panel pnlImage;
        private Panel pnlSelect;
        /// <summary>
        /// 
        /// </summary>
        public InfoTipForm()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        public InfoTipForm(Control parent)
        {
            this.ParentControl = parent;
            if(this.CreateParams != null){
                this.CreateParams.ClassName = "tooltips_class32";
            }
            this.InitializeComponent();
        }
        private Control ParentControl
        {
            get { return this._Control != null ? (Control)this._Control.Target : null; }
            set { this._Control = new WeakReference(value); }
        }
        public int SelectedIndex
        {
            get { return this._SelectedIndex; }
            set
            {
                if(value > this._Count){
                    value = 1;
                }
                if(value < 1){
                    value = this._Count;
                }
                this._SelectedIndex = value;
                this.OnSelectedIndexChanged();
                this.SetPos();
            }
        }
        public int Count
        {
            get { return this._Count; }
            set { this._Count = value; }
        }
        public Image Image
        {
            get { return this.picIcon.Image; }
            set
            {
                this.picIcon.Image = value;
                if(value == null){
                    this.pnlImage.Visible = false;
                } else{
                    this.pnlImage.Visible = true;
                    this.pnlImage.Width = this.Image.Width + 6;
                    this.picIcon.Size = this.Image.Size;
                }
                this.DoResize();
            }
        }
        public string Data
        {
            get { return this.InfoText.Text; }
            set { this.InfoText.Text = value; }
        }
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int message, int _data, int _id);
        public event EventHandler SelectedIndexChanged = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if(disposing){
                if(this.components != null){
                    this.components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        private void InfoText_Resize(object sender, EventArgs e)
        {
            this.DoResize();
        }
        private void DoResize()
        {
            int w = this.InfoText.Left + this.InfoText.Width + 8;
            if(this.Count > 1){
                w += this.pnlSelect.Width;
            }
            if(this.picIcon.Image != null){
                w += this.pnlImage.Width;
            }
            int h = this.InfoText.Top + this.InfoText.Height + 6;
            if(this.Image != null && this.Image.Height + this.picIcon.Top * 2 > h){
                h = this.Image.Height + this.picIcon.Top * 2;
            }
            this.ClientSize = new Size(w, h);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            this.SelectedIndex = 1;
            this.SetPos();
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            this.SelectedIndex++;
            this.SetPos();
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            this.SelectedIndex--;
            this.SetPos();
        }
        private void btnPrev_DoubleClick(object sender, EventArgs e)
        {
            this.SelectedIndex--;
            this.SetPos();
        }
        private void btnNext_DoubleClick(object sender, EventArgs e)
        {
            this.SelectedIndex++;
            this.SetPos();
        }
        private void SetPos()
        {
            if(this.Count == 1){
                this.pnlSelect.Visible = false;
            } else{
                this.pnlSelect.Visible = true;
            }
            this.DoResize();
            this.lblIndex.Text = this.SelectedIndex.ToString((CultureInfo.InvariantCulture)) + " of "
                                 + this.Count.ToString(CultureInfo.InvariantCulture);
            if(this.ParentControl != null){
                this.ParentControl.Focus();
            }
        }
        private void InfoTipForm_Enter(object sender, EventArgs e)
        {
            this.ParentControl.Focus();
        }
        private void InfoText_Enter(object sender, EventArgs e)
        {
            this.ParentControl.Focus();
        }
        private void OnSelectedIndexChanged()
        {
            if(this.SelectedIndexChanged != null){
                this.SelectedIndexChanged(this, null);
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(InfoTipForm));
            this.pnlSelect = new System.Windows.Forms.Panel();
            this.btnNext = new System.Windows.Forms.PictureBox();
            this.btnPrev = new System.Windows.Forms.PictureBox();
            this.lblIndex = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.InfoText = new Alsing.Windows.Forms.CoreLib.FormatLabelControl();
            this.pnlImage = new System.Windows.Forms.Panel();
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlSelect.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlImage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSelect
            // 
            this.pnlSelect.Controls.AddRange(new System.Windows.Forms.Control[]
                                             {this.btnNext, this.btnPrev, this.lblIndex});
            this.pnlSelect.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSelect.DockPadding.All = 4;
            this.pnlSelect.Location = new System.Drawing.Point(32, 0);
            this.pnlSelect.Name = "pnlSelect";
            this.pnlSelect.Size = new System.Drawing.Size(80, 35);
            this.pnlSelect.TabIndex = 0;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.SystemColors.Control;
            this.btnNext.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnNext.Image")));
            this.btnNext.Location = new System.Drawing.Point(68, 6);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(9, 11);
            this.btnNext.TabIndex = 1;
            this.btnNext.TabStop = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnNext.DoubleClick += new System.EventHandler(this.btnNext_DoubleClick);
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.SystemColors.Control;
            this.btnPrev.Image = ((System.Drawing.Bitmap)(resources.GetObject("btnPrev.Image")));
            this.btnPrev.Location = new System.Drawing.Point(4, 6);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(9, 11);
            this.btnPrev.TabIndex = 0;
            this.btnPrev.TabStop = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            this.btnPrev.DoubleClick += new System.EventHandler(this.btnPrev_DoubleClick);
            // 
            // lblIndex
            // 
            this.lblIndex.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblIndex.Location = new System.Drawing.Point(4, 4);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(72, 23);
            this.lblIndex.TabIndex = 2;
            this.lblIndex.Text = "20 of 20";
            this.lblIndex.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel2
            // 
            this.panel2.Controls.AddRange(new System.Windows.Forms.Control[]{this.InfoText});
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.DockPadding.All = 4;
            this.panel2.Location = new System.Drawing.Point(112, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(126, 35);
            this.panel2.TabIndex = 1;
            // 
            // InfoText
            // 
            this.InfoText.AutoSizeHorizontal = true;
            this.InfoText.AutoSizeVertical = true;
            this.InfoText.BackColor = System.Drawing.SystemColors.Info;
            this.InfoText.BorderColor = System.Drawing.Color.Black;
            this.InfoText.BorderStyle = Alsing.Windows.Forms.BorderStyle.None;
            this.InfoText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular,
                                                         System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.InfoText.ImageList = null;
            this.InfoText.Link_Color = System.Drawing.Color.Blue;
            this.InfoText.Link_Color_Hover = System.Drawing.Color.Blue;
            this.InfoText.Link_UnderLine = false;
            this.InfoText.Link_UnderLine_Hover = true;
            this.InfoText.Location = new System.Drawing.Point(2, 4);
            this.InfoText.Name = "InfoText";
            this.InfoText.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.InfoText.Size = new System.Drawing.Size(59, 13);
            this.InfoText.TabIndex = 0;
            this.InfoText.Text = "format <b>label</b>";
            this.InfoText.WordWrap = false;
            this.InfoText.Resize += new System.EventHandler(this.InfoText_Resize);
            this.InfoText.Enter += new System.EventHandler(this.InfoText_Enter);
            // 
            // pnlImage
            // 
            this.pnlImage.Controls.AddRange(new System.Windows.Forms.Control[]{this.picIcon});
            this.pnlImage.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(32, 35);
            this.pnlImage.TabIndex = 2;
            this.pnlImage.Visible = false;
            // 
            // picIcon
            // 
            this.picIcon.Location = new System.Drawing.Point(5, 3);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(19, 20);
            this.picIcon.TabIndex = 1;
            this.picIcon.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.AddRange(new System.Windows.Forms.Control[]{this.panel2, this.pnlSelect, this.pnlImage});
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 37);
            this.panel1.TabIndex = 3;
            // 
            // InfoTipForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ClientSize = new System.Drawing.Size(240, 37);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[]{this.panel1});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "InfoTipForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Enter += new System.EventHandler(this.InfoTipForm_Enter);
            this.pnlSelect.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.pnlImage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
    }
}