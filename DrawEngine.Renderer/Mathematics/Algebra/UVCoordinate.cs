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
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Mathematics.Algebra {
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UVCoordinate {
        public float U;
        public float V;
        public static readonly UVCoordinate Zero = new UVCoordinate();

        public UVCoordinate(float u, float v) {
            this.U = u;
            this.V = v;
            if (!this.IsValid) {
                throw new ArgumentException("Invalid coordinates!");
            }
        }

        public bool IsValid {
            get { return !(this.U < 0.0f || this.U > 1.0f || this.V < 0.0f || this.V > 1.0f || (this.U + this.V) > 1.0f); }
        }
    }
}