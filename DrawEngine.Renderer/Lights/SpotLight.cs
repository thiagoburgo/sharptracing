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
    public class SpotLight : Light
    {
        // A ideia eh ter 2 angulos para criar a ilusao de area de atuacao. Um mais interno onde a atenuacao nao existe.
        // A area entre os 2 circulos dah a ilusao de atenuacao
        protected double[] apertureAngles;
        protected double[] cosApertureAngles;
        protected Vector3D direction;
        // Contribuicao dada pela spotlight para ambiente fora do cone de luz.
        protected double fallOff;
        protected Point3D towardsAt;
        public SpotLight() : base()
        {
            this.TowardsAt = Point3D.Zero;
            this.fallOff = 0.5d;
            this.apertureAngles = new double[2]{15d, 25d};
            this.cosApertureAngles = new double[2];
            this.cosApertureAngles[0] = Math.Cos(Math.PI * this.apertureAngles[0] / 180);
            this.cosApertureAngles[1] = Math.Cos(Math.PI * this.apertureAngles[1] / 180);
        }
        public SpotLight(RGBColor luminousIntensityBase, Point3D luminousPoint, Point3D towardsAt, double fallOff,
                         double minApertureAngle, double maxApertureAngle) : base(luminousIntensityBase, luminousPoint)
        {
            this.TowardsAt = towardsAt;
            this.fallOff = fallOff;
            this.apertureAngles = new double[2];
            this.cosApertureAngles = new double[2];
            this.apertureAngles[0] = minApertureAngle;
            this.apertureAngles[1] = maxApertureAngle;
            this.cosApertureAngles[0] = Math.Cos(Math.PI * this.apertureAngles[0] / 180);
            this.cosApertureAngles[1] = Math.Cos(Math.PI * this.apertureAngles[1] / 180);
        }
        public Vector3D Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                this.direction.Normalize();
                //this.towardsAt = this.position + this.direction;
            }
        }
        public double FallOff
        {
            get { return this.fallOff; }
            set
            {
                if(value >= 0 && value <= 1){
                    this.fallOff = value;
                } else{
                    throw new ArgumentException("The value must be >= 0 and <=1!", "value");
                }
            }
        }
        public double MinApertureAngle
        {
            get { return this.apertureAngles[0]; }
            set
            {
                if(value > 0 && value <= 90){
                    this.apertureAngles[0] = value;
                } else{
                    throw new ArgumentException("The value must be > 0 and <=90!", "value");
                }
            }
        }
        public double MaxApertureAngle
        {
            get { return this.apertureAngles[1]; }
            set
            {
                if(value > 0 && value <= 90){
                    this.apertureAngles[1] = value;
                } else{
                    throw new ArgumentException("The value must be > 0 and <=90!", "value");
                }
            }
        }
        public Point3D TowardsAt
        {
            get { return this.towardsAt; }
            set
            {
                this.towardsAt = value;
                this.direction = (this.towardsAt - this.Position);
                this.direction.Normalize();
            }
        }
        /// <summary>
        /// Calcula fator multiplicativo da contribuicao da spotlight. Nao pressupoe que 'pointToLight' seja normalizado, para facilitar calculo
        /// de distancias.
        /// </summary>
        /// <param name="pointToLight"></param>
        /// <returns></returns>
        public override double GetColorFactor(Vector3D pointToLight)
        {
            //pointToLight.Normalize();
            double factor = -pointToLight * this.direction;
            if(factor > this.cosApertureAngles[0]){
                return factor;
            } else if(factor < this.cosApertureAngles[1]){
                return this.fallOff;
            } else{
                double r = 1.0d
                          -
                          ((factor - this.cosApertureAngles[0])
                           / (this.cosApertureAngles[1] - this.cosApertureAngles[0]));
                return r > this.fallOff ? r : this.fallOff;
            }
        }
        public override string ToString()
        {
            return "SpotLight[LuminousPoint" + this.position.ToString() + ", Intensity" + this.color.ToString() + "]";
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
            //TODO Essa geracao de photons está igual a PointLight, uma lógica deve ser implementada
            int nShot = 0;
            while(nShot < this.MaxPhotons){
                nShot++;
                yield return new Photon(Vector3D.CreateRandomVector().Normalized, this.position, this.color);
            }
        }
    }
}