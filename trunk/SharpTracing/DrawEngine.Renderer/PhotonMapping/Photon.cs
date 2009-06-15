using System;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;

namespace DrawEngine.Renderer.PhotonMapping
{
    public class Photon //: IEquatable<Photon>
    {
        private Vector3D direction;
        public byte Phi; // incoming direction
        public short Plane; // splitting plane for kd-tree
        public Point3D Position; // photon position
        public RGBColor Power; // photon power (uncompressed)
        public byte Theta; // incoming direction
        //Only for acceleration in search (avoid linear search)
        public int Index = -1;
        public Photon() {}
        public Photon(Vector3D dir, Point3D pos, RGBColor power)
        {
            //CalculatePhiTheta(dir);
            this.Direction = dir;
            this.Power = power;
            this.Position = pos;
        }
        public Photon(Photon p)
        {
            this.Position = p.Position;
            this.Plane = p.Plane;
            this.Theta = p.Theta;
            this.Phi = p.Phi;
            this.Power = p.Power;
        }
        public Vector3D Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                this.direction.Normalize();
                this.CalculatePhiTheta(this.direction);
            }
        }

        #region IEquatable<Photon> Members
        //public bool Equals(Photon p)
        //{
        //    return (this.Position == p.Position) && (this.Plane == p.Plane) && (this.Theta == p.Theta) && (this.Phi == p.Phi)
        //           && (this.Power == p.Power);
        //}
        #endregion

        private void CalculatePhiTheta(Vector3D dir)
        {
            int theta = (int)(Math.Acos(dir[2]) * (256.0 / Math.PI));
            if(theta > 255){
                this.Theta = 255;
            } else{
                this.Theta = (byte)theta;
            }
            int phi = (int)(Math.Atan2(dir[1], dir[0]) * (256.0 / (2.0 * Math.PI)));
            if(phi > 255){
                this.Phi = 255;
            } else if(phi < 0){
                this.Phi = (byte)(phi + 256);
            } else{
                this.Phi = (byte)phi;
            }
        }
        public static implicit operator Ray(Photon photon)
        {
            return new Ray(photon.Position, photon.direction);
        }
    }
}