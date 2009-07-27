using System;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.BasicStructures
{
    public struct Ray : ITransformable3D
    {
        private Vector3D direction;
        private Vector3D inv_direction;
        public Point3D Origin;
        public float PrevRefractIndex;
        public Primitive PrevPrimitive;
        public Ray(Point3D origin, Vector3D direction)
        {
            this.direction = direction;
            this.direction.Normalize();
            this.inv_direction.X = 1.0f / this.direction.X;
            this.inv_direction.Y = 1.0f / this.direction.Y;
            this.inv_direction.Z = 1.0f / this.direction.Z;
            this.Origin = origin;
            this.PrevRefractIndex = -1;
            this.PrevPrimitive = null;
        }
        public Vector3D Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                this.direction.Normalize();
                //this.inv_direction.X = this.direction.X != 0f ? 1.0f / this.direction.X : 1f;
                //this.inv_direction.Y = this.direction.Y != 0f ? 1.0f / this.direction.Y : 1f;
                //this.inv_direction.Z = this.direction.Z != 0f ? 1.0f / this.direction.Z : 1f;
                this.inv_direction.X = 1.0f / this.direction.X;
                this.inv_direction.Y = 1.0f / this.direction.Y;
                this.inv_direction.Z = 1.0f / this.direction.Z;
            }
        }
        public Vector3D Inv_Direction
        {
            get { return this.inv_direction; }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisX(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisY(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void RotateAxisZ(float angle)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Scale(float factor)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Translate(float tx, float ty, float tz)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override string ToString()
        {
            return "[O: " + this.Origin+ " D: " + this.direction+ "]";
        }
    }
}