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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Alsing.Windows.Forms.CoreLib
{
    public class SplitViewParentControl : BaseControl
    {
        protected SplitViewChildControl _ActiveView;
        private long _ticks; //splitter doubleclick timer
        public bool DisableScrollBars;
        protected bool DoOnce;
        protected SplitViewChildControl LowerLeft;
        protected SplitViewChildControl LowerRight;
        protected SplitViewControl splitView;
        protected SplitViewChildControl UpperLeft;
        protected SplitViewChildControl UpperRight;

        #region Private Properties
        private List<SplitViewChildControl> _Views;
        protected List<SplitViewChildControl> Views
        {
            get { return this._Views; }
            set { this._Views = value; }
        }
        #endregion

        public SplitViewParentControl()
        {
            this.OnCreate();
            this.InitializeComponent();
            this.InitializeComponentInternal();
            this.splitView.Resizing += this.SplitView_Resizing;
            this.splitView.HideLeft += this.SplitView_HideLeft;
            this.splitView.HideTop += this.SplitView_HideTop;
            this.LowerRight = this.GetNewView();
            this.LowerRight.AllowDrop = true;
            this.LowerRight.BorderColor = Color.White;
            this.LowerRight.BorderStyle = BorderStyle.None;
            this.LowerRight.Location = new Point(0, 0);
            this.LowerRight.Size = new Size(100, 100);
            this.Views = new List<SplitViewChildControl>();
            this.LowerRight.TopThumb.MouseDown += this.TopThumb_MouseDown;
            this.LowerRight.LeftThumb.MouseDown += this.LeftThumb_MouseDown;
            this.Views.Add(this.LowerRight);
            this.LowerRight.TopThumbVisible = true;
            this.LowerRight.LeftThumbVisible = true;
            this.splitView.Controls.Add(this.LowerRight);
            this.splitView.LowerRight = this.LowerRight;
            this.SplitView = true;
            this.ScrollBars = ScrollBars.Both;
            this.BorderStyle = BorderStyle.None;
            this.ChildBorderColor = SystemColors.ControlDark;
            this.ChildBorderStyle = BorderStyle.FixedSingle;
            this.BackColor = SystemColors.Window;
            this.Size = new Size(100, 100);
            this._ActiveView = this.LowerRight;
        }
        /// <summary>
        /// Gets or Sets the active view
        /// </summary>
        [Browsable(false)]
        public ActiveView ActiveView
        {
            get
            {
                if(this._ActiveView == this.UpperLeft){
                    return ActiveView.TopLeft;
                }
                if(this._ActiveView == this.UpperRight){
                    return ActiveView.TopRight;
                }
                if(this._ActiveView == this.LowerLeft){
                    return ActiveView.BottomLeft;
                }
                if(this._ActiveView == this.LowerRight){
                    return ActiveView.BottomRight;
                }
                return 0;
            }
            set
            {
                if(value != ActiveView.BottomRight){
                    this.ActivateSplits();
                }
                if(value == ActiveView.TopLeft){
                    this._ActiveView = this.UpperLeft;
                }
                if(value == ActiveView.TopRight){
                    this._ActiveView = this.UpperRight;
                }
                if(value == ActiveView.BottomLeft){
                    this._ActiveView = this.LowerLeft;
                }
                if(value == ActiveView.BottomRight){
                    this._ActiveView = this.LowerRight;
                }
            }
        }
        private void InitializeComponent() {}
        /// <summary>
        /// Resets the Splitview.
        /// </summary>
        public void ResetSplitview()
        {
            this.splitView.ResetSplitview();
        }
        private void SplitView_Resizing(object sender, EventArgs e)
        {
            this.LowerRight.TopThumbVisible = false;
            this.LowerRight.LeftThumbVisible = false;
        }
        private void SplitView_HideTop(object sender, EventArgs e)
        {
            this.LowerRight.TopThumbVisible = true;
        }
        private void SplitView_HideLeft(object sender, EventArgs e)
        {
            this.LowerRight.LeftThumbVisible = true;
        }
        protected virtual void ActivateSplits()
        {
            if(this.UpperLeft == null){
                this.UpperLeft = this.GetNewView();
                this.UpperRight = this.GetNewView();
                this.LowerLeft = this.GetNewView();
                this.splitView.Controls.AddRange(new Control[]{this.UpperLeft, this.LowerLeft, this.UpperRight});
                this.splitView.UpperRight = this.LowerLeft;
                this.splitView.UpperLeft = this.UpperLeft;
                this.splitView.LowerLeft = this.UpperRight;
                this.CreateViews();
            }
        }
        protected void TopThumb_MouseDown(object sender, MouseEventArgs e)
        {
            this.ActivateSplits();
            long t = DateTime.Now.Ticks - this._ticks;
            this._ticks = DateTime.Now.Ticks;
            if(t < 3000000){
                this.splitView.Split5050h();
            } else{
                this.splitView.InvokeMouseDownh();
            }
        }
        protected void LeftThumb_MouseDown(object sender, MouseEventArgs e)
        {
            this.ActivateSplits();
            long t = DateTime.Now.Ticks - this._ticks;
            this._ticks = DateTime.Now.Ticks;
            if(t < 3000000){
                this.splitView.Split5050v();
            } else{
                this.splitView.InvokeMouseDownv();
            }
        }
        protected virtual void OnCreate() {}
        protected virtual void CreateViews()
        {
            if(this.UpperRight != null){
                this.Views.Add(this.UpperRight);
                this.Views.Add(this.UpperLeft);
                this.Views.Add(this.LowerLeft);
            }
        }
        protected virtual SplitViewChildControl GetNewView()
        {
            return null;
        }
        protected void View_Enter(object sender, EventArgs e)
        {
            this._ActiveView = (SplitViewChildControl)sender;
        }
        protected void View_Leave(object sender, EventArgs e)
        {
            //	((EditViewControl)sender).RemoveFocus ();
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if(m.Msg == (int)WindowMessage.WM_SETFOCUS){
                if(this._ActiveView != null){
                    this._ActiveView.Focus();
                }
            }
        }

        #region PUBLIC PROPERTY SPLITVIEWV
        [Browsable(false)]
        public int SplitviewV
        {
            get { return this.splitView.SplitviewV; }
            set
            {
                if(this.splitView == null){
                    return;
                }
                this.splitView.SplitviewV = value;
            }
        }
        #endregion

        #region PUBLIC PROPERTY SPLITVIEWH
        [Browsable(false)]
        public int SplitviewH
        {
            get { return this.splitView.SplitviewH; }
            set
            {
                if(this.splitView == null){
                    return;
                }
                this.splitView.SplitviewH = value;
            }
        }
        #endregion

        #region public property ScrollBars
        private ScrollBars _ScrollBars;
        [Category("Appearance"), Description("Determines what Scrollbars should be visible"),
         DefaultValue(ScrollBars.Both)]
        public ScrollBars ScrollBars
        {
            get { return this._ScrollBars; }
            set
            {
                if(this._Views == null){
                    return;
                }
                if(this.DisableScrollBars){
                    value = ScrollBars.None;
                }
                foreach(SplitViewChildControl evc in this._Views){
                    evc.ScrollBars = value;
                }
                this._ScrollBars = value;
            }
        }
        #endregion

        #region public property SplitView
        //member variable
        private bool _SplitView;
        [Category("Appearance"), Description("Determines if the controls should use splitviews"), DefaultValue(true)]
        public bool SplitView
        {
            get { return this._SplitView; }
            set
            {
                this._SplitView = value;
                if(this.splitView == null){
                    return;
                }
                if(!this.SplitView){
                    this.splitView.Visible = false;
                    this.Controls.Add(this.LowerRight);
                    this.LowerRight.HideThumbs();
                    this.LowerRight.Dock = DockStyle.Fill;
                } else{
                    this.splitView.Visible = true;
                    this.splitView.LowerRight = this.LowerRight;
                    this.LowerRight.Dock = DockStyle.None;
                    this.LowerRight.ShowThumbs();
                }
            }
        }
        #endregion //END PROPERTY SplitView

        #region PUBLIC PROPERTY CHILDBODERSTYLE
        /// <summary>
        /// Gets or Sets the border styles of the split views.
        /// </summary>
        [Category("Appearance - Borders"), Description("Gets or Sets the border styles of the split views."),
         DefaultValue(BorderStyle.FixedSingle)]
        public BorderStyle ChildBorderStyle
        {
            get { return (this.Views[0]).BorderStyle; }
            set
            {
                foreach(SplitViewChildControl ev in this.Views){
                    ev.BorderStyle = value;
                }
            }
        }
        #endregion

        #region PUBLIC PROPERTY CHILDBORDERCOLOR
        /// <summary>
        /// Gets or Sets the border color of the split views.
        /// </summary>
        [Category("Appearance - Borders"), Description("Gets or Sets the border color of the split views."),
         DefaultValue(typeof(Color), "ControlDark")]
        public Color ChildBorderColor
        {
            get { return (this.Views[0]).BorderColor; }
            set
            {
                foreach(SplitViewChildControl ev in this.Views){
                    if(ev != null){
                        ev.BorderColor = value;
                    }
                }
            }
        }
        #endregion

        #region roger generated code
        private void InitializeComponentInternal()
        {
            this.splitView = new SplitViewControl();
            this.SuspendLayout();
            // 
            // splitView
            // 
            this.splitView.BackColor = Color.Empty;
            this.splitView.Dock = DockStyle.Fill;
            this.splitView.LowerLeft = null;
            this.splitView.LowerRight = null;
            this.splitView.Name = "splitView";
            this.splitView.Size = new Size(248, 216);
            this.splitView.SplitviewH = -4;
            this.splitView.SplitviewV = -4;
            this.splitView.TabIndex = 0;
            this.splitView.Text = "splitView";
            this.splitView.UpperLeft = null;
            this.splitView.UpperRight = null;
            // 
            // SplitViewParentControl
            // 
            this.Controls.AddRange(new Control[]{this.splitView});
            this.Name = "SplitViewParentControl";
            this.Size = new Size(248, 216);
            this.ResumeLayout(false);
        }
        #endregion
    }
}

namespace Alsing.Windows.Forms
{
    /// <summary>
    /// Represents which split view is currently active in the syntaxbox
    /// </summary>
    public enum ActiveView
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    }
}