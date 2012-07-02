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
    [Serializable]
    public class Sphere : Primitive, ITransformable3D
    {
        private float radius;
        private float radius2;
        public Sphere() : this(20.0f, new Point3D()) {}
        public Sphere(float radius, Point3D center)
        {
            this.radius = radius;
            this.radius2 = radius * radius;
            this.center = center;
            this.RecalcBoundBox();
        }
        //public override bool IntersectPoint(out Intersection intersect, Ray ray) {
        //    intersect = new Intersection();
        //    Vector3D v = ray.Origin - center;
        //    float b = -(v * ray.Direction);
        //    float det = (b * b) - (v * v) + radius2;
        //    bool hit = false;
        //    if (det > 0) {
        //        det = (float)Math.Sqrt(det);
        //        float t1 = b - det;
        //        float t2 = b + det;
        //        if (t2 > 0.1) {
        //            if (t1 < 0.1) {
        //                intersect.TMin = t2;
        //                intersect.HitFromOutSide = false;
        //                hit = true;
        //                //if (t2 < a_Dist) {
        //                //    a_Dist = t2;
        //                //    retval = INPRIM;
        //                //}
        //            } else {
        //                intersect.TMin = t1;
        //                intersect.HitFromOutSide = true;
        //                hit = true;
        //                //if (t1 < a_Dist) {
        //                //    a_Dist = t1;
        //                //    retval = HIT;
        //                //}
        //            }
        //        }
        //    }  
        //    if (hit) {
        //        if (intersect.TMin < 0.1) return false;
        //        intersect.HitPoint = ray.Origin + ray.Direction * intersect.TMin;
        //        intersect.Normal = intersect.HitPoint - center;
        //        intersect.HitPrimitive = this;
        //        intersect.Normal.Normalize();
        //        if (this.material != null && this.material.IsTexturized) {
        //            double uCoord, vCoord;
        //            double theta = Math.Atan2(-intersect.Normal.X, intersect.Normal.Z);
        //            double temp = -intersect.Normal.Y;
        //            double phi = Math.Acos(temp);
        //            uCoord = theta * (1.0 / (System.Math.PI + System.Math.PI));
        //            vCoord = 1.0 - phi * (1.0 / System.Math.PI);
        //            if (uCoord < 0.0) {
        //                uCoord++;
        //            }
        //            int w = this.material.Texture.Width;
        //            int h = this.material.Texture.Height;
        //            this.material.Color = this.material.Texture.GetPixel((int)(w * uCoord), (int)(h * vCoord));
        //        }
        //    }
        //    return hit;
        //}
        [RefreshProperties(RefreshProperties.All)]
        public float Radius
        {
            get { return this.radius; }
            set
            {
                this.radius = value;
                this.radius2 = this.radius * this.radius;
                this.RecalcBoundBox();
            }
        }
        public Point3D Center
        {
            get { return base.center; }
            set
            {
                base.center = value;
                this.RecalcBoundBox();
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.RecalcBoundBox();
        }
        public void RotateAxisX(float angle)
        {
            this.center.RotateAxisX(angle);
            this.RecalcBoundBox();
        }
        public void RotateAxisY(float angle)
        {
            this.center.RotateAxisY(angle);
            this.RecalcBoundBox();
        }
        public void RotateAxisZ(float angle)
        {
            this.center.RotateAxisZ(angle);
            this.RecalcBoundBox();
        }
        public void Scale(float factor)
        {
            this.radius = this.radius * factor;
            this.radius2 = this.radius * this.radius;
            this.RecalcBoundBox();
        }
        public void Translate(float tx, float ty, float tz)
        {
            this.center.Translate(tx, ty, tz);
            this.RecalcBoundBox();
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
            this.RecalcBoundBox();
        }
        #endregion

        private void RecalcBoundBox()
        {
            this.boundBox = new BoundBox((this.center.X - this.radius), (this.center.Y - this.radius),
                                         (this.center.Z - this.radius), (this.center.X + this.radius),
                                         (this.center.Y + this.radius), (this.center.Z + this.radius));
        }
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            if(!this.boundBox.Intersect(ray)){
                return false;
            }
            Vector3D toCenter = this.center - ray.Origin; // Vector view position to the center
            // D = Distance to pt on view line closest to sphere
            // v = vector from sphere center to the closest pt on view line
            // ASq = the distance from v to sphere center squared
            float D = (ray.Direction * toCenter);
            Vector3D v = ray.Direction * D - toCenter;
            float ASq = v.Length;
            ASq = ASq * ASq;
            // Ray-line completely misses sphere, or just grazes it.
            if(ASq >= this.radius2){
                return false;
            }
            float BSq = this.radius2 - ASq;
            if(D > 0.0 || D * D > BSq){
                // Return the point where view intersects with the outside of the sphere.
                intersect.TMin = D - (float)Math.Sqrt(BSq);
                intersect.TMax = D + (float)Math.Sqrt(BSq);
                intersect.HitFromInSide = false;
            } else if((D > 0.0 || D * D < BSq)){
                // return the point where view exits the sphere
                intersect.TMin = D + (float)Math.Sqrt(BSq);
                intersect.TMax = D - (float)Math.Sqrt(BSq);
                intersect.HitFromInSide = true;
            } else{
                return false;
            }
            if(intersect.TMin < 0.01){
                return false;
            }
            intersect.HitPoint = ray.Origin + ray.Direction * intersect.TMin;
            intersect.Normal = intersect.HitPoint - this.center;
            intersect.HitPrimitive = this;
            intersect.Normal.Normalize();
            if(this.material != null && this.material.IsTexturized){
                double uCoord, vCoord;
                double theta = Math.Atan2(-intersect.Normal.X, intersect.Normal.Z);
                double temp = -intersect.Normal.Y;
                double phi = Math.Acos(temp);
                uCoord = theta * (1.0 / (Math.PI + Math.PI));
                vCoord = 1.0 - phi * (1.0 / Math.PI);
                if(uCoord < 0.0){
                    uCoord++;
                }
                this.currentTextureCoordinate.U = (float)uCoord;
                this.currentTextureCoordinate.V = (float)vCoord;
                //int w = this.material.Texture.Width;
                //int h = this.material.Texture.Height;
                //this.material.Color = this.material.Texture.GetPixel((int)(w * uCoord), (int)(h * vCoord));
            }
            return true;
        }
        public override string ToString()
        {
            return base.ToString() + "[" + this.Center + "," + " R=" + this.radius + "]";
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            Vector3D normal = (pointInPrimitive - this.center);
            normal.Normalize();
            return normal;
        }
        public override bool IsInside(Point3D point)
        {
            Vector3D hitToCenter = (point - this.center);
            float distanceToCenter = hitToCenter.Length;
            if(distanceToCenter > this.radius + 0.001f){
                return false;
            }
            return true;
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }
    }
}