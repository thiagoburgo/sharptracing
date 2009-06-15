using System;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
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
        public override Ray CreateRayFromScreen(PointF pointOnScreen)
        {
            return this.CreateRayFromScreen(pointOnScreen.X, pointOnScreen.Y);
        }
        //public Point3D Center
        //{
        //    get { return eye; }
        //    set { eye = value; }
        //}
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
        }
    }
}