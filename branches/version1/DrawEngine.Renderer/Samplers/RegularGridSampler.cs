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

namespace DrawEngine.Renderer.Samplers
{
    [Serializable]
    public class RegularGridSampler : Sampler
    {
        public RegularGridSampler() : base() {}
        public RegularGridSampler(int samplesX, int sampleY) : base(samplesX, sampleY) {}
        public override IEnumerable<Point2D> GenerateSamples(float x, float y)
        {
            Point2D current_sample = new Point2D();
            float dx, dy;
            dx = x;
            for(int sX = 0; sX < this.samplesX; sX++, dx += this.slopeX){
                dy = y;
                for(int sY = 0; sY < this.samplesY; sY++, dy += this.slopeY){
                    current_sample.X = dx;
                    current_sample.Y = dy;
                    yield return current_sample;
                }
            }
            //Point2D current_sample = new Point2D();
            //for (float x = 0; x < 1f; x += this.slopeX)
            //{
            //    for (float y = 0; y < 1f; y += this.slopeY)
            //    {
            //        current_sample.X = x;
            //        current_sample.Y = y;
            //        yield return current_sample;
            //    }
            //}
        }
    }
}