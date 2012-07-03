using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal abstract class InertButtonBase : Control
    {
        private bool m_isMouseOver = false;
        protected InertButtonBase()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
        }
        public abstract Bitmap Image { get; }
        protected bool IsMouseOver
        {
            get { return this.m_isMouseOver; }
            private set
            {
                if(this.m_isMouseOver == value){
                    return;
                }
                this.m_isMouseOver = value;
                this.Invalidate();
            }
        }
        protected override Size DefaultSize
        {
            get { return Resources.DockPane_Close.Size; }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            bool over = this.ClientRectangle.Contains(e.X, e.Y);
            if(this.IsMouseOver != over){
                this.IsMouseOver = over;
            }
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if(!this.IsMouseOver){
                this.IsMouseOver = true;
            }
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if(this.IsMouseOver){
                this.IsMouseOver = false;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if(this.IsMouseOver && this.Enabled){
                using(Pen pen = new Pen(this.ForeColor)){
                    e.Graphics.DrawRectangle(pen, Rectangle.Inflate(this.ClientRectangle, -1, -1));
                }
            }
            using(ImageAttributes imageAttributes = new ImageAttributes()){
                ColorMap[] colorMap = new ColorMap[2];
                colorMap[0] = new ColorMap();
                colorMap[0].OldColor = Color.FromArgb(0, 0, 0);
                colorMap[0].NewColor = this.ForeColor;
                colorMap[1] = new ColorMap();
                colorMap[1].OldColor = this.Image.GetPixel(0, 0);
                colorMap[1].NewColor = Color.Transparent;
                imageAttributes.SetRemapTable(colorMap);
                e.Graphics.DrawImage(this.Image, new Rectangle(0, 0, this.Image.Width, this.Image.Height), 0, 0,
                                     this.Image.Width, this.Image.Height, GraphicsUnit.Pixel, imageAttributes);
            }
            base.OnPaint(e);
        }
        public void RefreshChanges()
        {
            if(this.IsDisposed){
                return;
            }
            bool mouseOver = this.ClientRectangle.Contains(this.PointToClient(MousePosition));
            if(mouseOver != this.IsMouseOver){
                this.IsMouseOver = mouseOver;
            }
            this.OnRefreshChanges();
        }
        protected virtual void OnRefreshChanges() {}
    }
}