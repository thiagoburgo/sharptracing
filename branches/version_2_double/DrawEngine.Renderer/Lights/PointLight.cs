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
        public override double GetColorFactor(Vector3D pointToLight)
        {
            return 1.0d;
        }
        public override string ToString()
        {
            return "OminiLight[LuminousPoint" + this.position.ToString() + ", Intensity" + this.color.ToString() + "]";
        }
        public override void Rotate(double angle, Vector3D axis)
        {
            this.position.Rotate(angle, axis);
        }
        public override void RotateAxisX(double angle)
        {
            this.position.RotateAxisX(angle);
        }
        public override void RotateAxisY(double angle)
        {
            this.position.RotateAxisY(angle);
        }
        public override void RotateAxisZ(double angle)
        {
            this.position.RotateAxisZ(angle);
        }
        public override void Scale(double factor)
        {
            this.position.Scale(factor);
        }
        public override void Translate(double tx, double ty, double tz)
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
                //double x, y, z;
                //Random rnd = new Random();
                //do{
                //    x = -1 + 2 * rnd.NextDouble();
                //    y = -1 + 2 * rnd.NextDouble();
                //    z = -1 + 2 * rnd.NextDouble();
                //} while(x * x + y *y + z * z > 1);
                //nShot++;
                //yield return new Photon(new Vector3D(x, y, z), this.position, this.color);
            }
        }
    }
}