namespace DrawEngine.Renderer.SpatialSubdivision.KDTree {
    /// <summary>
    /// Abstract distance metric class
    /// </summary>
    public abstract class DistanceMetric {
        public abstract double Distance(double[] a, double[] b);
    }
}