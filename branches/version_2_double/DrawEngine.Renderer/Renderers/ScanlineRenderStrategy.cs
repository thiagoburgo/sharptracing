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
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;

namespace DrawEngine.Renderer.Renderers
{
    [Serializable]
    public class ScanlineRenderStrategy : RenderStrategy
    {
        public ScanlineRenderStrategy(Scene scene) : base(scene) {}
        public override IEnumerable<Ray> GenerateRays()
        {
            for(int x = 0; x < this.Scene.DefaultCamera.ResX; x++){
                for(int y = 0; y < this.Scene.DefaultCamera.ResY; y++){
                    foreach(Point2D sample in this.Scene.Sampler.GenerateSamples(x, y)){
                        //yield return this.Scene.DefaultCamera.CreateRayFromScreen(x +sample.X, y + sample.Y);
                        yield return this.Scene.DefaultCamera.CreateRayFromScreen(x, y);
                    }
                }
            }
        }
    }
}