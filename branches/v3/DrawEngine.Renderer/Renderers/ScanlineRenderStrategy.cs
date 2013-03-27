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
using System.Threading.Tasks;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Tracers;

namespace DrawEngine.Renderer.Renderers
{
    [Serializable]
    public class ScanlineRenderStrategy : RenderStrategy
    {
        public override void Render(Action<PixelInfo> executeForeachXY, RayCasting caster)
        {
            float resX = caster.Scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            float resY = caster.Scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;

            PixelInfo pixelInfo;
            for (int y = 0; y < resY; y++)
            {
                for (int x = 0; x < resX; x++)
                {

                    Ray ray;
                    RGBColor finalColor = RGBColor.Black;
                    if (caster.Scene.Sampler.SamplesPerPixel > 1)
                    {
                        foreach (Point2D sample in caster.Scene.Sampler.GenerateSamples(x, y))
                        {
                            ray = caster.Scene.DefaultCamera.CreateRayFromScreen(sample.X, sample.Y);
                            ray.PrevRefractIndex = caster.Scene.RefractIndex;
                            finalColor += caster.Trace(ray, 0);
                        }
                        finalColor = (finalColor * 1f / caster.Scene.Sampler.SamplesPerPixel);
                    }
                    else
                    {
                        ray = caster.Scene.DefaultCamera.CreateRayFromScreen(x, y);
                        finalColor = caster.Trace(ray, 0);
                    }
                    pixelInfo.X = x;
                    pixelInfo.Y = y;
                    pixelInfo.Color = finalColor;
                    pixelInfo.Width = 1;
                    pixelInfo.Heigth = 1;
                    executeForeachXY(pixelInfo);
                }
            }
        }
    }
}