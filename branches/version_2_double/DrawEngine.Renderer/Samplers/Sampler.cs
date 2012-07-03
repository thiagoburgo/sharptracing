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
using DrawEngine.Renderer.Algebra;

namespace DrawEngine.Renderer.Samplers
{
    [XmlInclude(typeof(RegularGridSampler)), Serializable]
    public abstract class Sampler
    {
        protected int samplesX;
        protected int samplesY;
        protected float slopeX;
        protected float slopeY;
        public Sampler() : this(1, 1) {}
        public Sampler(int samplesX, int samplesY)
        {
            this.SamplesX = samplesX;
            this.SamplesY = samplesY;
        }
        public int SamplesPerPixel
        {
            get { return this.samplesX * this.samplesY; }
        }
        public int SamplesX
        {
            get { return this.samplesX; }
            set
            {
                this.samplesX = value;
                this.slopeX = 1f / this.samplesX;
            }
        }
        public int SamplesY
        {
            get { return this.samplesY; }
            set
            {
                this.samplesY = value;
                this.slopeY = 1f / this.samplesY;
            }
        }
        public abstract IEnumerable<Point2D> GenerateSamples(float x, float y);
    }
}