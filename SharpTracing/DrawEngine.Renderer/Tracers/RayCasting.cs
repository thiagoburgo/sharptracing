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
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Renderers;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer.Tracers
{
    public delegate void SceneRenderingEventHandler(Bitmap partOfImage, int percentageProcessed);

    public abstract class RayCasting
    {
        protected Scene scene;
        protected int maxDepth;
        protected RenderStrategy renderStrategy;
        protected RayCasting(Scene scene, RenderStrategy renderStrategy)
        {
            this.scene = scene;
            this.maxDepth = 5;
            this.RenderStrategy = renderStrategy;
        }
        protected RayCasting()
        {
            this.scene = new Scene();
            this.maxDepth = 5;
            this.RenderStrategy = new ScanlineRenderStrategy();
        }
        public Scene Scene
        {
            get { return this.scene; }
            set
            {
                if (value != null)
                {
                    this.scene = value;
                }
                else
                {
                    throw new ArgumentNullException("value:scene");
                }
            }
        }
        public int MaxDepth
        {
            get { return this.maxDepth; }
            set { this.maxDepth = value; }
        }

        public RenderStrategy RenderStrategy
        {
            get { return this.renderStrategy; }
            set { this.renderStrategy = value; }
        }

        public virtual void Render(IEnumerable<TiledBitmap.Tile> tiledBitmap) {
            this.RenderStrategy.Render(this, tiledBitmap);
        }

        public void CancelRender() {
            this.renderStrategy.CancelRender();
        }

        float srgbEncode(float c)
        {
            if (c <= 0.0031308f)
            {
                return 12.92f * c;
            }
            else
            {
                return 1.055f * (float)(Math.Pow(c, 0.4166667) - 0.055); // Inverse gamma 2.4
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract RGBColor Trace(Ray ray, int depth);
        protected static RGBColor AverageColors(params RGBColor[] colors)
        {
            float r = 0, g = 0, b = 0;
            float len_inv = 1f / colors.Length;
            foreach (RGBColor color in colors)
            {
                r = (r + color.R);
                g = (g + color.G);
                b = (b + color.B);
            }
            return new RGBColor((r * len_inv), (g * len_inv), (b * len_inv));
        }

        public abstract RayCasting Clone();

    }


}