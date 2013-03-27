using System;
using System.Runtime.InteropServices;

namespace DrawEngine.Renderer.Mathematics.Algebra {
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BarycentricCoordinate {
        public float Alpha;
        public float Beta;
        public float Gama;
        public static readonly BarycentricCoordinate Zero = new BarycentricCoordinate();

        public BarycentricCoordinate(float alpha, float beta, float gama) {
            this.Alpha = alpha;
            this.Beta = beta;
            this.Gama = gama;
        }

        public bool IsValid {
            get { return (this.Alpha >= 0 && this.Beta >= 0 && this.Alpha + this.Beta <= 1); }
        }
    }
}