using System;
using System.ComponentModel;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects
{
    [Serializable]
    public class Plane : Primitive, ITransformable3D, IPreprocess
    {
        private float d;
        private Vector3D normal;
        private Point3D pointOnPlane;
        //Equação do plano ax+by+cz+d=0        
        public Plane(Vector3D normal, Point3D pointOnPlane) : this(normal, pointOnPlane, null) {}
        public Plane() : this(Vector3D.UnitY, Point3D.Zero) {}
        public Plane(Vector3D normal, Point3D pointOnPlane, string name)
        {
            this.PointOnPlane = pointOnPlane;
            this.Normal = normal;
            //this.normal.Normalize();
            //this.BoundBox = new BoundBox(0.0f, 0.0f, float.PositiveInfinity, 0.0f, 0.0f, float.PositiveInfinity);
            this.Name = name;
        }
        [Browsable(false)]
        public float D
        {
            get { return this.d; }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D PointOnPlane
        {
            get { return this.pointOnPlane; }
            set
            {
                this.pointOnPlane = value;
                this.Preprocess();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Vector3D Normal
        {
            get { return this.normal; }
            set
            {
                this.normal = value;
                this.Preprocess();
            }
        }
        [Browsable(false)]
        public override BoundBox BoundBox
        {
            get { return base.BoundBox; }
            set { base.BoundBox = value; }
        }

        #region IPreprocess Members
        public void Preprocess()
        {
            this.d = -(this.normal.X * this.pointOnPlane.X) - (this.normal.Y * this.pointOnPlane.Y)
                     - (this.normal.Z * this.pointOnPlane.Z);
        }
        #endregion

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.pointOnPlane.Rotate(angle, axis);
            this.Preprocess();
        }
        public void RotateAxisX(float angle)
        {
            this.Rotate(angle, Vector3D.UnitX);
        }
        public void RotateAxisY(float angle)
        {
            this.Rotate(angle, Vector3D.UnitY);
        }
        public void RotateAxisZ(float angle)
        {
            this.Rotate(angle, Vector3D.UnitZ);
        }
        public void Scale(float factor) {}
        public void Translate(float tx, float ty, float tz)
        {
            this.pointOnPlane.Translate(tx, ty, tz);
            this.Preprocess();
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            //t = -(N • Ro + D) / (N • Rd)	
            //Vector3D origin = ray.Origin.ToVector3D();
            float NRd = this.normal * ray.Direction;
            if(NRd == 0.0f){
                return false;
            }
            float t = -(this.normal * ray.Origin.ToVector3D() + this.d) / NRd;
            if(t < 0.01f){
                return false;
            }
            intersect.Normal = this.normal;
            intersect.HitPoint = ray.Origin + (t * ray.Direction);
            intersect.HitPrimitive = this;
            intersect.TMin = t;
            //float size = 10f;
            //if(intersect.HitPoint.X > 0.0f){
            //    if(((int)(intersect.HitPoint.X / size) % 2) == Math.Abs(((int)(intersect.HitPoint.Z / size) % 2))
            //       ^ (intersect.HitPoint.Z > 0)){
            //        this.material.DiffuseColor = RGBColor.Black;
            //    } else{
            //        this.material.DiffuseColor = RGBColor.White;
            //    }
            //} else{
            //    if(Math.Abs(((int)(intersect.HitPoint.X / size) % 2))
            //       != Math.Abs(((int)(intersect.HitPoint.Z / size) % 2)) ^ (intersect.HitPoint.Z > 0)){
            //        this.material.DiffuseColor = RGBColor.Black;
            //    } else{
            //        this.material.DiffuseColor = RGBColor.White;
            //    }
            //}
            return true;
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            return this.normal;
        }
        public override bool IsInside(Point3D point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}