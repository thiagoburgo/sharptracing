using System;

namespace DrawEngine.Renderer.SpatialSubdivision.KDTree
{
    /// <summary>
    /// Hyper-Point public class supporting KDTree class
    /// </summary>
    public class HPoint : ICloneable, IEquatable<HPoint>
    {
        private readonly double[] coord;
        public HPoint(int n)
        {
            this.coord = new double[n];
        }
        public HPoint(double[] x)
        {
            this.coord = new double[x.Length];
            for(int i = 0; i < x.Length; ++i){
                this.coord[i] = x[i];
            }
        }
        public double[] Coord
        {
            get { return this.coord; }
        }

        #region ICloneable Members
        public object Clone()
        {
            return new HPoint(this.coord);
        }
        #endregion

        #region IEquatable<HPoint> Members
        public bool Equals(HPoint p)
        {
            // seems faster than java.util.Arrays.equals(), which is not 
            // currently supported by Matlab anyway
            for(int i = 0; i < this.coord.Length; ++i){
                if(this.coord[i] != p.coord[i]){
                    return false;
                }
            }
            return true;
        }
        #endregion

        public static double SqrDist(HPoint x, HPoint y)
        {
            return EuclideanDistance.SqrDist(x.coord, y.coord);
        }
        public override String ToString()
        {
            String s = "";
            for(int i = 0; i < this.coord.Length; ++i){
                s = s + this.coord[i] + " ";
            }
            return s;
        }
    }
}