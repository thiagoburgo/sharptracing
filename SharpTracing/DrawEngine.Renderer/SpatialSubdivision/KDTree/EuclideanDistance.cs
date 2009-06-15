using System;

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    /// <summary>
    /// Euclidean distance metric class
    /// </summary>
    public class EuclideanDistance : DistanceMetric
    {
        public override double Distance(double[] a, double[] b)
        {
            return Math.Sqrt(SqrDist(a, b));
        }
        public static double SqrDist(double[] a, double[] b)
        {
            double dist = 0;
            for(int i = 0; i < a.Length; ++i){
                double diff = (a[i] - b[i]);
                dist += diff * diff;
            }
            return dist;
        }
    }
}