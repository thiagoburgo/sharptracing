using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawEngine.SharpTracingUI
{
    public partial class RenderControl : UserControl
    {
        private static readonly object lockObject = new Object();
        private volatile Image image;
        public RenderControl()
        {
            this.InitializeComponent();
            this.image = new Bitmap(this.Width, this.Height);
            this.AdjustFormScrollbars(true);
        }
        public Image Image
        {
            get
            {
                lock(lockObject){
                    return this.image;
                }
            }
            set
            {
                lock(lockObject){
                    this.image = value;
                    if(!this.InvokeRequired){
                        this.Refresh();
                    }
                }
            }
        }
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            lock(lockObject){
                e.Graphics.DrawImageUnscaled(this.image, this.Location);
            }
            base.OnPaint(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        public override void Refresh()
        {
            base.Refresh();
        }
    }
}