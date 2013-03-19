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
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Tracers
{
    public delegate void SceneRenderingEventHandler(Bitmap partOfImage, int percentageProcessed);

    public abstract class RayCasting
    {
        protected Scene scene;
        protected int maxDepth;
        protected RayCasting(Scene scene)
        {
            this.scene = scene;
            this.maxDepth = 5;
        }
        protected RayCasting()
        {
            this.scene = new Scene();
            this.maxDepth = 5;
        }
        public Scene Scene
        {
            get { return this.scene; }
            set
            {
                if(value != null){
                    this.scene = value;
                } else{
                    throw new ArgumentNullException("value:scene");
                }
            }
        }
        public int MaxDepth
        {
            get { return this.maxDepth; }
            set { this.maxDepth = value; }
        }
        public virtual void Render(Graphics g) {
            #region Progressive Render from http://www.cc.gatech.edu/~phlosoft/photon/
            float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            int iterations = 0;
            int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;
            SolidBrush brush = new SolidBrush(Color.Black);
            float resTotal = resX * resY;
            while(iterations < resTotal) {
                //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
                if(pCol >= pMax) {
                    pRow++;
                    pCol = 0;
                    if(pRow >= pMax) {
                        pIteration++;
                        pRow = 0;
                        pMax <<= 1; //Equals: pMax = (int)Math.Pow(2, pIteration);
                    }
                }
                bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (pRow % 2 == 0 && (pCol % 2 != 0)));
                float x = pCol * (resX / pMax);
                float y = pRow * (resY / pMax);
                pCol++;
                if(pNeedsDrawing) {
                    iterations++;
                    Ray ray;
                    RGBColor finalColor = RGBColor.Black;
                    if(this.scene.Sampler.SamplesPerPixel > 1) {
                        foreach(Point2D sample in this.scene.Sampler.GenerateSamples(x, y)) {
                            //ray = this.scene.DefaultCamera.CreateRayFromScreen(x + sample.X, y + sample.Y);
                            ray = this.scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
                            ray.PrevRefractIndex = this.scene.RefractIndex;
                            finalColor += this.Trace(ray, 0);
                        }
                        //brush.Color = (finalColor * (1f / this.scene.Sampler.SamplesPerPixel)).ToColor();
                        finalColor = (finalColor * (1f / this.scene.Sampler.SamplesPerPixel));
                    }
                    else {
                        ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
                        finalColor = this.Trace(ray, 0);
                    }
                    // pseudo photo exposure
                    //finalColor.R = (float)(1.0 - Math.Exp(-1.5f * finalColor.R));
                    //finalColor.G = (float)(1.0 - Math.Exp(-1.5f * finalColor.G));
                    //finalColor.B = (float)(1.0 - Math.Exp(-1.5f * finalColor.B));

                    //finalColor.R = srgbEncode(finalColor.R);
                    //finalColor.G = srgbEncode(finalColor.G);
                    //finalColor.B = srgbEncode(finalColor.B);

                    brush.Color = finalColor.ToColor();
                    g.FillRectangle(brush, x, y, (resX / pMax), (resY / pMax));
                }
            }
            #endregion
            #region Linear Render
            //float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            //SolidBrush brush = new SolidBrush(Color.Black);

            //for(int y = 0; y < resY; y++) {
            //    for(int x = 0; x < resX; x++) {
            //        Ray ray;
            //        RGBColor finalColor = RGBColor.Black;
            //        if(this.scene.Sampler.SamplesPerPixel > 1) {
            //            foreach(Point2D sample in this.scene.Sampler.GenerateSamples(x, y)) {
            //                ray = this.scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
            //                ray.PrevRefractIndex = this.scene.RefractIndex;
            //                finalColor += this.Trace(ray, 0);
            //            }
            //            brush.Color = (finalColor * 1f / this.scene.Sampler.SamplesPerPixel).ToColor();
            //        }
            //        else {
            //            ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
            //            brush.Color = this.Trace(ray, 0).ToColor();
            //        }
            //        g.FillRectangle(brush, x, y, 1, 1);
            //    }
            //}
            #endregion
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
        protected abstract RGBColor Trace(Ray ray, int depth);
        protected static RGBColor AverageColors(params RGBColor[] colors)
        {
            float r = 0, g = 0, b = 0;
            float len_inv = 1f / colors.Length;
            foreach(RGBColor color in colors){
                r = (r + color.R);
                g = (g + color.G);
                b = (b + color.B);
            }
            return new RGBColor((r * len_inv), (g * len_inv), (b * len_inv));
        }
        

    }

    
}