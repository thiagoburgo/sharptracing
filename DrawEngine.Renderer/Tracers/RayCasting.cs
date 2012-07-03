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
            double resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            double resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            double x, y;
            int iterations = 0;
            int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;
            SolidBrush brush = new SolidBrush(Color.Black);
            double resTotal = resX * resY;
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
                bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (!(pRow % 2 != 0) && (pCol % 2 != 0)));
                x = pCol * (resX / pMax);
                y = pRow * (resY / pMax);
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
                        //brush.Color = (finalColor * (1d / this.scene.Sampler.SamplesPerPixel)).ToColor();
                        finalColor = (finalColor * (1d / this.scene.Sampler.SamplesPerPixel));
                    }
                    else {
                        ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
                        finalColor = this.Trace(ray, 0);
                    }
                    // pseudo photo exposure
                    //finalColor.R = (1.0 - Math.Exp(-1.5d*finalColor.R));
                    //finalColor.G = (1.0 - Math.Exp(-1.5d * finalColor.G));
                    //finalColor.B = (1.0 - Math.Exp(-1.5d * finalColor.B));

                    //finalColor.R = srgbEncode(finalColor.R);
                    //finalColor.G = srgbEncode(finalColor.G);
                    //finalColor.B = srgbEncode(finalColor.B);
                    brush.Color = finalColor.ToColor();
                    g.FillRectangle(brush, (float)x, (float)y, (float)(resX / pMax), (float)(resY / pMax));
                }
            }
            #endregion
            #region Linear Render
            //double resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //double resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
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
            //            brush.Color = (finalColor * 1d / this.scene.Sampler.SamplesPerPixel).ToColor();
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

        //double srgbEncode(double c) {
        //    if(c <= 0.0031308d) {
        //        return 12.92d * c;
        //    }
        //    else {
        //        return 1.055d * (Math.Pow(c, 0.4166667) - 0.055); // Inverse gamma 2.4
        //    }
        //}
        protected abstract RGBColor Trace(Ray ray, int depth);
        protected static RGBColor AverageColors(params RGBColor[] colors)
        {
            double r = 0, g = 0, b = 0;
            double len_inv = 1d / colors.Length;
            foreach(RGBColor color in colors){
                r = (r + color.R);
                g = (g + color.G);
                b = (b + color.B);
            }
            return new RGBColor((r * len_inv), (g * len_inv), (b * len_inv));
        }
        

    }

    
}