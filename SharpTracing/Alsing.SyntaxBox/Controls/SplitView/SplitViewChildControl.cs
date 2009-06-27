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
    /// Summary description for SplitViewChildControl.
    /// </summary>
    public class SplitViewChildControl : BaseControl
    {
        private Container components;
        private Panel Filler;
        protected HScrollBar hScroll;
        public ThumbControl LeftThumb;
        public ThumbControl TopThumb;
        protected VScrollBar vScroll;
        public SplitViewChildControl()
        {
            this.InitializeComponent();
            //			Alsing.Windows.NativeMethods.OpenThemeData (this.Handle,"EDIT");
            //			Alsing.Windows.NativeMethods.OpenThemeData (this.vScroll.Handle,"SCROLLBAR");
            //			Alsing.Windows.NativeMethods.OpenThemeData (this.hScroll.Handle,"SCROLLBAR");
        }
        protected Rectangle ClientArea
        {
            get
            {
                Rectangle r = this.ClientRectangle;
                r.Width -= this.vScroll.Width;
                r.Height -= this.hScroll.Height;
                return r;
            }
        }
        /// <summary>
        /// Gets or Sets if the Left side thumb control is visible or not.
        /// </summary>
        public bool LeftThumbVisible
        {
            get { return this.LeftThumb.Visible; }
            set
            {
                this.LeftThumb.Visible = value;
                this.DoResize();
            }
        }
        /// <summary>
        /// Getd ot Sets if the Top thumb control is visible or not.
        /// </summary>
        public bool TopThumbVisible
        {
            get { return this.TopThumb.Visible; }
            set
            {
                this.TopThumb.Visible = value;
                this.DoResize();
            }
        }
        [Browsable(false)]
        public int VisibleClientHeight
        {
            get
            {
                if(this.hScroll.Visible){
                    return this.ClientHeight - this.hScroll.Height;
                } else{
                    return this.ClientHeight;
                }
            }
        }
        [Browsable(false)]
        public int VisibleClientWidth
        {
            get
            {
                if(this.hScroll.Visible){
                    return this.ClientWidth - this.vScroll.Width;
                } else{
                    return this.ClientWidth;
                }
            }
        }

        #region public property ScrollBars
        private ScrollBars _ScrollBars;
        private bool HasThumbs;
        public ScrollBars ScrollBars
        {
            get { return this._ScrollBars; }
            set
            {
                this._ScrollBars = value;
                if(this.ScrollBars == ScrollBars.Both){
                    this.hScroll.Visible = true;
                    this.vScroll.Visible = true;
                }
                if(this.ScrollBars == ScrollBars.None){
                    this.hScroll.Visible = false;
                    this.vScroll.Visible = false;
                }
                if(this.ScrollBars == ScrollBars.Horizontal){
                    this.hScroll.Visible = true;
                    this.vScroll.Visible = false;
                }
                if(this.ScrollBars == ScrollBars.Vertical){
                    this.hScroll.Visible = false;
                    this.vScroll.Visible = true;
                }
                this.Filler.Visible = this.hScroll.Visible & this.vScroll.Visible;
                if(this.vScroll.Visible && this.HasThumbs){
                    this.TopThumb.Height = 8;
                } else{
                    this.TopThumb.Height = 0;
                }
                if(this.hScroll.Visible && this.HasThumbs){
                    this.LeftThumb.Width = 8;
                } else{
                    this.LeftThumb.Width = 0;
                }
                this.DoResize();
                this.Refresh();
            }
        }
        public void HideThumbs()
        {
            this.TopThumb.Height = 0;
            this.LeftThumb.Width = 0;
            this.HasThumbs = false;
            this.DoResize();
        }
        public void ShowThumbs()
        {
            this.TopThumb.Height = 8;
            this.LeftThumb.Width = 8;
            this.HasThumbs = true;
            this.DoResize();
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if(disposing){
                if(this.components != null){
                    this.components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.DoResize();
        }
        private void DoResize()
        {
            try{
                if(this.TopThumb == null){
                    return;
                }
                this.TopThumb.Width = SystemInformation.VerticalScrollBarWidth;
                this.LeftThumb.Height = SystemInformation.HorizontalScrollBarHeight;
                this.vScroll.Width = SystemInformation.VerticalScrollBarWidth;
                this.hScroll.Height = SystemInformation.HorizontalScrollBarHeight;
                if(this.TopThumbVisible){
                    this.vScroll.Top = this.TopThumb.Height;
                    if(this.hScroll.Visible){
                        this.vScroll.Height = this.ClientHeight - this.hScroll.Height - this.TopThumb.Height;
                    } else{
                        this.vScroll.Height = this.ClientHeight - this.TopThumb.Height;
                    }
                } else{
                    if(this.hScroll.Visible){
                        this.vScroll.Height = this.ClientHeight - this.hScroll.Height;
                    } else{
                        this.vScroll.Height = this.ClientHeight;
                    }
                    this.vScroll.Top = 0;
                }
                if(this.LeftThumbVisible){
                    this.hScroll.Left = this.LeftThumb.Width;
                    if(this.vScroll.Visible){
                        this.hScroll.Width = this.ClientWidth - this.vScroll.Width - this.LeftThumb.Width;
                    } else{
                        this.hScroll.Width = this.ClientWidth - this.LeftThumb.Width;
                    }
                } else{
                    if(this.vScroll.Visible){
                        this.hScroll.Width = this.ClientWidth - this.vScroll.Width;
                    } else{
                        this.hScroll.Width = this.ClientWidth;
                    }
                    this.hScroll.Left = 0;
                }
                this.vScroll.Left = this.ClientWidth - this.vScroll.Width;
                this.hScroll.Top = this.ClientHeight - this.hScroll.Height;
                this.LeftThumb.Left = 0;
                this.LeftThumb.Top = this.hScroll.Top;
                this.TopThumb.Left = this.vScroll.Left;
                ;
                this.TopThumb.Top = 0;
                this.Filler.Left = this.vScroll.Left;
                this.Filler.Top = this.hScroll.Top;
                this.Filler.Width = this.vScroll.Width;
                this.Filler.Height = this.hScroll.Height;
            } catch{}
        }

        #region Component Designer generated code
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(SplitViewChildControl));
            this.hScroll = new System.Windows.Forms.HScrollBar();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.Filler = new System.Windows.Forms.Panel();
            this.TopThumb = new Alsing.Windows.Forms.CoreLib.ThumbControl();
            this.LeftThumb = new Alsing.Windows.Forms.CoreLib.ThumbControl();
            this.SuspendLayout();
            // 
            // hScroll
            // 
            this.hScroll.Location = new System.Drawing.Point(-4, 292);
            this.hScroll.Name = "hScroll";
            this.hScroll.Size = new System.Drawing.Size(440, 16);
            this.hScroll.TabIndex = 0;
            // 
            // vScroll
            // 
            this.vScroll.Location = new System.Drawing.Point(440, 0);
            this.vScroll.Maximum = 300;
            this.vScroll.Minimum = 0;
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(16, 360);
            this.vScroll.TabIndex = 1;
            // 
            // Filler
            // 
            this.Filler.BackColor = System.Drawing.SystemColors.Control;
            this.Filler.Location = new System.Drawing.Point(64, 260);
            this.Filler.Name = "Filler";
            this.Filler.Size = new System.Drawing.Size(20, 20);
            this.Filler.TabIndex = 3;
            // 
            // TopThumb
            // 
            this.TopThumb.BackColor = System.Drawing.SystemColors.Control;
            this.TopThumb.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.TopThumb.Location = new System.Drawing.Point(101, 17);
            this.TopThumb.Name = "TopThumb";
            this.TopThumb.Size = new System.Drawing.Size(16, 8);
            this.TopThumb.TabIndex = 3;
            this.TopThumb.Visible = false;
            // 
            // LeftThumb
            // 
            this.LeftThumb.BackColor = System.Drawing.SystemColors.Control;
            this.LeftThumb.Cursor = System.Windows.Forms.Cursors.VSplit;
            this.LeftThumb.Location = new System.Drawing.Point(423, 17);
            this.LeftThumb.Name = "LeftThumb";
            this.LeftThumb.Size = new System.Drawing.Size(8, 16);
            this.LeftThumb.TabIndex = 3;
            this.LeftThumb.Visible = false;
            // 
            // SplitViewChildControl
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[]
                                   {this.TopThumb, this.LeftThumb, this.Filler, this.vScroll, this.hScroll});
            this.Name = "SplitViewChildControl";
            this.Size = new System.Drawing.Size(456, 376);
            this.ResumeLayout(false);
        }
        #endregion
    }
}