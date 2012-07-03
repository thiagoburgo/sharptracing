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
    //TODO analizar e melhorar essa primitiva, Implementar IPreprocess
    [Serializable]
    public class Ellipsoid : Primitive, ITransformable3D
    {
        private Vector3D axisA;
        private Vector3D axisANonNorm;
        private Vector3D axisB, axisC;
        private Vector3D axisCNonNorm;
        // private TextureType textureType = TextureType.Spherical;
        private double radiusA, radiusB, radiusC;
        public Ellipsoid() : this(Point3D.Zero, Vector3D.UnitY, 20, 20, 35) {}
        //public Ellipsoid(Point3D center, Vector3D axisA, Vector3D axisC, double radiusA, double radiusB, double radiusC)
        //{
        //    this.center = center;
        //    this.CentralAxis = axisC;
        //    this.RadialAxis = axisA;
        //    this.RadiusA = radiusA;
        //    this.RadiusB = radiusB;
        //    this.RadiusC = radiusC;
        //}
        public Ellipsoid(Point3D center, Vector3D axisC, double radiusA, double radiusB, double radiusC)
        {
            this.center = center;
            this.CentralAxis = axisC;
            this.RadiusA = radiusA;
            this.RadiusB = radiusB;
            this.RadiusC = radiusC;
        }
        /*
        private void RecalculateBoundBox() {
            double L2 = (radiusA * radiusA) + (radiusC * radiusC);
            double t = Math.Sqrt(L2 + (radiusB * radiusB));
            this.boundBox.PMin = (this.axisA - this.axisB - this.axisC) * t + center;
            this.boundBox.PMax = (this.axisA + this.axisB + this.axisC) * t + center;
        }
         */
        [RefreshProperties(RefreshProperties.All)]
        public Vector3D CentralAxis
        {
            get { return this.axisCNonNorm; }
            set
            {
                if(value.Length == 0.0d){
                    throw new Exception("O eixo central nao pode ter comprimento Zero!");
                }
                this.axisC = value;
                this.axisCNonNorm = value;
                this.axisC.Normalize();
                Vector3D.Orthonormalize(this.axisC, out this.axisA, out this.axisB);
            }
        }
        //[RefreshProperties(RefreshProperties.All)]
        //public Vector3D RadialAxis
        //{
        //    get { return this.axisANonNorm; }
        //    set
        //    {
        //        this.axisA = value;
        //        this.axisANonNorm = value;
        //        Vector3D.Orthonormalize(this.axisC, out this.axisA);
        //        //if((this.axisA * this.axisC) != 0.0d){
        //        //    throw new Exception("Erro: Os eixos A e C nao sao ortogonais!");
        //        //}
        //        this.axisB = this.axisC ^ this.axisA; // Form 3rd axis with crossproduct
        //        this.axisA.Normalize();
        //        this.axisC.Normalize();
        //        this.axisB.Normalize();
        //    }
        //}
        [RefreshProperties(RefreshProperties.All)]
        public double RadiusA
        {
            get { return this.radiusA; }
            set
            {
                if(value > 0.0d){
                    this.radiusA = value;
                    this.axisA.Normalize();
                    this.axisA /= this.radiusA;
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public double RadiusB
        {
            get { return this.radiusB; }
            set
            {
                if(value > 0.0d){
                    this.radiusB = value;
                    this.axisB.Normalize();
                    this.axisB /= this.radiusB;
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public double RadiusC
        {
            get { return this.radiusC; }
            set
            {
                if(value > 0.0d){
                    this.radiusC = value;
                    this.axisC.Normalize();
                    this.axisC /= this.radiusC;
                }
            }
        }

        #region ITransformable3D Members
        public void Rotate(double angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
        }
        public void RotateAxisX(double angle)
        {
            this.center.Rotate(angle, Vector3D.UnitX);
        }
        public void RotateAxisY(double angle)
        {
            this.center.Rotate(angle, Vector3D.UnitY);
        }
        public void RotateAxisZ(double angle)
        {
            this.center.Rotate(angle, Vector3D.UnitZ);
        }
        public void Scale(double factor)
        {
            this.RadiusA = this.radiusA * factor;
            this.RadiusB = this.radiusB * factor;
            this.RadiusC = this.radiusC * factor;
        }
        public void Translate(double tx, double ty, double tz)
        {
            this.center.Translate(tx, ty, tz);
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            Vector3D v = ray.Origin - this.center;
            double pdotuA = v * this.axisA;
            double pdotuB = v * this.axisB;
            double pdotuC = v * this.axisC;
            double udotuA = ray.Direction * this.axisA;
            double udotuB = ray.Direction * this.axisB;
            double udotuC = ray.Direction * this.axisC;
            double C = (pdotuA * pdotuA) + (pdotuB * pdotuB) + (pdotuC * pdotuC) - 1.0d;
            double B = (pdotuA * udotuA + pdotuB * udotuB + pdotuC * udotuC);
            if(C > 0.0d && B >= 0.0d){
                return false; // Pointing away from the ellipsoid
            }
            B += B; // Double B to get final factor of 2.
            double A = (udotuA * udotuA) + (udotuB * udotuB) + (udotuC * udotuC);
            double alpha1, alpha2;
            int numRoots = EquationSolver.SolveQuadric(A, B, C, out alpha1, out alpha2);
            if(numRoots == 0){
                return false;
            }
            if(alpha1 > 0.01d){
                // Found an intersection from outside.		    
                intersect.TMin = alpha1;
                intersect.TMax = alpha2;
                intersect.HitFromInSide = false;
            } else if(numRoots == 2 && alpha2 > 0.01d){
                // Found an intersection from inside.		
                intersect.TMin = alpha2;
                intersect.TMax = alpha1;
                intersect.HitFromInSide = true;
            } else{
                return false; // Both intersections behind us (should never get here)
            }
            // Calculate intersection position
            intersect.HitPoint = ray.Origin + intersect.TMin * ray.Direction;
            intersect.Normal = intersect.HitPoint - this.center; // Now v is the relative position
            double vdotuA = intersect.Normal * this.axisA;
            double vdotuB = intersect.Normal * this.axisB;
            double vdotuC = intersect.Normal * this.axisC;
            intersect.Normal = vdotuA * this.axisA + vdotuB * this.axisB + vdotuC * this.axisC;
            intersect.Normal.Normalize();
            intersect.HitPrimitive = this;
            if(this.material != null && this.material.IsTexturized){
                double uCoord = Math.Atan2(-vdotuB, vdotuA) * (1.0 / (Math.PI + Math.PI));
                double vCoord = 1.0 - Math.Cos(-vdotuC) * (1.0 / Math.PI);
                if(uCoord < 0.0){
                    uCoord++;
                }
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(uCoord * widthTex), (int)(vCoord * heightTex));
                intersect.CurrentTextureCoordinate.U = uCoord;
                intersect.CurrentTextureCoordinate.V = vCoord;
            }
            return true;
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            Vector3D normal = pointInPrimitive - this.center; // Now v is the relative position
            double vdotuA = normal * this.axisA;
            double vdotuB = normal * this.axisB;
            double vdotuC = normal * this.axisC;
            normal = vdotuA * this.axisA + vdotuB * this.axisB + vdotuC * this.axisC;
            normal.Normalize();
            return normal;
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