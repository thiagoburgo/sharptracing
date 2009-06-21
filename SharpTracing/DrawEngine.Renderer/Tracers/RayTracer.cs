using System;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Tracers
{
    public sealed class RayTracer : RayCasting
    {
        public RayTracer(Scene scene) : base(scene) {}
        public RayTracer() : base() {}
        public override void Render(Graphics g)
        {
            #region Progressive Render
            //float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            //float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            //float x, y;
            //int iterations = 0;
            //RGBColor[] colors = new RGBColor[this.scene.Sampler.SamplesPerPixel + 1];
            //int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;
            //SolidBrush brush = new SolidBrush(Color.Black);
            //float resTotal = resX * resY * this.scene.Sampler.SamplesPerPixel;
            //while(iterations <= resTotal){
            //    //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
            //    if(pCol >= pMax){
            //        pRow++;
            //        pCol = 0;
            //        if(pRow >= pMax){
            //            pIteration++;
            //            pRow = 0;
            //            pMax = (int)Math.Pow(2, pIteration);
            //        }
            //    }
            //    bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (!(pRow % 2 != 0) && (pCol % 2 != 0)));
            //    x = pCol * (resX / pMax);
            //    y = pRow * (resY / pMax);
            //    pCol++;
            //    if(pNeedsDrawing){
            //        iterations++;
            //        Ray ray;
            //        if(this.scene.Sampler.SamplesPerPixel > 1){
            //            int i = 0;
            //            foreach(Point2D sample in this.scene.Sampler.GenerateSamples(x, y)){
            //                //ray = this.scene.DefaultCamera.CreateRayFromScreen(x + sample.X, y + sample.Y);
            //                ray = this.scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
            //                ray.PrevRefractIndex = this.scene.RefractIndex;
            //                colors[i++] = this.Trace(ray, 0);
            //            }
            //            brush.Color = AverageColors(colors).ToColor();
            //        } else{
            //            ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
            //            brush.Color = this.Trace(ray, 0).ToColor();
            //        }
            //        g.FillRectangle(brush, x, y, (resX / pMax), (resY / pMax));
            //    }
            //}
            #endregion

            #region Linear Render
            float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            SolidBrush brush = new SolidBrush(Color.Black);
            RGBColor[] colors = new RGBColor[this.scene.Sampler.SamplesPerPixel];
            for(int y = 0; y < resY; y++) {
                for(int x = 0; x < resX; x++) {
                    Ray ray;
                    if(this.scene.Sampler.SamplesPerPixel > 1) {
                        int i = 0;
                        foreach(Point2D sample in this.scene.Sampler.GenerateSamples(x, y)) {
                            ray = this.scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
                            ray.PrevRefractIndex = this.scene.RefractIndex;
                            colors[i++] = this.Trace(ray, 0);
                        }
                        brush.Color = AverageColors(colors).ToColor();
                    }
                    else {
                        ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
                        brush.Color = this.Trace(ray, 0).ToColor();
                    }
                    g.FillRectangle(brush, x, y, 1, 1);
                }
            }
            #endregion
        }
        public RGBColor Trace(Ray ray, int depth)
        {
            Intersection intersection;
            if(this.scene.FindIntersection(ray, out intersection)){
                Material material = intersection.HitPrimitive.Material;
                this.scene.Shader = material.CreateShader(this.scene);
                RGBColor color = this.scene.Shader.Shade(ray, intersection);
                Ray rRay = new Ray();
                if(depth < 5){
                    if(material.IsReflective){
                        rRay.Origin = intersection.HitPoint;
                        rRay.Direction = Reflected(intersection.Normal, ray.Direction);
                        color += this.Trace(rRay, depth + 1) * material.KSpec;
                    }
                    if(material.IsTransparent){
                        Vector3D T;
                        //float eta = intersection.HitFromInSide
                        //                ? material.RefractIndex * 1 / this.scene.RefractIndex
                        //                : this.scene.RefractIndex * 1 / material.RefractIndex;
                        //float eta = this.scene.RefractIndex * 1 / material.RefractIndex;
                        float eta = (ray.PrevRefractIndex == material.RefractIndex)
                                        ? material.RefractIndex * 1 / this.scene.RefractIndex
                                        : this.scene.RefractIndex * 1 / material.RefractIndex;
                        if(Refracted(intersection.Normal, ray.Direction, out T, eta)){
                            rRay.Origin = intersection.HitPoint;
                            rRay.Direction = T;
                            rRay.PrevRefractIndex = material.RefractIndex;
                            color += this.Trace(rRay, depth + 1) * material.KTrans;
                        }
                    }
                }
                return color;
            }
            return this.scene.IsEnvironmentMapped ? this.scene.EnvironmentMap.GetColor(ray) : this.scene.BackgroundColor;
        }
    }
}