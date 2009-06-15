using System;

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    /// <summary>
    /// Hamming distance metric class
    /// </summary>
    public class HammingDistance : DistanceMetric
    {
        public override double Distance(double[] a, double[] b)
        {
            double dist = 0;
            for(int i = 0; i < a.Length; ++i){
                double diff = (a[i] - b[i]);
                dist += Math.Abs(diff);
            }
            return dist;
        }
    }
}