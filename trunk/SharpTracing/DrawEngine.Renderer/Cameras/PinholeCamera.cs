using System;
using System.Drawing;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.Cameras
{
    [Serializable]
    public class PinholeCamera : Camera
    {
        public PinholeCamera() : base() {}
        public PinholeCamera(Point3D eye, Point3D lookAt, Vector3D up, float fov, float resX, float resY)
                : base(eye, lookAt, up, fov, resX, resY) {}
       
        public override Ray CreateRayFromScreen(float x, float y)
        {
            float du = -this.au + ((2.0f * this.au * x) * 1f / (this.resX));
            float dv = -this.av + ((2.0f * this.av * y) * 1f / (this.resY));
            return new Ray(this.eye, this.basis.Transform(new Vector3D(du, dv, -1)));
            
        }
        //public PinholeCamera() { }
        //public PinholeCamera(Point3D eye, Point3D lookAt, Vector3D viewUp, float fov, float resX, float resY) {
        //    this.eye = eye;
        //    this.lookAt = lookAt;
        //    this.viewUp = viewUp;            
        //    this.fov = fov;
        //    this.resX = ((resX > 0) ? resX : 0.0f);
        //    this.resY = ((resY > 0) ? resY : 0.0f);
        //    this.n = new Vector3D();
        //    this.u = new Vector3D();
        //    this.v = new Vector3D();
        //    this.CalculateNUV();
        //}
        //public override string ToString() {
        //    return String.Format("[Camera: Eye<{0}> | LookAt<{1}> | ViewUp<2> ]", this.eye, this.lookAt, this.viewUp);
        //}
        //private void CalculateNUV() {
        //    //       LA-LF
        //    //N = -----------
        //    //    || LA-LF ||
        //    this.n = (lookAt - Eye);
        //    this.n.Normalize();
        //    //      N x VUP 
        //    //U = -------------
        //    //    || N x VUP ||    
        //    //Vector3D tempVUp = viewUp;
        //    //Vector3D.Orthogonalize(ref tempVUp, this.n);            
        //    this.u = (viewUp ^ this.n);
        //    this.u.Normalize();            
        //    //     U x N
        //    //V = -----------
        //    //    || U x N ||
        //    this.v = (this.u ^ this.n);
        //    this.v.Normalize();
        //}
    }
}