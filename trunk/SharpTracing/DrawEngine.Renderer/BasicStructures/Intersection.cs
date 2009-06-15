using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.BasicStructures
{
    public struct Intersection
    {
        public bool HitFromInSide;
        public Point3D HitPoint;
        public Primitive HitPrimitive;
        public Vector3D Normal;
        public float TMax;
        public float TMin;
        public Intersection(Primitive hitPrimitive, Point3D hitPoint, Vector3D normal, float tMin, float tMax,
                            bool hitFromInSide)
        {
            this.HitPoint = hitPoint;
            this.Normal = normal;
            this.TMin = tMin;
            this.TMax = tMax;
            this.HitPrimitive = hitPrimitive;
            this.HitFromInSide = hitFromInSide;
        }
        public override string ToString()
        {
            return "[HP: " + this.HitPoint.ToString() + " TMin: " + this.TMin + "]";
        }
    }
}