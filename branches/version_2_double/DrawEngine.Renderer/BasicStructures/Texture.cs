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
using System.IO;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Util;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.BasicStructures
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Texture
    {
        //private RGBColor[,] textureMatrix;
        private FastBitmap texture;
        private string texturePath;
        public Texture(String texturePath)
        {
            this.texturePath = texturePath;
            this.texture = new FastBitmap((Image.FromFile(texturePath) as Bitmap));
        }
        public string TexturePath
        {
            get { return this.texturePath; }
            set
            {
                this.texturePath = value;
                this.texture = new FastBitmap((Image.FromFile(this.texturePath) as Bitmap));
            }
        }
        public bool IsLoaded
        {
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
        public int Width
        {
            get
            {
                if(this.texture != null){
                    return this.texture.Width;
                }
                return 0;
            }
        }
        [Browsable(true)]
        public int Height
        {
            get
            {
                if(this.texture != null){
                    return this.texture.Height;
                }
                return 0;
            }
        }
        public override string ToString()
        {
            return String.Format("{0} ({1} X {2})",
                                 this.texturePath != null ? Path.GetFileName(this.texturePath) : "[Sem Textura]",
                                 this.texture != null ? this.texture.Width : 0,
                                 this.texture != null ? this.texture.Height : 0);
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
        public RGBColor GetPixel(UVCoordinate uv)
        {
            return this.texture.GetPixel((int)(uv.U * this.texture.Width), (int)(uv.V * this.texture.Height));
        }
        public RGBColor GetPixel(int x, int y)
        {
            return this.texture.GetPixel(x, y);
        }
        public RGBColor GetPixel(double x, double y)
        {
            return this.texture.GetPixel(x, y);
        }
    }
}