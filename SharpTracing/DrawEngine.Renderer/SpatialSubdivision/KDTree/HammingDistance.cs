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

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree {
    /// <summary>
    /// Hamming distance metric class
    /// </summary>
    public class HammingDistance : DistanceMetric {
        public override double Distance(double[] a, double[] b) {
            double dist = 0;
            for (int i = 0; i < a.Length; ++i) {
                double diff = (a[i] - b[i]);
                dist += Math.Abs(diff);
            }
            return dist;
        }
    }
}