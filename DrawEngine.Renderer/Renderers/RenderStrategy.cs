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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.Tracers;

namespace DrawEngine.Renderer.Renderers {
    public struct PixelInfo {
        public RGBColor Color;
        public float Width;
        public float Heigth;
        public float X;
        public float Y;

        public PixelInfo(RGBColor color, float x, float y, float width, float heigth)
        {
            this.Color = color;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Heigth = heigth;
        }
    }
    [Serializable]
    public abstract class RenderStrategy {
        public abstract void Render(Action<PixelInfo> executeForeachXy, RayCasting caster);
    }
}