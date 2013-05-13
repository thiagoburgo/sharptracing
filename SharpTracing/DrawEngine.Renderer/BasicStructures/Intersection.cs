using System;
using System.Runtime.InteropServices;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.BasicStructures {
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Intersection {
        public Point3D HitPoint;
        public Vector3D Normal;
        public float TMax;
        public float TMin;
        public bool HitFromInSide;
        public UVCoordinate CurrentTextureCoordinate;
        public IPrimitive HitPrimitive;
        public static readonly Intersection Zero;

        public Intersection(IPrimitive hitPrimitive, Point3D hitPoint, Vector3D normal, float tMin, float tMax,
                            bool hitFromInSide) {
            this.HitPoint = hitPoint;
            this.Normal = normal;
            this.TMin = tMin;
            this.TMax = tMax;
            this.HitPrimitive = hitPrimitive;
            this.HitFromInSide = hitFromInSide;
            this.CurrentTextureCoordinate = UVCoordinate.Zero;
        }

        public override string ToString() {
            return "[HP: " + this.HitPoint + " TMin: " + this.TMin + "]";
        }
    }
}