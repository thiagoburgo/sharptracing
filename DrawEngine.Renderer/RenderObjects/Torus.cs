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
    public class Torus : Primitive, ITransformable3D
    {
        private Vector3D axisA = Vector3D.UnitZ, axisB = Vector3D.UnitY, axisC = Vector3D.UnitX;
        private float majorRadius;
        private float majorRadius2;
        private float minorRadius;
        private float minorRadius2;
        private float outerRadius;
        //private float centerCoefA, centerCoefB, centerCoefC;
        public Torus(Point3D center, Vector3D centralAxis, float minorRadius, float majorRadius)
        {
            this.Center = center;
            this.CentralAxis = (centralAxis);
            this.MinorRadius = minorRadius;
            this.MajorRadius = majorRadius;
            this.RecalculateBoundBox();
        }
        public Torus(Point3D center, float minorRadius, float majorRadius)
                : this(center, Vector3D.UnitY, minorRadius, majorRadius) {}
        public Torus() : this(new Point3D(), 15, 25) {}
        [RefreshProperties(RefreshProperties.All)]
        public Vector3D RadialAxis
        {
            get { return this.axisA; }
            set
            {
                this.axisA = value;
                this.axisA -= (this.axisA * this.axisC) * this.axisC; // Make perpindicular to Center Axis
                if(this.axisA.Length == 0.0f){
                    // Must not be parallel to Center Axis
                    //throw new Exception("Erro: O eixo central nao pode ter tamanho ZERO!");
                }
                //this.axisA.Normalize();
                this.axisB = this.axisC ^ this.axisA; // Get third orthonormal axis (crossproduct)
                //this.axisB.Normalize();
                this.RecalcCoeficients();
                this.RecalculateBoundBox();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Vector3D CentralAxis
        {
            get { return this.axisC; }
            set
            {
                this.axisC = value;
                Vector3D.Orthonormalize(this.axisC, out this.axisA, out this.axisB);
                // Form right handed coordinate system
                this.RecalcCoeficients();
                this.RecalculateBoundBox();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public Point3D Center
        {
            get { return base.center; }
            set
            {
                base.center = value;
                this.RecalcCoeficients();
                this.RecalculateBoundBox();
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public float MinorRadius
        {
            get { return this.minorRadius; }
            set
            {
                if(value > 0.0f){
                    this.minorRadius = value;
                    this.minorRadius2 = value * value;
                    this.RecalcCoeficients();
                    this.RecalculateBoundBox();
                }
            }
        }
        [RefreshProperties(RefreshProperties.All)]
        public float MajorRadius
        {
            get { return this.majorRadius; }
            set
            {
                if(value > 0.0f){
                    this.majorRadius = value;
                    this.majorRadius2 = value * value;
                    this.RecalcCoeficients();
                    this.RecalculateBoundBox();
                }
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.Center = this.center;
            this.axisC.Rotate(angle, axis);
            this.CentralAxis = this.axisC;
            this.axisA.Rotate(angle, axis);
            this.RadialAxis = this.axisA;
        }
        public void RotateAxisX(float angle)
        {
            this.center.RotateAxisX(angle);
            this.Center = this.center;
            this.axisC.RotateAxisX(angle);
            this.CentralAxis = this.axisC;
            this.axisA.RotateAxisX(angle);
            this.RadialAxis = this.axisA;
        }
        public void RotateAxisY(float angle)
        {
            this.center.RotateAxisY(angle);
            this.Center = this.center;
            this.axisC.RotateAxisY(angle);
            this.CentralAxis = this.axisC;
            this.axisA.RotateAxisY(angle);
            this.RadialAxis = this.axisA;
        }
        public void RotateAxisZ(float angle)
        {
            this.center.RotateAxisZ(angle);
            this.Center = this.center;
            this.axisC.RotateAxisZ(angle);
            this.CentralAxis = this.axisC;
            this.axisA.RotateAxisZ(angle);
            this.RadialAxis = this.axisA;
        }
        public void Scale(float factor)
        {
            this.MinorRadius *= factor;
            this.MajorRadius *= factor;
        }
        public void Translate(float tx, float ty, float tz)
        {
            this.center.Translate(tx, ty, tz);
            this.Center = this.center;
        }
        public void Translate(Vector3D translateVector)
        {
            this.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        #endregion

        private void RecalculateBoundBox()
        {
            Point3D pMin = this.center + (-this.axisC * this.minorRadius);
            pMin = pMin - this.axisA * (this.outerRadius + this.minorRadius);
            pMin = pMin + this.axisB * (this.outerRadius + this.minorRadius);
            Point3D pMax = this.center + (this.axisC * this.minorRadius);
            pMax = pMax + this.axisA * (this.outerRadius + this.minorRadius);
            pMax = pMax - this.axisB * (this.outerRadius + this.minorRadius);
            this.boundBox = new BoundBox(pMin, pMax);
        }
        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            float A = 1.0f;
            Vector3D viewPosRel = ray.Origin - this.center; // Origin relative to center
            float udotp = (ray.Direction * viewPosRel);
            float B = (udotp + udotp + udotp + udotp);
            float RadiiSqSum = this.majorRadius2 + this.minorRadius2;
            float ucdotp = (this.axisC * viewPosRel);
            float ucdotu = (this.axisC * ray.Direction);
            float pSq = (viewPosRel * viewPosRel);
            float C = B * udotp + (pSq + pSq) - (RadiiSqSum + RadiiSqSum) + 4.0f * this.majorRadius2 * ucdotu * ucdotu;
            float D = 4.0f * ((pSq - RadiiSqSum) * udotp + (this.majorRadius2 + this.majorRadius2) * ucdotp * ucdotu);
            float E = (pSq - (RadiiSqSum + RadiiSqSum)) * pSq + 4.0f * this.majorRadius2 * ucdotp * ucdotp
                      + ((this.majorRadius2 - this.minorRadius2) * (this.majorRadius2 - this.minorRadius2));
            float[] roots = {0f, 0f, 0f, 0f};
            int numRoots = EquationSolver.SolveQuartic(A, B, C, D, E, out roots[0], out roots[1], out roots[2],
                                                       out roots[3]);
            if(numRoots > 0){
                if(roots[0] > 0.1f){
                    intersect.TMin = roots[0];
                    intersect.HitPoint = ray.Origin + intersect.TMin * ray.Direction;
                    // Intersection position (not relative to center)
                    intersect.HitPrimitive = this;
                    // Outward normal
                    Vector3D h = intersect.HitPoint - this.center; // Now its the relative point
                    float xCoord = h * this.axisA; // forward axis
                    float yCoord = h * this.axisB; // rightward axis
                    float zCoord = h * this.axisC; // upward axis
                    intersect.Normal = this.axisC * -zCoord + h;
                    float outNnorm = intersect.Normal.Length;
                    intersect.Normal.Normalize();
                    intersect.Normal = intersect.Normal * -this.majorRadius + h;
                    // Negative point projected to center path of torus
                    intersect.Normal.Normalize(); // Fix roundoff error problems
                    if(this.material != null && this.material.IsTexturized){
                        // u - v coordinates
                        double u = Math.Atan2(yCoord, xCoord);
                        u = u * (0.5 / Math.PI) + 0.5;
                        double bVal = outNnorm - this.majorRadius;
                        double v = Math.Atan2(zCoord, bVal);
                        v = v * (0.5 / Math.PI) + 0.5;
                        //int widthTex = this.material.Texture.Width - 1;
                        //int heightTex = this.material.Texture.Height - 1;
                        //this.material.Color = this.material.Texture.GetPixel((int)(u * widthTex), (int)(v * heightTex));
                        this.currentTextureCoordinate.U = (float)u;
                        this.currentTextureCoordinate.V = (float)v;
                    }
                    return true;
                }
            }
            return false;
        }
        private void RecalcCoeficients()
        {
            //this.centerCoefA = (this.center*this.axisA);
            //this.centerCoefB = (this.center*this.axisB);
            //this.centerCoefC = (this.center*this.axisC); // CenterAxis
            this.outerRadius = this.majorRadius + this.minorRadius;
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            // Outward normal
            Vector3D h = pointInPrimitive - this.center; // Now its the relative point
            Vector3D normal = this.axisC * -(h * this.axisC) + h;
            normal.Normalize();
            normal = normal * -this.majorRadius + h; // Negative point projected to center path of torus
            normal.Normalize(); // Fix roundoff error problems
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

        #region
        //private bool CollideTwoPlanes(float pdotn, float alpha, float dimen, ref bool insideFlag, ref float minDistBack, ref float maxDistFront) {
        //    if (alpha > 0.0f) {
        //        if (pdotn >= dimen) {
        //            return false;			// Above top & pointing up
        //        }
        //        float temp = dimen + pdotn;
        //        if (temp < 0.0f) {
        //            insideFlag = false;
        //            // Handle bottom from below 
        //            if (alpha * maxDistFront < -temp) {
        //                maxDistFront = -temp / alpha;
        //            }
        //            temp = (dimen - pdotn) / alpha;				// Dist thru to top
        //            if (temp < minDistBack) {
        //                minDistBack = (dimen - pdotn) / alpha;	// 2nd intersect w/ top
        //            }
        //            if (maxDistFront > minDistBack) {
        //                return false;
        //            }
        //        } else {
        //            // Handle top from inside
        //            temp = dimen - pdotn;
        //            if (alpha * minDistBack > temp) {
        //                minDistBack = temp / alpha;
        //                if (maxDistFront > minDistBack) {
        //                    return false;
        //                }
        //            }
        //        }
        //    } else if (alpha < 0.0f) {
        //        if (pdotn <= -dimen) {
        //            return false;			// Below bottom and pointing down
        //        }
        //        float temp = pdotn - dimen;
        //        if (temp > 0.0f) {
        //            insideFlag = false;
        //            // Handle top from above
        //            if (-alpha * maxDistFront < temp) {
        //                maxDistFront = (-temp / alpha);
        //            }
        //            temp = -(pdotn + dimen) / alpha;	// Dist. thru to bottom
        //            if (temp < minDistBack) {
        //                minDistBack = temp;		// 2nd intersect w/ bottom
        //            }
        //            if (maxDistFront > minDistBack) {
        //                return false;
        //            }
        //        } else {
        //            // Handle bottom from inside
        //            temp = pdotn + dimen;
        //            if (-alpha * minDistBack > temp) {
        //                minDistBack = -temp / alpha;
        //                if (maxDistFront > minDistBack) {
        //                    return false;
        //                }
        //            }
        //        }
        //    } else {		// alpha==0.0
        //        if (pdotn < -dimen || pdotn > dimen) {
        //            return false;
        //        }
        //    }
        //    return true;
        //}
        #endregion
    }
}