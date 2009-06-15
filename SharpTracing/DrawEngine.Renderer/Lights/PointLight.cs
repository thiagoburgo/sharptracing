using System;
using System.Collections.Generic;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;

namespace DrawEngine.Renderer.Lights
{
    [Serializable]
    public class PointLight : Light
    {
        public PointLight() : base() {}
        public PointLight(RGBColor luminousIntensityBase, Point3D luminousPoint)
                : base(luminousIntensityBase, luminousPoint) {}
        public override float GetColorFactor(Vector3D pointToLight)
        {
            return 1.0f;
        }
        public override string ToString()
        {
            return "OminiLight[LuminousPoint" + this.position.ToString() + ", Intensity" + this.color.ToString() + "]";
        }
        public override void Rotate(float angle, Vector3D axis)
        {
            this.position.Rotate(angle, axis);
        }
        public override void RotateAxisX(float angle)
        {
            this.position.RotateAxisX(angle);
        }
        public override void RotateAxisY(float angle)
        {
            this.position.RotateAxisY(angle);
        }
        public override void RotateAxisZ(float angle)
        {
            this.position.RotateAxisZ(angle);
        }
        public override void Scale(float factor)
        {
            this.position.Scale(factor);
        }
        public override void Translate(float tx, float ty, float tz)
        {
            this.position.Translate(tx, ty, tz);
        }
        public override void Translate(Vector3D translateVector)
        {
            this.position.Translate(translateVector.X, translateVector.Y, translateVector.Z);
        }
        public override IEnumerable<Photon> GeneratePhotons()
        {
            int nShot = 0;
            while(nShot < this.MaxPhotons){
                nShot++;
                yield return new Photon(Vector3D.CreateRandomVector().Normalized, this.position, this.color);
            }
        }
    }
}