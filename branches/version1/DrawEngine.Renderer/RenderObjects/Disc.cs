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
        private float d;
        private float innerRadius;
        private Vector3D normal;
        private float radius;
        public Disc() : this(Point3D.Zero, Vector3D.UnitY, 20) {}
        public Disc(Point3D center, Vector3D normal, float radius)
        {
            this.center = center;
            this.Radius = radius;
            this.Normal = normal;
            this.d = -(normal.X * center.X) - (normal.Y * center.Y) - (normal.Z * center.Z);
        }
        public Disc(Point3D center, Vector3D normal, float radius, float innerRadius) : this(center, normal, radius)
        {
            this.InnerRadius = innerRadius;
        }
        [RefreshProperties(RefreshProperties.All)]
        public float Radius
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
        public float InnerRadius
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
        public float D
        {
            get { return this.d; }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.Center = this.center;
        }
        public void RotateAxisX(float angle)
        {
            this.center.RotateAxisX(angle);
            this.Center = this.center;
        }
        public void RotateAxisY(float angle)
        {
            this.center.RotateAxisY(angle);
            this.Center = this.center;
        }
        public void RotateAxisZ(float angle)
        {
            this.center.RotateAxisZ(angle);
            this.Center = this.center;
        }
        public void Scale(float factor)
        {
            this.Radius = this.radius * factor;
            this.InnerRadius = this.innerRadius * factor;
        }
        public void Translate(float tx, float ty, float tz)
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
            float NRd = this.normal * ray.Direction;
            if(NRd == 0.0f){
                return false;
            }
            float t = -(this.normal * ray.Origin + this.d) * 1.0f / NRd;
            if(t < 0.1f){
                return false;
            }
            intersect.HitPoint = ray.Origin + (t * ray.Direction);
            intersect.HitPrimitive = this;
            Vector3D hitToCenter = (intersect.HitPoint - this.center);
            float distanceToCenter = hitToCenter.Length;
            if(distanceToCenter > this.radius || distanceToCenter < this.innerRadius){
                return false;
            }
            intersect.Normal = this.normal;
            intersect.TMin = t;
            if(this.material != null && this.material.IsTexturized){
                float vdotuA = hitToCenter * this.normal;
                vdotuA = 0.5f * (1.0f - vdotuA);
                Vector3D b;
                Vector3D.Orthonormalize(this.normal, out b);
                hitToCenter += new Vector3D(0, 50, 0);
                hitToCenter.Normalize();
                float vdotuB = hitToCenter * -b;
                vdotuB = 0.5f * (1.0f + vdotuB);
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(vdotuA * widthTex), (int)(vdotuB * heightTex));
                this.currentTextureCoordinate.U = vdotuA;
                this.currentTextureCoordinate.V = vdotuB;
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