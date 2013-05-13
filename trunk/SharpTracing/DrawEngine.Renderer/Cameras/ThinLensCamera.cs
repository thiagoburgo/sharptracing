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
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Cameras
{
    [Serializable]
    public class ThinLensCamera : Camera
    {
        private int lensSides; // < 3 means use circular lens
        private float lensRadius;
        private float focalDistance;
        private float lensRotation; // this rotates polygonal lenses
        private float lensRotationRadians; // this rotates polygonal lenses


        public ThinLensCamera() : this(0, 0, 50) { }

        public ThinLensCamera(Point3D eye, Point3D lookAt, Vector3D up, float fov, float resX, float resY, int lensSides,
                              float lensRadius, float focusDistance)
            : base(eye, lookAt, up, fov, resX, resY)
        {
            this.LensSides = lensSides;
            this.FocalDistance = focusDistance;
            this.LensRadius = lensRadius;
        }

        public ThinLensCamera(int lensSides, float lensRadius, float focusDistance)
        {
            this.LensSides = lensSides;
            this.FocalDistance = focusDistance;
            this.LensRadius = lensRadius;
        }

        public int LensSides
        {
            get { return this.lensSides; }
            set { this.lensSides = value; }
        }

        public float LensRadius
        {
            get { return this.lensRadius; }
            set { this.lensRadius = value; }
        }

        public float FocalDistance
        {
            get { return this.focalDistance; }
            set { this.focalDistance = value; }
        }

        public float LensRotation
        {
            get { return this.lensRotation; }
            set
            {
                this.lensRotation = value;
                this.lensRotationRadians = value.ConvertDegreeToRadians();
            }
        }
        public override Ray CreateRayFromScreen(float x, float y)
        {
            float du = -this.au + ((2.0f * this.au * x) * 1f / (this.resX));
            float dv = -this.av + ((2.0f * this.av * y) * 1f / (this.resY));
            Ray ray = new Ray(this.eye, this.basis.Transform(new Vector3D(du, dv, -1)));

            //Ray ray = new Ray(eye, this.basis.W + this.basis.U * x + this.basis.V * y);

            ray.Direction = ray.Direction * focalDistance;

            double r2 = new Random().NextDouble() * lensRadius;
            double r = Math.Sqrt(r2);

            double theta = new Random().NextDouble() * 2 * Math.PI;

            double xD = r * Math.Sin(theta);
            double yD = r * Math.Cos(theta);

            Vector3D lensU = this.basis.U * (float)(xD * lensRadius);
            Vector3D lensV = this.basis.V * (float)(yD * lensRadius);

            ray.Direction = ray.Direction - (lensU + lensV);
            ray.Origin = ray.Origin + (lensU + lensV);

            ray.Direction.Normalize();
            
            return ray;
        }
        //public override Ray CreateRayFromScreen(float x, float y) {
        //    float du = -au + ((2.0f * au * x) / (this.resX - 1.0f));
        //    float dv = -av + ((2.0f * av * y) / (this.resY - 1.0f));

        //    Random rdn = new Random((int)x ^ (int)y);
        //    double lensX = rdn.NextDouble();
        //    double lensY = rdn.NextDouble();

        //    float eyeX, eyeY;
        //    if (this.lensSides < 3) {
        //        double angle, r;
        //        // concentric map sampling
        //        double r1 = 2 * lensX - 1;
        //        double r2 = 2 * lensY - 1;
        //        if (r1 > -r2) {
        //            if (r1 > r2) {
        //                r = r1;
        //                angle = 0.25 * Math.PI * r2 / r1;
        //            } else {
        //                r = r2;
        //                angle = 0.25 * Math.PI * (2 - r1 / r2);
        //            }
        //        } else {
        //            if (r1 < r2) {
        //                r = -r1;
        //                angle = 0.25 * Math.PI * (4 + r2 / r1);
        //            } else {
        //                r = -r2;
        //                if (!r2.NearZero()) {
        //                    angle = 0.25 * Math.PI * (6 - r1 / r2);
        //                } else {
        //                    angle = 0;
        //                }
        //            }
        //        }
        //        r *= this.lensRadius;
        //        // point on the lens
        //        eyeX = (float) (Math.Cos(angle) * r);
        //        eyeY = (float) (Math.Sin(angle) * r);
        //    } else {
        //        // sample N-gon
        //        // FIXME: this could use concentric sampling
        //        lensY *= this.lensSides;
        //        float side = (int) lensY;
        //        float offs = (float) lensY - side;
        //        float dist = (float) Math.Sqrt(lensX);
        //        float a0 = (float) (side * Math.PI * 2.0f / this.lensSides + lensRotationRadians);
        //        float a1 = (float) ((side + 1.0f) * Math.PI * 2.0f / this.lensSides + lensRotationRadians);
        //        eyeX = (float) ((Math.Cos(a0) * (1.0f - offs) + Math.Cos(a1) * offs) * dist);
        //        eyeY = (float) ((Math.Sin(a0) * (1.0f - offs) + Math.Sin(a1) * offs) * dist);
        //        eyeX *= this.lensRadius;
        //        eyeY *= this.lensRadius;
        //    }
        //    //float eyeZ = 0;
        //    // point on the image plane 
        //    //float dirX = du;
        //    //float dirY = dv;
        //    //float dirZ = -this.FocalDistance;
        //    // ray
        //    //dirX - eyeX, dirY - eyeY, dirZ - eyeZ
        //    Point3D newEye = this.eye; //new Point3D(this.eye.X - eyeX, this.eye.Y - eyeY, -this.eye.Z);
        //    return new Ray(newEye, this.basis.Transform(new Vector3D(du - eyeX, dv - eyeY, -this.FocalDistance)));
        //}
    }
}