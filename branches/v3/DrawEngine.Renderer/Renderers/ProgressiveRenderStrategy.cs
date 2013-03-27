using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Tracers;
using DrawEngine.Renderer.Util;

namespace DrawEngine.Renderer.Renderers
{
    public class ProgressiveRenderStrategy : RenderStrategy
    {
        public override void Render(Action<PixelInfo> executeForeachXy, RayCasting caster)
        {
            #region Progressive Render from http://www.cc.gatech.edu/~phlosoft/photon/
            float resX = caster.Scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            float resY = caster.Scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            int iterations = 0;
            int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;

            PixelInfo pixelInfo;

            float resTotal = resX * resY;
            while (iterations < resTotal)
            {
                //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
                if (pCol >= pMax)
                {
                    pRow++;
                    pCol = 0;
                    if (pRow >= pMax)
                    {
                        pIteration++;
                        pRow = 0;
                        pMax <<= 1; //Equals: pMax = (int)Math.Pow(2, pIteration);
                    }
                }
                bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (pRow % 2 == 0 && (pCol % 2 != 0)));
                float x = pCol * (resX / pMax);
                float y = pRow * (resY / pMax);
                pCol++;
                if (pNeedsDrawing)
                {
                    iterations++;
                    Ray ray;
                    RGBColor finalColor = RGBColor.Black;
                    if (caster.Scene.Sampler.SamplesPerPixel > 1)
                    {
                        foreach (Point2D sample in caster.Scene.Sampler.GenerateSamples(x, y))
                        {
                            //ray = this.scene.DefaultCamera.CreateRayFromScreen(x + sample.X, y + sample.Y);
                            ray = caster.Scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
                            ray.PrevRefractIndex = caster.Scene.RefractIndex;
                            finalColor += caster.Trace(ray, 0);
                        }
                        //brush.Color = (finalColor * (1f / this.scene.Sampler.SamplesPerPixel)).ToColor();
                        finalColor = (finalColor * (1f / caster.Scene.Sampler.SamplesPerPixel));
                        //fastBitmap.SetPixel((int)x, (int)y, new PixelData(finalColor));
                    }
                    else
                    {
                        ray = caster.Scene.DefaultCamera.CreateRayFromScreen(x, y);
                        finalColor = caster.Trace(ray, 0);
                        //fastBitmap.SetPixel((int)x, (int)y, new PixelData(finalColor));
                    }
                    pixelInfo.X = x;
                    pixelInfo.Y = y;
                    pixelInfo.Color = finalColor;
                    pixelInfo.Width = (resX / pMax);
                    pixelInfo.Heigth = (resY / pMax);
                    executeForeachXy(pixelInfo);
                }
            }
            #endregion
        }
    }
}
