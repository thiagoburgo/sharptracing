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
    public class Disc : Primitive, ITransformable3D
    {
        private double d;
        private double innerRadius;
        private Vector3D normal;
        private double radius;
        public Disc() : this(Point3D.Zero, Vector3D.UnitY, 20) {}
        public Disc(Point3D center, Vector3D normal, double radius)
        {
            this.center = center;
            this.Radius = radius;
            this.Normal = normal;
            this.d = -(normal.X * center.X) - (normal.Y * center.Y) - (normal.Z * center.Z);
        }
        public Disc(Point3D center, Vector3D normal, double radius, double innerRadius) : this(center, normal, radius)
        {
            this.InnerRadius = innerRadius;
        }
        [RefreshProperties(RefreshProperties.All)]
        public double Radius
        {
            get { return this.radius; }
            set
            {
                if(value > 0){
                    this.radius = value;
                } else{
                    throw new Exception("O Raio deve ter valor maior que ZERO!");
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public double InnerRadius
        {
            get { return this.innerRadius; }
            set
            {
                if(value >= 0 && value < this.radius){
                    this.innerRadius = value;
                } else{
                    throw new Exception("O Raio interno deve ter valor maior ou igual a ZERO e ser menor que o Raio!");
                }
            }
        }
        public Vector3D Normal
        {
            get { return this.normal; }
            set
            {
                value.Normalize();
                this.normal = value;
                this.d = -(this.normal.X * this.center.X) - (this.normal.Y * this.center.Y)
                         - (this.normal.Z * this.center.Z);
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Center
        {
            get { return base.center; }
            set
            {
                base.center = value;
                this.d = -(this.normal.X * this.center.X) - (this.normal.Y * this.center.Y)
                         - (this.normal.Z * this.center.Z);
            }
        }
        public double D
        {
            get { return this.d; }
        }

        #region ITransformable3D Members
        public void Rotate(double angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.Center = this.center;
        }
        public void RotateAxisX(double angle)
        {
            this.center.RotateAxisX(angle);
            this.Center = this.center;
        }
        public void RotateAxisY(double angle)
        {
            this.center.RotateAxisY(angle);
            this.Center = this.center;
        }
        public void RotateAxisZ(double angle)
        {
            this.center.RotateAxisZ(angle);
            this.Center = this.center;
        }
        public void Scale(double factor)
        {
            this.Radius = this.radius * factor;
            this.InnerRadius = this.innerRadius * factor;
        }
        public void Translate(double tx, double ty, double tz)
        {
            this.Translate(new Vector3D(tx, ty, tz));
        }
        public void Translate(Vector3D translateVector)
        {
            this.center.Translate(translateVector);
            this.Center = this.center;
        }
        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            //t = -(N • Ro + D) / (N • Rd)	
            //Vector3D origin = ray.Origin.ToVector3D();
            double NRd = this.normal * ray.Direction;
            if(NRd == 0.0d){
                return false;
            }
            double t = -(this.normal * ray.Origin + this.d) * 1.0d / NRd;
            if(t < 0.1d){
                return false;
            }
            intersect.HitPoint = ray.Origin + (t * ray.Direction);
            intersect.HitPrimitive = this;
            Vector3D hitToCenter = (intersect.HitPoint - this.center);
            double distanceToCenter = hitToCenter.Length;
            if(distanceToCenter > this.radius || distanceToCenter < this.innerRadius){
                return false;
            }
            intersect.Normal = this.normal;
            intersect.TMin = t;
            if(this.material != null && this.material.IsTexturized){
                double vdotuA = hitToCenter * this.normal;
                vdotuA = 0.5d * (1.0d - vdotuA);
                Vector3D b;
                Vector3D.Orthonormalize(this.normal, out b);
                hitToCenter += new Vector3D(0, 50, 0);
                hitToCenter.Normalize();
                double vdotuB = hitToCenter * -b;
                vdotuB = 0.5d * (1.0d + vdotuB);
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(vdotuA * widthTex), (int)(vdotuB * heightTex));
                intersect.CurrentTextureCoordinate.U = vdotuA;
                intersect.CurrentTextureCoordinate.V = vdotuB;
            }
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