using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PictureBoxScroll {
    public partial class PicturePanel : Panel {
        private Bitmap image;

        public PicturePanel() : base() {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.Opaque, true);
        }

        public Bitmap Image {
            get { return this.image; }
            set {
                this.image = value;
                this.Refresh();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e) {}

        protected override void OnPaint(PaintEventArgs pe) {
            //these settings aren't even needed for good perf, but they are helpful
            pe.Graphics.InterpolationMode = InterpolationMode.Low;
            pe.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            pe.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            if (this.image != null) {
                //draw empty area, if it exists, in an optimized way.
                if (this.AutoScrollPosition.X == 0) {
                    int emptyRightAreaWidth = this.Width - this.image.Width;
                    if (emptyRightAreaWidth > 0) {
                        Rectangle fillRect = new Rectangle(this.image.Width, 0, emptyRightAreaWidth, this.Height);
                        fillRect.Intersect(pe.ClipRectangle);
                        pe.Graphics.FillRectangle(SystemBrushes.Control, fillRect);
                    }
                }
                if (this.AutoScrollPosition.Y == 0) {
                    int emptyRightAreaHeight = this.Height - this.image.Height;
                    if (emptyRightAreaHeight > 0) {
                        Rectangle fillRect = new Rectangle(0, this.image.Height, this.Width, emptyRightAreaHeight);
                        fillRect.Intersect(pe.ClipRectangle);
                        pe.Graphics.FillRectangle(SystemBrushes.Control, fillRect);
                    }
                }
                //calculate the visible area of the bitmap
                Rectangle bitmapRect = new Rectangle(this.AutoScrollPosition.X, this.AutoScrollPosition.Y,
                                                     this.image.Width, this.image.Height);
                Rectangle visibleClientRect = bitmapRect;
                visibleClientRect.Intersect(pe.ClipRectangle);
                if (visibleClientRect.Width == 0 || visibleClientRect.Height == 0) {
                    return;
                }
                Rectangle visibleBitmapRect = visibleClientRect;
                visibleBitmapRect.Offset(-this.AutoScrollPosition.X, -this.AutoScrollPosition.Y);
                pe.Graphics.DrawImage(this.image, visibleClientRect, visibleBitmapRect, GraphicsUnit.Pixel);
            } else //if no bitmap just fill with background color
            {
                pe.Graphics.FillRectangle(SystemBrushes.Control, pe.ClipRectangle);
            }
        }

        public void QuickUpdate(Rectangle rect) {
            this.OnPaint(new PaintEventArgs(this.CreateGraphics(), rect));
        }

        public void SetPixel(Brush brush, int bmpX, int bmpY) {
            using (Graphics g = this.CreateGraphics()) {
                g.FillRectangle(brush, new Rectangle(bmpX, bmpY, 1, 1));
            }
        }
    }
}