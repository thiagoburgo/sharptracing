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
        private float radiusA, radiusB, radiusC;
        public Ellipsoid() : this(Point3D.Zero, Vector3D.UnitY, 20, 20, 35) {}
        //public Ellipsoid(Point3D center, Vector3D axisA, Vector3D axisC, float radiusA, float radiusB, float radiusC)
        //{
        //    this.center = center;
        //    this.CentralAxis = axisC;
        //    this.RadialAxis = axisA;
        //    this.RadiusA = radiusA;
        //    this.RadiusB = radiusB;
        //    this.RadiusC = radiusC;
        //}
        public Ellipsoid(Point3D center, Vector3D axisC, float radiusA, float radiusB, float radiusC)
        {
            this.center = center;
            this.CentralAxis = axisC;
            this.RadiusA = radiusA;
            this.RadiusB = radiusB;
            this.RadiusC = radiusC;
        }
        /*
        private void RecalculateBoundBox() {
            float L2 = (radiusA * radiusA) + (radiusC * radiusC);
            float t = (float)Math.Sqrt(L2 + (radiusB * radiusB));
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
                if(value.Length == 0.0f){
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
        //        //if((this.axisA * this.axisC) != 0.0f){
        //        //    throw new Exception("Erro: Os eixos A e C nao sao ortogonais!");
        //        //}
        //        this.axisB = this.axisC ^ this.axisA; // Form 3rd axis with crossproduct
        //        this.axisA.Normalize();
        //        this.axisC.Normalize();
        //        this.axisB.Normalize();
        //    }
        //}
        [RefreshProperties(RefreshProperties.All)]
        public float RadiusA
        {
            get { return this.radiusA; }
            set
            {
                if(value > 0.0f){
                    this.radiusA = value;
                    this.axisA.Normalize();
                    this.axisA /= this.radiusA;
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public float RadiusB
        {
            get { return this.radiusB; }
            set
            {
                if(value > 0.0f){
                    this.radiusB = value;
                    this.axisB.Normalize();
                    this.axisB /= this.radiusB;
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public float RadiusC
        {
            get { return this.radiusC; }
            set
            {
                if(value > 0.0f){
                    this.radiusC = value;
                    this.axisC.Normalize();
                    this.axisC /= this.radiusC;
                }
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
        }
        public void RotateAxisX(float angle)
        {
            this.center.Rotate(angle, Vector3D.UnitX);
        }
        public void RotateAxisY(float angle)
        {
            this.center.Rotate(angle, Vector3D.UnitY);
        }
        public void RotateAxisZ(float angle)
        {
            this.center.Rotate(angle, Vector3D.UnitZ);
        }
        public void Scale(float factor)
        {
            this.RadiusA = this.radiusA * factor;
            this.RadiusB = this.radiusB * factor;
            this.RadiusC = this.radiusC * factor;
        }
        public void Translate(float tx, float ty, float tz)
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
            float pdotuA = v * this.axisA;
            float pdotuB = v * this.axisB;
            float pdotuC = v * this.axisC;
            float udotuA = ray.Direction * this.axisA;
            float udotuB = ray.Direction * this.axisB;
            float udotuC = ray.Direction * this.axisC;
            float C = (pdotuA * pdotuA) + (pdotuB * pdotuB) + (pdotuC * pdotuC) - 1.0f;
            float B = (pdotuA * udotuA + pdotuB * udotuB + pdotuC * udotuC);
            if(C > 0.0f && B >= 0.0f){
                return false; // Pointing away from the ellipsoid
            }
            B += B; // Double B to get final factor of 2.
            float A = (udotuA * udotuA) + (udotuB * udotuB) + (udotuC * udotuC);
            float alpha1, alpha2;
            int numRoots = EquationSolver.SolveQuadric(A, B, C, out alpha1, out alpha2);
            if(numRoots == 0){
                return false;
            }
            if(alpha1 > 0.01f){
                // Found an intersection from outside.		    
                intersect.TMin = alpha1;
                intersect.TMax = alpha2;
                intersect.HitFromInSide = false;
            } else if(numRoots == 2 && alpha2 > 0.01f){
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
            float vdotuA = intersect.Normal * this.axisA;
            float vdotuB = intersect.Normal * this.axisB;
            float vdotuC = intersect.Normal * this.axisC;
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
                intersect.CurrentTextureCoordinate.U = (float)uCoord;
                intersect.CurrentTextureCoordinate.V = (float)vCoord;
            }
            return true;
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            Vector3D normal = pointInPrimitive - this.center; // Now v is the relative position
            float vdotuA = normal * this.axisA;
            float vdotuB = normal * this.axisB;
            float vdotuC = normal * this.axisC;
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