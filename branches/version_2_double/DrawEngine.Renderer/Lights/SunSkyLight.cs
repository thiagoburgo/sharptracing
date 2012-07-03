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

        public override void Rotate(double angle, Vector3D axis) {
        }

        public override void RotateAxisX(double angle) {
        }

        public override void RotateAxisY(double angle) {
        }

        public override void RotateAxisZ(double angle) {
        }

        public override void Scale(double factor) {
        }

        public override void Translate(double tx, double ty, double tz) {
        }

        public override void Translate(Vector3D translateVector) {
        }

        public override double GetColorFactor(Vector3D pointToLight) {
            return 1;
        }
        #endregion
    }
}
