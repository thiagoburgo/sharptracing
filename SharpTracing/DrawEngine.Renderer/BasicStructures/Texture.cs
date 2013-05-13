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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer.BasicStructures {
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Texture : IDisposable {
        private RGBColor[,] textureMatrix = new RGBColor[0,0];
        private string texturePath;
        private Bitmap texture;
        internal Texture() {}

        public Texture(String texturePath) {
            this.texturePath = texturePath;
            this.texture = (Bitmap) System.Drawing.Image.FromFile(texturePath);
            this.textureMatrix = new RGBColor[this.texture.Width, this.texture.Height];
            BitmapToRGBColorMatrix(this.texture, ref textureMatrix);
        }

        private unsafe static void BitmapToRGBColorMatrix(Bitmap img, ref RGBColor[,] textureMatrix) {
            using (img) {
                BitmapData imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly,
                                               img.PixelFormat);
                byte bitsPerPixel = GetBitsPerPixel(imgData.PixelFormat);
                byte* scan0 = (byte*) imgData.Scan0.ToPointer();
                for (int x = 0; x < imgData.Height; ++x) {
                    for (int y = 0; y < imgData.Width; ++y) {
                        byte* data = scan0 + x * imgData.Stride + y * bitsPerPixel / 8;
                        textureMatrix[y, x] = new RGBColor(data[2], data[1], data[0]);
                        textureMatrix[y, x].Normalize();
                    }
                }
                img.UnlockBits(imgData);
            }
        }

        private static byte GetBitsPerPixel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return 24;
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format32bppRgb:
                    return 32;
                default:
                    throw new ArgumentException("Only 24 and 32 bit images are supported");

            }
        }
        public string TexturePath {
            get { return this.texturePath; }
            set {
                this.texturePath = value;
                if (this.texture != null){
                    this.texture.Dispose();
                }
                this.texture = System.Drawing.Image.FromFile(this.texturePath) as Bitmap;
                unsafe {
                    fixed (RGBColor* colors = this.textureMatrix)
                    {
                        Marshal.FreeHGlobal(new IntPtr(colors));
                    }    
                }
                this.textureMatrix = null;
                this.textureMatrix = new RGBColor[this.texture.Width, this.texture.Height];

                BitmapToRGBColorMatrix(this.texture, ref textureMatrix);
            }
        }

        public bool IsLoaded {
            get { return this.texture != null; }
        }

        //[XmlIgnore]
        //[Browsable(false)]
        //public Bitmap TextureImage
        //{
        //    get { return texture; }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            texture = value;
        //            this.ProcessTextureImage();
        //        }
        //    }
        //}
        [Browsable(true)]
        public int Width {
            get {
                if (this.texture != null) {
                    return this.textureMatrix.GetLength(0);
                }
                return 0;
            }
        }

        [Browsable(true)]
        public int Height {
            get {
                if (this.texture != null) {
                    return this.textureMatrix.GetLength(1);
                }
                return 0;
            }
        }

        public Bitmap Image {
             get {
                 unsafe {
                     Bitmap img = new Bitmap(this.Width, this.Height);
                     BitmapData imgData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                                       ImageLockMode.WriteOnly, img.PixelFormat);
                     byte bitsPerPixel = GetBitsPerPixel(imgData.PixelFormat);
                     byte* scan0 = (byte*) imgData.Scan0.ToPointer();
                     for (int x = 0; x < imgData.Height; ++x) {
                         for (int y = 0; y < imgData.Width; ++y) {
                             byte* data = scan0 + x * imgData.Stride + y * bitsPerPixel / 8;
                             Color color = textureMatrix[x, y].ToColor();
                             data[2] = color.R;
                             data[1] = color.G;
                             data[0] = color.B;
                         }
                     }
                     img.UnlockBits(imgData);
                     return img;
                 }
             }
        }

        public override string ToString() {
            return String.Format("{0} ({1} X {2})",
                                 this.texturePath != null ? Path.GetFileName(this.texturePath) : "[Sem Textura]",
                                 this.Width,
                                 this.Height);
        }

        public void Dispose() {
            unsafe
            {
                fixed (RGBColor* colors = this.textureMatrix)
                {
                    Marshal.FreeHGlobal(new IntPtr(colors));
                }
            }
            this.textureMatrix = null;
            this.texture.Dispose();
        }

        //public static implicit operator Texture(Bitmap texture)
        //{
        //    return new Texture(texture);
        //}
        //public void ProcessTextureImage(Bitmap texture)
        //{
        //    textureMatrix = new RGBColor[texture.Width, texture.Height];
        //    for (int i = 0; i < texture.Width; i++)
        //    {
        //        for (int j = 0; j < texture.Height; j++)
        //        {
        //            textureMatrix[i, j] = RGBColor.FromColor(texture.GetPixel(i, j));
        //        }
        //    }
        //    texture.Dispose();
        //}
        public RGBColor GetPixel(UVCoordinate uv) {
            return this.textureMatrix[(int) (uv.U * this.Width), (int) (uv.V * this.Height)];
        }

        public RGBColor GetPixel(int x, int y) {
            return this.textureMatrix[x, y];
        }

        public RGBColor GetPixel(float x, float y) {
            int pixelX1 = (int)Math.Floor(x);
            int pixelX2 = (int)Math.Ceiling(x);
            float xLerp = x - (float)Math.Truncate(x);
            int pixelY1 = (int)Math.Floor(y);
            int pixelY2 = (int)Math.Ceiling(y);
            float yLerp = y - (float)Math.Truncate(y);

            RGBColor c11 = GetPixel(pixelX1, pixelY1);
            RGBColor c12 = GetPixel(pixelX1, pixelY2);
            RGBColor c21 = GetPixel(pixelX2, pixelY1);
            RGBColor c22 = GetPixel(pixelX2, pixelY2);
            return c11 * (1 - xLerp) * (1 - yLerp)
                    + c12 * (1 - xLerp) * yLerp
                    + c21 * xLerp * (1 - yLerp)
                    + c22 * xLerp * yLerp;
        }
    }
}