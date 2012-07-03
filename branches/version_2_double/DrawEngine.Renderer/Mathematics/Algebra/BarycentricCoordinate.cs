using System.Runtime.InteropServices;
using System;
namespace DrawEngine.Renderer.Algebra
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BarycentricCoordinate
    {
        public double Alpha;
        public double Beta;
        public double Gama;
        public static readonly BarycentricCoordinate Zero = new BarycentricCoordinate();
        public BarycentricCoordinate(double alpha, double beta, double gama)
        {
            this.Alpha = alpha;
            this.Beta = beta;
            this.Gama = gama;
        }
        public bool IsValid
        {
            get { return (this.Alpha >= 0 && this.Beta >= 0 && this.Alpha + this.Beta <= 1); }
        }
    }
}