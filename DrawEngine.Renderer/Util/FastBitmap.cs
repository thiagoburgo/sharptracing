/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.Util
{
    public unsafe class FastBitmap : IDisposable
    {
        private Bitmap sourceBitmap;
        // three elements used for MakeGreyUnsafe

        private BitmapData bitmapData = null;
        private byte* _base = null;

        public FastBitmap(Bitmap bitmap)
        {
            this.sourceBitmap = bitmap;
        }

        public FastBitmap(int width, int height)
        {
            this.sourceBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        }

        public void Dispose()
        {
            this.sourceBitmap.Dispose();
            this.sourceBitmap = null;
        }

        public int Width
        {
            get
            {
                return this.sourceBitmap.Width;
            }
        }
        public int Height
        {
            get
            {
                return this.sourceBitmap.Height;
            }
        }


        private int width;
        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = this.sourceBitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle(
                (int)boundsF.X,
                (int)boundsF.Y,
                (int)boundsF.Width,
                (int)boundsF.Height
            );

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length.
            this.width = (int)boundsF.Width * sizeof(PixelData);
            if (this.width % 4 != 0)
            {
                this.width = 4 * (this.width / 4 + 1);
            }

            //this.bitmapData = this.sourceBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            this.bitmapData = this.sourceBitmap.LockBits(bounds, ImageLockMode.ReadWrite, sourceBitmap.PixelFormat);

            _base = (Byte*)this.bitmapData.Scan0.ToPointer();
        }

        public PixelData GetPixel(float x, float y)
        {
            PixelData returnValue = *PixelAt((int)x, (int)y);
            return returnValue;
        }

        public void SetPixel(int x, int y, PixelData colour)
        {
            PixelData* pixel = PixelAt(x, y);
            *pixel = colour;
        }

        public void UnlockBitmap()
        {
            this.sourceBitmap.UnlockBits(this.bitmapData);
            this.bitmapData = null;
            _base = null;
        }

        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(_base + y * this.width + x * sizeof(PixelData));
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.sourceBitmap.Dispose();
        }

        #endregion
    }

    public struct PixelData
    {
        public byte Blue;
        public byte Green;
        public byte Red;

        public PixelData(RGBColor color)
        {
            Blue = (byte)(color.B * 255);
            Green = (byte)(color.G * 255);
            Red = (byte)(color.R * 255);
        }
        public static implicit operator RGBColor(PixelData pixelData) {
            float oneInv255 = 1f / 255;
            return new RGBColor(pixelData.Red * oneInv255, pixelData.Green * oneInv255, pixelData.Blue * oneInv255);
        }
    }
}