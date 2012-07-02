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
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.RenderObjects
{
    [Serializable]
    public class Cylinder : Primitive, ITransformable3D
    {
        private Vector3D axisA;
        private Vector3D axisB;
        private Plane bottom = new Plane();
        private float centerDotcentralAxis;
        private Vector3D centralAxis;
        private float halfHeight;
        private float height;
        private Point3D pBase;
        private Point3D pTop;
        private float radiusA;
        private float radiusB;
        private Plane top = new Plane();
        public Cylinder() : this(Point3D.Zero, new Point3D(0, 20, 0), 20) {}
        public Cylinder(Point3D pBase, Point3D pTop, float radius)
                : this(pBase, pTop, radius, radius, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)) {}
        public Cylinder(Point3D pBase, Point3D pTop, float radiusA, float radiusB, Vector3D axisA, Vector3D axisB)
        {
            this.pBase = pBase;
            this.pTop = pTop;
            this.center = (pTop + pBase) * 0.5f;
            this.RadiusA = radiusA;
            this.RadiusB = radiusB;
            this.AxisA = axisA;
            this.AxisB = axisB;
            this.CentralAxis = pTop - pBase;
            this.Height = (pTop - pBase).Length;
            this.top = new Plane(this.centralAxis, pTop, "TopCap");
            this.bottom = new Plane(-this.centralAxis, pBase, "BottomCap");
            this.centerDotcentralAxis = this.center * this.centralAxis;
            this.RecalculateBoundBox();
        }
        public Cylinder(Point3D center, float height, Vector3D centralAxis, float radiusA, float radiusB, Vector3D axisA,
                        Vector3D axisB)
                : this(
                        center - height * 0.5f * centralAxis.Normalized, center + height * 0.5f * centralAxis.Normalized,
                        radiusA, radiusB, axisA, axisB) {}
        public Cylinder(Point3D center, float height, Vector3D centralAxis, float radius)
                : this(
                        center - height * 0.5f * centralAxis.Normalized, center + height * 0.5f * centralAxis.Normalized,
                        radius) {}
        public Point3D Center
        {
            get { return base.center; }
            set
            {
                base.center = value;
                this.pBase = this.center - this.halfHeight * this.centralAxis;
                this.pTop = this.center + this.halfHeight * this.centralAxis;
                this.centerDotcentralAxis = this.center * this.centralAxis;
                this.RecalculateBoundBox();
            }
        }
        public Point3D PointBase
        {
            get { return this.pBase; }
            set
            {
                this.pBase = value;
                this.centralAxis = (this.pTop - this.pBase);
                this.height = this.centralAxis.Length;
                this.halfHeight = this.height * 0.5f;
                this.centralAxis.Normalize();
                this.bottom.PointOnPlane = this.pBase;
                this.centerDotcentralAxis = this.center * this.centralAxis;
                this.RecalculateBoundBox();
            }
        }
        public Point3D PointTop
        {
            get { return this.pTop; }
            set
            {
                this.pTop = value;
                this.centralAxis = (this.pTop - this.pBase);
                this.height = this.centralAxis.Length;
                this.halfHeight = this.height * 0.5f;
                this.centralAxis.Normalize();
                this.top.PointOnPlane = this.pTop;
                this.centerDotcentralAxis = this.center * this.centralAxis;
                this.RecalculateBoundBox();
            }
        }
        public Vector3D CentralAxis
        {
            get { return this.centralAxis; }
            set
            {
                this.centralAxis = value;
                this.centralAxis.Normalize();
                Vector3D.Orthonormalize(this.centralAxis, out this.axisA, out this.axisB);
                if(this.radiusA != 0){
                    this.axisA /= this.radiusA;
                }
                if(this.radiusB != 0){
                    this.axisB /= this.radiusB;
                }
                this.centerDotcentralAxis = this.center * this.centralAxis;
            }
        }
        public Vector3D AxisA
        {
            get { return this.axisA; }
            set
            {
                this.axisA = value;
                this.axisA = this.axisA - (this.axisA * this.centralAxis) * this.centralAxis;
                // Make perpindicular to centralAxis
                //assert(AxisA.Norm() != 0.0);			// Must not be parallel to centralAxis
                if(this.axisA.Length == 0.0f){
                    throw new Exception(
                            "Erro: O eixo A do plano eliptico (base ou topo) tem a mesma direcao do eixo central!");
                }
                this.axisA.Normalize();
                if(this.radiusA != 0){
                    this.axisA *= 1.0f / this.radiusA;
                }
            }
        }
        public Vector3D AxisB
        {
            get { return this.axisB; }
            set
            {
                this.axisB = value;
                this.axisB = this.axisB - (this.axisB * this.centralAxis) * this.centralAxis;
                // Make perpindicular to centralAxis
                //assert(AxisA.Norm() != 0.0);			// Must not be parallel to centralAxis
                if(this.axisB.Length == 0.0f){
                    throw new Exception(
                            "Erro: O eixo B do plano eliptico (base ou topo) tem a mesma direcao do eixo central!");
                }
                this.axisB.Normalize();
                if(this.radiusB != 0){
                    this.axisB *= 1.0f / this.radiusB;
                }
            }
        }
        public float Height
        {
            get { return this.height; }
            set
            {
                if(value > 0){
                    this.height = value;
                    this.halfHeight = this.height * 0.5f;
                    this.pBase = this.center - this.halfHeight * this.centralAxis;
                    this.pTop = this.center + this.halfHeight * this.centralAxis;
                    this.RecalculateBoundBox();
                }
            }
        }
        public float RadiusA
        {
            get { return this.radiusA; }
            set
            {
                if(value > 0){
                    this.radiusA = value;
                    if(this.axisA.Length != 0.0f){
                        this.axisA.Normalize();
                        this.axisA *= 1.0f / this.radiusA;
                    }
                    this.RecalculateBoundBox();
                }
            }
        }
        public float RadiusB
        {
            get { return this.radiusB; }
            set
            {
                if(value > 0){
                    this.radiusB = value;
                    if(this.axisB.Length != 0.0f){
                        this.axisB.Normalize();
                        this.axisB *= 1.0f / this.radiusB;
                    }
                    this.RecalculateBoundBox();
                }
            }
        }

        #region ITransformable3D Members
        public void Rotate(float angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.Center = this.center;
        }
        public void RotateAxisX(float angle)
        {
            this.pBase.RotateAxisX(angle);
            this.PointBase = this.pBase;
            this.pTop.RotateAxisX(angle);
            this.PointTop = this.pTop;
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
            this.RadiusA = this.radiusA * factor;
            this.RadiusB = this.radiusB * factor;
            this.Height = this.height * factor;
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

        public override bool FindIntersection(Ray ray, out Intersection intersect)
        {
            intersect = new Intersection();
            float maxFrontDist = float.MinValue;
            float minBackDist = float.MaxValue;
            HitPrimitive frontType = HitPrimitive.None, backType = HitPrimitive.None; // 0, 1, 2 = top, bottom, side
            // Start with the bounding planes
            float pdotn = (ray.Origin * this.centralAxis) - this.centerDotcentralAxis;
            float udotn = ray.Direction * this.centralAxis;
            if(pdotn > this.halfHeight){
                if(udotn >= 0.0){
                    return false; // Above top plane pointing up
                }
                // Hits top from above
                maxFrontDist = (this.halfHeight - pdotn) / udotn;
                frontType = HitPrimitive.TopPlane;
                minBackDist = -(this.halfHeight + pdotn) / udotn;
                backType = HitPrimitive.BottomPlane;
            } else if(pdotn < -this.halfHeight){
                if(udotn <= 0.0){
                    return false; // Below bottom, pointing down
                }
                // Hits bottom plane from below
                maxFrontDist = -(this.halfHeight + pdotn) / udotn;
                frontType = HitPrimitive.BottomPlane;
                minBackDist = (this.halfHeight - pdotn) / udotn;
                backType = HitPrimitive.TopPlane;
            } else if(udotn < 0.0){
                // Inside, pointing down
                minBackDist = -(this.halfHeight + pdotn) / udotn;
                backType = HitPrimitive.BottomPlane;
            } else if(udotn > 0.0){
                // Inside, pointing up
                minBackDist = (this.halfHeight - pdotn) / udotn;
                backType = HitPrimitive.TopPlane;
            }
            if(maxFrontDist < 0){
                return false;
            }
            // Now handle the cylinder sides
            Vector3D v = ray.Origin - this.center;
            float pdotuA = v * this.axisA;
            float pdotuB = v * this.axisB;
            float udotuA = ray.Direction * this.axisA;
            float udotuB = ray.Direction * this.axisB;
            float C = pdotuA * pdotuA + pdotuB * pdotuB - 1.0f;
            float B = (pdotuA * udotuA + pdotuB * udotuB);
            if(C >= 0.0 && B > 0.0){
                return false; // Pointing away from the cylinder
            }
            B += B; // Double B for final 2.0 factor
            float A = udotuA * udotuA + udotuB * udotuB;
            float alpha1, alpha2; // The roots, in order
            int numRoots = EquationSolver.SolveQuadric(A, B, C, out alpha1, out alpha2);
            if(numRoots == 0){
                return false; // No intersection
            }
            if(alpha1 > maxFrontDist){
                if(alpha1 > minBackDist){
                    return false;
                }
                maxFrontDist = alpha1;
                frontType = HitPrimitive.Cylinder;
            }
            if(numRoots == 2 && alpha2 < minBackDist){
                if(alpha2 < maxFrontDist){
                    return false;
                }
                minBackDist = alpha2;
                backType = HitPrimitive.Cylinder;
            }
            // Put it all together:
            float alpha;
            HitPrimitive hitSurface;
            if(maxFrontDist > 0.0){
                intersect.HitFromInSide = true; // Hit from outside
                alpha = maxFrontDist;
                hitSurface = frontType;
            } else{
                intersect.HitFromInSide = false; // Hit from inside
                alpha = minBackDist;
                hitSurface = backType;
            }
            if(alpha < 0.01){
                return false;
            }
            intersect.TMin = alpha;
            intersect.TMax = alpha2;
            // Set v to the intersection point
            intersect.HitPoint = ray.Origin + ray.Direction * alpha;
            intersect.HitPrimitive = this;
            // Now set v equal to returned position relative to the center
            v = intersect.HitPoint - this.center;
            float vdotuA = v * this.axisA;
            float vdotuB = v * this.axisB;
            float uCoord = 0;
            float vCoord = 0;
            switch(hitSurface){
                case HitPrimitive.TopPlane: // Top surface
                    intersect.Normal = this.top.Normal;
                    // Calculate U-V values for texture coordinates                    
                    uCoord = 0.5f * (1.0f - vdotuA);
                    vCoord = 0.5f * (1.0f + vdotuB);
                    break;
                case HitPrimitive.BottomPlane: // Bottom face
                    intersect.Normal = this.bottom.Normal;
                    // Calculate U-V values for texture coordinates
                    uCoord = 0.5f * (1.0f + vdotuA);
                    vCoord = 0.5f * (1.0f + vdotuB);
                    break;
                case HitPrimitive.Cylinder: // Cylinder's side                    
                    intersect.Normal = ((vdotuA * this.axisA) + (vdotuB * this.axisB));
                    intersect.Normal.Normalize();
                    // Calculate u-v coordinates for texture mapping (in range[0,1]x[0,1])
                    uCoord = (float)(Math.Atan2(vdotuB, vdotuA) / (Math.PI + Math.PI) + 0.5);
                    vCoord = ((v * this.centralAxis) + this.halfHeight) * 1.0f / this.height;
                    break;
            }
            if(this.material != null && this.material.IsTexturized){
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(uCoord * widthTex), (int)(vCoord * heightTex));
                this.currentTextureCoordinate.U = uCoord;
                this.currentTextureCoordinate.V = vCoord;
            }
            return true;
        }
        private void RecalculateBoundBox()
        {
            float maxRadius = (float)Math.Max(this.radiusA, this.radiusB);
            Point3D pMin = new Point3D(this.pBase.X - maxRadius, this.pBase.Y, this.pBase.Z - maxRadius);
            Point3D pMax = new Point3D(this.pTop.X + maxRadius, this.pTop.Y, this.pTop.Z + maxRadius);
            this.boundBox = new BoundBox(pMin, pMax);
        }
        public override Vector3D NormalOnPoint(Point3D pointInPrimitive)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsInside(Point3D point)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public override bool IsOverlap(BoundBox boundBox)
        {
            throw new NotImplementedException();
        }

        #region Nested type: HitPrimitive
        private enum HitPrimitive
        {
            TopPlane,
            BottomPlane,
            Cylinder,
            None
        }
        #endregion
    }
}