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
using System.Windows.Forms;

namespace Alsing.Windows.Forms.CoreLib
{
    /// <summary>
    /// Summary description for SplitView.
    /// </summary>
    public class SplitViewControl : Control
    {
        private Control _LowerLeft;
        private Control _LowerRight;
        private Control _UpperLeft;
        private Control _UpperRight;
        private SizeAction Action = 0;
        private Panel Center;
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private Container components;
        private bool FirstTime;
        private Panel Horizontal;
        private Point PrevPos = new Point(0);
        private Point StartPos = new Point(0, 0);
        private Panel Vertical;
        /// <summary>
        /// Default constructor for the splitview control
        /// </summary>
        public SplitViewControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            this.SetStyle(ControlStyles.ContainerControl, true);
            //			this.SetStyle(ControlStyles.AllPaintingInWmPaint ,false);
            //			this.SetStyle(ControlStyles.DoubleBuffer ,false);
            //			this.SetStyle(ControlStyles.Selectable,true);
            //			this.SetStyle(ControlStyles.ResizeRedraw ,true);
            //			this.SetStyle(ControlStyles.Opaque ,true);			
            //			this.SetStyle(ControlStyles.UserPaint,true);
            //SetStyle(ControlStyles.Selectable ,true);
            this.InitializeComponent();
            this.DoResize();
            this.ReSize(0, 0);
            //this.Refresh ();
        }
        /// <summary>
        /// Gets or Sets the control that should be confined to the upper left view.
        /// </summary>
        public Control UpperLeft
        {
            get { return this._UpperLeft; }
            set
            {
                this._UpperLeft = value;
                this.DoResize();
            }
        }
        /// <summary>
        /// Gets or Sets the control that should be confined to the upper right view.
        /// </summary>
        public Control UpperRight
        {
            get { return this._UpperRight; }
            set
            {
                this._UpperRight = value;
                this.DoResize();
            }
        }
        /// <summary>
        /// Gets or Sets the control that should be confined to the lower left view.
        /// </summary>
        public Control LowerLeft
        {
            get { return this._LowerLeft; }
            set
            {
                this._LowerLeft = value;
                this.DoResize();
            }
        }
        /// <summary>
        /// Gets or Sets the control that should be confined to the lower right view.
        /// </summary>
        public Control LowerRight
        {
            get { return this._LowerRight; }
            set
            {
                this._LowerRight = value;
                this.DoResize();
            }
        }
        public int SplitviewV
        {
            get { return this.Vertical.Left; }
            set
            {
                this.Vertical.Left = value;
                this.DoResize();
            }
        }
        public int SplitviewH
        {
            get { return this.Horizontal.Top; }
            set
            {
                this.Horizontal.Top = value;
                this.DoResize();
            }
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Vertical = new System.Windows.Forms.Panel();
            this.Horizontal = new System.Windows.Forms.Panel();
            this.Center = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Vertical
            // 
            this.Vertical.BackColor = System.Drawing.SystemColors.Control;
            this.Vertical.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.Vertical.Name = "Vertical";
            this.Vertical.Size = new System.Drawing.Size(4, 264);
            this.Vertical.TabIndex = 0;
            this.Vertical.Resize += new System.EventHandler(this.Vertical_Resize);
            this.Vertical.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Vertical_MouseUp);
            this.Vertical.DoubleClick += new System.EventHandler(this.Vertical_DoubleClick);
            this.Vertical.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Vertical_MouseMove);
            this.Vertical.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Vertical_MouseDown);
            // 
            // Horizontal
            // 
            this.Horizontal.BackColor = System.Drawing.SystemColors.Control;
            this.Horizontal.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.Horizontal.Name = "Horizontal";
            this.Horizontal.Size = new System.Drawing.Size(320, 4);
            this.Horizontal.TabIndex = 1;
            this.Horizontal.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Horizontal_MouseUp);
            this.Horizontal.DoubleClick += new System.EventHandler(this.Horizontal_DoubleClick);
            this.Horizontal.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Horizontal_MouseMove);
            this.Horizontal.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Horizontal_MouseDown);
            // 
            // Center
            // 
            this.Center.BackColor = System.Drawing.SystemColors.Control;
            this.Center.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.Center.Location = new System.Drawing.Point(146, 69);
            this.Center.Name = "Center";
            this.Center.Size = new System.Drawing.Size(24, 24);
            this.Center.TabIndex = 2;
            this.Center.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Center_MouseUp);
            this.Center.DoubleClick += new System.EventHandler(this.Center_DoubleClick);
            this.Center.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Center_MouseMove);
            this.Center.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Center_MouseDown);
            // 
            // SplitViewControl
            // 
            this.BackColor = System.Drawing.Color.Magenta;
            this.Controls.AddRange(new System.Windows.Forms.Control[]{this.Center, this.Horizontal, this.Vertical});
            this.Size = new System.Drawing.Size(200, 200);
            this.VisibleChanged += new System.EventHandler(this.SplitViewControl_VisibleChanged);
            this.Enter += new System.EventHandler(this.SplitViewControl_Enter);
            this.Leave += new System.EventHandler(this.SplitViewControl_Leave);
            this.ResumeLayout(false);
        }
        #endregion

        /// <summary>
        /// an event fired when the split view is resized.
        /// </summary>
        public event EventHandler Resizing = null;
        /// <summary>
        /// an event fired when the top views are hidden.
        /// </summary>
        public event EventHandler HideTop = null;
        /// <summary>
        /// an event fired when the left views are hidden.
        /// </summary>
        public event EventHandler HideLeft = null;
        private void OnResizing()
        {
            if(this.Resizing != null){
                this.Resizing(this, new EventArgs());
            }
        }
        private void OnHideLeft()
        {
            if(this.HideLeft != null){
                this.HideLeft(this, new EventArgs());
            }
        }
        private void OnHideTop()
        {
            if(this.HideTop != null){
                this.HideTop(this, new EventArgs());
            }
        }
        //		protected override void OnLoad(EventArgs e)
        //		{
        //			
        //		}
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
#if DEBUG
            try
            {
                Console.WriteLine("disposing splitview");
            }
            catch {}
#endif
            if(disposing){
                if(this.components != null){
                    this.components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            this.DoResize();
        }
        private void DoResize()
        {
            //			int OldWidth=Horizontal.Width ;
            //			int OldHeight=Vertical.Height;
            int NewHeight = this.Height;
            int NewWidth = this.Width;
            if(NewHeight != 0 && NewWidth != 0){
                this.SuspendLayout();
                //				Horizontal.Top = (int)(NewHeight*HorizontalPos);
                //				Vertical.Left =(int)(NewWidth*VerticalPos);
                //
                //				int CenterY=(Horizontal.Top+Horizontal.Height /2)-Center.Height/2;
                //				int CenterX=(Vertical.Left+Vertical.Width /2)-Center.Width /2;
                //
                //				Center.Location =new Point (CenterX,CenterY);
                //ReSize (0,0);
                this.ReSize2();
                this.OnResizing();
                if(this.Horizontal.Top < 15){
                    this.Horizontal.Top = 0 - this.Horizontal.Height;
                    this.OnHideTop();
                }
                if(this.Vertical.Left < 15){
                    this.Vertical.Left = 0 - this.Vertical.Width;
                    this.OnHideLeft();
                }
                this.Horizontal.Width = this.Width;
                this.Vertical.Height = this.Height;
                this.Horizontal.SendToBack();
                this.Vertical.SendToBack();
                this.Horizontal.BackColor = SystemColors.Control;
                this.Vertical.BackColor = SystemColors.Control;
                //this.SendToBack ();
                int RightLeft = this.Vertical.Left + this.Vertical.Width;
                int RightLowerTop = this.Horizontal.Top + this.Horizontal.Height;
                int RightWidth = this.Width - RightLeft;
                int LowerHeight = this.Height - RightLowerTop;
                int UpperHeight = this.Horizontal.Top;
                int LeftWidth = this.Vertical.Left;
                if(this.LowerRight != null){
                    this.LowerRight.BringToFront();
                    this.LowerRight.SetBounds(RightLeft, RightLowerTop, RightWidth, LowerHeight);
                }
                if(this.UpperRight != null){
                    this.UpperRight.BringToFront();
                    this.UpperRight.SetBounds(RightLeft, 0, RightWidth, UpperHeight);
                }
                if(this.LowerLeft != null){
                    this.LowerLeft.BringToFront();
                    this.LowerLeft.SetBounds(0, RightLowerTop, LeftWidth, LowerHeight);
                }
                if(this.UpperLeft != null){
                    this.UpperLeft.BringToFront();
                    this.UpperLeft.SetBounds(0, 0, LeftWidth, UpperHeight);
                }
                this.ResumeLayout(); //ggf
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlAdded(ControlEventArgs e)
        {
            this.DoResize();
        }
        private void Vertical_Resize(object sender, EventArgs e) {}
        private void Horizontal_MouseDown(object sender, MouseEventArgs e)
        {
            this.Action = SizeAction.SizeH;
            this.StartPos = new Point(e.X, e.Y);
            this.Horizontal.BringToFront();
            this.Horizontal.BackColor = SystemColors.ControlDark;
            this.FirstTime = true;
        }
        private void Vertical_MouseDown(object sender, MouseEventArgs e)
        {
            this.Action = SizeAction.SizeV;
            this.StartPos = new Point(e.X, e.Y);
            this.Vertical.BringToFront();
            this.Vertical.BackColor = SystemColors.ControlDark;
            this.FirstTime = true;
        }
        private void Center_MouseDown(object sender, MouseEventArgs e)
        {
            this.Action = SizeAction.SizeA;
            this.StartPos = new Point(e.X, e.Y);
            this.Vertical.BringToFront();
            this.Horizontal.BringToFront();
            this.Vertical.BackColor = SystemColors.ControlDark;
            this.Horizontal.BackColor = SystemColors.ControlDark;
            this.FirstTime = true;
        }
        private void Horizontal_MouseUp(object sender, MouseEventArgs e)
        {
            int xDiff = 0;
            int yDiff = this.StartPos.Y - e.Y;
            //	StartPos=new Point (e.X,e.Y);
            this.ReSize(xDiff, yDiff);
            this.Action = SizeAction.None;
            this.DoResize();
        }
        private void Vertical_MouseUp(object sender, MouseEventArgs e)
        {
            int xDiff = this.StartPos.X - e.X;
            int yDiff = 0;
            //	StartPos=new Point (e.X,e.Y);
            this.ReSize(xDiff, yDiff);
            this.Action = SizeAction.None;
            this.DoResize();
            this.Refresh();
        }
        private void Center_MouseUp(object sender, MouseEventArgs e)
        {
            int xDiff = this.StartPos.X - e.X;
            int yDiff = this.StartPos.Y - e.Y;
            //	StartPos=new Point (e.X,e.Y);
            this.ReSize(xDiff, yDiff);
            this.Action = SizeAction.None;
            this.DoResize();
        }
        private void Horizontal_MouseMove(object sender, MouseEventArgs e)
        {
            if(this.Action == SizeAction.SizeH && e.Button == MouseButtons.Left){
                Point start;
                int x = e.X;
                int y = e.Y;
                if(y + this.Horizontal.Top > this.Height - 4){
                    y = this.Height - 4 - this.Horizontal.Top;
                }
                if(y + this.Horizontal.Top < 0){
                    y = 0 - this.Horizontal.Top;
                }
                if(!this.FirstTime){
                    start = this.PointToScreen(this.Location);
                    start.Y += this.PrevPos.Y + this.Horizontal.Location.Y;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3), Color.Black);
                } else{
                    this.FirstTime = false;
                }
                start = this.PointToScreen(this.Location);
                start.Y += y + this.Horizontal.Location.Y;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3), Color.Black);
                this.PrevPos = new Point(x, y);
            }
        }
        private void Vertical_MouseMove(object sender, MouseEventArgs e)
        {
            if(this.Action == SizeAction.SizeV && e.Button == MouseButtons.Left){
                Point start;
                int x = e.X;
                int y = e.Y;
                if(x + this.Vertical.Left > this.Width - 4){
                    x = this.Width - 4 - this.Vertical.Left;
                }
                if(x + this.Vertical.Left < 0){
                    x = 0 - this.Vertical.Left;
                }
                if(!this.FirstTime){
                    start = this.PointToScreen(this.Location);
                    start.X += this.PrevPos.X + this.Vertical.Location.X;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);
                } else{
                    this.FirstTime = false;
                }
                start = this.PointToScreen(this.Location);
                start.X += x + this.Vertical.Location.X;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);
                this.PrevPos = new Point(x, y);
            }
        }
        private void Center_MouseMove(object sender, MouseEventArgs e)
        {
            if(this.Action == SizeAction.SizeA && e.Button == MouseButtons.Left){
                Point start;
                int x = e.X - this.Center.Width / 2;
                int y = e.Y - this.Center.Height / 2;
                // ROB: Added fix for graphics splatter when sizing both splitters.
                if(y + this.Horizontal.Top > this.Height - 4){
                    y = this.Height - 4 - this.Horizontal.Top;
                }
                if(y + this.Horizontal.Top < 0){
                    y = 0 - this.Horizontal.Top;
                }
                if(x + this.Vertical.Left > this.Width - 4){
                    x = this.Width - 4 - this.Vertical.Left;
                }
                if(x + this.Vertical.Left < 0){
                    x = 0 - this.Vertical.Left;
                }
                // END-ROB
                if(!this.FirstTime){
                    start = this.PointToScreen(this.Location);
                    start.X += this.PrevPos.X + this.Vertical.Location.X;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);
                    start = this.PointToScreen(this.Location);
                    start.Y += this.PrevPos.Y + this.Horizontal.Location.Y;
                    ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3),
                                                         SystemColors.ControlDark);
                } else{
                    this.FirstTime = false;
                }
                start = this.PointToScreen(this.Location);
                start.X += x + this.Vertical.Location.X;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, 3, this.Height), Color.Black);
                start = this.PointToScreen(this.Location);
                start.Y += y + this.Horizontal.Location.Y;
                ControlPaint.FillReversibleRectangle(new Rectangle(start.X, start.Y, this.Width, 3),
                                                     SystemColors.ControlDark);
                this.PrevPos = new Point(x, y);
            }
        }
        private void ReSize(int x, int y)
        {
            //if (x==0 && y==0)
            //	return;
            this.SuspendLayout();
            int xx = this.Vertical.Left - x;
            int yy = this.Horizontal.Top - y;
            if(xx < 0){
                xx = 0;
            }
            if(yy < 0){
                yy = 0;
            }
            if(yy > this.Height - this.Horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3){
                yy = this.Height - this.Horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3;
            }
            if(xx > this.Width - this.Vertical.Width - SystemInformation.VerticalScrollBarWidth * 3){
                xx = this.Width - this.Vertical.Width - SystemInformation.VerticalScrollBarWidth * 3;
            }
            if(xx != this.Vertical.Left){
                this.Vertical.Left = xx;
            }
            if(yy != this.Horizontal.Top){
                this.Horizontal.Top = yy;
            }
            int CenterY = (this.Horizontal.Top + this.Horizontal.Height / 2) - this.Center.Height / 2;
            int CenterX = (this.Vertical.Left + this.Vertical.Width / 2) - this.Center.Width / 2;
            this.Center.Location = new Point(CenterX, CenterY);
            this.ResumeLayout();
            this.Invalidate();
            try{
                if(this.UpperLeft != null){
                    this.UpperLeft.Refresh();
                }
                if(this.UpperLeft != null){
                    this.UpperLeft.Refresh();
                }
                if(this.UpperLeft != null){
                    this.UpperLeft.Refresh();
                }
                if(this.UpperLeft != null){
                    this.UpperLeft.Refresh();
                }
            } catch{}
            this.OnResizing();
            //DoResize();	
            //this.Refresh ();
        }
        private void ReSize2()
        {
            int xx = this.Vertical.Left;
            int yy = this.Horizontal.Top;
            if(xx < 0){
                xx = 0;
            }
            if(yy < 0){
                yy = 0;
            }
            if(yy > this.Height - this.Horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3){
                yy = this.Height - this.Horizontal.Height - SystemInformation.VerticalScrollBarWidth * 3;
                if(yy != this.Horizontal.Top){
                    this.Horizontal.Top = yy;
                }
            }
            if(xx > this.Width - this.Vertical.Width - SystemInformation.VerticalScrollBarWidth * 3){
                xx = this.Width - this.Vertical.Width - SystemInformation.VerticalScrollBarWidth * 3;
                if(xx != this.Vertical.Left){
                    this.Vertical.Left = xx;
                }
            }
            int CenterY = (this.Horizontal.Top + this.Horizontal.Height / 2) - this.Center.Height / 2;
            int CenterX = (this.Vertical.Left + this.Vertical.Width / 2) - this.Center.Width / 2;
            this.Center.Location = new Point(CenterX, CenterY);
        }
        private void Center_DoubleClick(object sender, EventArgs e)
        {
            this.Horizontal.Top = -100;
            this.Vertical.Left = -100;
            this.DoResize();
        }
        private void Vertical_DoubleClick(object sender, EventArgs e)
        {
            this.Vertical.Left = -100;
            this.DoResize();
        }
        private void Horizontal_DoubleClick(object sender, EventArgs e)
        {
            this.Horizontal.Top = -100;
            this.DoResize();
        }
        /// <summary>
        /// Splits the view horiziontally.
        /// </summary>
        public void Split5050h()
        {
            this.Horizontal.Top = this.Height / 2;
            this.DoResize();
        }
        /// <summary>
        /// Splits teh view vertically.
        /// </summary>
        public void Split5050v()
        {
            this.Vertical.Left = this.Width / 2;
            this.DoResize();
        }
        public void ResetSplitview()
        {
            this.Vertical.Left = 0;
            this.Horizontal.Top = 0;
            this.DoResize();
        }
        /// <summary>
        /// Start dragging the horizontal splitter.
        /// </summary>
        public void InvokeMouseDownh()
        {
            IntPtr hwnd = this.Horizontal.Handle;
            NativeMethods.SendMessage(hwnd, (int)WindowMessage.WM_LBUTTONDOWN, 0, 0);
        }
        /// <summary>
        /// Start dragging the vertical splitter.
        /// </summary>
        public void InvokeMouseDownv()
        {
            IntPtr hwnd = this.Vertical.Handle;
            NativeMethods.SendMessage(hwnd, (int)WindowMessage.WM_LBUTTONDOWN, 0, 0);
        }
        private void SplitViewControl_Leave(object sender, EventArgs e) {}
        private void SplitViewControl_Enter(object sender, EventArgs e) {}
        private void SplitViewControl_VisibleChanged(object sender, EventArgs e) {}

        #region Nested type: SizeAction
        private enum SizeAction
        {
            None = 0,
            SizeH = 1,
            SizeV = 2,
            SizeA = 3
        }
        #endregion
    }
}