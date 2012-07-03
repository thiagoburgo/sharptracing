using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;
using System.Runtime.InteropServices;
using System;
using DrawEngine.Renderer.Algebra;

namespace DrawEngine.Renderer.BasicStructures
{
    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Intersection
    {
        public bool HitFromInSide;
        public Point3D HitPoint;
        public IPrimitive HitPrimitive;
        public Vector3D Normal;
        public UVCoordinate CurrentTextureCoordinate;
        public double TMax;
        public double TMin;
        public static readonly Intersection Zero = new Intersection();
        public Intersection(IPrimitive hitPrimitive, Point3D hitPoint, Vector3D normal, double tMin, double tMax,
                            bool hitFromInSide)
        {
            this.HitPoint = hitPoint;
            this.Normal = normal;
            this.TMin = tMin;
            this.TMax = tMax;
            this.HitPrimitive = hitPrimitive;
            this.HitFromInSide = hitFromInSide;
            this.CurrentTextureCoordinate = UVCoordinate.Zero;
        }
        public override string ToString()
        {
            return "[HP: " + this.HitPoint.ToString() + " TMin: " + this.TMin + "]";
        }
    }
}