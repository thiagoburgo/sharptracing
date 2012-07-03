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
using System.Drawing;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Cameras
{
    [Serializable]
    public class ThinLensCamera : Camera
    {
        private int lensSides; // < 3 means use circular lens
        private double lensRadius;
        private double focusDistance;
        private double lensRotation;// this rotates polygonal lenses
        private double lensRotationRadians;// this rotates polygonal lenses

     
        public ThinLensCamera() : this(0, 0, 50){   
        }
        public ThinLensCamera(Point3D eye, Point3D lookAt, Vector3D up,
                              double fov, double resX, double resY,
                              int lensSides, double lensRadius, double focusDistance) : base(eye, lookAt, up, fov,resX, resY)
        {
            this.LensSides = lensSides;
            this.FocusDistance = focusDistance;
            this.LensRadius = lensRadius;
        }
        public ThinLensCamera(int lensSides, double lensRadius, double focusDistance) {
            this.LensSides = lensSides;
            this.FocusDistance = focusDistance;
            this.LensRadius = lensRadius;
        }
        public int LensSides
        {
            get { return this.lensSides; }
            set { this.lensSides = value; }
        }
        public double LensRadius
        {
            get { return this.lensRadius; }
            set { this.lensRadius = value; }
        }
        public double FocusDistance
        {
            get { return this.focusDistance; }
            set { this.focusDistance = value; }
        }
        public double LensRotation
        {
            get { return this.lensRotation; }
            set
            {
                this.lensRotation = value;
                this.lensRotationRadians = MathUtil.ConvertDegreeToRadians(value);
            }
        }
        public override Ray CreateRayFromScreen(double x, double y)
        {
            double du = -au + ((2.0d * au * x) / (this.resX - 1.0d));
            double dv = -av + ((2.0d * av * y) / (this.resY - 1.0d));

            Random rdn = new Random();
            double lensX = rdn.NextDouble();
            double lensY = rdn.NextDouble(); 

            double eyeX, eyeY;
            if(this.lensSides < 3) {
                double angle, r;
                // concentric map sampling
                double r1 = 2 * lensX - 1;
                double r2 = 2 * lensY - 1;
                if(r1 > -r2) {
                    if(r1 > r2) {
                        r = r1;
                        angle = 0.25 * Math.PI * r2 / r1;
                    }
                    else {
                        r = r2;
                        angle = 0.25 * Math.PI * (2 - r1 / r2);
                    }
                }
                else {
                    if(r1 < r2) {
                        r = -r1;
                        angle = 0.25 * Math.PI * (4 + r2 / r1);
                    }
                    else {
                        r = -r2;
                        if(r2 != 0)
                            angle = 0.25 * Math.PI * (6 - r1 / r2);
                        else
                            angle = 0;
                    }
                }
                r *= this.lensRadius;
                // point on the lens
                eyeX = (Math.Cos(angle) * r);
                eyeY = (Math.Sin(angle) * r);
            }
            else {
                // sample N-gon
                // FIXME: this could use concentric sampling
                lensY *= this.lensSides;
                double side = (int)lensY;
                double offs = lensY - side;
                double dist = Math.Sqrt(lensX);
                double a0 = (side * Math.PI * 2.0d / this.lensSides + lensRotationRadians);
                double a1 = ((side + 1.0d) * Math.PI * 2.0d / this.lensSides + lensRotationRadians);
                eyeX = ((Math.Cos(a0) * (1.0d - offs) + Math.Cos(a1) * offs) * dist);
                eyeY = ((Math.Sin(a0) * (1.0d - offs) + Math.Sin(a1) * offs) * dist);
                eyeX *= this.lensRadius;
                eyeY *= this.lensRadius;
            }
            //double eyeZ = 0;
            // point on the image plane 
            //double dirX = du;
            //double dirY = dv;
            //double dirZ = -this.focusDistance;
            // ray
            //dirX - eyeX, dirY - eyeY, dirZ - eyeZ
            Point3D newEye = this.eye;//new Point3D(this.eye.X - eyeX, this.eye.Y - eyeY, -this.eye.Z);
            return new Ray(newEye, this.basis.Transform(new Vector3D(du - eyeX, dv - eyeY, -this.focusDistance)));
        }
        
    }
}