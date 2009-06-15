using System;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects.CSG
{
    [Serializable]
    public class UnionPrimitive : Primitive, IConstrutive
    {
        #region IConstrutive Members
        public Primitive BasePrimitive
        {
            get { throw new Exception("The method or operation is not implemented."); }
            set { throw new Exception("The method or operation is not implemented."); }
        }
        public Primitive OperandPrimitive
        {
            get { throw new Exception("The method or operation is not implemented."); }
            set { throw new Exception("The method or operation is not implemented."); }
        }
        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsInside(Point3D point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}