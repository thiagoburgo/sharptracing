using System;
using System.Collections.Generic;
using DrawEngine.Renderer.Algebra;
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
        protected float[] apertureAngles;
        protected float[] cosApertureAngles;
        protected Vector3D direction;
        // Contribuicao dada pela spotlight para ambiente fora do cone de luz.
        protected float fallOff;
        protected Point3D towardsAt;
        public SpotLight() : base()
        {
            this.TowardsAt = Point3D.Zero;
            this.fallOff = 0.5f;
            this.apertureAngles = new float[2]{15f, 25f};
            this.cosApertureAngles = new float[2];
            this.cosApertureAngles[0] = (float)Math.Cos(Math.PI * this.apertureAngles[0] / 180);
            this.cosApertureAngles[1] = (float)Math.Cos(Math.PI * this.apertureAngles[1] / 180);
        }
        public SpotLight(RGBColor luminousIntensityBase, Point3D luminousPoint, Point3D towardsAt, float fallOff,
                         float minApertureAngle, float maxApertureAngle) : base(luminousIntensityBase, luminousPoint)
        {
            this.TowardsAt = towardsAt;
            this.fallOff = fallOff;
            this.apertureAngles = new float[2];
            this.cosApertureAngles = new float[2];
            this.apertureAngles[0] = minApertureAngle;
            this.apertureAngles[1] = maxApertureAngle;
            this.cosApertureAngles[0] = (float)Math.Cos(Math.PI * this.apertureAngles[0] / 180);
            this.cosApertureAngles[1] = (float)Math.Cos(Math.PI * this.apertureAngles[1] / 180);
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
        public float FallOff
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
        public float MinApertureAngle
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
        public float MaxApertureAngle
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
        public override float GetColorFactor(Vector3D pointToLight)
        {
            //pointToLight.Normalize();
            float factor = -pointToLight * this.direction;
            if(factor > this.cosApertureAngles[0]){
                return factor;
            } else if(factor < this.cosApertureAngles[1]){
                return this.fallOff;
            } else{
                float r = 1.0f
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
            //TODO Essa geracao de photons está igual a PointLight, uma lógica deve ser implementada
            int nShot = 0;
            while(nShot < this.MaxPhotons){
                nShot++;
                yield return new Photon(Vector3D.CreateRandomVector().Normalized, this.position, this.color);
            }
        }
    }
}