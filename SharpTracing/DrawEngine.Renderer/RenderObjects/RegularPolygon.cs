using System;
using System.ComponentModel;
using System.Linq;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Mathematics.Algebra;
using System.Xml.Serialization;

namespace DrawEngine.Renderer.RenderObjects
{
    public class RegularPolygon : Polygon, ITransformable3D
    {
        private float radius;
        private int verticesCount;
        public RegularPolygon() : this(Point3D.Zero, Vector3D.UnitY, 5, 20f) {}
        public RegularPolygon(Point3D center, Vector3D normal, int numVertices, float radius)
        {
            this.vertices.NotificationsEnabled = false;
            this.VerticesCount = numVertices;
            this.Radius = radius;
            this.Normal = normal;
            this.Center = center;
        }
        protected override bool IsCoplanar
        {
            get { return true; }
        }
        public Point3D Center
        {
            get { return base.center; }
            set
            {
                base.center = value;
                this.Preprocess();
            }
        }
        //[XmlIgnore]
        //[TypeConverter(typeof(ExpandableObjectConverter)), ReadOnly(true)]
        //public new Point3D[] Vertices
        //{
        //    get { return base.vertices.ToArray(); }
        //}
        public Vector3D Normal
        {
            get { return this.normal; }
            set
            {
                this.normal = value.Normalized;
                this.Preprocess();
            }
        }
        [Browsable(true), ReadOnly(false)]
        public new int VerticesCount
        {
            get { return this.verticesCount; }
            set
            {
                if(value < 3){
                    throw new ArgumentOutOfRangeException("VerticesCount",
                                                          "The number of vertices must be greater than 3!");
                }
                this.verticesCount = value;
                this.Preprocess();
            }
        }
        public float Radius
        {
            get { return this.radius; }
            set
            {
                if(value <= 0){
                    throw new ArgumentOutOfRangeException("Radius", "Radius must be greater than zero!");
                }
                this.radius = value;
                this.Preprocess();
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
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
        public void Scale(float factor)
        {
            this.Radius = this.radius * factor;
        }
        public void Translate(float tx, float ty, float tz)
        {
            this.center.Translate(tx, ty, tz);
            this.Preprocess();
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override void Preprocess()
        {
            if(this.verticesCount >= 3){
                this.d = -(this.normal.X * this.center.X) - (this.normal.Y * this.center.Y)
                         - (this.normal.Z * this.center.Z);
                Vector3D inPlaneVector;
                Vector3D.Orthonormalize(this.normal, out inPlaneVector);
                Point3D initialPoint = this.center + this.radius * inPlaneVector;
                this.vertices.Clear();
                this.vertices.Add(initialPoint);
                float angleToRotation = (float)(2 * Math.PI) / this.verticesCount;
                for(int i = 1; i < this.verticesCount; i++){
                    inPlaneVector.Rotate(angleToRotation, this.normal);
                    initialPoint = this.center + this.radius * inPlaneVector;
                    this.vertices.Add(initialPoint);
                }
            }
        }
    }
}