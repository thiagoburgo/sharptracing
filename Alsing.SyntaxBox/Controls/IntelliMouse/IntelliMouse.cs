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
using System.IO;
using System.Windows.Forms;
using Alsing.SyntaxBox.Properties;
using ScrollEventArgs=Alsing.Windows.Forms.IntelliMouse.ScrollEventArgs;
using ScrollEventHandler=Alsing.Windows.Forms.IntelliMouse.ScrollEventHandler;

namespace Alsing.Windows.Forms.CoreLib
{
    /// <summary>
    /// Summary description for IntelliMouseControl.
    /// </summary>
    public class IntelliMouseControl : Control
    {
        protected const int WM_CAPTURECHANGED = 0x0215;
        protected const int WM_LBUTTONDOWN = 0x0201;
        protected const int WM_MBUTTONDOWN = 0x0207;
        protected const int WM_MBUTTONUP = 0x0208;
        protected const int WM_MOUSELEAVE = 0x02A3;
        protected const int WM_RBUTTONDOWN = 0x0204;
        protected bool Active;
        protected IContainer components;
        private bool IsDirty;

        #region GENERAL DECLARATIONS
        protected WeakReference _CurrentParent;
        protected Point ActivationPoint = new Point(0, 0);
        protected Point CurrentDelta = new Point(0, 0);
        protected Control CurrentParent
        {
            get
            {
                if(this._CurrentParent != null){
                    return (Control)this._CurrentParent.Target;
                }
                return null;
            }
            set { this._CurrentParent = new WeakReference(value); }
        }
        #endregion

        #region EVENTS
        public event EventHandler BeginScroll = null;
        public event EventHandler EndScroll = null;
        public event ScrollEventHandler Scroll = null;
        #endregion

        #region PUBLIC PROPERTY IMAGE
        protected Bitmap _Image;
        protected PictureBox picImage;
        protected RegionHandler regionHandler1;
        protected Timer tmrFeedback;
        public Bitmap Image
        {
            get { return this._Image; }
            set
            {
                this._Image = value;
                this.IsDirty = true;
            }
        }
        #endregion

        #region PUBLIC PROPERTY TRANSPARENCYKEY
        protected Color _TransparencyKey = Color.FromArgb(255, 0, 255);
        public Color TransparencyKey
        {
            get { return this._TransparencyKey; }
            set
            {
                this._TransparencyKey = value;
                this.IsDirty = true;
            }
        }
        #endregion

        #region CONSTRUCTOR
        public IntelliMouseControl()
        {
            this.InitializeComponent();
            //			SetStyle(ControlStyles.Selectable,false);			
            //			this.Image = (Bitmap)this.picImage.Image;
            //			this.Visible =false;
        }
        #endregion

        #region DISPOSE
        protected override void Dispose(bool disposing)
        {
#if DEBUG
            try
            {
                Console.WriteLine("disposing intellimouse");
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
        #endregion

        #region FINALIZE
        ~IntelliMouseControl()
        {
#if DEBUG
            try
            {
                Console.WriteLine("finalizing intellimouse");
            }
            catch {}
#endif
        }
        #endregion

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        protected void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources = new System.Resources.ResourceManager(typeof(IntelliMouseControl));
            this.tmrFeedback = new Timer();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.regionHandler1 = new Alsing.Windows.Forms.CoreLib.RegionHandler(this.components);
            // 
            // tmrFeedback
            // 
            this.tmrFeedback.Enabled = true;
            this.tmrFeedback.Interval = 10;
            this.tmrFeedback.Tick += new System.EventHandler(this.tmrFeedback_Tick);
            // 
            // picImage
            // 
            this.picImage.Image = ((System.Drawing.Bitmap)(resources.GetObject("picImage.Image")));
            this.picImage.Location = new System.Drawing.Point(17, 17);
            this.picImage.Name = "picImage";
            this.picImage.TabIndex = 0;
            this.picImage.TabStop = false;
            // 
            // regionHandler1
            // 
            this.regionHandler1.Control = null;
            this.regionHandler1.MaskImage = null;
            this.regionHandler1.TransparencyKey = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(0)),
                                                                                ((System.Byte)(255)));
            // 
            // IntelliMouseControl
            // 
            this.ParentChanged += new System.EventHandler(this.IntelliMouseControl_ParentChanged);
        }
        #endregion

        protected void CreateRegion()
        {
            this.regionHandler1.ApplyRegion(this, this.Image, this.TransparencyKey);
            this.IsDirty = false;
        }
        public void Activate(int x, int y)
        {
            if(this.IsDirty){
                this.CreateRegion();
            }
            this.Size = new Size(this.Image.Width, this.Image.Height);
            this.Location = new Point(x - this.Image.Width / 2, y - this.Image.Height / 2);
            this.ActivationPoint.X = x;
            this.ActivationPoint.Y = y;
            this.BringToFront();
            this.Visible = true;
            this.Focus();
            this.Active = false;
            Application.DoEvents();
            this.SetCursor(0, 0);
            this.tmrFeedback.Enabled = true;
            this.onBeginScroll(new EventArgs());
            NativeMethods.SendMessage(this.Handle, WM_MBUTTONDOWN, 0, 0);
            this.Active = true;
        }
        protected void SetCursor(int x, int y)
        {
            int dY = y;
            int dX = x;
            this.CurrentDelta.X = dX;
            this.CurrentDelta.Y = dY;
            if(dY > 16){
                var ms = new MemoryStream(Resources.MoveDown);
                this.Cursor = new Cursor(ms);
                this.CurrentDelta.Y -= 16;
            } else if(dY < -16){
                var ms = new MemoryStream(Resources.MoveUp);
                this.Cursor = new Cursor(ms);
                this.CurrentDelta.Y += 16;
            } else{
                var ms = new MemoryStream(Resources.MoveUpDown);
                this.Cursor = new Cursor(ms);
                this.CurrentDelta = new Point(0, 0);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if(this.Active){
                if(e.Button != MouseButtons.None && (e.Button != MouseButtons.Middle && e.X != 0 && e.Y != 0)){
                    this.Deactivate();
                    var p = new Point(e.X + this.Left, e.Y + this.Top);
                    NativeMethods.SendMessage(this.Parent.Handle, WM_LBUTTONDOWN, 0, p.Y * 0x10000 + p.X);
                }
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if(this.Active){
                if(e.Button != MouseButtons.Middle && e.Button != MouseButtons.None){
                    this.Deactivate();
                } else{
                    int x = e.X;
                    int y = e.Y;
                    x -= this.Image.Width / 2;
                    y -= this.Image.Height / 2;
                    this.SetCursor(x, y);
                    NativeMethods.SendMessage(this.Handle, WM_MBUTTONDOWN, 0, 0);
                }
            } else{
                base.OnMouseMove(e);
            }
        }
        protected void Deactivate()
        {
            NativeMethods.SendMessage(this.Handle, WM_MBUTTONUP, 0, 0);
            this.Active = false;
            this.tmrFeedback.Enabled = false;
            this.Hide();
            this.onEndScroll(new EventArgs());
            this.Parent.Focus();
        }
        protected override void OnResize(EventArgs e)
        {
            this.Size = this.Image != null ? new Size(this.Image.Width, this.Image.Height) : new Size(32, 32);
        }
        protected void Parent_MouseDown(object s, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Middle){
                this.Activate(e.X, e.Y);
            }
        }
        protected void tmrFeedback_Tick(object sender, EventArgs e)
        {
            var a = new ScrollEventArgs{DeltaX = this.CurrentDelta.X, DeltaY = this.CurrentDelta.Y};
            this.onScroll(a);
        }
        protected virtual void onBeginScroll(EventArgs e)
        {
            if(this.BeginScroll != null){
                this.BeginScroll(this, e);
            }
        }
        protected virtual void onEndScroll(EventArgs e)
        {
            if(this.EndScroll != null){
                this.EndScroll(this, e);
            }
        }
        protected virtual void onScroll(ScrollEventArgs e)
        {
            if(this.Scroll != null){
                this.Scroll(this, e);
            }
        }
        protected void IntelliMouseControl_ParentChanged(object sender, EventArgs e)
        {
            if(this.CurrentParent != null){
                this.CurrentParent.MouseDown -= this.Parent_MouseDown;
            }
            if(this.Parent != null){
                this.Parent.MouseDown += this.Parent_MouseDown;
                this.Deactivate();
            }
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            this.Deactivate();
        }
        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.Deactivate();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            this.Deactivate();
        }
        protected override void WndProc(ref Message m)
        {
            if(m.Msg == WM_MOUSELEAVE){
                base.WndProc(ref m);
                this.Deactivate();
                return;
            }
            base.WndProc(ref m);
        }
    }
}