/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing � um projeto feito inicialmente para disciplina
 * Computa��o Gr�fica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar corre��es e 
 * sugest�es. Mantenha os cr�ditos!
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
    public class SphericalCamera : Camera
    {
        private SphericalCamera() {}
        public SphericalCamera(Point3D eye, Point3D lookAt, Vector3D up, double fov, double resX, double resY)
                : base(eye, lookAt, up, fov, resX, resY) {}
       
        public override Ray CreateRayFromScreen(double x, double y)
        {
            double cx = 2.0d * x / this.resX - 1;
            double cy = 2.0d * y / this.resY - 1;
            double r2 = cx * cx + cy * cy;
            //if (r2 > 1) {
            //    // outside the fisheye
            //    return new Ray(new Point3D(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity),
            //                   new Vector3D(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity));
            //}
            return new Ray(this.eye, new Vector3D(cx, -cy, Math.Sqrt(1.0d - r2)));
            // Generate environment camera ray direction
            //double theta = 2 * Math.PI * x / this.resX + Math.PI / 2;
            //double phi = Math.PI * (this.resY - 1 - y) / this.resY;
            //return new Ray(this.eye, this.basis.Transform(new Vector3D((Math.Cos(theta) * Math.Sin(phi)), -(Math.Cos(phi)), (Math.Sin(theta) * Math.Sin(phi)))));
        }
    }
}