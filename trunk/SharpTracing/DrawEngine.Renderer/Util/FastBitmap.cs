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
    public class FastBitmap
    {
        private RGBColor[,] colors;
        private int height;
        public Bitmap sourceBitmap;
        private int width;
        public FastBitmap()
        {
            this.sourceBitmap = null;
            this.width = this.height = -1;
            this.colors = null;
        }
        public FastBitmap(Bitmap sourceBitmap)
        {
            this.SetSourceBitmap(sourceBitmap);
        }
        public int Width
        {
            get { return this.width; }
        }
        public int Height
        {
            get { return this.height; }
        }
        public void SetSourceBitmap(Bitmap sourceBitmap)
        {
            #region MyRegion
            //this.sourceBitmap = sourceBitmap;
            //this.width = sourceBitmap.Width;
            //this.height = sourceBitmap.Height;
            //this.colors = new RGBColor[width, height];
            //BitmapData bmpData = sourceBitmap.LockBits(new Rectangle(0, 0, width, height),
            //                                           ImageLockMode.ReadOnly,
            //                                           sourceBitmap.PixelFormat);
            //IntPtr ptr = bmpData.Scan0;
            //int stride = bmpData.Stride;
            //int length = stride * height;
            //byte[] rgbValues = new byte[length];
            //System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, length);
            //sourceBitmap.UnlockBits(bmpData);
            //switch (sourceBitmap.PixelFormat)
            //{
            //    case PixelFormat.Format32bppArgb:
            //        for (int y = 0; y < height; y++)
            //            for (int x = 0; x < width; x++)
            //                colors[x, y] = Color.FromArgb(rgbValues[y * stride + x * 4 + 3],
            //                                             rgbValues[y * stride + x * 4 + 2],
            //                                             rgbValues[y * stride + x * 4 + 1],
            //                                             rgbValues[y * stride + x * 4]);
            //        break;
            //    case PixelFormat.Format24bppRgb:
            //        for (int y = 0; y < height; y++)
            //            for (int x = 0; x < width; x++)
            //                colors[x, y] = Color.FromArgb(rgbValues[y * stride + x * 3 + 2],
            //                                             rgbValues[y * stride + x * 3 + 1],
            //                                             rgbValues[y * stride + x * 3]);
            //        break;
            //    default:
            //        throw new Exception("Unsupported Pixelformat.");
            //} 
            #endregion

            this.sourceBitmap = sourceBitmap;
            this.width = sourceBitmap.Width;
            this.height = sourceBitmap.Height;
            this.colors = new RGBColor[this.width,this.height];
            BitmapData bmpData = sourceBitmap.LockBits(new Rectangle(0, 0, this.width, this.height),
                                                       ImageLockMode.ReadOnly, sourceBitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            int stride = bmpData.Stride;
            int length = stride * this.height;
            byte[] rgbValues = new byte[length];
            Marshal.Copy(ptr, rgbValues, 0, length);
            sourceBitmap.UnlockBits(bmpData);
            RGBColor color = RGBColor.Black;
            switch(sourceBitmap.PixelFormat){
                case PixelFormat.Format32bppArgb:
                    for(int y = 0; y < this.height; y++){
                        for(int x = 0; x < this.width; x++){
                            //Color.FromArgb(rgbValues[y * stride + x * 4 + 3],
                            color.R = rgbValues[y * stride + x * 4 + 2];
                            color.G = rgbValues[y * stride + x * 4 + 1];
                            color.B = rgbValues[y * stride + x * 4];
                            color.Normalize();
                            this.colors[x, y] = color;
                            //this.colors[x, y] = new RGBColor(rgbValues[y * stride + x * 4 + 2],
                            //                                 rgbValues[y * stride + x * 4 + 1],
                            //                                 rgbValues[y * stride + x * 4]);
                        }
                    }
                    break;
                case PixelFormat.Format24bppRgb:
                    for(int y = 0; y < this.height; y++){
                        for(int x = 0; x < this.width; x++){
                            color.R = rgbValues[y * stride + x * 3 + 2];
                            color.G = rgbValues[y * stride + x * 3 + 1];
                            color.B = rgbValues[y * stride + x * 3];
                            color.Normalize();
                            this.colors[x, y] = color;
                            //this.colors[x, y] = new RGBColor(rgbValues[y * stride + x * 3 + 2],
                            //                                 rgbValues[y * stride + x * 3 + 1],
                            //                                 rgbValues[y * stride + x * 3]);
                        }
                    }
                    break;
                default:
                    for(int y = 0; y < this.height; y++){
                        for(int x = 0; x < this.width; x++){
                            this.colors[x, y] = RGBColor.FromColor(this.sourceBitmap.GetPixel(x, y));
                        }
                    }
                    break;
            }
        }
        // Bilinear interpolation 
        public RGBColor GetPixel(float x, float y)
        {
            int pixelX1 = (int)Math.Floor(x);
            int pixelX2 = (int)Math.Ceiling(x);
            float xLerp = x - (float)Math.Truncate(x);
            int pixelY1 = (int)Math.Floor(y);
            int pixelY2 = (int)Math.Ceiling(y);
            float yLerp = y - (float)Math.Truncate(y);
            RGBColor c11 = this.GetPixel(pixelX1, pixelY1);
            RGBColor c12 = this.GetPixel(pixelX1, pixelY2);
            RGBColor c21 = this.GetPixel(pixelX2, pixelY1);
            RGBColor c22 = this.GetPixel(pixelX2, pixelY2);
            return c11 * (1 - xLerp) * (1 - yLerp) + c12 * (1 - xLerp) * yLerp + c21 * xLerp * (1 - yLerp)
                   + c22 * xLerp * yLerp;
        }
        public RGBColor GetPixel(int x, int y)
        {
            //Color color = this.colors[x, y];
            //return RGBColor.FromColor(color);
            return this.colors[x, y];
        }
    }
}