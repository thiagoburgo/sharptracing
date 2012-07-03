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
        private double centerDotcentralAxis;
        private Vector3D centralAxis;
        private double halfHeight;
        private double height;
        private Point3D pBase;
        private Point3D pTop;
        private double radiusA;
        private double radiusB;
        private Plane top = new Plane();
        public Cylinder() : this(Point3D.Zero, new Point3D(0, 20, 0), 20) {}
        public Cylinder(Point3D pBase, Point3D pTop, double radius)
                : this(pBase, pTop, radius, radius, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1)) {}
        public Cylinder(Point3D pBase, Point3D pTop, double radiusA, double radiusB, Vector3D axisA, Vector3D axisB)
        {
            this.pBase = pBase;
            this.pTop = pTop;
            this.center = (pTop + pBase) * 0.5d;
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
        public Cylinder(Point3D center, double height, Vector3D centralAxis, double radiusA, double radiusB, Vector3D axisA,
                        Vector3D axisB)
                : this(
                        center - height * 0.5d * centralAxis.Normalized, center + height * 0.5d * centralAxis.Normalized,
                        radiusA, radiusB, axisA, axisB) {}
        public Cylinder(Point3D center, double height, Vector3D centralAxis, double radius)
                : this(
                        center - height * 0.5d * centralAxis.Normalized, center + height * 0.5d * centralAxis.Normalized,
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
                this.halfHeight = this.height * 0.5d;
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
                this.halfHeight = this.height * 0.5d;
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
                if(this.axisA.Length == 0.0d){
                    throw new Exception(
                            "Erro: O eixo A do plano eliptico (base ou topo) tem a mesma direcao do eixo central!");
                }
                this.axisA.Normalize();
                if(this.radiusA != 0){
                    this.axisA *= 1.0d / this.radiusA;
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
                if(this.axisB.Length == 0.0d){
                    throw new Exception(
                            "Erro: O eixo B do plano eliptico (base ou topo) tem a mesma direcao do eixo central!");
                }
                this.axisB.Normalize();
                if(this.radiusB != 0){
                    this.axisB *= 1.0d / this.radiusB;
                }
            }
        }
        public double Height
        {
            get { return this.height; }
            set
            {
                if(value > 0){
                    this.height = value;
                    this.halfHeight = this.height * 0.5d;
                    this.pBase = this.center - this.halfHeight * this.centralAxis;
                    this.pTop = this.center + this.halfHeight * this.centralAxis;
                    this.RecalculateBoundBox();
                }
            }
        }
        public double RadiusA
        {
            get { return this.radiusA; }
            set
            {
                if(value > 0){
                    this.radiusA = value;
                    if(this.axisA.Length != 0.0d){
                        this.axisA.Normalize();
                        this.axisA *= 1.0d / this.radiusA;
                    }
                    this.RecalculateBoundBox();
                }
            }
        }
        public double RadiusB
        {
            get { return this.radiusB; }
            set
            {
                if(value > 0){
                    this.radiusB = value;
                    if(this.axisB.Length != 0.0d){
                        this.axisB.Normalize();
                        this.axisB *= 1.0d / this.radiusB;
                    }
                    this.RecalculateBoundBox();
                }
            }
        }

        #region ITransformable3D Members
        public void Rotate(double angle, Vector3D axis)
        {
            this.center.Rotate(angle, axis);
            this.Center = this.center;
        }
        public void RotateAxisX(double angle)
        {
            this.pBase.RotateAxisX(angle);
            this.PointBase = this.pBase;
            this.pTop.RotateAxisX(angle);
            this.PointTop = this.pTop;
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
            this.RadiusA = this.radiusA * factor;
            this.RadiusB = this.radiusB * factor;
            this.Height = this.height * factor;
        }
        public void Translate(double tx, double ty, double tz)
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
            double maxFrontDist = double.MinValue;
            double minBackDist = double.MaxValue;
            HitPrimitive frontType = HitPrimitive.None, backType = HitPrimitive.None; // 0, 1, 2 = top, bottom, side
            // Start with the bounding planes
            double pdotn = (ray.Origin * this.centralAxis) - this.centerDotcentralAxis;
            double udotn = ray.Direction * this.centralAxis;
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
            double pdotuA = v * this.axisA;
            double pdotuB = v * this.axisB;
            double udotuA = ray.Direction * this.axisA;
            double udotuB = ray.Direction * this.axisB;
            double C = pdotuA * pdotuA + pdotuB * pdotuB - 1.0d;
            double B = (pdotuA * udotuA + pdotuB * udotuB);
            if(C >= 0.0 && B > 0.0){
                return false; // Pointing away from the cylinder
            }
            B += B; // Double B for final 2.0 factor
            double A = udotuA * udotuA + udotuB * udotuB;
            double alpha1, alpha2; // The roots, in order
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
            double alpha;
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
            double vdotuA = v * this.axisA;
            double vdotuB = v * this.axisB;
            double uCoord = 0;
            double vCoord = 0;
            switch(hitSurface){
                case HitPrimitive.TopPlane: // Top surface
                    intersect.Normal = this.top.Normal;
                    // Calculate U-V values for texture coordinates                    
                    uCoord = 0.5d * (1.0d - vdotuA);
                    vCoord = 0.5d * (1.0d + vdotuB);
                    break;
                case HitPrimitive.BottomPlane: // Bottom face
                    intersect.Normal = this.bottom.Normal;
                    // Calculate U-V values for texture coordinates
                    uCoord = 0.5d * (1.0d + vdotuA);
                    vCoord = 0.5d * (1.0d + vdotuB);
                    break;
                case HitPrimitive.Cylinder: // Cylinder's side                    
                    intersect.Normal = ((vdotuA * this.axisA) + (vdotuB * this.axisB));
                    intersect.Normal.Normalize();
                    // Calculate u-v coordinates for texture mapping (in range[0,1]x[0,1])
                    uCoord = (Math.Atan2(vdotuB, vdotuA) / (Math.PI + Math.PI) + 0.5);
                    vCoord = ((v * this.centralAxis) + this.halfHeight) * 1.0d / this.height;
                    break;
            }
            if(this.material != null && this.material.IsTexturized){
                //int widthTex = this.material.Texture.Width - 1;
                //int heightTex = this.material.Texture.Height - 1;
                //this.material.Color = this.material.Texture.GetPixel((int)(uCoord * widthTex), (int)(vCoord * heightTex));
                intersect.CurrentTextureCoordinate.U = uCoord;
                intersect.CurrentTextureCoordinate.V = vCoord;
            }
            return true;
        }
        private void RecalculateBoundBox()
        {
            double maxRadius = Math.Max(this.radiusA, this.radiusB);
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