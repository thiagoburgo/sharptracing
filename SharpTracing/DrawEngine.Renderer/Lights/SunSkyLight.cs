using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.PhotonMapping;
using DrawEngine.Renderer.RenderObjects;

namespace DrawEngine.Renderer.Lights {
    //TODO: implement
    public class SunSkyLight : Light {
        
        #region Inherit from Light
        public override IEnumerable<Photon> GeneratePhotons() {
            return null;
        }

        public override void Rotate(float angle, Vector3D axis) {
        }

        public override void RotateAxisX(float angle) {
        }

        public override void RotateAxisY(float angle) {
        }

        public override void RotateAxisZ(float angle) {
        }

        public override void Scale(float factor) {
        }

        public override void Translate(float tx, float ty, float tz) {
        }

        public override void Translate(Vector3D translateVector) {
        }

        public override float GetColorFactor(Vector3D pointToLight) {
            return 1;
        }
        #endregion
    }
}
