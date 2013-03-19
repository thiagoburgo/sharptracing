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
using System.Drawing.Design;
using System.IO;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects.EnvironmentMaps.Design;

namespace DrawEngine.Renderer.RenderObjects.EnvironmentMaps
{
    [Editor(typeof(CubeMapUIEditor), typeof(UITypeEditor)), Serializable]
    public class CubeMap : EnvironmentMap
    {
        private String basePath;
        private string fileNamePattern;
        private bool isLoaded = false;
        private float xMax;
        private Texture xMaxTexture;
        private float xMin;
        private Texture xMinTexture;
        private float yMax;
        private Texture yMaxTexture;
        private float yMin;
        private Texture yMinTexture;
        private float zMax;
        private Texture zMaxTexture;
        private float zMin;
        private Texture zMinTexture;
        public CubeMap() : this(300, 300, 300, null, null) {}
        public CubeMap(float width, float height, float depth, string pathBase, string fileNamePrefix)
        {
            this.FileNamePattern = fileNamePrefix;
            this.BasePath = pathBase;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }
        public float Width
        {
            get { return this.xMax - this.xMin; }
            set
            {
                this.xMin = -(value * 0.5f);
                this.xMax = value * 0.5f;
            }
        }
        public float Height
        {
            get { return this.yMax - this.yMin; }
            set
            {
                this.yMin = -(value * 0.5f);
                this.yMax = value * 0.5f;
            }
        }
        public float Depth
        {
            get { return this.zMax - this.zMin; }
            set
            {
                this.zMin = -(value * 0.5f);
                this.zMax = value * 0.5f;
            }
        }
        public string FileNamePattern
        {
            get { return this.fileNamePattern; }
            set
            {
                if(!String.IsNullOrEmpty(value)){
                    this.fileNamePattern = value;
                    this.isLoaded = false;
                    if(!String.IsNullOrEmpty(this.basePath)){
                        this.SetUpTextures();
                    }
                }
            }
        }
        public String BasePath
        {
            get { return this.basePath; }
            set
            {
                if(!String.IsNullOrEmpty(value)){
                    this.basePath = value;
                    this.isLoaded = false;
                    if(!String.IsNullOrEmpty(this.fileNamePattern)){
                        this.SetUpTextures();
                    }
                }
            }
        }
        public bool IsLoaded
        {
            get { return this.isLoaded; }
        }
        private void SetUpTextures()
        {
            this.xMinTexture = new Texture(Path.Combine(this.basePath, this.fileNamePattern.Replace("{#}", "_NX")));
            this.xMaxTexture = new Texture(Path.Combine(this.basePath, this.fileNamePattern.Replace("{#}", "_PX")));
            this.yMinTexture = new Texture(Path.Combine(this.basePath, this.fileNamePattern.Replace("{#}", "_NY")));
            this.yMaxTexture = new Texture(Path.Combine(this.basePath, this.fileNamePattern.Replace("{#}", "_PY")));
            this.zMinTexture = new Texture(Path.Combine(this.basePath, this.fileNamePattern.Replace("{#}", "_NZ")));
            this.zMaxTexture = new Texture(Path.Combine(this.basePath, this.fileNamePattern.Replace("{#}", "_PZ")));
            this.isLoaded = true;
        }
        public override RGBColor GetColor(Ray ray)
        {
            if(this.isLoaded){
                float t;
                Point3D posWS = ray.Origin; // Position where the ray starts in world space
                Vector3D dirWS = ray.Direction; // Direction of the ray in world space
                // Test if ray intersects right plane
                if(dirWS.X > 0){
                    t = (this.xMax - posWS.X) / dirWS.X;
                    Point3D p = posWS + dirWS * t;
                    if(p.Y <= this.yMax && p.Y >= this.yMin && p.Z >= this.zMin && p.Z <= this.zMax){
                        float xTex = (-p.Z + this.zMax) / (this.zMax - this.zMin);
                        float yTex = (-p.Y + this.yMax) / (this.yMax - this.yMin);
                        float pixelX = (xTex * (this.xMaxTexture.Width - 1));
                        float pixelY = (yTex * (this.xMaxTexture.Height - 1));
                        return this.xMaxTexture.GetPixel(pixelX, pixelY);
                    }
                } else if(dirWS.X < 0){
                    t = (this.xMin - posWS.X) / dirWS.X;
                    Point3D p = posWS + dirWS * t;
                    if(p.Y <= this.yMax && p.Y >= this.yMin && p.Z >= this.zMin && p.Z <= this.zMax){
                        float xTex = (p.Z + this.zMax) / (this.zMax - this.zMin);
                        float yTex = (-p.Y + this.yMax) / (this.yMax - this.yMin);
                        float pixelX = (xTex * (this.xMinTexture.Width - 1));
                        float pixelY = (yTex * (this.xMinTexture.Height - 1));
                        //int pixelX = (int)(xTex * (xMinTexture.Width-1));
                        //int pixelY = (int)(yTex * (xMinTexture.Height-1));
                        return this.xMinTexture.GetPixel(pixelX, pixelY);
                    }
                }
                if(dirWS.Y > 0){
                    t = (this.yMax - posWS.Y) / dirWS.Y;
                    Point3D p = posWS + dirWS * t;
                    if(p.X <= this.xMax && p.X >= this.xMin && p.Z >= this.zMin && p.Z <= this.zMax){
                        float xTex = (p.X + this.xMax) / (this.xMax - this.xMin);
                        float yTex = (p.Z + this.zMax) / (this.zMax - this.zMin);
                        float pixelX = (xTex * (this.yMaxTexture.Width - 1));
                        float pixelY = (yTex * (this.yMaxTexture.Height - 1));
                        return this.yMaxTexture.GetPixel(pixelX, pixelY);
                    }
                } else if(dirWS.Y < 0){
                    t = (this.yMin - posWS.Y) / dirWS.Y;
                    Point3D p = posWS + dirWS * t;
                    if(p.X <= this.xMax && p.X >= this.xMin && p.Z >= this.zMin && p.Z <= this.zMax){
                        float xTex = (p.X + this.xMax) / (this.xMax - this.xMin);
                        float yTex = (-p.Z + this.zMax) / (this.zMax - this.zMin);
                        float pixelX = (xTex * (this.yMinTexture.Width - 1));
                        float pixelY = (yTex * (this.yMinTexture.Height - 1));
                        return this.yMinTexture.GetPixel(pixelX, pixelY);
                    }
                }
                if(dirWS.Z > 0){
                    t = (this.zMax - posWS.Z) / dirWS.Z;
                    Point3D p = posWS + dirWS * t;
                    if(p.X <= this.xMax && p.X >= this.xMin && p.Y >= this.yMin && p.Y <= this.yMax){
                        float xTex = (p.X + this.xMax) / (this.xMax - this.xMin);
                        float yTex = (-p.Y + this.yMax) / (this.yMax - this.yMin);
                        float pixelX = xTex * (this.zMaxTexture.Width - 1);
                        float pixelY = yTex * (this.zMaxTexture.Height - 1);
                        return this.zMaxTexture.GetPixel(pixelX, pixelY);
                    }
                } else if(dirWS.Z < 0){
                    t = (this.zMin - posWS.Z) / dirWS.Z;
                    Point3D p = posWS + dirWS * t;
                    if(p.X <= this.xMax && p.X >= this.xMin && p.Y >= this.yMin && p.Y <= this.yMax){
                        float xTex = (-p.X + this.xMax) / (this.xMax - this.xMin);
                        float yTex = (-p.Y + this.yMax) / (this.yMax - this.yMin);
                        float pixelX = (xTex * (this.zMinTexture.Width - 1));
                        float pixelY = (yTex * (this.zMinTexture.Height - 1));
                        return this.zMinTexture.GetPixel(pixelX, pixelY);
                    }
                }
                //throw new Exception("No valid direction for the ray specified.");
            }
            return RGBColor.Black;
        }
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.FileNamePattern) ? "CubeMap" : this.FileNamePattern;
        }
    }
}