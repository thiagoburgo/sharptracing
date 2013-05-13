using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.Util
{
    public class TiledBitmap : IDisposable, IEnumerable<TiledBitmap.Tile>
    {
        private Tile[,] tiles;
        public TiledBitmap(int tilesX, int tilesY, int totalWidth, int totalHeight)
        {
            this.TilesX = tilesX;
            this.TilesY = tilesY;
            this.Width = totalWidth;
            this.Height = totalHeight;
            this.tiles = new Tile[tilesX, tilesY];
            this.GenerateTiles();
        }

        private void GenerateTiles()
        {
            int tileWidth = this.Width / this.TilesX;
            int tileHeight = this.Height / this.TilesY;
            for (int x = 0, relativeX = 0; x < this.TilesX; x++, relativeX += tileWidth)
            {
                for (int y = 0, relativeY = 0; y < this.TilesY; y++, relativeY += tileHeight)
                {
                    this.tiles[x, y] = new Tile(
                       relativeX,
                       relativeY,
                       tileWidth,
                       tileHeight){
                       XGridPosition = x,
                       YGridPosition = y
                    };
                }
            }
        }

        public Tile this[int x, int y]
        {
            get { return this.tiles[x, y]; }

        }

        public void Dispose()
        {
            Array.Clear(this.tiles, 0, this.tiles.Length);
            this.tiles = null;
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int TilesX { get; private set; }
        public int TilesY { get; private set; }
        public Bitmap Image
        {
            get
            {
                Bitmap totalImage = new Bitmap(this.Width, this.Height);
                Graphics graphics = Graphics.FromImage(totalImage);
                foreach (var tile in tiles)
                {
                    graphics.DrawImage(tile.Image, tile.X, tile.Y);
                }
                return totalImage;
            }
        }

        public enum RenderCycleType
        {
            Start,
            Finish,
            Partial
        }
        public class Tile : IDisposable
        {
            public delegate void RenderCycleCompletedHandle(Tile tile, RenderCycleType cycleType);
            public event RenderCycleCompletedHandle RenderCycleCompleted;

            public int X;
            public int Y;
            public int XGridPosition;
            public int YGridPosition;
            private int width;
            private int height;
            private Bitmap image;
            private Graphics graphics;
            public Tile(int x, int y, int width, int height)
                : this(x, y, new Bitmap(width, height))
            {
            }

            public Tile(int x, int y, Bitmap image)
            {
                this.X = x;
                this.Y = y;
                this.Image = image;
            }

            public Bitmap Image
            {
                get { return image; }
                set
                {
                    this.image = value;
                    this.width = image.Width;
                    this.height = image.Height;
                    this.graphics = Graphics.FromImage(this.image);
                }
            }

            public int Width
            {
                get { return width; }
                //set { width = value; }
            }

            public int Height
            {
                get { return height; }
                //set { height = value; }
            }

            public Graphics Graphics
            {
                get { return graphics; }
                //set { graphics = value; }
            }

            internal void CompleteCycle(RenderCycleType cycleType)
            {
                if (this.RenderCycleCompleted != null)
                {
                    this.RenderCycleCompleted(this, cycleType);
                }
            }
            public void Dispose()
            {
                if (this.Image != null)
                {
                    this.Image.Dispose();
                    this.graphics.Dispose();
                }
            }
        }



        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (Tile tile in tiles)
            {
                yield return tile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tiles.GetEnumerator();
        }
    }
}
