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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Alsing.Windows.Forms.SyntaxBox
{
    /// <summary>
    /// Summary description for AutoListForm.
    /// </summary>
    [ToolboxItem(false)]
    public class AutoListForm : Form
    {
        private IContainer components;
        /// <summary>
        /// The imagelist that should be used by the AutoListForm
        /// </summary>
        public ImageList Images;
        private TabListBox LB;
        private ToolTip tooltip;
        /// <summary>
        /// Default AltoListControl constructor.
        /// </summary>
        public AutoListForm()
        {
            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();
            //SetStyle(ControlStyles.ContainerControl  ,false);
            this.SetStyle(ControlStyles.Selectable, true);
            // TODO: Add any initialization after the InitForm call
        }
        /// <summary>
        /// Gets the "insert text" from the selected item.
        /// </summary>
        public string SelectedText
        {
            get
            {
                if(this.LB.SelectedItem == null){
                    return "";
                }
                var li = (ListItem)this.LB.SelectedItem;
                return li.InsertText;
            }
        }
        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int message, int _data, int _id);
        public void SendKey(int KeyCode)
        {
            SendMessage(this.LB.Handle, (int)WindowMessage.WM_KEYDOWN, KeyCode, 0);
        }
        /// <summary>		
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(SystemColors.Control);
            ControlPaint.DrawBorder3D(e.Graphics, 0, 0, this.Width, this.Height, Border3DStyle.Raised);
        }
        public void SelectItem(string text)
        {
            text = text.ToLowerInvariant();
            for(int i = 0; i < this.LB.Items.Count; i++){
                var li = (ListItem)this.LB.Items[i];
                string lis = li.Text.ToLowerInvariant();
                if(lis.StartsWith(text)){
                    this.LB.SelectedIndex = i;
                    break;
                }
            }
        }
        private void LB_KeyDown(object sender, KeyEventArgs e)
        {
            this.OnKeyDown(e);
            //	e.Handled =true;
        }
        private void LB_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
            //	e.Handled =true;
        }
        private void LB_KeyUp(object sender, KeyEventArgs e)
        {
            this.OnKeyUp(e);
            //	e.Handled =true;
        }
        /// <summary>
        /// For public use only.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }
        /// <summary>
        /// For public use only.
        /// </summary>
        /// <param name="charCode"></param>
        /// <returns></returns>
        protected override bool IsInputChar(char charCode)
        {
            return true;
        }
        /// <summary>
        /// Adds a new ListItem to the AutoListForm.
        /// </summary>
        /// <param name="text">Text of the new ListItem</param>
        /// <param name="ImageIndex">Image index that should be assigned to the new ListItem</param>
        /// <returns></returns>
        public ListItem Add(string text, int ImageIndex)
        {
            return this.Add(text, text, ImageIndex);
        }
        /// <summary>
        /// Adds a new ListItem to the AutoListForm.
        /// </summary>
        /// <param name="text">Text of the new ListItem</param>
        /// <param name="InsertText">text to insert when this item is selected</param>
        /// <param name="ImageIndex">Image index that should be assigned to the new ListItem</param>
        /// <returns></returns>
        public ListItem Add(string text, string InsertText, int ImageIndex)
        {
            var li = new ListItem(text, ImageIndex, "", InsertText);
            this.LB.Items.Add(li);
            //this.LB.Sorted =true;
            return li;
        }
        public ListItem Add(string text, string InsertText, string ToolTip, int ImageIndex)
        {
            var li = new ListItem(text, ImageIndex, "", InsertText);
            this.LB.Items.Add(li);
            li.ToolTip = ToolTip;
            //this.LB.Sorted =true;
            return li;
        }
        /// <summary>
        /// Clears the content of the AutoList.
        /// </summary>
        public void Clear()
        {
            this.LB.Items.Clear();
        }
        private void LB_DrawItem(object sender, DrawItemEventArgs e)
        {
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            if(e.Index == - 1){
                return;
            }
            const int Offset = 24;
            var li = (ListItem)this.LB.Items[e.Index];
            string text = li.Text;
            Brush bg, fg;
            if(selected){
                bg = SystemBrushes.Highlight;
                fg = SystemBrushes.HighlightText;
                //fg=Brushes.Black;
            } else{
                bg = SystemBrushes.Window;
                fg = SystemBrushes.WindowText;
                //bg=Brushes.White;
                //fg=Brushes.Black;
            }
            if(!selected){
                e.Graphics.FillRectangle(bg, 0, e.Bounds.Top, e.Bounds.Width, this.LB.ItemHeight);
                //e.Graphics.FillRectangle (SystemBrushes.Highlight,0,e.Bounds.Top,27 ,LB.ItemHeight); 
            } else{
                e.Graphics.FillRectangle(SystemBrushes.Window, Offset, e.Bounds.Top, e.Bounds.Width - Offset,
                                         this.LB.ItemHeight);
                e.Graphics.FillRectangle(SystemBrushes.Highlight,
                                         new Rectangle(Offset + 1, e.Bounds.Top + 1, e.Bounds.Width - Offset - 2,
                                                       this.LB.ItemHeight - 2));
                //e.Graphics.FillRectangle (SystemBrushes.Highlight,27,e.Bounds.Top,e.Bounds.Width-27 ,LB.ItemHeight); 
                //e.Graphics.FillRectangle (new SolidBrush(Color.FromArgb (182,189,210)),new Rectangle (1+27,e.Bounds.Top+1,e.Bounds.Width-2- ,LB.ItemHeight-2));
                ControlPaint.DrawFocusRectangle(e.Graphics,
                                                new Rectangle(Offset, e.Bounds.Top, e.Bounds.Width - Offset,
                                                              this.LB.ItemHeight));
            }
            e.Graphics.DrawString(text, e.Font, fg, Offset + 2, e.Bounds.Top + 1);
            if(this.Images != null){
                e.Graphics.DrawImage(this.Images.Images[li.Type], 6, e.Bounds.Top + 0);
            }
        }
        private void LB_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }
        public void BeginLoad()
        {
            this.LB.Sorted = false;
            this.LB.DrawMode = DrawMode.Normal;
            this.LB.SuspendLayout();
        }
        public void EndLoad()
        {
            this.LB.ResumeLayout();
            this.LB.Sorted = true;
            this.LB.DrawMode = DrawMode.OwnerDrawFixed;
            //set height
            this.Height = 0;
            if(this.LB.Items.Count > 10){
                this.Height = this.LB.ItemHeight * 11 + 12;
            } else{
                this.Height = this.LB.ItemHeight * (this.LB.Items.Count) + 12;
            }
            int max = 0;
            Graphics g = this.LB.CreateGraphics();
            foreach(ListItem li in this.LB.Items){
                int w = (int)g.MeasureString(li.Text, this.LB.Font).Width + 45;
                if(w > max){
                    max = w;
                }
            }
            this.Width = max + SystemInformation.VerticalScrollBarWidth;
            this.Refresh();
            g.Dispose();
        }
        private void AutoListForm_Resize(object sender, EventArgs e)
        {
            this.LB.Size = new Size(this.Width - 8, this.Height - 8);
        }
        private void LB_SelectedIndexChanged(object sender, EventArgs e)
        {
            var li = (ListItem)this.LB.SelectedItem;
            if(li.ToolTip != ""){
                this.tooltip.ShowAlways = true;
                this.tooltip.SetToolTip(this.LB, li.ToolTip);
                this.tooltip.InitialDelay = 2;
                this.tooltip.Active = true;
            }
        }
        private void LB_MouseDown(object sender, MouseEventArgs e)
        {
            this.SelectItem(e.X, e.Y);
        }
        private void SelectItem(int x, int y)
        {
            var p = new Point(x, y);
            int r = (p.Y / this.LB.ItemHeight) + this.LB.TopIndex;
            if(r != this.LB.SelectedIndex){
                if(r < this.LB.Items.Count && r >= 0){
                    this.LB.SelectedIndex = r;
                }
            }
        }
        private void LB_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button != 0){
                this.SelectItem(e.X, e.Y);
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.LB = new Alsing.Windows.Forms.SyntaxBox.TabListBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // LB
            // 
            this.LB.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.LB.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular,
                                                   System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
            this.LB.IntegralHeight = false;
            this.LB.ItemHeight = 16;
            this.LB.Location = new System.Drawing.Point(4, 4);
            this.LB.Name = "LB";
            this.LB.Size = new System.Drawing.Size(168, 184);
            this.LB.Sorted = true;
            this.LB.TabIndex = 0;
            this.LB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LB_KeyDown);
            this.LB.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LB_MouseDown);
            this.LB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LB_KeyPress);
            this.LB.DoubleClick += new System.EventHandler(this.LB_DoubleClick);
            this.LB.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LB_KeyUp);
            this.LB.SelectedIndexChanged += new System.EventHandler(this.LB_SelectedIndexChanged);
            this.LB.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LB_MouseMove);
            this.LB.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.LB_DrawItem);
            // 
            // tooltip
            // 
            this.tooltip.AutoPopDelay = 5000;
            this.tooltip.InitialDelay = 100;
            this.tooltip.ReshowDelay = 100;
            // 
            // AutoListForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(168, 165);
            this.Controls.AddRange(new System.Windows.Forms.Control[]{this.LB});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AutoListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Resize += new System.EventHandler(this.AutoListForm_Resize);
            this.ResumeLayout(false);
        }
        #endregion
    }
}