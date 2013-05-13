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
using System.Xml.Serialization;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Samplers
{
    [XmlInclude(typeof(JitteredSampler)), Serializable]
    public abstract class RegularGridSampler : Sampler
    {
        
        protected RegularGridSampler() { }
        public RegularGridSampler(int numberOfSamples, int numberOfSets) : base(numberOfSamples, numberOfSets) { }

        protected override void GenerateSamples()
        {
            samples.Clear();
            int n = (int)Math.Sqrt(numberOfSamples);

            for (int j = 0; j < numberOfSets; j++)
                for (int p = 0; p < n; p++)
                    for (int q = 0; q < n; q++)
                        samples.Add(new Point2D((q + 0.5f) / n, (p + 0.5f) / n));
        }
        //public override IEnumerable<Point2D> GetSamplesFor(float x, float y)
        //{
            //Point2D currentSample = new Point2D();
            //float dy;
            //float dx = x;

            //for (int sX = 0; sX < this.samplesX; sX++, dx += this.slopeX)
            //{
            //    dy = y;
            //    for (int sY = 0; sY < this.samplesY; sY++, dy += this.slopeY)
            //    {
            //        currentSample.X = dx;
            //        currentSample.Y = dy;
            //        yield return currentSample;
            //    }
            //}
        //}
    }
}