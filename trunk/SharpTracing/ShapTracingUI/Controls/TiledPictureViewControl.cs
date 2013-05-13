using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawEngine.Renderer.Util;
using DrawEngine.SharpTracingUI.Util;

namespace DrawEngine.SharpTracingUI.Controls
{
    /// <summary>
    /// Summary description for TransPanel.
    /// </summary>
    public class TransparentPanel : Panel
    {

        public TransparentPanel()
        {

        }


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            InvalidateEx();
        }
        protected void InvalidateEx()
        {
            if (Parent == null)
                return;

            Rectangle rc = new Rectangle(this.Location, this.Size);
            Parent.Invalidate(rc, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //do not allow the background to be painted 
        }




    }

    public partial class TiledPictureViewControl : UserControl
    {
        private Control[,] tilePanels;
        public TiledPictureViewControl()
        {
            InitializeComponent();
        }


        private TiledBitmap tiledBitmap;
        [Browsable(false)]
        public TiledBitmap TiledBitmap
        {
            get { return this.tiledBitmap; }
            set
            {
                if (value != null)
                {
                    this.tiledBitmap = value;
                    this.GenerateControlTiles();
                }

            }
        }


        private void GenerateControlTiles()
        {
            this.panelTiledContainer.Controls.Clear();
            this.panelDummyEvents.Width = this.panelTiledContainer.Width = this.tiledBitmap.Width;
            this.panelDummyEvents.Height = this.panelTiledContainer.Height = this.tiledBitmap.Height;
            //Array.Clear(this.tilePanels, 0, this.tilePanels.Length);
            this.tilePanels = null;
            this.tilePanels = new Control[this.tiledBitmap.TilesX, this.tiledBitmap.TilesY];
            int tileWidth = this.tiledBitmap.Width / this.tiledBitmap.TilesX;
            int tileHeight = this.tiledBitmap.Height / this.tiledBitmap.TilesY;

            this.CopyEvents<Control>(this.panelTiledContainer);
            this.CopyEvents<Control>(this.panelDummyEvents);

            for (int x = 0, relativeX = 0; x < this.tiledBitmap.TilesX; x++, relativeX += tileWidth)
            {
                for (int y = 0, relativeY = 0; y < this.tiledBitmap.TilesY; y++, relativeY += tileHeight)
                {
                    Control tilePanel = new Control
                    {
                        Anchor = AnchorStyles.None,
                        BackColor = Color.Black,
                        BackgroundImageLayout = ImageLayout.None,
                        //ErrorImage = null,
                        //InitialImage = null,
                        Location = new Point(relativeX, relativeY),
                        Margin = new Padding(0),
                        Size = new Size(tileWidth, tileHeight),
                        BackgroundImage = new Bitmap(tileWidth, tileHeight)
                    };


                    this.tilePanels[x, y] = tilePanel;
                    //this.tiledBitmap[x, y].Image = tilePanel.BackgroundImage as Bitmap;
                    this.tiledBitmap[x, y].RenderCycleCompleted += (tile, type) =>
                    {
                        if (type == TiledBitmap.RenderCycleType.Partial || type == TiledBitmap.RenderCycleType.Finish)
                        {
                            //tile.Graphics.Flush(FlushIntention.Sync);
                            //this.panelTiledContainer.Invalidate(true);
                            if (this.tilePanels[tile.XGridPosition, tile.YGridPosition].InvokeRequired)
                            {
                                this.tilePanels[tile.XGridPosition, tile.YGridPosition].Invoke(
                                    new Action(() =>
                                    {
                                        this.tilePanels[tile.XGridPosition, tile.YGridPosition]
                                            .BackgroundImage = tile.Image.Clone() as Bitmap;
                                    }));
                            }
                            else
                            {

                                //this.tilePanels[tile.XGridPosition, tile.XGridPosition].Refresh();
                                this.tilePanels[tile.XGridPosition, tile.YGridPosition]
                                                       .BackgroundImage = tile.Image.Clone() as Bitmap;
                            }
                        }
                    };
                    this.panelTiledContainer.Controls.Add(tilePanel);
                }
            }
            CenterControlInParent(this.panelDummyEvents);
            CenterControlInParent(this.panelTiledContainer);
        }


        protected override void OnResize(EventArgs e)
        {
            CenterControlInParent(this.panelDummyEvents);
            CenterControlInParent(this.panelTiledContainer);
            base.OnResize(e);
        }
        private void CenterControlInParent(Control ctrlToCenter)
        {
            ctrlToCenter.Left = (ctrlToCenter.Parent.Width - ctrlToCenter.Width) / 2;
            ctrlToCenter.Top = (ctrlToCenter.Parent.Height - ctrlToCenter.Height) / 2;
        }

    }
}
