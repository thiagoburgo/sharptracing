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
    public class SphericalCamera : Camera
    {
        private SphericalCamera() {}
        public SphericalCamera(Point3D eye, Point3D lookAt, Vector3D up, float fov, float resX, float resY)
                : base(eye, lookAt, up, fov, resX, resY) {}
       
        public override Ray CreateRayFromScreen(float x, float y)
        {
            float cx = 2.0f * x / this.resX - 1;
            float cy = 2.0f * y / this.resY - 1;
            float r2 = cx * cx + cy * cy;
            //if (r2 > 1) {
            //    // outside the fisheye
            //    return new Ray(new Point3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
            //                   new Vector3D(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity));
            //}
            return new Ray(this.eye, new Vector3D(cx, -cy, (float)Math.Sqrt(1.0f - r2)));
            // Generate environment camera ray direction
            //double theta = 2 * Math.PI * x / this.resX + Math.PI / 2;
            //double phi = Math.PI * (this.resY - 1 - y) / this.resY;
            //return new Ray(this.eye, this.basis.Transform(new Vector3D((float)(Math.Cos(theta) * Math.Sin(phi)), -(float)(Math.Cos(phi)), (float)(Math.Sin(theta) * Math.Sin(phi)))));
        }
    }
}