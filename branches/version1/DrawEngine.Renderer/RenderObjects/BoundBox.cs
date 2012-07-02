/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
 using System;
using System.ComponentModel;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects
{
    [TypeConverter(typeof(ExpandableObjectConverter)), ReadOnly(true)]
    public struct BoundBox : ITransformable3D
    {
        private Point3D center;
        private Vector3D halfVector;
        private Point3D pMax;
        private Point3D pMin;
        public BoundBox(float xMin, float yMin, float zMin, float xMax, float yMax, float zMax)
        {
            this.pMin = new Point3D(xMin, yMin, zMin);
            this.pMax = new Point3D(xMax, yMax, zMax);
            this.halfVector = (this.pMax - this.pMin) * 0.5f;
            this.center = (this.pMax + this.pMin) * 0.5f;
        }
        public BoundBox(Point3D pMin, Point3D pMax)
        {
            this.pMin = pMin;
            this.pMax = pMax;
            this.halfVector = (pMax - pMin) * 0.5f;
            this.center = (pMax + pMin) * 0.5f;
        }
        public Point3D PMin
        {
            get { return this.pMin; }
            set
            {
                this.pMin = value;
                this.halfVector = (this.pMax - this.pMin) * 0.5f;
                this.center = (this.pMax + this.pMin) * 0.5f;
            }
        }
        public Point3D PMax
        {
            get { return this.pMax; }
            set
            {
                this.pMax = value;
                this.halfVector = (this.pMax - this.pMin) * 0.5f;
                this.center = (this.pMax + this.pMin) * 0.5f;
            }
        }
        public Point3D this[int index]
        {
            get
            {
                if(index % 2 == 0){
                    return this.pMin;
                } else{
                    return this.pMax;
                }
            }
            set
            {
                if(index % 2 == 0){
                    this.pMin = value;
                } else{
                    this.pMax = value;
                }
            }
        }
        public Point3D Center
        {
            get { return this.center; }
        }
        public Vector3D HalfVector
        {
            get { return this.halfVector; }
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
            this.pMin.Scale(factor);
            this.pMax.Scale(factor);
            this.PMax = this.pMax;
            this.PMin = this.pMin;
        }
        public void Translate(float tx, float ty, float tz)
        {
            this.pMin.Translate(tx, ty, tz);
            this.pMax.Translate(tx, ty, tz);
            //this.pMin = pMin;
            //this.pMax = pMax;
            this.halfVector = (this.pMax - this.pMin) * 0.5f;
            this.center = (this.pMax + this.pMin) * 0.5f;
            //this.PMax = this.pMax;
            //this.PMin = this.pMin;
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public bool IsInside(Point3D point)
        {
            return ((point.X >= this.pMin.X - 0.001f && point.X <= this.pMax.X + 0.001f)
                    && (point.Y >= this.pMin.Y - 0.001f && point.Y <= this.pMax.Y + 0.001f)
                    && (point.Z >= this.pMin.Z - 0.001f && point.Z <= this.pMax.Z + 0.001f));
        }
        public void IncludePoint(Point3D p)
        {
            this.pMin.X = p.X < this.pMin.X ? p.X : this.pMin.X;
            this.pMin.Y = p.Y < this.pMin.Y ? p.Y : this.pMin.Y;
            this.pMin.Z = p.Z < this.pMin.Z ? p.Z : this.pMin.Z;
            this.pMax.X = p.X > this.pMax.X ? p.X : this.pMax.X;
            this.pMax.Y = p.Y > this.pMax.Y ? p.Y : this.pMax.Y;
            this.pMax.Z = p.Z > this.pMax.Z ? p.Z : this.pMax.Z;
            this.PMin = this.pMin;
            this.PMax = this.pMax;
        }
        //public bool IsEmpty
        //{
        //    get { return (this == new BoundBox()); }
        //}
        public override string ToString()
        {
            return String.Format("[PMin<{0}> | PMax<{1}>]", this.pMin, this.pMax);
        }
        public static bool operator ==(BoundBox b1, BoundBox b2)
        {
            return b1.Equals(b2);
        }
        public static bool operator !=(BoundBox b1, BoundBox b2)
        {
            return !b1.Equals(b2);
        }
        public void Include(Point3D p)
        {
            if(p != Point3D.Zero){
                if(p.X < this.pMin.X){
                    this.pMin.X = p.X;
                }
                if(p.X > this.pMax.X){
                    this.pMax.X = p.X;
                }
                if(p.Y < this.pMin.Y){
                    this.pMin.Y = p.Y;
                }
                if(p.Y > this.pMax.Y){
                    this.pMax.Y = p.Y;
                }
                if(p.Z < this.pMin.Z){
                    this.pMin.Z = p.Z;
                }
                if(p.Z > this.pMax.Z){
                    this.pMax.Z = p.Z;
                }
            }
        }
        public bool Intersect(Ray ray)
        {
            //if (this.IsEmpty) {
            //    return false;
            //}
            float tmin, tmax, tymin, tymax, tzmin, tzmax;
            float oX = ray.Origin.X, oY = ray.Origin.Y, oZ = ray.Origin.Z;
            float inv_dX = ray.Inv_Direction.X, inv_dY = ray.Inv_Direction.Y, inv_dZ = ray.Inv_Direction.Z;
            if(inv_dX > 0){
                tmin = (this.pMin.X - oX) * inv_dX;
                tmax = (this.pMax.X - oX) * inv_dX;
            } else{
                tmin = (this.pMax.X - oX) * inv_dX;
                tmax = (this.pMin.X - oX) * inv_dX;
            }
            if(inv_dY > 0){
                tymin = (this.pMin.Y - oY) * inv_dY;
                tymax = (this.pMax.Y - oY) * inv_dY;
            } else{
                tymin = (this.pMax.Y - oY) * inv_dY;
                tymax = (this.pMin.Y - oY) * inv_dY;
            }
            if((tmin > tymax) || (tymin > tmax)){
                return false;
            }
            if(tymin > tmin){
                tmin = tymin;
            }
            if(tymax < tmax){
                tmax = tymax;
            }
            if(inv_dZ > 0){
                tzmin = (this.pMin.Z - oZ) * inv_dZ;
                tzmax = (this.pMax.Z - oZ) * inv_dZ;
            } else{
                tzmin = (this.pMax.Z - oZ) * inv_dZ;
                tzmax = (this.pMin.Z - oZ) * inv_dZ;
            }
            if((tmin > tzmax) || (tzmin > tmax)){
                return false;
            }
            if(tzmin > tmin){
                tmin = tzmin;
            }
            if(tzmax < tmax){
                tmax = tzmax;
            }
            return true;
        }
        public bool Intersect(Ray ray, out float t)
        {
            t = float.PositiveInfinity;
            //if (this.IsEmpty)
            //{
            //    return false;
            //}
            float tmin, tmax, tymin, tymax, tzmin, tzmax;
            float oX = ray.Origin.X, oY = ray.Origin.Y, oZ = ray.Origin.Z;
            float inv_dX = ray.Inv_Direction.X, inv_dY = ray.Inv_Direction.Y, inv_dZ = ray.Inv_Direction.Z;
            if(ray.Inv_Direction.X >= 0){
                tmin = (this.pMin.X - oX) * inv_dX;
                tmax = (this.pMax.X - oX) * inv_dX;
            } else{
                tmin = (this.pMax.X - oX) * inv_dX;
                tmax = (this.pMin.X - oX) * inv_dX;
            }
            if(ray.Inv_Direction.Y >= 0){
                tymin = (this.pMin.Y - oY) * inv_dY;
                tymax = (this.pMax.Y - oY) * inv_dY;
            } else{
                tymin = (this.pMax.Y - oY) * inv_dY;
                tymax = (this.pMin.Y - oY) * inv_dY;
            }
            if((tmin > tymax) || (tymin > tmax)){
                return false;
            }
            if(tymin > tmin){
                tmin = tymin;
            }
            if(tymax < tmax){
                tmax = tymax;
            }
            if(ray.Inv_Direction.Z >= 0){
                tzmin = (this.pMin.Z - oZ) * inv_dZ;
                tzmax = (this.pMax.Z - oZ) * inv_dZ;
            } else{
                tzmin = (this.pMax.Z - oZ) * inv_dZ;
                tzmax = (this.pMin.Z - oZ) * inv_dZ;
            }
            if((tmin > tzmax) || (tzmin > tmax)){
                return false;
            }
            if(tzmin > tmin){
                tmin = tzmin;
            }
            if(tzmax < tmax){
                tmax = tzmax;
            }
            t = tmin;
            return true;
        }
        public bool Intersect(Ray ray, ref float ltmin, ref float ltmax)
        {
            ltmin = float.MaxValue;
            ltmax = float.MaxValue;
            float tmin, tmax, tymin, tymax, tzmin, tzmax;
            if(ray.Inv_Direction.X >= 0){
                tmin = (this.pMin.X - ray.Origin.X) * ray.Inv_Direction.X;
                tmax = (this.pMax.X - ray.Origin.X) * ray.Inv_Direction.X;
            } else{
                tmin = (this.pMax.X - ray.Origin.X) * ray.Inv_Direction.X;
                tmax = (this.pMin.X - ray.Origin.X) * ray.Inv_Direction.X;
            }
            if(ray.Inv_Direction.Y >= 0){
                tymin = (this.pMin.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
                tymax = (this.pMax.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
            } else{
                tymin = (this.pMax.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
                tymax = (this.pMin.Y - ray.Origin.Y) * ray.Inv_Direction.Y;
            }
            if((tmin > tymax) || (tymin > tmax)){
                return false;
            }
            if(tymin > tmin){
                tmin = tymin;
            }
            if(tymax < tmax){
                tmax = tymax;
            }
            if(ray.Inv_Direction.Z >= 0){
                tzmin = (this.pMin.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
                tzmax = (this.pMax.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
            } else{
                tzmin = (this.pMax.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
                tzmax = (this.pMin.Z - ray.Origin.Z) * ray.Inv_Direction.Z;
            }
            if((tmin > tzmax) || (tzmin > tmax)){
                return false;
            }
            if(tzmin > tmin){
                tmin = tzmin;
            }
            if(tzmax < tmax){
                tmax = tzmax;
            }
            if((tmin < ltmin) && (tmax > 0)){
                ltmin = tmin;
                ltmax = tmax;
                return true;
            }
            return false;
        }
    }
}